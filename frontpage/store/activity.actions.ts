import { Activity } from "../components/activity/activity.models";

export const LOAD_ACTIVITY = 'LOAD_ACTIVITY';
export const LOAD_ACTIVITY_COMPLETE = 'LOAD_ACTIVITY_COMPLETE';
export const LOAD_ACTIVITY_FAILED = 'LOAD_ACTIVITY_FAILED';

export interface LoadActivityAction {
  type: typeof LOAD_ACTIVITY;
  payload: { 
    activityId: string,
    revision?: number
  }
}

export interface LoadActivityCompleteAction {
  type: typeof LOAD_ACTIVITY_COMPLETE;
  payload: {
    activity: Activity;
  };
}

export interface LoadActivityFailedAction {
  type: typeof LOAD_ACTIVITY_FAILED;
  payload: {
    errorCode: string;
    errorMessage: string;
  };
}

export type ActivityAction =
  | LoadActivityAction
  | LoadActivityCompleteAction
  | LoadActivityFailedAction;
