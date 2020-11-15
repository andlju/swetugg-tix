import { LOAD_ORDER, LOAD_ORDER_COMPLETE, LOAD_ORDER_FAILED, OrderAction } from "./order.actions";

const orderLoadMiddleware = (dispatch : React.Dispatch<OrderAction>) => (action: OrderAction): void => {
  switch (action.type) {
    case LOAD_ORDER:
      setTimeout(() => dispatch({type: LOAD_ORDER_COMPLETE, payload: { orderId: action.payload.orderId, tickets: 2 }}), 1000);
      break;
  }
  return dispatch(action);
};


export { orderLoadMiddleware };