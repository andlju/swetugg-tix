import { Reducer } from "redux";
import { Activity } from "../components/activity/activity.models";

import { ActivityAction, LOAD_ACTIVITY, LOAD_ACTIVITY_COMPLETE, LOAD_ACTIVITY_FAILED } from "./activity.actions";

export interface ActivityState {
  currentActivity: Activity | null;
  loading: boolean;
}

const initialState : ActivityState = {
  loading: false,
  currentActivity: null
};

export const activityHydrator = (state: ActivityState | undefined, hydratedState: ActivityState): ActivityState => ({
  ...state,
  ...hydratedState // At the moment we can just overwrite anything in the state
  });

const activityReducer : Reducer<ActivityState, ActivityAction> = (state, action) => {
  console.log("Reducing", action);
  if (!state) {
    state = initialState;
  }
  switch (action.type) {
    case LOAD_ACTIVITY:
      return {
        ...state,
        loading: true
      };
    case LOAD_ACTIVITY_COMPLETE:
      return {
        ...state,
        currentActivity: action.payload.activity,
        loading: false
      };
    case LOAD_ACTIVITY_FAILED:
      return {
        ...state,
        loading: false
      };
    default:
      return state;
  }
};

export { activityReducer };