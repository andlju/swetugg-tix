import { call, put, takeEvery } from "redux-saga/effects";
import { getView } from "../../../src/services/view-fetcher.service";
import { buildUrl } from "../../../src/url-utils";
import { Activity } from "../activity.models";
import { LoadActivityAction, LOAD_ACTIVITIES, LOAD_ACTIVITIES_COMPLETE, LOAD_ACTIVITY } from "./activities.actions";

function* loadActivitiesAction() {
  const result = yield call(async () => {
    const resp = await fetch(buildUrl('/activities'));
    const data = await resp.json() as Activity[];
    return data;
  });

  yield put({
    type: LOAD_ACTIVITIES_COMPLETE,
    payload: { activities: result },
  });
}

function* loadActivityAction(action: LoadActivityAction) {
  const result = yield call(async () => {
    const data = await getView(buildUrl(`/activities/${action.payload.activityId}`), { revision: action.payload.revision });
    return data;
  });

  yield put({
    type: LOAD_ACTIVITIES_COMPLETE,
    payload: { activities: [result] },
  });
}

export function* activitiesSaga() {
  yield takeEvery(LOAD_ACTIVITIES, loadActivitiesAction);
  yield takeEvery(LOAD_ACTIVITY, loadActivityAction);
}
