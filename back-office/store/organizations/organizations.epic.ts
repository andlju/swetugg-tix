import { ActionsObservable, combineEpics, Epic } from 'redux-observable';
import { Observable, of } from 'rxjs';
import { ajax } from 'rxjs/ajax';
import { catchError, filter, map, mergeMap, tap } from 'rxjs/operators';
import { isOfType } from 'typesafe-actions';

import { buildUrl } from '../../src/url-utils';
import { withTokenAndUser$ } from '../auth/auth.epic';
import { RootState } from '../store';
import {
  createOrganizationComplete,
  createOrganizationFailed,
  createOrganizationInviteComplete,
  createOrganizationInviteFailed,
  loadOrganizations,
  loadOrganizationsComplete,
  Organization,
  OrganizationActionTypes,
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

const loadOrganizationsEpic: Epic<OrganizationsAction, OrganizationsAction, RootState> = (
  action$,
  state$
) =>
  withTokenAndUser$(loadOrganizationsAction$(action$), state$).pipe(
    tap(() => console.log('Loading organizations in epic')),
    mergeMap(([, token]) =>
      fetchOrganizations(token || '').pipe(map((resp) => loadOrganizationsComplete(resp)))
    )
  );

const loadOrganizationEpic: Epic<OrganizationsAction, OrganizationsAction, RootState> = (
  action$,
  state$
) =>
  withTokenAndUser$(loadOrganizationAction$(action$), state$).pipe(
    tap(() => console.log('Loading organizations in epic')),
    mergeMap(([action, token]) =>
      fetchOrganization(action.payload.organizationId, token || '').pipe(
        map((resp) => loadOrganizationsComplete([resp]))
      )
    )
  );

const createOrganizationEpic: Epic<OrganizationsAction, OrganizationsAction, RootState> = (
  action$,
  state$
) =>
  withTokenAndUser$(
    action$.pipe(filter(isOfType(OrganizationActionTypes.CREATE_ORGANIZATION))),
    state$
  ).pipe(
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

const createOrganizationInviteEpic: Epic<OrganizationsAction, OrganizationsAction, RootState> = (
  action$,
  state$
) =>
  withTokenAndUser$(
    action$.pipe(filter(isOfType(OrganizationActionTypes.CREATE_ORGANIZATION_INVITE))),
    state$
  ).pipe(
    mergeMap(([action, token]) =>
      ajax
        .post(
          buildUrl(`/organizations/${action.payload.organizationId}/invite`),
          {},
          { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` }
        )
        .pipe(
          map((a) => createOrganizationInviteComplete(a.response)),
          catchError((err, caught) => {
            const errString = String(err);
            return of(createOrganizationInviteFailed('CreateOrganizationInviteFailed', errString));
          })
        )
    )
  );

const reloadOnCreateComplete: Epic<OrganizationsAction, OrganizationsAction, RootState> = (
  action$
) =>
  action$.pipe(
    filter(isOfType(OrganizationActionTypes.CREATE_ORGANIZATION_COMPLETE)),
    map((action) => loadOrganizations())
  );

export const organizationsEpic = combineEpics(
  loadOrganizationsEpic,
  loadOrganizationEpic,
  createOrganizationEpic,
  createOrganizationInviteEpic,
  reloadOnCreateComplete
);
