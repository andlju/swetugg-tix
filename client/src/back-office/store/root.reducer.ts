import { createStore, combineReducers, AnyAction } from 'redux';
import { MakeStore, createWrapper, Context, HYDRATE } from 'next-redux-wrapper';
import { activitiesReducer, ActivitiesState } from './activities.reducer';

const combinedReducer = combineReducers({
    activities: activitiesReducer
});

const rootReducer = (state: any, action: AnyAction) => {
    if (action.type === HYDRATE) {
        const nextState = {
            ...state, // use previous state
            ...action.payload, // apply delta from hydration
        };
        return nextState;
    } else {
        return combinedReducer(state, action);
    }
};

export default rootReducer; 