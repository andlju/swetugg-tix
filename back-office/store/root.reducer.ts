import { combineReducers } from 'redux';
import { combineEpics } from 'redux-observable';

import { activitiesEpic } from './activities/activities.epic';
import { activitiesReducer } from './activities/activities.reducer';
import { authEpic } from './auth/auth.epic';
import { authReducer } from './auth/auth.reducer';
import { organizationsEpic } from './organizations/organizations.epic';
import { organizationsReducer } from './organizations/organizations.reducer';

export const rootReducer = combineReducers({
  activities: activitiesReducer,
  auth: authReducer,
  organizations: organizationsReducer,
});

export const rootEpic = combineEpics(activitiesEpic, authEpic, organizationsEpic);
