import { createStore, applyMiddleware } from 'redux'
import { createEpicMiddleware } from 'redux-observable';

import { rootEpic, rootReducer } from './root.reducer'

const epicMiddleware = createEpicMiddleware();

function configureStore() {
  const store = createStore(
    rootReducer,
    applyMiddleware(epicMiddleware)
  );

  epicMiddleware.run(rootEpic);

  return store;
}

const store = configureStore();

export type RootState = ReturnType<typeof rootReducer>;

export default store;