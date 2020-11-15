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

const PublicStore = createContext<{ state: MainState; dispatch: React.Dispatch<Action>; }>({
  state: initialState,
  dispatch: () => null
});

const mainReducer = ({ order } : MainState, action : Action) => ({
  order: orderReducer(order, action),
}); 

const PublicStateProvider: React.FC = ({ children }) => {

  const [state, dispatch] = useReducer(mainReducer, initialState);

  return <PublicStore.Provider value={{ state, dispatch: orderLoadMiddleware(dispatch) }}> {children} </PublicStore.Provider>;
};

export { PublicStore, PublicStateProvider };