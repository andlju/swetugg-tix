import { call, put, takeEvery } from "redux-saga/effects";
import { Activity } from "./activity.models";
import { getView } from "../src/services/view-fetcher.service";
import { buildUrl } from "../src/url-utils";

import { LoadActivityAction, LOAD_ACTIVITY, LOAD_ACTIVITY_COMPLETE } from "./activity.actions";

export function* loadActivityAction(action: LoadActivityAction) : Generator {
  const result = yield call(async () => {
    const data = await getView<Activity>(buildUrl(`/activities/${action.payload.activityId}?ownerId=${action.payload.ownerId}`), { revision: action.payload.revision });
    return data;
  });

  yield put({
    type: LOAD_ACTIVITY_COMPLETE,
    payload: { activity: result as Activity },
  });
}

export function* activitySaga() : Generator {
  yield takeEvery(LOAD_ACTIVITY, loadActivityAction);
}
