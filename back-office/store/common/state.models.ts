
export interface TixError {
  code: string,
  message: string
}

export interface TixState<TModel> {
  fetching: boolean;
  updating: boolean;
  current?: TModel;
  error?: TixError;
}

export function setFetching<TModel>(state: TixState<TModel>): TixState<TModel> {
  return {
    ...state,
    fetching: true
  }
}

export function setUpdating<TModel>(state: TixState<TModel>): TixState<TModel> {
  return {
    ...state,
    updating: true
  }
}

export function setState<TModel>(state: TixState<TModel>, model?: TModel): TixState<TModel> {
  return {
    ...state,
    error: undefined,
    fetching: false,
    updating: false,
    current: model,
  }
}

export function setError<TModel>(state: TixState<TModel>, code: string, message?: string): TixState<TModel> {
  return {
    ...state,
    error: { 
      code, 
      message: message || code
    },
    fetching: false,
    updating: false
  }
}
