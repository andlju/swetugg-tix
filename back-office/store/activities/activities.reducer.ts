import { useImperativeHandle } from "react";
import { Reducer } from "redux";
import { Activity } from "../../components/activities/activity.models";
import { ActivitiesAction, ActivityActionTypes } from "./activities.actions";

export interface ActivityCommandState {
  uiId?: string;
  commandId: string;
  isProcessing: boolean;
  activityId?: string;
  revision?: number;
}

export interface ActivitiesState {
  activities: {
    [key: string]: Activity;
  },
  commands: ActivityCommandState[];
  visibleActivities: {
    ids: string[],
    loading: boolean;
  };
}

const initialState: ActivitiesState = {
  activities: {},
  commands: [],
  visibleActivities: {
    ids: [],
    loading: false
  }
};

const maxNumberOfOldCommands = 10;

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
    case ActivityActionTypes.SEND_ACTIVITY_COMMAND:
      return {
        ...state,
        commands: [{ commandId: action.payload.uiId, uiId: action.payload.uiId, isProcessing: true }, ...state.commands.filter((c, i) => c.isProcessing || i > maxNumberOfOldCommands)]
      };
    case ActivityActionTypes.ACTIVITY_COMMAND_SENT: {
      const currentCommand = state.commands.find((c) => action.payload.uiId && c.uiId === action.payload.uiId);
      const commands = currentCommand ? state.commands.map(c => c === currentCommand ? { ...c, ...action.payload } : c) : [{...action.payload, isProcessing: true }, ...state.commands];
      return {
        ...state,
        commands: commands
      };
    }
    case ActivityActionTypes.ACTIVITY_COMMAND_COMPLETE:
      return {
        ...state,
        commands: state.commands.map(c =>
          c.commandId === action.payload.commandId ? { ...c, isProcessing: false, activityId: action.payload.activityId, revision: action.payload.revision } : c
        )
      };
    case ActivityActionTypes.ACTIVITY_COMMAND_FAILED: {
      const currentCommand = state.commands.find((c) => c.commandId === action.payload.commandId || action.payload.uiId && c.uiId === action.payload.uiId);
      const commands = currentCommand ? state.commands.map(c => c === currentCommand ? { ...c, ...action.payload, isProcessing: false } : c) : [{...action.payload, isProcessing: false }, ...state.commands];
      return {
        ...state,
        commands: commands
      };
    }
    default:
      return state;
  }
};

export { activitiesReducer };