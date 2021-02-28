import { Action } from "redux";
import { Activity } from "../activity.models";

export enum ActivityActionTypes {
  LOAD_ACTIVITY = 'LOAD_ACTIVITY',
  LOAD_ACTIVITIES = 'LOAD_ACTIVITIES',
  LOAD_ACTIVITIES_COMPLETE = 'LOAD_ACTIVITIES_COMPLETE',
  LOAD_ACTIVITIES_FAILED = 'LOAD_ACTIVITIES_FAILED'
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

export type ActivitiesAction =
  | LoadActivityAction
  | LoadAllActivitiesAction
  | LoadActivitiesCompleteAction
  | LoadActivitiesFailedAction;