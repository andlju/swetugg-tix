import { combineReducers, AnyAction } from 'redux';
import { HYDRATE } from 'next-redux-wrapper';
import { activitiesHydrator, activitiesReducer } from './activities.reducer';

const combinedReducer = combineReducers({
    activities: activitiesReducer
});

export type RootState = ReturnType<typeof combinedReducer>

const rootReducer = (state: RootState, action: AnyAction): RootState => {
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