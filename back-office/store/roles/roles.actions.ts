import { Action } from 'redux';

export interface Role {
  code: string;
  description: string;
}

export enum RoleActionTypes {
  LOAD_ROLE = 'LOAD_ROLE',
  LOAD_ROLES = 'LOAD_ROLES',
  LOAD_ROLES_COMPLETE = 'LOAD_ROLES_COMPLETE',
  LOAD_ROLES_FAILED = 'LOAD_ROLES_FAILED',
}

export function loadRoles(): LoadRolesAction {
  return {
    type: RoleActionTypes.LOAD_ROLES,
  };
}

export function loadRole(code: string): LoadRoleAction {
  return {
    type: RoleActionTypes.LOAD_ROLE,
    payload: {
      code: code,
    },
  };
}

export function loadRolesComplete(roles: Role[]): LoadRolesCompleteAction {
  return {
    type: RoleActionTypes.LOAD_ROLES_COMPLETE,
    payload: {
      roles: roles,
    },
  };
}

export interface LoadRolesAction extends Action {
  type: RoleActionTypes.LOAD_ROLES;
}

export interface LoadRoleAction extends Action {
  type: RoleActionTypes.LOAD_ROLE;
  payload: {
    code: string;
  };
}

export interface LoadRolesCompleteAction extends Action {
  type: RoleActionTypes.LOAD_ROLES_COMPLETE;
  payload: {
    roles: Role[];
  };
}

export interface LoadRolesFailedAction extends Action {
  type: RoleActionTypes.LOAD_ROLES_FAILED;
  payload: {
    errorCode: string;
    errorMessage: string;
  };
}

export type RolesAction = LoadRolesAction | LoadRoleAction | LoadRolesCompleteAction | LoadRolesFailedAction;
