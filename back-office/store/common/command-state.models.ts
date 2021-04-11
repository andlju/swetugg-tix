export interface TixCommandState<TBody = unknown, TResult = unknown> {
  sending: boolean;
  body?: TBody;
  result?: TResult;
  errorCode?: string;
  errorMessage?: string;
}

export function initCommandState<TBody, TResult>(): TixCommandState<TBody, TResult> {
  return {
    sending: false,
  };
}

export function setCommandSending<TBody, TResult>(
  state: TixCommandState<TBody, TResult>,
  body: TBody
): TixCommandState<TBody, TResult> {
  return {
    ...state,
    sending: true,
    body: body,
    result: undefined,
    errorCode: undefined,
    errorMessage: undefined,
  };
}

export function setCommandComplete<TBody, TResult>(
  state: TixCommandState<TBody, TResult>,
  result?: TResult
): TixCommandState<TBody, TResult> {
  return {
    ...state,
    sending: false,
    result: result,
  };
}

export function setCommandFailed<TBody, TResult>(
  state: TixCommandState<TBody, TResult>,
  errorCode: string,
  errorMessage?: string
): TixCommandState<TBody, TResult> {
  return {
    ...state,
    sending: false,
    errorCode: errorCode,
    errorMessage: errorMessage,
  };
}
