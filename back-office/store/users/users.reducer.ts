import { Reducer } from 'redux';

import {
  initCommandState,
  setCommandComplete,
  setCommandFailed,
  setCommandSending,
  TixCommandState,
} from '../common/command-state.models';
import { initListState, loadList, loadListComplete, loadListFailed, TixListState } from '../common/list-state.models';
import { User, UserAction, UserRole, UsersActionTypes } from './users.actions';

export interface UsersState {
  users: TixListState<User>;
  userRoles: TixListState<UserRole>;
  addUserRoleCommand: TixCommandState;
  removeUserRoleCommand: TixCommandState;
}

const initialState: UsersState = {
  userRoles: initListState((userRole) => userRole.userRoleId || ''),
  users: initListState((user) => user.userId),
  addUserRoleCommand: initCommandState(),
  removeUserRoleCommand: initCommandState(),
};

const usersReducer: Reducer<UsersState, UserAction> = (state, action) => {
  if (!state) {
    state = initialState;
  }
  switch (action.type) {
    case UsersActionTypes.LOAD_USER_ROLES:
      return {
        ...state,
        userRoles: loadList(state.userRoles),
      };
    case UsersActionTypes.LOAD_USER_ROLES_COMPLETE:
      return {
        ...state,
        userRoles: loadListComplete(state.userRoles, action.payload.userRoles),
      };
    case UsersActionTypes.LOAD_USER_ROLES_FAILED:
      return {
        ...state,
        userRoles: loadListFailed(state.userRoles, action.payload.errorCode, action.payload.errorMessage),
      };
    case UsersActionTypes.ADD_USER_ROLE:
      return {
        ...state,
        addUserRoleCommand: setCommandSending(state.addUserRoleCommand, action.payload),
      };
    case UsersActionTypes.ADD_USER_ROLE_COMPLETE:
      return {
        ...state,
        addUserRoleCommand: setCommandComplete(state.addUserRoleCommand, action.payload),
      };
    case UsersActionTypes.ADD_USER_ROLE_FAILED:
      return {
        ...state,
        addUserRoleCommand: setCommandFailed(state.addUserRoleCommand, action.payload.errorCode, action.payload.errorMessage),
      };
    case UsersActionTypes.REMOVE_USER_ROLE:
      return {
        ...state,
        removeUserRoleCommand: setCommandSending(state.removeUserRoleCommand, action.payload),
      };
    case UsersActionTypes.REMOVE_USER_ROLE_COMPLETE:
      return {
        ...state,
        removeUserRoleCommand: setCommandComplete(state.removeUserRoleCommand),
      };
    case UsersActionTypes.REMOVE_USER_ROLE_FAILED:
      return {
        ...state,
        removeUserRoleCommand: setCommandFailed(
          state.removeUserRoleCommand,
          action.payload.errorCode,
          action.payload.errorMessage
        ),
      };
    default:
      return state;
  }
};

export { usersReducer };
