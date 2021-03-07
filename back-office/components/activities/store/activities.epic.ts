import { ActivitiesAction, activityCommandComplete, activityCommandFailed, activityCommandSent, loadActivitiesComplete, loadActivity, sendActivityCommand } from "./activities.actions";
import { filter, mergeMap, map, tap, catchError } from "rxjs/operators";
import { Observable, throwError } from "rxjs";
import { combineEpics, Epic, ActionsObservable } from "redux-observable";
import { ajax } from "rxjs/ajax";
import { isOfType } from "typesafe-actions";
import { buildUrl } from "../../../src/url-utils";
import { Activity } from "../activity.models";
import { ActivityActionTypes } from "./activities.actions";
import { getView$ } from "../../../src/services/view-fetcher.service";
import { RootState } from "../../../store/store";
import { withToken$ } from "../../auth/store/auth.epic";
import { sendActivityCommand$, waitForCommandResult$ } from "../../../src/services/activity-command.service";

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
    mergeMap(([action, token]) => sendActivityCommand$(action.payload.commandName, action.payload.body, { method: action.payload.options.method, token: token })),
    map(status => activityCommandSent(status.commandId)),
    catchError((err) => {
      console.log(`Failed when sending command`, err);
      return throwError(err);
    })
  );

const waitForResultCommandEpic: Epic<ActivitiesAction, ActivitiesAction, RootState> = (action$, state$) =>
  withToken$(action$.pipe(filter(isOfType(ActivityActionTypes.ACTIVITY_COMMAND_SENT))), state$).pipe(
    mergeMap(([action, token]) => waitForCommandResult$(action.payload.commandId, token)),
    map(status => {
      switch (status.status) {
        case 'Completed': return activityCommandComplete(status.commandId, status.aggregateId, status.revision);
        default: {
          const message = status.messages && status.messages[0];
          return activityCommandFailed(status.commandId, message?.code || 'UnknownCommandFailure', message?.message || 'An unknown error occurred when sending command. Please update the page.');
        }
      }
    })
  );


const reloadOnCommandCompleteEpic: Epic<ActivitiesAction, ActivitiesAction, RootState> = (action$) =>
  action$.pipe(
    filter(isOfType(ActivityActionTypes.ACTIVITY_COMMAND_COMPLETE)),
    filter((action) => !!action.payload.activityId),
    map((action) => loadActivity(action.payload.activityId || '', action.payload.revision))
  );


export const activitiesEpic = combineEpics(loadActivitiesEpic, loadActivityEpic, sendActivityCommandEpic, waitForResultCommandEpic, reloadOnCommandCompleteEpic);