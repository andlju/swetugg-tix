import { applyMiddleware, createStore, Store} from 'redux'
import createSagaMiddleware, { Task } from 'redux-saga'
import {createWrapper} from 'next-redux-wrapper'

import rootReducer from './root.reducer'
import rootSaga from './root.saga'

export interface SagaStore extends Store {
  sagaTask: Task;
}

const makeStore = () => {
  const sagaMiddleware = createSagaMiddleware();
  const store = createStore(
    rootReducer,
    applyMiddleware(sagaMiddleware),
  );

  (store as SagaStore).sagaTask = sagaMiddleware.run(rootSaga);

  return store
}

const wrapper = createWrapper(makeStore)

export default wrapper