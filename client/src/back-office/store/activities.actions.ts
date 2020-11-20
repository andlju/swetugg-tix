import { Activity } from "..";

export const LOAD_ACTIVITY = 'LOAD_ACTIVITY';
export const LOAD_ACTIVITIES = 'LOAD_ACTIVITIES';
export const LOAD_ACTIVITIES_COMPLETE = 'LOAD_ACTIVITIES_COMPLETE';
export const LOAD_ACTIVITIES_FAILED = 'LOAD_ACTIVITIES_FAILED';

export interface LoadActivityAction {
  type: typeof LOAD_ACTIVITY;
  payload: { 
    activityId: string
  }
}

export interface LoadAllActivitiesAction {
  type: typeof LOAD_ACTIVITIES;
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
