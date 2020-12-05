import { all, call, fork } from "redux-saga/effects";
import { activitySaga } from "./activity.saga";
import { orderSaga } from "./order.saga";


export default function* rootSaga() : Generator {
  const sagas = [
    activitySaga,
    orderSaga
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