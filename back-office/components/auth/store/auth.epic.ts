import { combineEpics, Epic } from "redux-observable";
import { EMPTY, iif, of, throwError } from "rxjs";
import { filter, map, mergeMap, tap, withLatestFrom, catchError, switchMap, distinctUntilChanged } from "rxjs/operators";
import { isOfType } from "typesafe-actions";
import { loginRequest } from "../../../src/auth-config";
import { msalService } from "../../../src/services/msal-auth.service";
import { RootState } from "../../../store/store";
import { AuthAction, AuthActionTypes, getUser, setAccessToken, setInProgress, setUser } from "./auth.actions";

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
  map(() => getUser())
);

const getScopesEpic: Epic<AuthAction, AuthAction, RootState> = (action$) => action$.pipe(
  filter(isOfType(AuthActionTypes.GET_SCOPES)),
  mergeMap((action) =>
    msalService.acquireTokenSilent({ scopes: ["profile", "openid", ...action.payload.scopes] }).pipe(
      switchMap(res => {
        if (!res.accessToken) {
          return msalService.acquireTokenPopup({ scopes: ["profile", "openid", ...action.payload.scopes] });
        }
        return of(res);
      }),
      catchError((err) => {
        const errString = String(err);
        if (errString.indexOf('popup_window_error') >= 0) {
          // Popup not OK - let's do it with a redirect instead.
          msalService.acquireTokenRedirect({ scopes: ["profile", "openid", ...action.payload.scopes] });
          return EMPTY;
        }
        // Other error
        return throwError({ code: "AcquireTokenFailed" });
      }),
    )),
  map((res) => res.accessToken),
  distinctUntilChanged(),
  map((token) => setAccessToken(token))
);

const logoutEpic: Epic<AuthAction, AuthAction, RootState> = (action$) => action$.pipe(
  filter(isOfType(AuthActionTypes.LOGOUT)),
  mergeMap(() => msalService.logout()),
  mergeMap(() => EMPTY)
);

const getUserEpic: Epic<AuthAction, AuthAction> = (action$) => action$.pipe(
  filter(isOfType(AuthActionTypes.GET_USER)),
  switchMap(() => msalService.currentAccount$()),
  map(account => account && {
    displayName: account.name || 'Unknown',
    username: account.username
  } || undefined),
  map((user) => setUser(user))
);

const inProgressEpic: Epic<AuthAction, AuthAction> = () => msalService.isInProgress$().pipe(
  distinctUntilChanged(),
  map((inProgress) => setInProgress(inProgress))
);

export const authEpic = combineEpics(loginEpic, logoutEpic, getScopesEpic, getUserEpic, inProgressEpic);