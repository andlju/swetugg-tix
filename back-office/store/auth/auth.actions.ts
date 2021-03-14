import { Action } from "redux";

export enum AuthActionTypes {
  LOGIN = 'LOGIN',
  LOGOUT = 'LOGOUT',
  GET_SCOPES = 'GET_SCOPE',
  
  VALIDATE_LOGIN = 'VALIDATE_LOGIN',
  VALIDATE_LOGIN_COMPLETE = 'VALIDATE_LOGIN_COMPLETE',
  VALIDATE_LOGIN_FAILED = 'VALIDATE_LOGIN_FAILED',

  SET_ACCESS_TOKEN = 'SET_ACCESS_TOKEN',
  SET_USER = 'SET_USER',
  SET_IN_PROGRESS = 'SET_IN_PROGRESS',

  CREATE_USER = 'CREATE_USER',
  CREATE_USER_COMPLETE = 'CREATE_USER_COMPLETE',
  CREATE_USER_FAILED = 'CREATE_USER_FAILED',

  UPDATE_USER = 'UPDATE_USER',
  UPDATE_USER_COMPLETE = 'UPDATE_USER_COMPLETE',
  UPDATE_USER_FAILED = 'UPDATE_USER_FAILED',

  REQUEST_USER_UPDATE = 'REQUEST_USER_UPDATE',
}

export enum InteractionKind {
  SILENT = 'Silent',
  POPUP = 'Popup',
  REDIRECT = 'Redirect'
}

export enum UserStatus {
  None = 0,
  Created = 1,
  Validated = 2,
  Deleted = 100
}

export interface User {
  name: string,
  username: string,
  status: UserStatus
}

export function login(interactionKind: InteractionKind = InteractionKind.SILENT): LoginAction {
  return {
    type: AuthActionTypes.LOGIN,
    payload: {
      interactionKind
    }
  };
}

export function logout(): LogoutAction {
  return {
    type: AuthActionTypes.LOGOUT
  };
}

export function validateLogin(scopes: string[]): ValidateLoginAction {
  return {
    type: AuthActionTypes.VALIDATE_LOGIN,
    payload: {
      scopes,
    }
  };
}

export function validateLoginComplete(): ValidateLoginCompleteAction {
  return {
    type: AuthActionTypes.VALIDATE_LOGIN_COMPLETE,
  };
}

export function validateLoginFailed(): ValidateLoginFailedAction {
  return {
    type: AuthActionTypes.VALIDATE_LOGIN_FAILED,
  };
}

export function getScopes(scopes: string[], interactionKind: InteractionKind = InteractionKind.SILENT): GetScopesAction {
  return {
    type: AuthActionTypes.GET_SCOPES,
    payload: {
      scopes,
      interactionKind
    }
  };
}

export function setAccessToken(token: string): SetAccessTokenAction {
  return {
    type: AuthActionTypes.SET_ACCESS_TOKEN,
    payload: { token }
  };
}

export function setInProgress(inProgress: boolean): SetInProgressAction {
  return {
    type: AuthActionTypes.SET_IN_PROGRESS,
    payload: { inProgress }
  };
}

export function setUser(user?: User): SetUserAction {
  return {
    type: AuthActionTypes.SET_USER,
    payload: {
      user
    }
  };
}

export function createUser(user: User): CreateUserAction {
  return {
    type: AuthActionTypes.CREATE_USER,
    payload: {
      user
    }
  };
}

export function createUserComplete(): CreateUserCompleteAction {
  return {
    type: AuthActionTypes.CREATE_USER_COMPLETE,
  };
}

export function createUserFailed(errorCode: string, errorMessage: string): CreateUserFailedAction {
  return {
    type: AuthActionTypes.CREATE_USER_FAILED,
    payload: {
      errorCode,
      errorMessage
    }
  };
}


export function updateUser(user: User): UpdateUserAction {
  return {
    type: AuthActionTypes.UPDATE_USER,
    payload: {
      user
    }
  };
}

export function updateUserComplete(): UpdateUserCompleteAction {
  return {
    type: AuthActionTypes.UPDATE_USER_COMPLETE,
  };
}

export function updateUserFailed(errorCode: string, errorMessage: string): UpdateUserFailedAction {
  return {
    type: AuthActionTypes.UPDATE_USER_FAILED,
    payload: {
      errorCode,
      errorMessage
    }
  };
}

export function requestUserUpdate(user: User): RequestUserUpdateAction {
  return {
    type: AuthActionTypes.REQUEST_USER_UPDATE,
    payload: {
      user
    }
  };
}

export interface LoginAction extends Action {
  type: AuthActionTypes.LOGIN;
  payload: {
    interactionKind: InteractionKind;
  }
}

export interface LogoutAction extends Action {
  type: AuthActionTypes.LOGOUT;
}

export interface ValidateLoginAction extends Action {
  type: AuthActionTypes.VALIDATE_LOGIN;
  payload: {
    scopes: string[];
  };
}

export interface ValidateLoginCompleteAction extends Action {
  type: AuthActionTypes.VALIDATE_LOGIN_COMPLETE
}

export interface ValidateLoginFailedAction extends Action {
  type: AuthActionTypes.VALIDATE_LOGIN_FAILED
}

export interface GetScopesAction extends Action {
  type: AuthActionTypes.GET_SCOPES,
  payload: {
    scopes: string[];
    interactionKind: InteractionKind
  };
}

export interface SetAccessTokenAction extends Action {
  type: AuthActionTypes.SET_ACCESS_TOKEN,
  payload: {
    token: string;
  };
}

export interface SetInProgressAction extends Action {
  type: AuthActionTypes.SET_IN_PROGRESS,
  payload: {
    inProgress: boolean;
  };
}

export interface SetUserAction extends Action {
  type: AuthActionTypes.SET_USER,
  payload: {
    user?: User;
  };
}

export interface CreateUserAction extends Action {
  type: AuthActionTypes.CREATE_USER,
  payload: {
    user: User;
  };
}

export interface CreateUserCompleteAction extends Action {
  type: AuthActionTypes.CREATE_USER_COMPLETE
}

export interface CreateUserFailedAction extends Action {
  type: AuthActionTypes.CREATE_USER_FAILED,
  payload: {
    errorCode: string,
    errorMessage: string
  }
}

export interface UpdateUserAction extends Action {
  type: AuthActionTypes.UPDATE_USER,
  payload: {
    user: User;
  };
}

export interface UpdateUserCompleteAction extends Action {
  type: AuthActionTypes.UPDATE_USER_COMPLETE
}

export interface UpdateUserFailedAction extends Action {
  type: AuthActionTypes.UPDATE_USER_FAILED,
  payload: {
    errorCode: string,
    errorMessage: string
  }
}

export interface RequestUserUpdateAction extends Action {
  type: AuthActionTypes.REQUEST_USER_UPDATE;
  payload: {
    user: User;
  };
}


export type AuthAction =
  | LoginAction
  | LogoutAction
  | ValidateLoginAction
  | ValidateLoginCompleteAction
  | ValidateLoginFailedAction
  | GetScopesAction
  | SetAccessTokenAction
  | SetInProgressAction
  | SetUserAction
  | CreateUserAction
  | CreateUserCompleteAction
  | CreateUserFailedAction
  | UpdateUserAction
  | UpdateUserCompleteAction
  | UpdateUserFailedAction
  | RequestUserUpdateAction;
