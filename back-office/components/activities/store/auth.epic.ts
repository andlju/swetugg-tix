import { combineEpics, Epic } from "redux-observable";
import { EMPTY } from "rxjs";
import { filter, mergeMap, tap } from "rxjs/operators";
import { isOfType } from "typesafe-actions";
import { AuthAction, AuthActionTypes } from "./auth.actions";


const authenticateEpic : Epic<AuthAction, AuthAction> = (action$) => action$.pipe(
  filter(isOfType(AuthActionTypes.AUTHENTICATE)),
  tap(action => console.log('Authenticate action received.', action)),
  mergeMap(() => EMPTY)
);

export const authEpic = combineEpics(authenticateEpic);