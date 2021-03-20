import { combineEpics, Epic, StateObservable } from "redux-observable";
import { EMPTY, throwError, combineLatest, Observable, of } from "rxjs";
import { ajax } from "rxjs/ajax";
import { filter, map, mergeMap, tap, withLatestFrom, catchError, distinctUntilChanged } from "rxjs/operators";
import { isOfType } from "typesafe-actions";
import { loginRequest } from "../../src/auth-config";
import { msalService } from "../../src/services/msal-auth.service";
import { buildUrl } from "../../src/url-utils";
import { RootState } from "../store";
import { getScopesPopupEpic, getScopesRedirectEpic, getScopesSilentEpic } from "./auth-scopes.epic";
import { AuthAction, AuthActionTypes, createUserComplete, createUserFailed, getScopes, requestUserUpdate, setInProgress, setUser, updateUserComplete, updateUserFailed, User, UserStatus, validateLogin, validateLoginComplete, validateLoginFailed } from "./auth.actions";

const loginEpic: Epic<AuthAction, AuthAction, RootState> = (action$, state$) => action$.pipe(
  filter(isOfType(AuthActionTypes.LOGIN)),
  withLatestFrom(state$),
  mergeMap(([, state]) => {
    if (state.auth.user.current?.username) {
      console.log(`We have a hint, so let's try a silent login`);
      return msalService.ssoSilent({ ...loginRequest, loginHint: state.auth.user.current?.username });
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

const validateLoginEpic: Epic<AuthAction, AuthAction, RootState> = (action$) => action$.pipe(
  filter(isOfType(AuthActionTypes.VALIDATE_LOGIN)),
  tap(() => console.log(`Validating Login`)),
  mergeMap((action) => msalService.currentAccount$().pipe(
    mergeMap((account) => {
      if (account) {
        return of(validateLoginComplete(), getScopes(action.payload.scopes));
      }
      return of(validateLoginFailed());
    })
  )),
);

const setUserEpic: Epic<AuthAction, AuthAction, RootState> = (action$, state$) => withToken$(action$.pipe(
  filter(action => 
    isOfType(AuthActionTypes.VALIDATE_LOGIN_COMPLETE)(action) ||
    isOfType(AuthActionTypes.CREATE_USER_COMPLETE)(action) ||
    isOfType(AuthActionTypes.UPDATE_USER_COMPLETE)(action))), state$).pipe(
    filter(([, token]) => !!token),
    mergeMap(([, token]) => ajax.getJSON<User>(buildUrl(`/me`), { 'Authorization': `Bearer ${token}` }).pipe(
      map(user => setUser(user)),
      // catchError(err => of(validateLoginFailed()))
    ))
  );

const requestUserUpdateEpic: Epic<AuthAction, AuthAction, RootState> = (action$, state$) => action$.pipe(
  filter(isOfType(AuthActionTypes.SET_USER)),
  map(action => action.payload.user),
  filter(user => !!user),
  filter(user => user?.status === UserStatus.None),
  mergeMap((user) => user && of(requestUserUpdate(user)) || EMPTY)
);

const inProgressEpic: Epic<AuthAction, AuthAction> = () => msalService.isInProgress$().pipe(
  distinctUntilChanged(),
  map((inProgress) => setInProgress(inProgress))
);

const createUserEpic: Epic<AuthAction, AuthAction, RootState> = (action$, state$) => withToken$(action$.pipe(
  filter(isOfType(AuthActionTypes.CREATE_USER))), state$).pipe(
    filter(([, token]) => !!token),
    mergeMap(([action, token]) => ajax.post(buildUrl(`/me`), action.payload.user, { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' })),
    map(() => createUserComplete()),
    catchError((err) => of(createUserFailed('CreateUserFailed', err)))
);

const updateUserEpic: Epic<AuthAction, AuthAction, RootState> = (action$, state$) => withToken$(action$.pipe(
  filter(isOfType(AuthActionTypes.UPDATE_USER))), state$).pipe(
    filter(([, token]) => !!token),
    mergeMap(([action, token]) => ajax.put(buildUrl(`/me`), action.payload.user, { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' })),
    map(() => updateUserComplete()),
    catchError((err) => of(updateUserFailed('UpdateUserFailed', err)))
);

const currentAccessToken$ = (state$: StateObservable<RootState>) => state$.pipe(
  map(state => state.auth.accessToken),
  distinctUntilChanged()
);

const currentUser$ = (state$: StateObservable<RootState>) => state$.pipe(
  map(state => state.auth.user.current),
  distinctUntilChanged()
);

export function whenLoggedIn$<TAction>(action$: Observable<TAction>): Observable<TAction> {
  return combineLatest([action$, msalService.currentAccount$()]).pipe(
    filter(([, account]) => !!account),
    map(([action]) => action),
  );
}

export function withTokenAndUser$<TAction>(action$: Observable<TAction>, state$: StateObservable<RootState>): Observable<[TAction, string | undefined, User | undefined]> {
  return combineLatest([action$, currentUser$(state$)]).pipe(
    filter(([, user]) => !!user && user.status !== UserStatus.None),
    withLatestFrom(currentAccessToken$(state$)),
    map(([[action, user], token]) => [action, token, user])
  );
}

export function withToken$<TAction>(action$: Observable<TAction>, state$: StateObservable<RootState>): Observable<[TAction, string | undefined]> {
  return combineLatest([action$, currentAccessToken$(state$)]).pipe(
    filter(([, accessToken]) => !!accessToken),
  );
}


export const authEpic = combineEpics(loginEpic, logoutEpic, getScopesSilentEpic, getScopesPopupEpic, getScopesRedirectEpic, validateLoginEpic, inProgressEpic, setUserEpic, requestUserUpdateEpic, createUserEpic, updateUserEpic);