import { Reducer } from 'redux';

import { setFailed, setFetching, setState, setSaving, TixState } from '../common/state.models';
import { AuthAction, AuthActionTypes, User } from './auth.actions';

export interface AuthState {
  accessToken?: string;
  user: TixState<User>;
  inProgress: boolean;
}

const initialState: AuthState = {
  inProgress: false,
  user: {
    fetching: false,
    saving: false,
  },
};

const authReducer: Reducer<AuthState, AuthAction> = (state, action) => {
  if (!state) {
    state = initialState;
  }
  switch (action.type) {
    case AuthActionTypes.LOGIN:
      return {
        ...state,
      };
    case AuthActionTypes.LOGOUT:
      return {
        ...state,
      };
    case AuthActionTypes.VALIDATE_LOGIN:
      return {
        ...state,
      };
    case AuthActionTypes.VALIDATE_LOGIN_FAILED:
      return {
        ...state,
        user: setFailed(state.user, 'NotLoggedIn', 'The user is not logged in'),
        accessToken: undefined,
      };
    case AuthActionTypes.SET_ACCESS_TOKEN:
      return {
        ...state,
        accessToken: action.payload.token,
      };
    case AuthActionTypes.SET_IN_PROGRESS:
      return {
        ...state,
        inProgress: action.payload.inProgress,
      };
    case AuthActionTypes.SET_USER:
      return {
        ...state,
        user: setState(state.user, action.payload.user),
      };
    case AuthActionTypes.CREATE_USER:
      return {
        ...state,
        user: setSaving(state.user),
      };
    case AuthActionTypes.CREATE_USER_COMPLETE:
      return {
        ...state,
        user: setFetching(state.user),
      };
    case AuthActionTypes.CREATE_USER_FAILED:
      return {
        ...state,
        user: setFailed(state.user, action.payload.errorCode, action.payload.errorMessage),
      };
    case AuthActionTypes.UPDATE_USER:
      return {
        ...state,
        user: setSaving(state.user),
      };
    case AuthActionTypes.UPDATE_USER_COMPLETE:
      return {
        ...state,
        user: setState(state.user, state.user.current),
      };
    case AuthActionTypes.UPDATE_USER_FAILED:
      return {
        ...state,
        user: setFailed(state.user, action.payload.errorCode, action.payload.errorMessage),
      };
    default:
      return state;
  }
};

export { authReducer };
