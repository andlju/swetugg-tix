import { Action } from "redux";
import { Activity } from "../activity.models";

export enum ActivityActionTypes {
  LOAD_ACTIVITY = 'LOAD_ACTIVITY',
  LOAD_ACTIVITIES = 'LOAD_ACTIVITIES',
  LOAD_ACTIVITIES_COMPLETE = 'LOAD_ACTIVITIES_COMPLETE',
  LOAD_ACTIVITIES_FAILED = 'LOAD_ACTIVITIES_FAILED',
  
  SEND_ACTIVITY_COMMAND = 'SEND_ACTIVITY_COMMAND',

  ACTIVITY_COMMAND_SENT = 'ACTIVITY_COMMAND_SENT',
  ACTIVITY_COMMAND_COMPLETE = 'ACTIVITY_COMMAND_COMPLETE',
  ACTIVITY_COMMAND_FAILED = 'ACTIVITY_COMMAND_FAILED',

}

export function loadActivities() : LoadAllActivitiesAction {
  return {
    type: ActivityActionTypes.LOAD_ACTIVITIES,
  }
}

export function loadActivity(activityId: string, revision?: number) : LoadActivityAction {
  return {
    type: ActivityActionTypes.LOAD_ACTIVITY,
    payload: {
      activityId: activityId,
      revision: revision
    }
  }
}

export function loadActivitiesComplete(activities: Activity[]) : LoadActivitiesCompleteAction {
  return {
    type: ActivityActionTypes.LOAD_ACTIVITIES_COMPLETE,
    payload: {
      activities: activities
    }
  };
}

interface CommandOptions {
  method?: string;
}

export function sendActivityCommand<TBody>(commandName: string, body?: TBody, options?: CommandOptions) : SendActivityCommandAction {
  return {
    type: ActivityActionTypes.SEND_ACTIVITY_COMMAND,
    payload: {
      commandName,
      body,
      options: options || { method: "POST" }
    }
  }
}

export function activityCommandSent(commandId: string) : ActivityCommandSentAction {
  return {
    type: ActivityActionTypes.ACTIVITY_COMMAND_SENT,
    payload: {
      commandId
    }
  }
}

export function activityCommandComplete(commandId: string, activityId?: string, revision?: number) : ActivityCommandCompleteAction {
  return {
    type: ActivityActionTypes.ACTIVITY_COMMAND_COMPLETE,
    payload: {
      commandId,
      activityId,
      revision
    }
  }
}

export function activityCommandFailed(commandId: string, errorCode: string, errorMessage: string) : ActivityCommandFailedAction {
  return {
    type: ActivityActionTypes.ACTIVITY_COMMAND_FAILED,
    payload: {
      commandId,
      errorCode,
      errorMessage
    }
  }
}

export interface LoadActivityAction extends Action {
  type: ActivityActionTypes.LOAD_ACTIVITY;
  payload: { 
    activityId: string,
    revision?: number
  }
}

export interface LoadAllActivitiesAction extends Action {
  type: ActivityActionTypes.LOAD_ACTIVITIES
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
    commandName: string;
    body?: unknown;
    options: CommandOptions;
  }
}

export interface ActivityCommandSentAction extends Action {
  type: ActivityActionTypes.ACTIVITY_COMMAND_SENT;
  payload: {
    commandId: string;
  }
}

export interface ActivityCommandCompleteAction extends Action {
  type: ActivityActionTypes.ACTIVITY_COMMAND_COMPLETE;
  payload: {
    commandId: string;
    activityId?: string;
    revision?: number
  }
}

export interface ActivityCommandFailedAction extends Action {
  type: ActivityActionTypes.ACTIVITY_COMMAND_FAILED;
  payload: {
    commandId: string;
    errorCode: string;
    errorMessage: string;
  }
}

export type ActivitiesAction =
  | LoadActivityAction
  | LoadAllActivitiesAction
  | LoadActivitiesCompleteAction
  | LoadActivitiesFailedAction
  | SendActivityCommandAction
  | ActivityCommandSentAction
  | ActivityCommandCompleteAction
  | ActivityCommandFailedAction;