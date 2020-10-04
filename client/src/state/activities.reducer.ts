import { HYDRATE } from "next-redux-wrapper";
import { ActivitiesActionTypes, ACTIVITY_LOAD_SUCCESS, LIST_LOAD, LIST_LOAD_SUCCESS } from "./activities.actions";
import { ActivitiesState, Activity } from "./activity.models";

const initialState: ActivitiesState = {
  activities: [],
  activity: undefined
}

export default function activitiesReducer(state = initialState, action: ActivitiesActionTypes): ActivitiesState {
  switch (action.type) {
    case LIST_LOAD: {
      return { ...state }
    }
    case LIST_LOAD_SUCCESS: {
      return { ...state, ...action.payload }
    }
    case ACTIVITY_LOAD_SUCCESS: {
      return { ...state, ...action.payload }
    }

  }
  return state
}
