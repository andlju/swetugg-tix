import {createStore, AnyAction, combineReducers} from 'redux';
import {MakeStore, createWrapper, Context, HYDRATE} from 'next-redux-wrapper';
import activitiesReducer from './state/activities.reducer';

export interface State {
    tick: string;
}

const rootReducer = combineReducers({
    activities: activitiesReducer
});

// create your reducer
/*const reducer = (state: State = {tick: 'init'}, action: AnyAction) => {
    switch (action.type) {
        case HYDRATE:
            // Attention! This will overwrite client state! Real apps should use proper reconciliation.
            return {...state, ...action.payload};
        case 'TICK':
            return {...state, tick: action.payload};
        default:
            return state;
    }
};*/

// create a makeStore function
const makeStore: MakeStore<State> = (context: Context) => createStore(rootReducer);

// export an assembled wrapper
export const wrapper = createWrapper<State>(makeStore, {debug: true});