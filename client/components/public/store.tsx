import React, { createContext, useReducer } from 'react';

interface OrderState {
  orderId?: string,
  loading: boolean,
}

export const SET_ORDER_ID = 'SET_ORDER_ID';
export const LOAD_ORDER = 'LOAD_ORDER';
export const LOAD_ORDER_COMPLETE = 'LOAD_ORDER_COMPLETE';
export const LOAD_ORDER_FAILED = 'LOAD_ORDER_FAILED';

interface LoadOrderAction {
  type: typeof LOAD_ORDER;
  payload: {
    orderId: string;
  };
}

interface LoadOrderCompleteAction {
  type: typeof LOAD_ORDER_COMPLETE;
  payload: {
    orderId: string;
    tickets: number;
  };
}

interface LoadOrderFailedAction {
  type: typeof LOAD_ORDER_FAILED;
  payload: {
    orderId: string;
    errorCode: string;
    errorMessage: string;
  };
}

type OrderAction =
  | LoadOrderAction
  | LoadOrderCompleteAction
  | LoadOrderFailedAction;

const initialState: OrderState = { loading: false };

const store = createContext<{
  state: OrderState;
  dispatch: React.Dispatch<OrderAction>;
}>({
  state: initialState,
  dispatch: () => null
});

const { Provider } = store;

const reducer = (state: OrderState, action: OrderAction) => {
  switch (action.type) {
    case LOAD_ORDER:
      return {
        ...state,
        orderId: action.payload.orderId,
        loading: true
      };
    default:
      throw new Error();
  }
  return state;
};

const StateProvider: React.FC = ({ children }) => {

  const [state, dispatch] = useReducer(reducer, initialState);

  return <Provider value={{ state, dispatch }}> {children} </Provider>;
};

export { store, StateProvider };