import { combineReducers } from 'redux';
import { combineEpics } from 'redux-observable';

import { activitiesEpic } from './activities/activities.epic';
import { activitiesReducer } from './activities/activities.reducer';
import { authEpic } from './auth/auth.epic';
import { authReducer } from './auth/auth.reducer';
import { organizationsEpic } from './organizations/organizations.epic';
import { organizationsReducer } from './organizations/organizations.reducer';
import { rolesEpic } from './roles/roles.epic';
import { rolesReducer } from './roles/roles.reducer';
import { usersEpic } from './users/users.epic';
import { usersReducer } from './users/users.reducer';

export const rootReducer = combineReducers({
  activities: activitiesReducer,
  auth: authReducer,
  organizations: organizationsReducer,
  roles: rolesReducer,
  users: usersReducer,
});

export const rootEpic = combineEpics(activitiesEpic, authEpic, organizationsEpic, rolesEpic, usersEpic);
