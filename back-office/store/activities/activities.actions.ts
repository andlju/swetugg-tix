import { Action } from 'redux';

import { CommandStatus } from '../../src/services/activity-command.service';
import { Activity } from './activity.models';

export enum ActivityActionTypes {
  LOAD_ACTIVITY = 'LOAD_ACTIVITY',
  LOAD_ACTIVITIES = 'LOAD_ACTIVITIES',
  LOAD_ACTIVITIES_COMPLETE = 'LOAD_ACTIVITIES_COMPLETE',
  LOAD_ACTIVITIES_FAILED = 'LOAD_ACTIVITIES_FAILED',

  SEND_ACTIVITY_COMMAND = 'SEND_ACTIVITY_COMMAND',

  ACTIVITY_COMMAND_SENT = 'ACTIVITY_COMMAND_SENT',
  ACTIVITY_COMMAND_STATUS_SET = 'ACTIVITY_COMMAND_STATUS_SET',
}

export function loadActivities(): LoadAllActivitiesAction {
  return {
    type: ActivityActionTypes.LOAD_ACTIVITIES,
  };
}

export function loadActivity(
  activityId: string,
  ownerId: string,
  revision?: number
): LoadActivityAction {
  return {
    type: ActivityActionTypes.LOAD_ACTIVITY,
    payload: {
      activityId: activityId,
      ownerId: ownerId,
      revision: revision,
    },
  };
}

export function loadActivitiesComplete(activities: Activity[]): LoadActivitiesCompleteAction {
  return {
    type: ActivityActionTypes.LOAD_ACTIVITIES_COMPLETE,
    payload: {
      activities: activities,
    },
  };
}

export interface CommandOptions {
  method?: string;
}

let nextTempCommandId = 0;

export function sendActivityCommand<TBody>(
  url: string,
  body?: TBody,
  options?: CommandOptions
): SendActivityCommandAction {
  return {
    type: ActivityActionTypes.SEND_ACTIVITY_COMMAND,
    payload: {
      uiId: `TMP_CMD_${nextTempCommandId++}`,
      url,
      body,
      options: options || { method: 'POST' },
    },
  };
}

export function activityCommandSent(commandId: string, uiId?: string): ActivityCommandSentAction {
  return {
    type: ActivityActionTypes.ACTIVITY_COMMAND_SENT,
    payload: {
      uiId,
      commandId,
    },
  };
}

export function activityCommandStatusSet(
  commandStatus: CommandStatus,
  uiId?: string
): ActivityCommandStatusSetAction {
  return {
    type: ActivityActionTypes.ACTIVITY_COMMAND_STATUS_SET,
    payload: {
      uiId,
      commandStatus,
    },
  };
}

export interface LoadActivityAction extends Action {
  type: ActivityActionTypes.LOAD_ACTIVITY;
  payload: {
    activityId: string;
    ownerId: string;
    revision?: number;
  };
}

export interface LoadAllActivitiesAction extends Action {
  type: ActivityActionTypes.LOAD_ACTIVITIES;
}

export interface LoadActivitiesCompleteAction extends Action {
  type: ActivityActionTypes.LOAD_ACTIVITIES_COMPLETE;
  payload: {
    activities: Activity[];
  };
}

export interface LoadActivitiesFailedAction extends Action {
  type: ActivityActionTypes.LOAD_ACTIVITIES_FAILED;
  payload: {
    errorCode: string;
    errorMessage: string;
  };
}

export interface SendActivityCommandAction extends Action {
  type: ActivityActionTypes.SEND_ACTIVITY_COMMAND;
  payload: {
    uiId: string;
    url: string;
    body?: unknown;
    options: CommandOptions;
  };
}

export interface ActivityCommandSentAction extends Action {
  type: ActivityActionTypes.ACTIVITY_COMMAND_SENT;
  payload: {
    commandId: string;
    uiId?: string;
  };
}

export interface ActivityCommandStatusSetAction extends Action {
  type: ActivityActionTypes.ACTIVITY_COMMAND_STATUS_SET;
  payload: {
    commandStatus: CommandStatus;
    uiId?: string;
  };
}

export type ActivitiesAction =
  | LoadActivityAction
  | LoadAllActivitiesAction
  | LoadActivitiesCompleteAction
  | LoadActivitiesFailedAction
  | SendActivityCommandAction
  | ActivityCommandSentAction
  | ActivityCommandStatusSetAction;
