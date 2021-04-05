import { ActionsObservable, combineEpics, Epic } from 'redux-observable';
import { Observable, of } from 'rxjs';
import { ajax } from 'rxjs/ajax';
import { catchError, filter, map, mergeMap, tap } from 'rxjs/operators';
import { isOfType } from 'typesafe-actions';

import { buildUrl } from '../../src/url-utils';
import { User } from '../auth/auth.actions';
import { withTokenAndUser$ } from '../auth/auth.epic';
import { RootState } from '../store';
import { loadRoles, loadRolesComplete, Role, RoleActionTypes, RolesAction } from './roles.actions';

const fetchRoles = (token: string): Observable<Role[]> => {
  return ajax.getJSON(buildUrl('/roles'), { Authorization: `Bearer ${token}` });
};

const fetchRole = (code: string, token: string): Observable<Role> => {
  return ajax.getJSON(buildUrl(`/roles/${code}`), {
    Authorization: `Bearer ${token}`,
  });
};

const loadRolesAction$ = (action$: ActionsObservable<RolesAction>) => action$.pipe(filter(isOfType(RoleActionTypes.LOAD_ROLES)));

const loadRoleAction$ = (action$: ActionsObservable<RolesAction>) => action$.pipe(filter(isOfType(RoleActionTypes.LOAD_ROLE)));

const loadRolesEpic: Epic<RolesAction, RolesAction, RootState> = (action$, state$) =>
  withTokenAndUser$(loadRolesAction$(action$), state$).pipe(
    tap(() => console.log('Loading roles in epic')),
    mergeMap(([, token]) => fetchRoles(token || '').pipe(map((resp) => loadRolesComplete(resp))))
  );

const loadRoleEpic: Epic<RolesAction, RolesAction, RootState> = (action$, state$) =>
  withTokenAndUser$(loadRoleAction$(action$), state$).pipe(
    tap(() => console.log('Loading organizations in epic')),
    mergeMap(([action, token]) => fetchRole(action.payload.code, token || '').pipe(map((resp) => loadRolesComplete([resp]))))
  );

export const rolesEpic = combineEpics(loadRolesEpic, loadRoleEpic);
