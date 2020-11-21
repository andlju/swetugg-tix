import { applyMiddleware, createStore, Store} from 'redux'
import createSagaMiddleware, { Task } from 'redux-saga'
import {createWrapper} from 'next-redux-wrapper'

import rootReducer, { RootState } from './root.reducer'
import { ActivitiesState } from './activities.reducer';
import rootSaga from './root.saga'

export interface State extends RootState {
  server: {
    activities: ActivitiesState
  }
}

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