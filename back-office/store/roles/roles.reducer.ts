import { Reducer } from 'redux';

import {
  initListState,
  loadList,
  loadListComplete,
  loadListFailed,
  loadListItem,
  TixListState,
} from '../common/list-state.models';
import { RoleActionTypes, RolesAction } from './roles.actions';
import { Role } from './roles.actions';

export interface RolesState {
  roles: TixListState<Role>;
}

const initialState: RolesState = {
  roles: initListState((role) => role.roleId),
};

const rolesReducer: Reducer<RolesState, RolesAction> = (state, action) => {
  if (!state) {
    state = initialState;
  }
  switch (action.type) {
    case RoleActionTypes.LOAD_ROLES:
      return {
        ...state,
        roles: loadList(state.roles),
      };
    case RoleActionTypes.LOAD_ROLE:
      return {
        ...state,
        roles: loadListItem(state.roles, action.payload.code),
      };
    case RoleActionTypes.LOAD_ROLES_COMPLETE:
      return {
        ...state,
        roles: loadListComplete(state.roles, action.payload.roles),
      };
    case RoleActionTypes.LOAD_ROLES_FAILED:
      return {
        ...state,
        roles: loadListFailed(state.roles, action.payload.errorCode, action.payload.errorMessage),
      };
    default:
      return state;
  }
};

export { rolesReducer };
