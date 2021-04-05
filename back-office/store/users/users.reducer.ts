import { Reducer } from 'redux';

import { initListState, loadList, loadListComplete, loadListFailed, TixListState } from '../common/list-state.models';
import { User, UserAction, UserRole, UsersActionTypes } from './users.actions';

export interface UsersState {
  users: TixListState<User>;
  userRoles: TixListState<UserRole>;
}

const initialState: UsersState = {
  userRoles: initListState((userRole) => userRole.code),
  users: initListState((user) => user.userId),
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
    default:
      return state;
  }
};

export { usersReducer };
