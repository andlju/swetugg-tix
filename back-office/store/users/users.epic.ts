import { combineEpics, Epic } from 'redux-observable';
import { Observable, of } from 'rxjs';
import { ajax } from 'rxjs/ajax';
import { catchError, filter, map, mergeMap, tap } from 'rxjs/operators';
import { isOfType } from 'typesafe-actions';

import { buildUrl } from '../../src/url-utils';
import { withTokenAndUser$ } from '../auth/auth.epic';
import { RootState } from '../store';
import {
  addUserRoleComplete,
  addUserRoleFailed,
  loadUserRolesComplete,
  UserAction,
  UserRole,
  UsersActionTypes,
} from './users.actions';

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

const addUserRoleEpic: Epic<UserAction, UserAction, RootState> = (action$, state$) =>
  withTokenAndUser$(action$.pipe(filter(isOfType(UsersActionTypes.ADD_USER_ROLE))), state$).pipe(
    mergeMap(([action, token]) =>
      ajax
        .post(buildUrl(`/users/${action.payload.userId}/roles`), action.payload.userRole, {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        })
        .pipe(
          map((a) => addUserRoleComplete()),
          catchError((err, caught) => {
            const errString = String(err);
            return of(addUserRoleFailed('AddUserRoleFailed', errString));
          })
        )
    )
  );

export const usersEpic = combineEpics(loadRolesEpic, addUserRoleEpic);
