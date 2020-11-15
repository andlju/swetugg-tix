import React, { createContext, useReducer } from 'react';
import { ActivitiesAction } from "./activities.actions";
import { activitiesLoadMiddleware } from './activities.effects';
import { activitiesReducer, ActivitiesState } from './activities.reducer';

export interface BackOfficeState {
  activities: ActivitiesState
}

const initialState : BackOfficeState = {
  activities : {
    activities: [],
    loading: false
  }
};

type Action = 
  | ActivitiesAction;

const BackOfficeStore = createContext<{ state: BackOfficeState; dispatch: React.Dispatch<Action>; }>({
  state: initialState,
  dispatch: () => null
});

const mainReducer = ({ activities } : BackOfficeState, action : Action) => ({
  activities: activitiesReducer(activities, action),
}); 

const BackOfficeStateProvider: React.FC = ({ children }) => {

  const [state, dispatch] = useReducer(mainReducer, initialState);

  return <BackOfficeStore.Provider value={{ state, dispatch: activitiesLoadMiddleware(dispatch) }}> {children} </BackOfficeStore.Provider>;
};

export { BackOfficeStore, BackOfficeStateProvider };