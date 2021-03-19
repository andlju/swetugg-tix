import { Observable } from "rxjs";
import { ActionsObservable, combineEpics, Epic } from "redux-observable";
import { filter, map, mergeMap, tap } from "rxjs/operators";
import { ajax } from "rxjs/ajax";
import { createOrganizationComplete, loadOrganizationsComplete, Organization, OrganizationActionTypes, OrganizationsAction } from "./organizations.actions";
import { buildUrl } from "../../src/url-utils";
import { isOfType } from "typesafe-actions";
import { withTokenAndUser$ } from "../auth/auth.epic";
import { RootState } from "../store";

const fetchOrganizations = (token: string): Observable<Organization[]> => {
  return ajax.getJSON(buildUrl('/organizations'), { 'Authorization': `Bearer ${token}` });
};

const loadOrganizationsAction$ = (action$: ActionsObservable<OrganizationsAction>) => action$.pipe(
  filter(isOfType(OrganizationActionTypes.LOAD_ORGANIZATIONS))
);


const loadOrganizationsEpic: Epic<OrganizationsAction, OrganizationsAction, RootState> = (action$, state$) =>
  withTokenAndUser$(loadOrganizationsAction$(action$), state$).pipe(
    tap(() => console.log('Loading organizations in epic')),
    mergeMap(([, token]) => fetchOrganizations(token || '').pipe(
      map(resp => loadOrganizationsComplete(resp))
    ))
  );

const createOrganizationEpic: Epic<OrganizationsAction, OrganizationsAction, RootState> = (action$, state$) =>
  withTokenAndUser$(action$.pipe(filter(isOfType(OrganizationActionTypes.CREATE_ORGANIZATION))), state$).pipe(
    mergeMap(([action, token]) => ajax.post(buildUrl(`/organizations`), action.payload, { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` })),
    map(a => createOrganizationComplete())
  );

export const organizationsEpic = combineEpics(loadOrganizationsEpic, createOrganizationEpic);