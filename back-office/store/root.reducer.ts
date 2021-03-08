import { combineReducers } from 'redux';
import { combineEpics } from 'redux-observable';
import { activitiesReducer } from './activities/activities.reducer';
import { activitiesEpic } from './activities/activities.epic';
import { authReducer } from './auth/auth.reducer';
import { authEpic } from './auth/auth.epic';

export const rootReducer = combineReducers({
    activities: activitiesReducer,
    auth: authReducer
});

export const rootEpic = combineEpics(
    activitiesEpic,
    authEpic
);
