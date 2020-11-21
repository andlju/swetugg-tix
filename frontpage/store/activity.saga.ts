import { call, put, takeEvery } from "redux-saga/effects";
import { getView } from "../src/services/view-fetcher.service";
import { buildUrl } from "../src/url-utils";

import { LoadActivityAction, LOAD_ACTIVITY, LOAD_ACTIVITY_COMPLETE } from "./activity.actions";

export function* loadActivityAction(action: LoadActivityAction) {
  const result = yield call(async () => {
    const data = await getView(buildUrl(`/activities/${action.payload.activityId}`), { revision: action.payload.revision });
    return data;
  });

  yield put({
    type: LOAD_ACTIVITY_COMPLETE,
    payload: { activity: result },
  });
}

export function* activitySaga() {
  yield takeEvery(LOAD_ACTIVITY, loadActivityAction);
}
