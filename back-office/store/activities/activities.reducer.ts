import { Reducer } from 'redux';

import { CommandStatus } from '../../src/services/activity-command.service';
import {
  initListState,
  loadList,
  loadListComplete,
  loadListFailed,
  loadListItem,
  TixListState,
} from '../common/list-state.models';
import { ActivitiesAction, ActivityActionTypes } from './activities.actions';
import { Activity } from './activity.models';

export interface CommandStatusState extends CommandStatus {
  uiId?: string;
}

export interface ActivitiesState {
  activities: TixListState<Activity>;
  commands: CommandStatusState[];
}

const initialState: ActivitiesState = {
  activities: initListState<Activity>((activity) => activity.activityId),
  commands: [],
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
        activities: loadListItem(state.activities, action.payload.activityId),
      };
    case ActivityActionTypes.LOAD_ACTIVITIES:
      return {
        ...state,
        activities: loadList(state.activities),
      };
    case ActivityActionTypes.LOAD_ACTIVITIES_COMPLETE:
      return {
        ...state,
        activities: loadListComplete(state.activities, action.payload.activities),
      };
    case ActivityActionTypes.LOAD_ACTIVITIES_FAILED:
      return {
        ...state,
        activities: loadListFailed(state.activities, action.payload.errorCode, action.payload.errorMessage),
      };
    case ActivityActionTypes.SEND_ACTIVITY_COMMAND:
      return {
        ...state,
        commands: [
          {
            commandId: action.payload.uiId,
            uiId: action.payload.uiId,
            status: 'Dispatched',
            body: action.payload.body,
            jsonBody: JSON.stringify(action.payload.body),
          },
          ...state.commands.filter((c, i) => c.status === 'Dispatched' || c.status === 'Created' || i > maxNumberOfOldCommands),
        ],
      };
    case ActivityActionTypes.ACTIVITY_COMMAND_STATUS_SET: {
      const currentCommand = state.commands.find((c) => action.payload.uiId && c.uiId === action.payload.uiId);
      const commands = currentCommand
        ? state.commands.map((c) => (c === currentCommand ? { ...c, ...action.payload.commandStatus } : c))
        : [{ ...action.payload.commandStatus }, ...state.commands];
      return {
        ...state,
        commands: commands,
      };
    }
    default:
      return state;
  }
};

export { activitiesReducer };
