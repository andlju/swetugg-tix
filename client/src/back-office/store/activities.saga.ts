import { Activity } from "..";
import { buildUrl } from "../../url-utils";
import { ActivitiesAction, LOAD_ACTIVITY, LOAD_ACTIVITIES, LOAD_ACTIVITIES_COMPLETE, LOAD_ACTIVITIES_FAILED } from "./activities.actions";

const activitiesLoadMiddleware = (dispatch : React.Dispatch<ActivitiesAction>) => (action: ActivitiesAction): void => {
  switch (action.type) {
    case LOAD_ACTIVITY:
      setTimeout(() => dispatch({type: LOAD_ACTIVITIES_COMPLETE, payload: { activities: [] }}), 1000);
      break;
    case LOAD_ACTIVITIES:
      fetch(buildUrl('/activities')).then(async resp => {
        const data = await resp.json() as Activity[]
        dispatch({type: LOAD_ACTIVITIES_COMPLETE, payload : { activities: data }});
      }).catch(err => dispatch({ type: LOAD_ACTIVITIES_FAILED, payload: { errorCode: "LoadinFailed", errorMessage: "Failed to load activities"}}));
      break;
  }
  return dispatch(action);
};

export { activitiesLoadMiddleware };