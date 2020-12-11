import { call, put, take, takeEvery } from "redux-saga/effects";
import { CommandStatus, sendOrderCommand } from "../src/services/order-command.service";
import { getView } from "../src/services/view-fetcher.service";
import { buildUrl } from "../src/url-utils";
import { AddTicketsAction, ADD_TICKETS, LoadOrderAction, LOAD_ORDER, LOAD_ORDER_COMPLETE, LOAD_ORDER_FAILED, SendOrderCommandAction, SEND_ORDER_COMMAND, SEND_ORDER_COMMAND_COMPLETE } from "./order.actions";
import { Order } from "./order.models";

function* loadOrderAction(action: LoadOrderAction): Generator {
  const result = yield call(async () => {
    return await getView<Order>(buildUrl(`/orders/${action.payload.orderId}`), { revision: action.payload.revision });
  });

  const order = result as Order;
  yield put({
    type: LOAD_ORDER_COMPLETE,
    payload: {
      order: order
    },
  });
}

function* sendOrderCommandAction(action: SendOrderCommandAction) {
  const result = yield call(async () => {
    return await sendOrderCommand(action.payload.url, action.payload.body, { method: action.payload.method });
  });
  const commandResult = result as CommandStatus;
  yield put({
    type: SEND_ORDER_COMMAND_COMPLETE,
    payload: {
      result: commandResult
    }
  });
  
  // If the order has a new revision, let's ensure that it's reloaded
  if (commandResult.aggregateId && commandResult.revision) {
    yield put({
      type: LOAD_ORDER,
      payload: {
        orderId: commandResult.aggregateId,
        revision: commandResult.revision
      }
    });
  }
}

function* addTicketsAction(action: AddTicketsAction): Generator {
  if (action.payload.orderId) {
    yield put({
      type: SEND_ORDER_COMMAND,
      payload: {
        url: `/orders/${action.payload.orderId}/tickets`,
        method: 'POST',
        body: {
          activityId: action.payload.activityId,
          tickets: [{
            ticketTypeId: action.payload.ticketTypeId,
            quantity: action.payload.quantity
          }]
        }
      }
    });
  } else {
    yield put({
      type: SEND_ORDER_COMMAND,
      payload: {
        url: `/orders`,
        method: 'POST',
        body: {
          activityId: action.payload.activityId,
          tickets: [{
            ticketTypeId: action.payload.ticketTypeId,
            quantity: action.payload.quantity
          }]
        }
      }
    });
  }
}

export function* orderSaga(): Generator {
  yield takeEvery(LOAD_ORDER, loadOrderAction);
  yield takeEvery(SEND_ORDER_COMMAND, sendOrderCommandAction);
  yield takeEvery(ADD_TICKETS, addTicketsAction);
}
