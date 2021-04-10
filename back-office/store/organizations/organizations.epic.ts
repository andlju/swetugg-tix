import { ActionsObservable, combineEpics, Epic } from 'redux-observable';
import { Observable, of } from 'rxjs';
import { ajax } from 'rxjs/ajax';
import { catchError, filter, map, mergeMap, tap } from 'rxjs/operators';
import { isOfType } from 'typesafe-actions';

import { buildUrl } from '../../src/url-utils';
import { User } from '../auth/auth.actions';
import { withTokenAndUser$ } from '../auth/auth.epic';
import { RootState } from '../store';
import {
  acceptOrganizationInviteComplete,
  acceptOrganizationInviteFailed,
  createOrganizationComplete,
  createOrganizationFailed,
  createOrganizationInviteComplete,
  createOrganizationInviteFailed,
  loadOrganizationInviteComplete,
  loadOrganizations,
  loadOrganizationsComplete,
  loadOrganizationUsersComplete,
  Organization,
  OrganizationActionTypes,
  OrganizationInvite,
  OrganizationsAction,
} from './organizations.actions';

const fetchOrganizations = (token: string): Observable<Organization[]> => {
  return ajax.getJSON(buildUrl('/organizations'), { Authorization: `Bearer ${token}` });
};

const fetchOrganization = (organizationId: string, token: string): Observable<Organization> => {
  return ajax.getJSON(buildUrl(`/organizations/${organizationId}`), {
    Authorization: `Bearer ${token}`,
  });
};

const loadOrganizationsAction$ = (action$: ActionsObservable<OrganizationsAction>) =>
  action$.pipe(filter(isOfType(OrganizationActionTypes.LOAD_ORGANIZATIONS)));

const loadOrganizationAction$ = (action$: ActionsObservable<OrganizationsAction>) =>
  action$.pipe(filter(isOfType(OrganizationActionTypes.LOAD_ORGANIZATION)));

const loadOrganizationsEpic: Epic<OrganizationsAction, OrganizationsAction, RootState> = (action$, state$) =>
  withTokenAndUser$(loadOrganizationsAction$(action$), state$).pipe(
    tap(() => console.log('Loading organizations in epic')),
    mergeMap(([, token]) => fetchOrganizations(token || '').pipe(map((resp) => loadOrganizationsComplete(resp))))
  );

const loadOrganizationEpic: Epic<OrganizationsAction, OrganizationsAction, RootState> = (action$, state$) =>
  withTokenAndUser$(loadOrganizationAction$(action$), state$).pipe(
    tap(() => console.log('Loading organizations in epic')),
    mergeMap(([action, token]) =>
      fetchOrganization(action.payload.organizationId, token || '').pipe(map((resp) => loadOrganizationsComplete([resp])))
    )
  );

const fetchOrganizationUsers = (organizationId: string, token: string): Observable<User[]> => {
  return ajax.getJSON(buildUrl(`/organizations/${organizationId}/users`), { Authorization: `Bearer ${token}` });
};

const loadOrganizationUsersAction$ = (action$: ActionsObservable<OrganizationsAction>) =>
  action$.pipe(filter(isOfType(OrganizationActionTypes.LOAD_ORGANIZATION_USERS)));

const loadOrganizationUsersEpic: Epic<OrganizationsAction, OrganizationsAction, RootState> = (action$, state$) =>
  withTokenAndUser$(loadOrganizationUsersAction$(action$), state$).pipe(
    tap(() => console.log('Loading organization users in epic')),
    mergeMap(([action, token]) =>
      fetchOrganizationUsers(action.payload.organizationId, token || '').pipe(map((resp) => loadOrganizationUsersComplete(resp)))
    )
  );

const fetchOrganizationInvite = (inviteToken: string, token: string): Observable<OrganizationInvite> => {
  return ajax.getJSON(buildUrl(`/organizations/invite?token=${inviteToken}`), { Authorization: `Bearer ${token}` });
};

const loadOrganizationInviteAction$ = (action$: ActionsObservable<OrganizationsAction>) =>
  action$.pipe(filter(isOfType(OrganizationActionTypes.LOAD_ORGANIZATION_INVITE)));

const loadOrganizationInviteEpic: Epic<OrganizationsAction, OrganizationsAction, RootState> = (action$, state$) =>
  withTokenAndUser$(loadOrganizationInviteAction$(action$), state$).pipe(
    tap(() => console.log('Loading organization users in epic')),
    mergeMap(([action, token]) =>
      fetchOrganizationInvite(action.payload.token, token || '').pipe(map((resp) => loadOrganizationInviteComplete(resp)))
    )
  );

const createOrganizationEpic: Epic<OrganizationsAction, OrganizationsAction, RootState> = (action$, state$) =>
  withTokenAndUser$(action$.pipe(filter(isOfType(OrganizationActionTypes.CREATE_ORGANIZATION))), state$).pipe(
    mergeMap(([action, token]) =>
      ajax
        .post(buildUrl(`/organizations`), action.payload, {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        })
        .pipe(
          map((a) => createOrganizationComplete()),
          catchError((err, caught) => {
            const errString = String(err);
            return of(createOrganizationFailed('CreateOrganizationFailed', errString));
          })
        )
    )
  );

const createOrganizationInviteEpic: Epic<OrganizationsAction, OrganizationsAction, RootState> = (action$, state$) =>
  withTokenAndUser$(action$.pipe(filter(isOfType(OrganizationActionTypes.CREATE_ORGANIZATION_INVITE))), state$).pipe(
    mergeMap(([action, token]) =>
      ajax
        .post(
          buildUrl(`/organizations/${action.payload.organizationId}/invite`),
          {},
          { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` }
        )
        .pipe(
          map((a) => createOrganizationInviteComplete(a.response.token)),
          catchError((err, caught) => {
            const errString = String(err);
            return of(createOrganizationInviteFailed('CreateOrganizationInviteFailed', errString));
          })
        )
    )
  );

const acceptOrganizationInviteEpic: Epic<OrganizationsAction, OrganizationsAction, RootState> = (action$, state$) =>
  withTokenAndUser$(action$.pipe(filter(isOfType(OrganizationActionTypes.ACCEPT_ORGANIZATION_INVITE))), state$).pipe(
    mergeMap(([action, token]) =>
      ajax
        .post(
          buildUrl(`/organizations/invite/accept`),
          {
            token: action.payload.token,
          },
          { Authorization: `Bearer ${token}` }
        )
        .pipe(
          map((a) => acceptOrganizationInviteComplete()),
          catchError((err, caught) => {
            const errString = String(err);
            return of(acceptOrganizationInviteFailed('AcceptOrganizationInviteFailed', errString));
          })
        )
    )
  );

const reloadOnCreateComplete: Epic<OrganizationsAction, OrganizationsAction, RootState> = (action$) =>
  action$.pipe(
    filter(isOfType(OrganizationActionTypes.CREATE_ORGANIZATION_COMPLETE)),
    map((action) => loadOrganizations())
  );

export const organizationsEpic = combineEpics(
  loadOrganizationsEpic,
  loadOrganizationEpic,
  loadOrganizationUsersEpic,
  loadOrganizationInviteEpic,
  createOrganizationEpic,
  createOrganizationInviteEpic,
  acceptOrganizationInviteEpic,
  reloadOnCreateComplete
);
