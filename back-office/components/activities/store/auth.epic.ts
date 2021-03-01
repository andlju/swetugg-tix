import { EventType } from "@azure/msal-browser";
import { getDisplayName } from "next/dist/next-server/lib/utils";
import { combineEpics, Epic } from "redux-observable";
import { EMPTY, iif, throwError } from "rxjs";
import { distinctUntilChanged, filter, map, mergeMap, tap, withLatestFrom, catchError } from "rxjs/operators";
import { isOfType } from "typesafe-actions";
import { loginRequest } from "../../../src/auth-config";
import { msalService } from "../../../src/services/msal-auth.service";
import { RootState } from "../../../store/store";
import { AuthAction, AuthActionTypes, setInProgress, setUser } from "./auth.actions";

const loginEpic : Epic<AuthAction, AuthAction, RootState> = (action$, state$) => action$.pipe(
  filter(isOfType(AuthActionTypes.LOGIN)),
  withLatestFrom(state$),
  filter(([,state]) => !state.auth.inProgress),
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
  mergeMap(() => EMPTY)
);

const getScopesEpic : Epic<AuthAction, AuthAction, RootState> = (action$, state$) => action$.pipe(
  filter(isOfType(AuthActionTypes.GET_SCOPES)),
  tap(() => console.log("Getting scopes")),
  withLatestFrom(state$),
  filter(([,state]) => !state.auth.inProgress),
  mergeMap(([action]) => msalService.acquireTokenSilent({scopes: action.payload.scopes})),
  mergeMap(() => EMPTY)
);

const logoutEpic : Epic<AuthAction, AuthAction, RootState> = (action$, state$) => action$.pipe(
  filter(isOfType(AuthActionTypes.LOGOUT)),
  withLatestFrom(state$),
  filter(([,state]) => !state.auth.inProgress),
  mergeMap(() => msalService.logout()),
  mergeMap(() => EMPTY)
);

const inProgressEpic : Epic<AuthAction, AuthAction> = () => msalService.isInProgress$().pipe(
  distinctUntilChanged(),
  map((inProgress) => setInProgress(inProgress))
);

const currentUserEpic: Epic<AuthAction, AuthAction> = () => msalService.currentAccount$().pipe(
  map(account => account && {
    displayName: account.name || 'Unknown'
  } || undefined),
  map(user => setUser(user))
);

export const authEpic = combineEpics(loginEpic, logoutEpic, getScopesEpic, inProgressEpic, currentUserEpic);