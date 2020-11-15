import { Activity } from "..";

export const SET_ACTIVITY_ID = 'SET_ACTIVITY_ID';
export const LOAD_ACTIVITIES = 'LOAD_ACTIVITIES';
export const LOAD_ACTIVITIES_COMPLETE = 'LOAD_ACTIVITIES_COMPLETE';
export const LOAD_ACTIVITIES_FAILED = 'LOAD_ACTIVITIES_FAILED';

interface LoadAllActivitiesAction {
  type: typeof LOAD_ACTIVITIES;
}

interface LoadActivitiesCompleteAction {
  type: typeof LOAD_ACTIVITIES_COMPLETE;
  payload: {
    activities: Activity[];
  };
}

interface LoadActivitiesFailedAction {
  type: typeof LOAD_ACTIVITIES_FAILED;
  payload: {
    errorCode: string;
    errorMessage: string;
  };
}

export type ActivitiesAction =
  | LoadAllActivitiesAction
  | LoadActivitiesCompleteAction
  | LoadActivitiesFailedAction;
