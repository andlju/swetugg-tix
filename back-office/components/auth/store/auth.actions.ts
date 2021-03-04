import { Action } from "redux";

export enum AuthActionTypes {
  LOGIN = 'LOGIN',
  LOGOUT = 'LOGOUT',
  GET_USER = 'GET_USER',
  GET_SCOPES = 'GET_SCOPE',

  SET_ACCESS_TOKEN = 'SET_ACCESS_TOKEN',
  SET_USER = 'SET_USER',
  SET_IN_PROGRESS = 'SET_IN_PROGRESS',
}

export interface User {
  displayName: string,
  username: string
}

export function login(): LoginAction {
  return {
    type: AuthActionTypes.LOGIN
  };
}

export function logout(): LogoutAction {
  return {
    type: AuthActionTypes.LOGOUT
  };
}

export function getUser(): GetUserAction {
  return {
    type: AuthActionTypes.GET_USER,
  };
}

export function getScopes(scopes: string[]): GetScopesAction {
  return {
    type: AuthActionTypes.GET_SCOPES,
    payload: {
      scopes
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
}

export interface LogoutAction extends Action {
  type: AuthActionTypes.LOGOUT;
}

export interface GetUserAction extends Action {
  type: AuthActionTypes.GET_USER
}

export interface GetScopesAction extends Action {
  type: AuthActionTypes.GET_SCOPES,
  payload: {
    scopes: string[];
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
  | GetUserAction
  | GetScopesAction
  | SetAccessTokenAction
  | SetInProgressAction
  | SetUserAction;
