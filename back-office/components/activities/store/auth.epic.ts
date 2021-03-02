import { EventType } from "@azure/msal-browser";
import { getDisplayName } from "next/dist/next-server/lib/utils";
import { combineEpics, Epic } from "redux-observable";
import { EMPTY, iif, throwError } from "rxjs";
import { filter, map, mergeMap, tap, withLatestFrom, catchError, switchMap } from "rxjs/operators";
import { isOfType } from "typesafe-actions";
import { loginRequest } from "../../../src/auth-config";
import { msalService } from "../../../src/services/msal-auth.service";
import { RootState } from "../../../store/store";
import { AuthAction, AuthActionTypes, getUser, setUser } from "./auth.actions";

const loginEpic : Epic<AuthAction, AuthAction, RootState> = (action$, state$) => action$.pipe(
  filter(isOfType(AuthActionTypes.LOGIN)),
  withLatestFrom(state$),
/*  mergeMap(() => msalService.ssoSilent(loginRequest)),
  catchError((err) => {
    console.log('Silent failed - try a popup', err);
    return msalService.loginPopup(loginRequest);
  }),*/
  mergeMap(([,state]) => {
    if (state.auth.user) {
      // Already logged in - return without action
      return EMPTY;
    } else {
      // Not logged in - try a popup first
      return msalService.loginPopup(loginRequest);
    }
  }),
  catchError((err) => {
    const errString = String(err);
    if (errString.indexOf('popup_window_error') >= 0) {
      // Popup not OK - let's do it with a redirect instead.
      return msalService.loginRedirect(loginRequest);
    }
    // User cancelled login (or other error)
    return throwError({ code: "LoginFailed"});
  }),
  map(() => getUser())
);

const getScopesEpic : Epic<AuthAction, AuthAction, RootState> = (action$) => action$.pipe(
  filter(isOfType(AuthActionTypes.GET_SCOPES)),
  tap(() => console.log("Getting scopes")),
  mergeMap((action) => msalService.acquireTokenSilent({scopes: action.payload.scopes})),
  mergeMap(() => EMPTY)
);

const logoutEpic : Epic<AuthAction, AuthAction, RootState> = (action$) => action$.pipe(
  filter(isOfType(AuthActionTypes.LOGOUT)),
  mergeMap(() => msalService.logout()),
  mergeMap(() => EMPTY)
);

const getUserEpic : Epic<AuthAction, AuthAction> = (action$) => action$.pipe(
  filter(isOfType(AuthActionTypes.GET_USER)),
  switchMap(() => msalService.currentAccount$()),
  map(account => account && {
    displayName: account.name || 'Unknown'
  } || undefined),
  map((user) => setUser(user))
);

/*const inProgressEpic : Epic<AuthAction, AuthAction> = () => msalService.isInProgress$().pipe(
  distinctUntilChanged(),
  map((inProgress) => setInProgress(inProgress))
);*/

export const authEpic = combineEpics(loginEpic, logoutEpic, getScopesEpic, getUserEpic);