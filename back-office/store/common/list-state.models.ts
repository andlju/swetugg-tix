export interface TixListState<TModel> {
  fetching: boolean;
  models: {
    [key: string]: { loading: boolean } & TModel;
  };
  visibleIds: string[];
  keyFunc: (model: TModel) => string;
  errorCode?: string;
  errorMessage?: string;
}

export function initListState<TModel>(keyFunc: (model: TModel) => string): TixListState<TModel> {
  return { fetching: false, models: {}, visibleIds: [], keyFunc };
}

export function loadListItem<TModel>(state: TixListState<TModel>, key: string): TixListState<TModel> {
  return {
    ...state,
    models: {
      ...state.models,
      [key]: { ...state.models[key], loading: true }
    },
    fetching: true,
  };
}

export function loadList<TModel>(state: TixListState<TModel>): TixListState<TModel> {
  return {
    ...state,
    fetching: true,
  };
}

export function loadListComplete<TModel>(state: TixListState<TModel>, list: TModel[]): TixListState<TModel> {
  return {
    ...state,
    models: list.reduce((models, model) => ({ ...models, [state.keyFunc(model)]: { ...model, loading: false } }), state.models),
    visibleIds: list.map(state.keyFunc),
    fetching: false,
  };
}

export function loadListFailed<TModel>(
  state: TixListState<TModel>,
  errorCode: string,
  errorMessage: string
): TixListState<TModel> {
  return {
    ...state,
    fetching: false,
    errorCode: errorCode,
    errorMessage: errorMessage,
  };
}

export function listVisible<TModel>(state: TixListState<TModel>): TModel[] {
  return state.visibleIds.map((id) => state.models[id]);
}
