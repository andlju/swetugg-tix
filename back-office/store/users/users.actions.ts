import { Action } from 'redux';

export enum UsersActionTypes {
  LOAD_USER_ROLES = 'LOAD_USER_ROLES',
  LOAD_USER_ROLES_COMPLETE = 'LOAD_USER_ROLES_COMPLETE',
  LOAD_USER_ROLES_FAILED = 'LOAD_USER_ROLES_FAILED',
}

export interface User {
  userId: string;
  name: string;
}

export interface UserRole {
  code: string;
  attributes: { [key: string]: string };
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

export type UserAction = LoadUserRolesAction | LoadUserRolesCompleteAction | LoadUserRolesFailedAction;
