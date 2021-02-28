import { Action } from "redux";

export enum AuthActionTypes {
  AUTHENTICATE = 'AUTHENTICATE',
  SET_ACCESS_TOKEN = 'SET_ACCESS_TOKEN',
  SET_USER = 'SET_USER',
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

export function setUser(user: User) : SetUserAction {
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

export interface SetUserAction extends Action {
  type: AuthActionTypes.SET_USER,
  payload: {
    user: User
  }
}

export type AuthAction = 
  | AuthenticateAction
  | SetAccessTokenAction
  | SetUserAction
