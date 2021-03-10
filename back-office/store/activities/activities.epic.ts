import { ActivitiesAction, activityCommandSent, activityCommandStatusSet, loadActivitiesComplete, loadActivity } from "./activities.actions";
import { filter, mergeMap, map, tap, catchError, take } from "rxjs/operators";
import { Observable, of } from "rxjs";
import { combineEpics, Epic, ActionsObservable } from "redux-observable";
import { ajax } from "rxjs/ajax";
import { isOfType } from "typesafe-actions";
import { buildUrl } from "../../src/url-utils";
import { Activity } from "../../components/activities/activity.models";
import { ActivityActionTypes } from "./activities.actions";
import { getView$ } from "../../src/services/view-fetcher.service";
import { RootState } from "../store";
import { withToken$ } from "../auth/auth.epic";
import { CommandLogSeverity, sendActivityCommand$, waitForCommandResult$ } from "../../src/services/activity-command.service";

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

const sendActivityCommandEpic: Epic<ActivitiesAction, ActivitiesAction, RootState> = (action$, state$) =>
  withToken$(action$.pipe(filter(isOfType(ActivityActionTypes.SEND_ACTIVITY_COMMAND))), state$).pipe(
    mergeMap(([action, token]) => sendActivityCommand$(action.payload.url, action.payload.body, { method: action.payload.options.method, token: token }).pipe(
      map(status => activityCommandSent(status.commandId, action.payload.uiId)),
      catchError((err) => {
        console.log(`Failed when sending command`, err);
        // TODO Report a generic failure instead of command failure?
        return of(activityCommandStatusSet({ commandId: '', status: "Failed", messages: [{ code: 'CommandSendFailed', message: String(err), severity: CommandLogSeverity.Error }] }, action.payload.uiId));
      })
    )),
  );

const waitForResultCommandEpic: Epic<ActivitiesAction, ActivitiesAction, RootState> = (action$, state$) =>
  withToken$(action$.pipe(filter(isOfType(ActivityActionTypes.ACTIVITY_COMMAND_SENT))), state$).pipe(
    mergeMap(([action, token]) => waitForCommandResult$(action.payload.commandId, token).pipe(
      map(status => activityCommandStatusSet(status, action.payload.uiId))
    )),
  );

const reloadOnCommandCompleteEpic: Epic<ActivitiesAction, ActivitiesAction, RootState> = (action$) =>
  action$.pipe(
    filter(isOfType(ActivityActionTypes.ACTIVITY_COMMAND_STATUS_SET)),
    filter((action) => action.payload.commandStatus.status === "Completed"),
    filter((action) => !!action.payload.commandStatus.aggregateId),
    map((action) => loadActivity(action.payload.commandStatus.aggregateId || '', action.payload.commandStatus.revision))
  );


export const activitiesEpic = combineEpics(loadActivitiesEpic, loadActivityEpic, sendActivityCommandEpic, waitForResultCommandEpic, reloadOnCommandCompleteEpic);