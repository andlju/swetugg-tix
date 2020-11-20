
import { delay, put, takeEvery, call } from 'redux-saga/effects';
import { Activity } from '..';
import { buildUrl } from '../../url-utils';
import { LoadActivityAction, LOAD_ACTIVITIES, LOAD_ACTIVITIES_COMPLETE, LOAD_ACTIVITY } from './activities.actions';

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
    console.log("getting", action.payload.activityId);
    const resp = await fetch(buildUrl(`/activities/${action.payload.activityId}`));
    const data = await resp.json() as Activity;
    console.log("got activity", data);
    return data;
  });

  yield put({
    type: LOAD_ACTIVITIES_COMPLETE,
    payload: { activities: [result] },
  });

}

function* rootSaga() {
  yield takeEvery(LOAD_ACTIVITIES, loadActivitiesAction);
  yield takeEvery(LOAD_ACTIVITY, loadActivityAction);
}

export default rootSaga;