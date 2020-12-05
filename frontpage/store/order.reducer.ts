import { Reducer } from "redux";
import { LOAD_ORDER, LOAD_ORDER_COMPLETE, LOAD_ORDER_FAILED, OrderAction } from "./order.actions";
import { Order } from "./order.models";

export interface OrderState {
  currentOrder?: Order,
  loading: boolean,
}

const initialState: OrderState = {
  loading: false
};

export const orderHydrator = (state: OrderState | undefined, hydratedState: OrderState): OrderState => ({
  ...state,
  ...hydratedState // At the moment we can just overwrite anything in the state
  });

const orderReducer : Reducer<OrderState, OrderAction> = (state, action) => {
  if (!state) {
    state = initialState;
  }
  switch (action.type) {
    case LOAD_ORDER:
      return {
        ...state,
        currentOrder: undefined,
        loading: true
      };
    case LOAD_ORDER_COMPLETE:
      return {
        ...state,
        currentOrder: action.payload.order,
        loading: false
      };
      case LOAD_ORDER_FAILED:
        return {
          ...state,
          loading: false
        };
    default:
      return state;
  }
};

export { orderReducer };