import { Action } from "redux";

export enum AuthActionTypes {
  AUTHENTICATE = 'AUTHENTICATE',
  SET_ACCESS_TOKEN = 'SET_ACCESS_TOKEN',
  SET_USER = 'SET_USER',
  SET_IN_PROGRESS = 'SET_IN_PROGRESS',
}

export interface User {
  displayName: string,
}

export function authenticate() : AuthenticateAction {
  return {
    type: AuthActionTypes.AUTHENTICATE
  }
}

export function setAccessToken(token: string) : SetAccessTokenAction {
  return {
    type: AuthActionTypes.SET_ACCESS_TOKEN,
    payload: { token }
  }
}

export function setInProgress(inProgress: boolean) : SetInProgressAction {
  return {
    type: AuthActionTypes.SET_IN_PROGRESS,
    payload: { inProgress }
  }
}

export function setUser(user?: User) : SetUserAction {
  return {
    type: AuthActionTypes.SET_USER,
    payload: {
      user
    }
  }
}

export interface AuthenticateAction extends Action {
  type: AuthActionTypes.AUTHENTICATE
}

export interface SetAccessTokenAction extends Action {
  type: AuthActionTypes.SET_ACCESS_TOKEN,
  payload: {
    token: string
  }
}

export interface SetInProgressAction extends Action {
  type: AuthActionTypes.SET_IN_PROGRESS,
  payload: {
    inProgress: boolean
  }
}

export interface SetUserAction extends Action {
  type: AuthActionTypes.SET_USER,
  payload: {
    user?: User
  }
}

export type AuthAction = 
  | AuthenticateAction
  | SetAccessTokenAction
  | SetInProgressAction
  | SetUserAction
