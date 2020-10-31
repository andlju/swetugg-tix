import React, { createContext, useReducer } from 'react';
import { OrderAction } from './order.actions';
import { orderLoadMiddleware } from './order.effects';
import { orderReducer, OrderState } from './order.reducer';

export interface MainState {
  order: OrderState
}

const initialState : MainState = {
  order: {
    loading: false
  }
};

type Action = 
  | OrderAction;

const MainStore = createContext<{ state: MainState; dispatch: React.Dispatch<Action>; }>({
  state: initialState,
  dispatch: () => null
});

const mainReducer = ({ order } : MainState, action : Action) => ({
  order: orderReducer(order, action),
}); 

const StateProvider: React.FC = ({ children }) => {

  const [state, dispatch] = useReducer(mainReducer, initialState);

  return <MainStore.Provider value={{ state, dispatch: orderLoadMiddleware(dispatch) }}> {children} </MainStore.Provider>;
};

export { MainStore, StateProvider };