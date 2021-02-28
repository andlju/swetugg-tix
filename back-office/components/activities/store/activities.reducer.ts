import { Reducer } from "redux";
import { Activity } from "../activity.models";
import { ActivitiesAction, ActivityActionTypes } from "./activities.actions";

export interface ActivitiesState {
  activities: {
    [key: string]: Activity;
  },
  visibleActivities: {
    ids: string[],
    loading: boolean;
  };
}

const initialState: ActivitiesState = {
  activities: {},
  visibleActivities: {
    ids: [],
    loading: false
  }
};

const activitiesReducer: Reducer<ActivitiesState, ActivitiesAction> = (state, action) => {
  if (!state) {
    state = initialState;
  }
  switch (action.type) {
    case ActivityActionTypes.LOAD_ACTIVITY: 
      return {
        ...state,
        visibleActivities: {
          ids: state?.visibleActivities.ids ?? [],
          loading: true
        }
      };
    case ActivityActionTypes.LOAD_ACTIVITIES:
      return {
        ...state,
        visibleActivities: {
          ids: state?.visibleActivities.ids ?? [],
          loading: true
        }
      };
    case ActivityActionTypes.LOAD_ACTIVITIES_COMPLETE:
      return {
        ...state,
        activities: action.payload.activities.reduce((activities, activity) => ({ ...activities, [activity.activityId]: activity }), state.activities),
        visibleActivities: {
          ids: action.payload.activities.map(a => a.activityId),
          loading: false
        }
      };
    case ActivityActionTypes.LOAD_ACTIVITIES_FAILED:
      return {
        ...state,
        visibleActivities: {
          ids: state?.visibleActivities.ids ?? [],
          loading: false
        }
      };
    default:
      return state;
  }
};

export { activitiesReducer };