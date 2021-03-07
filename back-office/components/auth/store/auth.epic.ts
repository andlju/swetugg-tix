import { AnyAction } from "redux";
import { ActionsObservable, combineEpics, Epic, StateObservable } from "redux-observable";
import { EMPTY, throwError, combineLatest, Observable } from "rxjs";
import { filter, map, mergeMap, tap, withLatestFrom, catchError, distinctUntilChanged } from "rxjs/operators";
import { isOfType } from "typesafe-actions";
import { loginRequest } from "../../../src/auth-config";
import { msalService } from "../../../src/services/msal-auth.service";
import { RootState } from "../../../store/store";
import { getScopesPopupEpic, getScopesRedirectEpic, getScopesSilentEpic } from "./auth-scopes.epic";
import { AuthAction, AuthActionTypes, setInProgress, setUser, User, validateUserFailed } from "./auth.actions";

const loginEpic: Epic<AuthAction, AuthAction, RootState> = (action$, state$) => action$.pipe(
  filter(isOfType(AuthActionTypes.LOGIN)),
  withLatestFrom(state$),
  mergeMap(([, state]) => {
    if (state?.auth?.user?.username) {
      console.log(`We have a hint, so let's try a silent login`);
      return msalService.ssoSilent({ ...loginRequest, loginHint: state?.auth?.user?.username });
    }
    throw { errorCode: "No hint found" };
  }),
  catchError((err) => {
    console.log('Silent failed - try a popup', err);
    return msalService.loginPopup(loginRequest);
  }),
  catchError((err) => {
    const errString = String(err);
    if (errString.indexOf('popup_window_error') >= 0) {
      // Popup not OK - let's do it with a redirect instead.
      return msalService.loginRedirect(loginRequest);
    }
    // User cancelled login (or other error)
    return throwError({ code: "LoginFailed" });
  }),
  mergeMap(() => EMPTY)
);

const logoutEpic: Epic<AuthAction, AuthAction, RootState> = (action$) => action$.pipe(
  filter(isOfType(AuthActionTypes.LOGOUT)),
  mergeMap(() => msalService.logout()),
  mergeMap(() => EMPTY)
);

const validateUserEpic: Epic<AuthAction, AuthAction> = (action$) => action$.pipe(
  filter(isOfType(AuthActionTypes.VALIDATE_USER)),
  tap(() => console.log(`Validating User`)),
  mergeMap(() => msalService.currentAccount$()),
  map(account => account && {
    displayName: account.name || 'Unknown',
    username: account.username
  } || undefined),
  map((user) => {
    if (user) {
      console.log("Validate user successful");
      return setUser(user);
    }
    console.log("Validate user failed");
    return validateUserFailed();
  })
);

const inProgressEpic: Epic<AuthAction, AuthAction> = () => msalService.isInProgress$().pipe(
  distinctUntilChanged(),
  map((inProgress) => setInProgress(inProgress))
);


const currentAccessToken$ = (state$: StateObservable<RootState>) => state$.pipe(
  map(state => state.auth.accessToken),
  distinctUntilChanged()
);

const currentUser$ = (state$: StateObservable<RootState>) => state$.pipe(
  map(state => state.auth?.user),
  distinctUntilChanged()
);

export function whenLoggedIn$<TAction>(action$: Observable<TAction>): Observable<TAction> {
  return combineLatest([action$, msalService.currentAccount$()]).pipe(
    filter(([, user]) => !!user),
    tap(([action, user]) => console.log('When logged in triggered for action', action)),
    map(([action]) => action),
  );
}

export function withLoggedInUser$<TAction>(action$: Observable<TAction>, state$: StateObservable<RootState>): Observable<[TAction, User | undefined]> {
  return combineLatest([action$, currentUser$(state$)]).pipe(
    filter(([, user]) => !!user),
  );
}

export function withToken$<TAction>(action$: Observable<TAction>, state$: StateObservable<RootState>): Observable<[TAction, string | undefined]> {
  return combineLatest([action$, currentAccessToken$(state$)]).pipe(
    filter(([, accessToken]) => !!accessToken),
  );
}


export const authEpic = combineEpics(loginEpic, logoutEpic, getScopesSilentEpic, getScopesPopupEpic, getScopesRedirectEpic, validateUserEpic, inProgressEpic);