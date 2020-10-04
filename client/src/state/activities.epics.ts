import { ActivitiesActionTypes, LIST_LOAD, loadActivitiesFailure, loadActivitiesSuccess } from './activities.actions'
import { filter, switchMap, map, catchError } from 'rxjs/operators';
import { from, of } from 'rxjs';
import { Epic } from 'redux-observable';
import axios from 'axios';
import { ActivitiesState } from './activity.models';
import { buildUrl } from '../url-utils';

const list: Epic<
  ActivitiesActionTypes,
  ActivitiesActionTypes,
  ActivitiesState> = (action$, store) =>
  action$
    .pipe(filter(action => action.type === LIST_LOAD))
    .pipe(switchMap(() =>
          from(axios.get(buildUrl('/activities')))
              .pipe(map(response => response.data))
      ))
      .pipe(map(loadActivitiesSuccess))
      .pipe(catchError(() => of(loadActivitiesFailure())));

// Epics
export default {
  list
};

/*
  user: action$ => action$
      .pipe(filter(action => action.type === USER_LOAD))
      .pipe(switchMap(({ payload }) =>
          from(axios.get(`https://api.github.com/user/${payload.id}`))
              .pipe(map(response => response.data))
      ))
      .pipe(map(loadUserSuccess))
      .pipe(catchError(() => of(loadUserFailure())))
}
*/