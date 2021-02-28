import { ActivitiesAction, loadActivitiesComplete } from "./activities.actions";
import { filter, mergeMap, map, distinctUntilChanged, tap } from "rxjs/operators";
import { Observable, combineLatest } from "rxjs";
import { combineEpics, Epic, ActionsObservable, StateObservable } from "redux-observable";
import { ajax } from "rxjs/ajax";
import { isOfType } from "typesafe-actions";
import { buildUrl } from "../../../src/url-utils";
import { Activity } from "../activity.models";
import { ActivityActionTypes } from "./activities.actions";
import { getView$ } from "../../../src/services/view-fetcher.service";
import { RootState } from "../../../store/store";

const fetchActivities = (token: string): Observable<Activity[]> => {
  return ajax.getJSON(buildUrl('/activities'), { 'Authorization': `Bearer ${token}` });
};

const loadActivitiesAction$ = (action$: ActionsObservable<ActivitiesAction>) => action$.pipe(
  filter(isOfType(ActivityActionTypes.LOAD_ACTIVITIES))
);

const loadActivityAction$ = (action$: ActionsObservable<ActivitiesAction>) => action$.pipe(
  filter(isOfType(ActivityActionTypes.LOAD_ACTIVITY))
);

const getAccessToken$ = (state$: StateObservable<RootState>) => state$.pipe(
  map(state => state.auth.token),
  distinctUntilChanged()
);

const loadActivitiesEpic: Epic<ActivitiesAction, ActivitiesAction, RootState> = (action$, state$) =>
  combineLatest([loadActivitiesAction$(action$), getAccessToken$(state$)]).pipe(
    filter(([, token]) => !!token),
    mergeMap(([, token]) => fetchActivities(token || '').pipe(
      map(resp => loadActivitiesComplete(resp))
    ))
  );

const loadActivityEpic: Epic<ActivitiesAction, ActivitiesAction, RootState> = (action$, state$) =>
  combineLatest([loadActivityAction$(action$), getAccessToken$(state$)]).pipe(
    filter(([, token]) => !!token),
    mergeMap(([action, token]) => getView$<Activity>(buildUrl(`/activities/${action.payload.activityId}`), { revision: action.payload.revision, token: token || '' }).pipe(
      map(view => loadActivitiesComplete([view]))
    ))
  );

export const activitiesEpic = combineEpics(loadActivitiesEpic, loadActivityEpic);