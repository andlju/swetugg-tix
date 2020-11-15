import { Activity } from "../components/activities/activity.models";
import { ActivitiesAction, LOAD_ACTIVITIES, LOAD_ACTIVITIES_COMPLETE, LOAD_ACTIVITIES_FAILED } from "./activities.actions";

export interface ActivitiesState {
  activities: Activity[],
  loading: boolean,
}

const activitiesReducer = (state: ActivitiesState, action: ActivitiesAction): ActivitiesState => {
  switch (action.type) {
    case LOAD_ACTIVITIES:
      return {
        ...state,
        loading: true
      };
    case LOAD_ACTIVITIES_COMPLETE:
      return {
        ...state,
        activities: action.payload.activities,
        loading: false
      };
      case LOAD_ACTIVITIES_FAILED:
        return {
          ...state,
          activities: [],
          loading: false
        };
    default:
      throw new Error();
  }
};

export { activitiesReducer };