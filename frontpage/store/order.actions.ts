import { CommandStatus } from "../src/services/order-command.service";
import { Order } from "./order.models";

export const SET_ORDER_ID = 'SET_ORDER_ID';
export const LOAD_ORDER = 'LOAD_ORDER';
export const LOAD_ORDER_COMPLETE = 'LOAD_ORDER_COMPLETE';
export const LOAD_ORDER_FAILED = 'LOAD_ORDER_FAILED';

export const SEND_ORDER_COMMAND = 'SEND_ORDER_COMMAND';
export const SEND_ORDER_COMMAND_COMPLETE = 'SEND_ORDER_COMMAND_COMPLETE';

export const ADD_TICKETS = 'ADD_TICKETS';

export interface LoadOrderAction {
  type: typeof LOAD_ORDER;
  payload: {
    orderId: string;
    revision: number;
  };
}

export interface LoadOrderCompleteAction {
  type: typeof LOAD_ORDER_COMPLETE;
  payload: {
    order: Order;
  };
}

export interface LoadOrderFailedAction {
  type: typeof LOAD_ORDER_FAILED;
  payload: {
    orderId: string;
    errorCode: string;
    errorMessage: string;
  };
}

export interface AddTicketsAction {
  type: typeof ADD_TICKETS;
  payload: {
    orderId?: string;
    activityId: string;
    ticketTypeId: string;
    quantity: number;
  }
}

export interface SendOrderCommandAction {
  type: typeof SEND_ORDER_COMMAND;
  payload: {
    url: string;
    method?: string;
    body: unknown;
  }
}

export interface SendOrderCommandCompleteAction {
  type: typeof SEND_ORDER_COMMAND_COMPLETE;
  payload: {
    result: CommandStatus;
  }
}

export type OrderAction =
  | LoadOrderAction
  | LoadOrderCompleteAction
  | LoadOrderFailedAction
  | SendOrderCommandAction
  | SendOrderCommandCompleteAction
  | AddTicketsAction;
