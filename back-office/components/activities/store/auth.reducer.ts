import { Reducer } from "redux";
import { AuthAction, AuthActionTypes, User } from "./auth.actions";

export interface AuthState {
  token?: string,
  user?: User
}

const initialState : AuthState = {
};

const authReducer : Reducer<AuthState, AuthAction> = (state, action) => {
  if (!state) {
    state = initialState;
  }  
  switch (action.type) {
    case AuthActionTypes.AUTHENTICATE:
      return {
        ...state,
      };
    case AuthActionTypes.SET_ACCESS_TOKEN:
      return {
        ...state,
        token: action.payload.token
      };
    default:
      return state;
  }
};

export { authReducer };