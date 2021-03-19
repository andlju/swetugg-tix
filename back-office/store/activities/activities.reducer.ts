import { Reducer } from "redux";
import { Activity } from "./activity.models";
import { CommandStatus } from "../../src/services/activity-command.service";
import { ActivitiesAction, ActivityActionTypes } from "./activities.actions";

export interface CommandStatusState extends CommandStatus {
  uiId?: string;
}

export interface ActivitiesState {
  activities: {
    [key: string]: Activity;
  },
  commands: CommandStatusState[];
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
        commands: [{ 
          commandId: action.payload.uiId,
          uiId: action.payload.uiId,
          status: "Dispatched",
          body: action.payload.body,
          jsonBody: JSON.stringify(action.payload.body)
        }, ...state.commands.filter((c, i) => c.status === "Dispatched" || c.status === "Created" || i > maxNumberOfOldCommands)]
      };
    case ActivityActionTypes.ACTIVITY_COMMAND_STATUS_SET: {
      const currentCommand = state.commands.find((c) => action.payload.uiId && c.uiId === action.payload.uiId);
      const commands = currentCommand ? state.commands.map(c => c === currentCommand ? { ...c, ...action.payload.commandStatus } : c) : [{...action.payload.commandStatus }, ...state.commands];
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