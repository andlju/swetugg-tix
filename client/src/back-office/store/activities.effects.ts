import { ActivitiesAction, LOAD_ACTIVITIES, LOAD_ACTIVITIES_COMPLETE } from "./activities.actions";

const activitiesLoadMiddleware = (dispatch : React.Dispatch<ActivitiesAction>) => (action: ActivitiesAction): void => {
  switch (action.type) {
    case LOAD_ACTIVITIES:
      setTimeout(() => dispatch({type: LOAD_ACTIVITIES_COMPLETE, payload: { activities: [] }}), 1000);
      break;
  }
  return dispatch(action);
};

export { activitiesLoadMiddleware };