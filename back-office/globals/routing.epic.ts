import router from "next/router";
import { combineEpics, Epic } from "redux-observable";
import { EMPTY } from "rxjs";
import { filter, map, mergeMap, tap } from "rxjs/operators";
import { isOfType } from "typesafe-actions";
import { AuthAction, AuthActionTypes } from "../store/auth/auth.actions";
import { RootState } from "../store/store";

const userUpdateRequestedEpic: Epic<AuthAction, AuthAction, RootState> = (action$, store$) => action$.pipe(
  filter(isOfType(AuthActionTypes.REQUEST_USER_UPDATE)),
  tap(action => router.push(`/profile`)),
  mergeMap(() => EMPTY)
);

export default userUpdateRequestedEpic;