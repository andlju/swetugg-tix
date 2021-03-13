import { Reducer } from "redux";
import { AuthAction, AuthActionTypes, User } from "./auth.actions";

export interface AuthState {
  accessToken?: string,
  user?: User,
  inProgress: boolean;
}

const initialState: AuthState = {
  inProgress: false
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
        user: undefined,
        accessToken: undefined
      };
    case AuthActionTypes.SET_ACCESS_TOKEN:
      return {
        ...state,
        accessToken: action.payload.token
      };
    case AuthActionTypes.SET_IN_PROGRESS:
      return {
        ...state,
        inProgress: action.payload.inProgress
      };
    case AuthActionTypes.SET_USER:
      return {
        ...state,
        user: action.payload.user
      };
    default:
      return state;
  }
};

export { authReducer };