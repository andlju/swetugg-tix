export interface TixError {
  code: string;
  message: string;
}

export interface TixState<TModel> {
  fetching: boolean;
  saving: boolean;
  current?: TModel;
  error?: TixError;
}

export function setFetching<TModel>(state: TixState<TModel>): TixState<TModel> {
  return {
    ...state,
    fetching: true,
  };
}

export function setSaving<TModel>(state: TixState<TModel>): TixState<TModel> {
  return {
    ...state,
    saving: true,
  };
}

export function setState<TModel>(state: TixState<TModel>, model?: TModel): TixState<TModel> {
  return {
    ...state,
    error: undefined,
    fetching: false,
    saving: false,
    current: model,
  };
}

export function setFailed<TModel>(state: TixState<TModel>, code: string, message?: string): TixState<TModel> {
  return {
    ...state,
    error: {
      code,
      message: message || code,
    },
    fetching: false,
    saving: false,
  };
}

export function initState<TModel>(): TixState<TModel> {
  return {
    fetching: false,
    saving: false,
  };
}
