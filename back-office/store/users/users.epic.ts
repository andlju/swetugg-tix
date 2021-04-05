import { combineEpics, Epic } from 'redux-observable';
import { Observable } from 'rxjs';
import { ajax } from 'rxjs/ajax';
import { filter, map, mergeMap, tap } from 'rxjs/operators';
import { isOfType } from 'typesafe-actions';

import { buildUrl } from '../../src/url-utils';
import { withTokenAndUser$ } from '../auth/auth.epic';
import { RootState } from '../store';
import { loadUserRolesComplete, UserAction, UserRole, UsersActionTypes } from './users.actions';

const fetchUserRoles = (token: string, userId: string, organizationId?: string): Observable<UserRole[]> => {
  return ajax.getJSON(buildUrl(`/users/${userId}/roles?organizationId=${organizationId}`), { Authorization: `Bearer ${token}` });
};

const loadUserRolesAction$ = (action$: Observable<UserAction>) =>
  action$.pipe(filter(isOfType(UsersActionTypes.LOAD_USER_ROLES)));

const loadRolesEpic: Epic<UserAction, UserAction, RootState> = (action$, state$) =>
  withTokenAndUser$(loadUserRolesAction$(action$), state$).pipe(
    tap(() => console.log('Loading user roles in epic')),
    mergeMap(([action, token]) =>
      fetchUserRoles(token || '', action.payload.userId, action.payload.organizationId).pipe(
        map((resp) => loadUserRolesComplete(resp))
      )
    )
  );

export const usersEpic = combineEpics(loadRolesEpic);
