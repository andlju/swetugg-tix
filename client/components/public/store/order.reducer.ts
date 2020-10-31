import { LOAD_ORDER, LOAD_ORDER_COMPLETE, LOAD_ORDER_FAILED, OrderAction } from "./order.actions";

export interface OrderState {
  orderId?: string,
  tickets?: number,
  loading: boolean,
}

const orderReducer = (state: OrderState, action: OrderAction): OrderState => {
  switch (action.type) {
    case LOAD_ORDER:
      return {
        ...state,
        orderId: action.payload.orderId,
        tickets: undefined,
        loading: true
      };
    case LOAD_ORDER_COMPLETE:
      return {
        ...state,
        tickets: action.payload.tickets,
        loading: false
      };
      case LOAD_ORDER_FAILED:
        return {
          ...state,
          tickets: undefined,
          loading: false
        };
    default:
      throw new Error();
  }
};

export { orderReducer };