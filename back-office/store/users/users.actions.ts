import { Action } from 'redux';

export enum UsersActionTypes {
  LOAD_USER_ROLES = 'LOAD_USER_ROLES',
  LOAD_USER_ROLES_COMPLETE = 'LOAD_USER_ROLES_COMPLETE',
  LOAD_USER_ROLES_FAILED = 'LOAD_USER_ROLES_FAILED',

  ADD_USER_ROLE = 'ADD_USER_ROLE',
  ADD_USER_ROLE_COMPLETE = 'ADD_USER_ROLE_COMPLETE',
  ADD_USER_ROLE_FAILED = 'ADD_USER_ROLE_FAILED',
}

export interface User {
  userId: string;
  name: string;
}

export interface UserRole {
  userRoleId?: string;
  roleName?: string;
  roleId: string;
  attributes: { name: string; value: string }[];
}

export function loadUserRoles(userId: string, organizationId?: string): LoadUserRolesAction {
  return {
    type: UsersActionTypes.LOAD_USER_ROLES,
    payload: {
      userId,
      organizationId,
    },
  };
}

export function loadUserRolesComplete(userRoles: UserRole[]): LoadUserRolesCompleteAction {
  return {
    type: UsersActionTypes.LOAD_USER_ROLES_COMPLETE,
    payload: {
      userRoles,
    },
  };
}

export function loadUserRolesFailed(errorCode: string, errorMessage: string): LoadUserRolesFailedAction {
  return {
    type: UsersActionTypes.LOAD_USER_ROLES_FAILED,
    payload: {
      errorCode,
      errorMessage,
    },
  };
}

export function addUserRole(userId: string, userRole: UserRole): AddUserRoleAction {
  return {
    type: UsersActionTypes.ADD_USER_ROLE,
    payload: {
      userId,
      userRole,
    },
  };
}

export function addUserRoleComplete(): AddUserRoleCompleteAction {
  return {
    type: UsersActionTypes.ADD_USER_ROLE_COMPLETE,
  };
}

export function addUserRoleFailed(errorCode: string, errorMessage: string): AddUserRoleFailedAction {
  return {
    type: UsersActionTypes.ADD_USER_ROLE_FAILED,
    payload: {
      errorCode,
      errorMessage,
    },
  };
}

export interface LoadUserRolesAction extends Action {
  type: UsersActionTypes.LOAD_USER_ROLES;
  payload: {
    userId: string;
    organizationId?: string;
  };
}

export interface LoadUserRolesCompleteAction extends Action {
  type: UsersActionTypes.LOAD_USER_ROLES_COMPLETE;
  payload: {
    userRoles: UserRole[];
  };
}

export interface LoadUserRolesFailedAction extends Action {
  type: UsersActionTypes.LOAD_USER_ROLES_FAILED;
  payload: {
    errorCode: string;
    errorMessage: string;
  };
}

export interface AddUserRoleAction extends Action {
  type: UsersActionTypes.ADD_USER_ROLE;
  payload: {
    userId: string;
    userRole: UserRole;
  };
}

export interface AddUserRoleCompleteAction extends Action {
  type: UsersActionTypes.ADD_USER_ROLE_COMPLETE;
}

export interface AddUserRoleFailedAction extends Action {
  type: UsersActionTypes.ADD_USER_ROLE_FAILED;
  payload: {
    errorCode: string;
    errorMessage: string;
  };
}

export type UserAction =
  | LoadUserRolesAction
  | LoadUserRolesCompleteAction
  | LoadUserRolesFailedAction
  | AddUserRoleAction
  | AddUserRoleCompleteAction
  | AddUserRoleFailedAction;
