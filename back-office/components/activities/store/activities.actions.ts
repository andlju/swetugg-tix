import { Activity } from "../activity.models";

export const LOAD_ACTIVITY = 'LOAD_ACTIVITY';
export const LOAD_ACTIVITIES = 'LOAD_ACTIVITIES';
export const LOAD_ACTIVITIES_COMPLETE = 'LOAD_ACTIVITIES_COMPLETE';
export const LOAD_ACTIVITIES_FAILED = 'LOAD_ACTIVITIES_FAILED';

export function loadActivities(token?: string) : LoadAllActivitiesAction {
  return {
    type: LOAD_ACTIVITIES,
    payload: {
      token: token
    }
  }
}

export function loadActivity(activityId: string, token: string, revision?: number) : LoadActivityAction {
  return {
    type: LOAD_ACTIVITY,
    payload: {
      token: token,
      activityId: activityId,
      revision: revision
    }
  }
}

export interface LoadActivityAction {
  type: typeof LOAD_ACTIVITY;
  payload: { 
    token: string,
    activityId: string,
    revision?: number
  }
}

export interface LoadAllActivitiesAction {
  type: typeof LOAD_ACTIVITIES;
  payload: {
    token?: string
  }
}

export interface LoadActivitiesCompleteAction {
  type: typeof LOAD_ACTIVITIES_COMPLETE;
  payload: {
    activities: Activity[];
  };
}

export interface LoadActivitiesFailedAction {
  type: typeof LOAD_ACTIVITIES_FAILED;
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