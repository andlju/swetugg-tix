import { combineEpics, Epic, StateObservable } from "redux-observable";
import { EMPTY, throwError, combineLatest, Observable, of } from "rxjs";
import { ajax } from "rxjs/ajax";
import { filter, map, mergeMap, tap, withLatestFrom, catchError, distinctUntilChanged, take } from "rxjs/operators";
import { isOfType } from "typesafe-actions";
import { loginRequest } from "../../src/auth-config";
import { msalService } from "../../src/services/msal-auth.service";
import { buildUrl } from "../../src/url-utils";
import { RootState } from "../store";
import { getScopesPopupEpic, getScopesRedirectEpic, getScopesSilentEpic } from "./auth-scopes.epic";
import { AuthAction, AuthActionTypes, createUserComplete, createUserFailed, getScopes, InteractionKind, login, loginCompleted, loginFailed, requestUserUpdate, setInProgress, setUser, updateUserComplete, updateUserFailed, User, UserStatus, validateLogin, validateLoginComplete, validateLoginFailed } from "./auth.actions";

const loginSilentEpic: Epic<AuthAction, AuthAction, RootState> = (action$, state$) => action$.pipe(
  filter(isOfType(AuthActionTypes.LOGIN)),
  filter(action => action.payload.interactionKind === InteractionKind.SILENT),
  tap(() => console.log('Attempt Silent Login')),
  withLatestFrom(state$),
  mergeMap(([, state]) => {
    if (state.auth.user.current?.subject) {
      console.log(`We have a hint, so let's try a silent login`);
      return msalService.ssoSilent({ ...loginRequest, loginHint: state.auth.user.current?.subject });
    }
    return of(login(InteractionKind.POPUP));
  }),
  catchError((err, caught) => {
    return caught.pipe(
      map(() => login(InteractionKind.POPUP))
    );
  }),
  map(() => loginCompleted())
);

const loginPopupEpic: Epic<AuthAction, AuthAction, RootState> = (action$, state$) => action$.pipe(
  filter(isOfType(AuthActionTypes.LOGIN)),
  filter(action => action.payload.interactionKind === InteractionKind.POPUP),
  tap(() => console.log('Attempt Popup Login')),
  mergeMap(() => {
    return msalService.loginPopup(loginRequest);
  }),
  catchError((err, caught) => {
    const errString = String(err);
    if (errString.indexOf('popup_window_error') >= 0) {
      // Popup not OK - let's do it with a redirect instead.
      return caught.pipe(map(() => login(InteractionKind.REDIRECT)));
    }
    // User cancelled login (or other error)
    console.log('Login Failed', err);
    return caught.pipe(
      map(() => loginFailed('LoginFailed', errString))
    )
  }),
  map(() => loginCompleted())
);


const loginRedirectEpic: Epic<AuthAction, AuthAction, RootState> = (action$, state$) => action$.pipe(
  filter(isOfType(AuthActionTypes.LOGIN)),
  filter(action => action.payload.interactionKind === InteractionKind.REDIRECT),
  tap(() => console.log('Attempt Redirect Login')),
  mergeMap(() => {
    return msalService.loginRedirect(loginRequest);
  }),
  catchError((err, caught) => {
    const errString = String(err);
    // User cancelled login (or other error)
    console.log('Login Failed', errString);
    return caught.pipe(
      map(() => loginFailed('LoginFailed', errString))
    );
  }),
  map(() => loginCompleted())
);

const logoutEpic: Epic<AuthAction, AuthAction, RootState> = (action$) => action$.pipe(
  filter(isOfType(AuthActionTypes.LOGOUT)),
  mergeMap(() => msalService.logout()),
  mergeMap(() => EMPTY)
);

const validateLoginEpic: Epic<AuthAction, AuthAction, RootState> = (action$) => action$.pipe(
  filter(isOfType(AuthActionTypes.VALIDATE_LOGIN)),
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

const createUserEpic: Epic<AuthAction, AuthAction, RootState> = (action$, state$) => action$.pipe(
  filter(isOfType(AuthActionTypes.CREATE_USER)),
  withLatestFrom(currentAccessToken$(state$)),
  filter(([, token]) => !!token),
  take(1),
  mergeMap(([action, token]) => ajax.post(buildUrl(`/me`), action.payload.user, { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' })),
  map(() => createUserComplete()),
  catchError((err) => of(createUserFailed('CreateUserFailed', err)))
);

const updateUserEpic: Epic<AuthAction, AuthAction, RootState> = (action$, state$) => action$.pipe(
  filter(isOfType(AuthActionTypes.UPDATE_USER)),
  withLatestFrom(currentAccessToken$(state$)),
  filter(([, token]) => !!token),
  take(1),
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

const whenUser$ = (state$: StateObservable<RootState>) =>
  currentUser$(state$).pipe(
    filter((user) => !!user && user.status !== UserStatus.None),
    take(1)
  );

const whenToken$ = (state$: StateObservable<RootState>) =>
  currentAccessToken$(state$).pipe(
    filter((accessToken) => !!accessToken),
    take(1)
  );

export function whenLoggedIn$<TAction>(action$: Observable<TAction>): Observable<TAction> {
  return combineLatest([action$, msalService.currentAccount$()]).pipe(
    filter(([, account]) => !!account),
    map(([action]) => action),
  );
}

export function withTokenAndUser$<TAction>(action$: Observable<TAction>, state$: StateObservable<RootState>): Observable<[TAction, string | undefined, User | undefined]> {
  return combineLatest([action$, whenUser$(state$)]).pipe(
    withLatestFrom(currentAccessToken$(state$)),
    map(([[action, user], token]) => [action, token, user]),
  );
}

export function withToken$<TAction>(action$: Observable<TAction>, state$: StateObservable<RootState>): Observable<[TAction, string | undefined]> {
  return combineLatest([action$, whenToken$(state$)]).pipe(
    filter(([, accessToken]) => !!accessToken),
  );
}

export const authEpic = combineEpics(loginSilentEpic, loginPopupEpic, loginRedirectEpic, logoutEpic, getScopesSilentEpic, getScopesPopupEpic, getScopesRedirectEpic, validateLoginEpic, inProgressEpic, setUserEpic, requestUserUpdateEpic, createUserEpic, updateUserEpic);
