import { combineReducers } from 'redux';
import { HYDRATE } from 'next-redux-wrapper';
import { activitiesHydrator, activitiesReducer } from '../components/activities/store/activities.reducer';
import { ActivitiesAction } from '../components/activities/store/activities.actions';


const combinedReducer = combineReducers({
    activities: activitiesReducer
});

export type RootState = ReturnType<typeof combinedReducer>
export type RootAction = ActivitiesAction;

const rootReducer = (state: RootState, action: RootAction | any): RootState => {
    if (action.type === HYDRATE) {
        const nextState = {
            ...state, // use previous state
            activities: activitiesHydrator(state.activities, action.payload.activities),
        };
        return nextState;
    } else {
        return combinedReducer(state, action);
    }
};

export default rootReducer; 