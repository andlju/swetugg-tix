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

export type OrderAction =
  | LoadOrderAction
  | LoadOrderCompleteAction
  | LoadOrderFailedAction;
