import { ActionsObservable, combineEpics, Epic } from 'redux-observable';
import { Observable, of } from 'rxjs';
import { ajax } from 'rxjs/ajax';
import { catchError, filter, map, mergeMap, take, tap } from 'rxjs/operators';
import { isOfType } from 'typesafe-actions';

import { CommandLogSeverity, sendActivityCommand$, waitForCommandResult$ } from '../../src/services/activity-command.service';
import { getView$ } from '../../src/services/view-fetcher.service';
import { buildUrl } from '../../src/url-utils';
import { withToken$, withTokenAndUser$ } from '../auth/auth.epic';
import { RootState } from '../store';
import {
  ActivitiesAction,
  activityCommandSent,
  activityCommandStatusSet,
  loadActivitiesComplete,
  loadActivity,
} from './activities.actions';
import { ActivityActionTypes } from './activities.actions';
import { Activity } from './activity.models';

const fetchActivities = (token: string): Observable<Activity[]> => {
  return ajax.getJSON(buildUrl('/activities'), { Authorization: `Bearer ${token}` });
};

const loadActivitiesAction$ = (action$: ActionsObservable<ActivitiesAction>) =>
  action$.pipe(filter(isOfType(ActivityActionTypes.LOAD_ACTIVITIES)));

const loadActivityAction$ = (action$: ActionsObservable<ActivitiesAction>) =>
  action$.pipe(filter(isOfType(ActivityActionTypes.LOAD_ACTIVITY)));

const loadActivitiesEpic: Epic<ActivitiesAction, ActivitiesAction, RootState> = (action$, state$) =>
  withTokenAndUser$(loadActivitiesAction$(action$), state$).pipe(
    tap(() => console.log('Loading activities in epic')),
    mergeMap(([, token]) => fetchActivities(token || '').pipe(map((resp) => loadActivitiesComplete(resp))))
  );

const loadActivityEpic: Epic<ActivitiesAction, ActivitiesAction, RootState> = (action$, state$) =>
  withTokenAndUser$(loadActivityAction$(action$), state$).pipe(
    mergeMap(([action, token]) =>
      getView$<Activity>(buildUrl(`/activities/${action.payload.activityId}?ownerId=${action.payload.ownerId}`), {
        revision: action.payload.revision,
        token: token || '',
      }).pipe(map((view) => loadActivitiesComplete([view])))
    )
  );

const sendActivityCommandEpic: Epic<ActivitiesAction, ActivitiesAction, RootState> = (action$, state$) =>
  withTokenAndUser$(action$.pipe(filter(isOfType(ActivityActionTypes.SEND_ACTIVITY_COMMAND))), state$).pipe(
    tap(([action]) => console.log(`Sending command to ${action.payload.url}`)),
    mergeMap(([action, token]) =>
      sendActivityCommand$(action.payload.url, action.payload.body, {
        method: action.payload.options.method,
        token: token,
      }).pipe(
        map((status) => activityCommandSent(status.commandId, action.payload.uiId)),
        catchError((err) => {
          console.log(`Failed when sending command`, err);
          // TODO Report a generic failure instead of command failure?
          return of(
            activityCommandStatusSet(
              {
                commandId: '',
                status: 'Failed',
                messages: [
                  {
                    code: 'CommandSendFailed',
                    message: String(err),
                    severity: CommandLogSeverity.Error,
                  },
                ],
              },
              action.payload.uiId
            )
          );
        })
      )
    )
  );

const waitForResultCommandEpic: Epic<ActivitiesAction, ActivitiesAction, RootState> = (action$, state$) =>
  withToken$(action$.pipe(filter(isOfType(ActivityActionTypes.ACTIVITY_COMMAND_SENT))), state$).pipe(
    mergeMap(([action, token]) =>
      waitForCommandResult$(action.payload.commandId, token).pipe(
        map((status) => activityCommandStatusSet(status, action.payload.uiId))
      )
    )
  );

const reloadOnCommandCompleteEpic: Epic<ActivitiesAction, ActivitiesAction, RootState> = (action$) =>
  action$.pipe(
    filter(isOfType(ActivityActionTypes.ACTIVITY_COMMAND_STATUS_SET)),
    filter((action) => action.payload.commandStatus.status === 'Completed'),
    filter((action) => !!action.payload.commandStatus.aggregateId),
    map((action) =>
      loadActivity(
        action.payload.commandStatus.aggregateId || '',
        (action.payload.commandStatus.body as any).ownerId,
        action.payload.commandStatus.revision
      )
    )
  );

export const activitiesEpic = combineEpics(
  loadActivitiesEpic,
  loadActivityEpic,
  sendActivityCommandEpic,
  waitForResultCommandEpic,
  reloadOnCommandCompleteEpic
);
