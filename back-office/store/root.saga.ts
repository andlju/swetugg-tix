import { all, call, fork } from "redux-saga/effects";
import { activitiesSaga } from "../components/activities/store/activities.saga";

export default function* rootSaga () {
  const sagas = [
    activitiesSaga
  ];

  yield all(sagas.map(saga =>
    fork(function* () {
      while (true) {
        try {
          yield call(saga);
          break;
        } catch (e) {
          console.log("Saga failure", e);
        }
      }
    }))
  );
}