import { EventType } from "@azure/msal-browser";
import { getDisplayName } from "next/dist/next-server/lib/utils";
import { combineEpics, Epic } from "redux-observable";
import { EMPTY } from "rxjs";
import { distinctUntilChanged, filter, map, mergeMap, tap, withLatestFrom } from "rxjs/operators";
import { isOfType } from "typesafe-actions";
import { loginRequest } from "../../../src/auth-config";
import { msalService } from "../../../src/services/msal-auth.service";
import { RootState } from "../../../store/store";
import { AuthAction, AuthActionTypes, setInProgress, setUser } from "./auth.actions";

const authenticateEpic : Epic<AuthAction, AuthAction, RootState> = (action$, state$) => action$.pipe(
  filter(isOfType(AuthActionTypes.AUTHENTICATE)),
  withLatestFrom(state$),
  tap(([action,state]) => console.log('Authenticate action received.', action, state)),
  filter(([,state]) => !state.auth.inProgress),
  mergeMap(() => msalService.loginPopup(loginRequest)),
  // mergeMap(() => msalService.loginRedirect(loginRequest)),
  tap(resp => console.log('Response from login', resp)),
  mergeMap(resp => EMPTY)
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

export const authEpic = combineEpics(authenticateEpic, inProgressEpic, currentUserEpic);