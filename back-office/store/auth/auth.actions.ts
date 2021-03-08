import { Action } from "redux";

export enum AuthActionTypes {
  LOGIN = 'LOGIN',
  LOGOUT = 'LOGOUT',
  GET_SCOPES = 'GET_SCOPE',
  
  VALIDATE_USER = 'VALIDATE_USER',
  VALIDATE_USER_COMPLETE = 'VALIDATE_USER_COMPLETE',
  VALIDATE_USER_FAILED = 'VALIDATE_USER_FAILED',

  SET_ACCESS_TOKEN = 'SET_ACCESS_TOKEN',
  SET_USER = 'SET_USER',
  SET_IN_PROGRESS = 'SET_IN_PROGRESS',
}

export enum InteractionKind {
  SILENT = 'Silent',
  POPUP = 'Popup',
  REDIRECT = 'Redirect'
}

export interface User {
  displayName: string,
  username: string
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

export function validateUser(): ValidateUserAction {
  return {
    type: AuthActionTypes.VALIDATE_USER,
  };
}

export function validateUserFailed(): ValidateUserFailedAction {
  return {
    type: AuthActionTypes.VALIDATE_USER_FAILED,
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

export interface LoginAction extends Action {
  type: AuthActionTypes.LOGIN;
  payload: {
    interactionKind: InteractionKind;
  }
}

export interface LogoutAction extends Action {
  type: AuthActionTypes.LOGOUT;
}

export interface ValidateUserAction extends Action {
  type: AuthActionTypes.VALIDATE_USER
}

export interface ValidateUserFailedAction extends Action {
  type: AuthActionTypes.VALIDATE_USER_FAILED
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

export type AuthAction =
  | LoginAction
  | LogoutAction
  | ValidateUserAction
  | ValidateUserFailedAction
  | GetScopesAction
  | SetAccessTokenAction
  | SetInProgressAction
  | SetUserAction;
