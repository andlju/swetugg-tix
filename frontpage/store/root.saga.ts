import { all, call, fork } from "redux-saga/effects";
import { activitySaga } from "./activity.saga";


export default function* rootSaga () {
  const sagas = [
    activitySaga
  ];

  yield all(sagas.map(saga =>
    fork(function* () {
      while (true) {
        try {
          yield call(saga)
          break
        } catch (e) {
          console.log(e)
        }
      }
    }))
  );
}