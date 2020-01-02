const requestAdminType = 'REQUEST_ADMIN';
const receiveAdminType = 'RECEIVE_ADMIN';
const initialState = { isLoading: false };

export const actionCreators = {
    requestAdmin: () => async (dispatch, getState) => {
  
      dispatch({ type: requestAdminType });

      // const url = `api/SampleData/WeatherForecasts?startDateIndex=${startDateIndex}`;
      // const response = await fetch(url);
      // const forecasts = await response.json();
  
      dispatch({ type: receiveAdminType });
    }
  };
  
  export const reducer = (state, action) => {
    state = state || initialState;
  
    if (action.type === requestAdminType) {
      return {
        ...state,
        isLoading: true
      };
    }
  
    if (action.type === receiveAdminType) {
      return {
        ...state,
        isLoading: false
      };
    }
  
    return state;
  };
  