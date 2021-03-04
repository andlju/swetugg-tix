import { combineReducers } from 'redux';
import { combineEpics } from 'redux-observable';
import { activitiesReducer } from '../components/activities/store/activities.reducer';
import { activitiesEpic } from '../components/activities/store/activities.epic';
import { authReducer } from '../components/auth/store/auth.reducer';
import { authEpic } from '../components/auth/store/auth.epic';

export const rootReducer = combineReducers({
    activities: activitiesReducer,
    auth: authReducer
});

export const rootEpic = combineEpics(
    activitiesEpic,
    authEpic
);
