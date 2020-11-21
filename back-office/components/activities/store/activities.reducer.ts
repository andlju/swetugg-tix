import { Reducer } from "redux";
import { Activity } from "../activity.models";
import { ActivitiesAction, LOAD_ACTIVITIES, LOAD_ACTIVITIES_COMPLETE, LOAD_ACTIVITIES_FAILED } from "./activities.actions";

export interface ActivitiesState {
  activities: {
    [key: string]: Activity;
  },
  visibleActivities: {
    ids: string[],
    loading: boolean;
  };
}

const initialState : ActivitiesState = {
  activities: {},
  visibleActivities: {
    ids: [],
    loading: false
  }
};

export const activitiesHydrator = (state: ActivitiesState | undefined, hydratedState: ActivitiesState): ActivitiesState => ({
  activities: {
    ...state?.activities,
    ...hydratedState.activities,
  },
  visibleActivities: {
    ids: Array.from(new Set([...hydratedState.visibleActivities.ids, ...state?.visibleActivities.ids ?? []])),
    loading: false
  }
});

const activitiesReducer : Reducer<ActivitiesState, ActivitiesAction> = (state, action) => {
  if (!state) {
    state = initialState;
  }  
  switch (action.type) {
    case LOAD_ACTIVITIES:
      return {
        ...state,
        visibleActivities: {
          ids: state?.visibleActivities.ids ?? [],
          loading: true
        }
      };
    case LOAD_ACTIVITIES_COMPLETE:
      return {
        ...state,
        activities: action.payload.activities.reduce((activities, activity) => ({ ...activities, [activity.activityId]: activity }), state.activities),
        visibleActivities: {
          ids: action.payload.activities.map(a => a.activityId),
          loading: false
        }
      };
    case LOAD_ACTIVITIES_FAILED:
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