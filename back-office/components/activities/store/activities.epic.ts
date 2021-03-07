import { ActivitiesAction, loadActivitiesComplete } from "./activities.actions";
import { filter, mergeMap, map, tap } from "rxjs/operators";
import { Observable } from "rxjs";
import { combineEpics, Epic, ActionsObservable } from "redux-observable";
import { ajax } from "rxjs/ajax";
import { isOfType } from "typesafe-actions";
import { buildUrl } from "../../../src/url-utils";
import { Activity } from "../activity.models";
import { ActivityActionTypes } from "./activities.actions";
import { getView$ } from "../../../src/services/view-fetcher.service";
import { RootState } from "../../../store/store";
import { withToken$ } from "../../auth/store/auth.epic";

const fetchActivities = (token: string): Observable<Activity[]> => {
  return ajax.getJSON(buildUrl('/activities'), { 'Authorization': `Bearer ${token}` });
};

const loadActivitiesAction$ = (action$: ActionsObservable<ActivitiesAction>) => action$.pipe(
  filter(isOfType(ActivityActionTypes.LOAD_ACTIVITIES))
);

const loadActivityAction$ = (action$: ActionsObservable<ActivitiesAction>) => action$.pipe(
  filter(isOfType(ActivityActionTypes.LOAD_ACTIVITY))
);

const loadActivitiesEpic: Epic<ActivitiesAction, ActivitiesAction, RootState> = (action$, state$) =>
  withToken$(loadActivitiesAction$(action$), state$).pipe(
    tap(() => console.log('Loading activities in epic')),
    mergeMap(([, token]) => fetchActivities(token || '').pipe(
      map(resp => loadActivitiesComplete(resp))
    ))
  );


const loadActivityEpic: Epic<ActivitiesAction, ActivitiesAction, RootState> = (action$, state$) =>
  withToken$(loadActivityAction$(action$), state$).pipe(
    mergeMap(([action, token]) => getView$<Activity>(buildUrl(`/activities/${action.payload.activityId}`), { revision: action.payload.revision, token: token || '' }).pipe(
      map(view => loadActivitiesComplete([view]))
    ))
  );

export const activitiesEpic = combineEpics(loadActivitiesEpic, loadActivityEpic);