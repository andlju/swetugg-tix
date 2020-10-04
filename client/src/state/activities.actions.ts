import { Activity } from "./activity.models";

// Actions
export const LIST_LOAD = 'ACTIVITIES LOAD';
export const LIST_LOAD_SUCCESS = 'ACTIVITIES LOAD SUCCESS';
export const LIST_LOAD_FAILURE = 'ACTIVITIES LOAD FAILURE';

export const ACTIVITY_LOAD = 'ACTIVITY LOAD';
export const ACTIVITY_LOAD_SUCCESS = 'ACTIVITY LOAD SUCCESS';
export const ACTIVITY_LOAD_FAILURE = 'ACTIVITY LOAD FAILURE';

interface ListLoadAction {
  type: typeof LIST_LOAD
}

interface ListLoadSuccessAction {
  type: typeof LIST_LOAD_SUCCESS,
  payload: {
    activities: Activity[]
  }
}

interface ListLoadFailureAction {
  type: typeof LIST_LOAD_FAILURE
}

interface ActivityLoadAction {
  type: typeof ACTIVITY_LOAD,
  payload: { id: string }
}

interface ActivityLoadSuccessAction {
  type: typeof ACTIVITY_LOAD_SUCCESS,
  payload: {
    activity: Activity
  }
}

interface ActivityLoadFailureAction {
  type: typeof ACTIVITY_LOAD_FAILURE
}

export type ActivitiesActionTypes = ListLoadAction | ListLoadSuccessAction | ListLoadFailureAction |ActivityLoadAction | ActivityLoadSuccessAction | ActivityLoadFailureAction;

// Actions creators
export function loadActivities(): ActivitiesActionTypes {
  return { type: LIST_LOAD }
}

export function loadActivitiesSuccess(activities: Activity[]): ActivitiesActionTypes {
  return { type: LIST_LOAD_SUCCESS, payload: { activities } }
}

export function loadActivitiesFailure(): ActivitiesActionTypes {
  return { type: LIST_LOAD_FAILURE };
}

export function loadActivity(id: string): ActivitiesActionTypes {
  return { type: ACTIVITY_LOAD, payload: { id } };
}

export function loadActivitySuccess(activity: Activity): ActivitiesActionTypes {
  return { type: ACTIVITY_LOAD_SUCCESS, payload: { activity } }
}

export function loadUserFailure(id: string): ActivitiesActionTypes {
  return { type: ACTIVITY_LOAD_FAILURE }
}
