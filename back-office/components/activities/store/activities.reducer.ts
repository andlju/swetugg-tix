import { Reducer } from "redux";
import { Activity } from "../activity.models";
import { ActivitiesAction, ActivityActionTypes } from "./activities.actions";

export interface ActivityCommandState {
  commandId: string;
  // isCompleted: boolean;
  activityId?: string;
}

export interface ActivitiesState {
  activities: {
    [key: string]: Activity;
  },
  activeCommands: ActivityCommandState[];
  visibleActivities: {
    ids: string[],
    loading: boolean;
  };
}

const initialState: ActivitiesState = {
  activities: {},
  activeCommands: [],
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
    case ActivityActionTypes.ACTIVITY_COMMAND_SENT:
      return {
        ...state,
        activeCommands: [...state.activeCommands, { commandId: action.payload.commandId }]
      };
    case ActivityActionTypes.ACTIVITY_COMMAND_COMPLETE:
      return {
        ...state,
        activeCommands: state.activeCommands.filter(c => c.commandId !== action.payload.commandId)
      };
    case ActivityActionTypes.ACTIVITY_COMMAND_FAILED:
      return {
        ...state,
        activeCommands: state.activeCommands.filter(c => c.commandId !== action.payload.commandId)
      };
    default:
      return state;
  }
};

export { activitiesReducer };