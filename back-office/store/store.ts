import { createStore, applyMiddleware, Action } from 'redux'
import { combineEpics, createEpicMiddleware, Epic } from 'redux-observable';

import { rootEpic, rootReducer } from './root.reducer'

const epicMiddleware = createEpicMiddleware();

const configureStore = (uiEpic: Epic<Action, Action, RootState>) => {

  const completeEpic = combineEpics(rootEpic, uiEpic);

  const store = createStore(
    rootReducer,
    applyMiddleware(epicMiddleware)
  );

  epicMiddleware.run(completeEpic);

  return store;
}

export type RootState = ReturnType<typeof rootReducer>;

export default configureStore;