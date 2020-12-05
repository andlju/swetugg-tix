import { combineReducers, AnyAction, Reducer } from 'redux';
import { HYDRATE } from 'next-redux-wrapper';
import { activityHydrator, activityReducer } from './activity.reducer';
import { orderReducer, orderHydrator } from './order.reducer';
import { ActivityAction } from './activity.actions';
import { OrderAction } from './order.actions';

const combinedReducer = combineReducers({
    activity: activityReducer,
    order: orderReducer
});

export type RootState = ReturnType<typeof combinedReducer>
export type RootAction = ActivityAction | OrderAction;

const rootReducer : Reducer<RootState, RootAction | AnyAction> = (state, action) => {
    if (action.type === HYDRATE) {
        const nextState : RootState = {
            ...state, // use previous state
            activity: activityHydrator(state?.activity, action.payload.activity),
            order: orderHydrator(state?.order, action.payload.order)
        };
        return nextState;
    } else {
        return combinedReducer(state, action as RootAction);
    }
};

export default rootReducer; 