import { Epic } from 'redux-observable';
import { EMPTY, of, throwError } from 'rxjs';
import { catchError, filter, map, mergeMap, take, tap } from 'rxjs/operators';
import { isOfType } from 'typesafe-actions';

import { msalService } from '../../src/services/msal-auth.service';
import { RootState } from '../store';
import { AuthAction, AuthActionTypes, getScopes, InteractionKind, setAccessToken } from './auth.actions';
import { whenLoggedIn$ } from './auth.epic';

export const getScopesSilentEpic: Epic<AuthAction, AuthAction, RootState> = (action$, state$) =>
  whenLoggedIn$(action$.pipe(filter(isOfType(AuthActionTypes.GET_SCOPES)))).pipe(
    filter((action) => action.payload.interactionKind === InteractionKind.SILENT),
    take(1), // Only try this once
    tap(() => console.log(`Attempting Silent Token Acquire`)),
    mergeMap((action) =>
      msalService.acquireTokenSilent({ scopes: ['profile', 'openid', ...action.payload.scopes] }).pipe(
        map((res) => {
          if (!res.accessToken) {
            throw { code: 'NoAccessToken' };
          }
          return setAccessToken(res.accessToken);
        }),
        catchError((err) => {
          return of(getScopes(action.payload.scopes, InteractionKind.POPUP));
        })
      )
    )
  );

export const getScopesPopupEpic: Epic<AuthAction, AuthAction, RootState> = (action$, state$) =>
  whenLoggedIn$(action$.pipe(filter(isOfType(AuthActionTypes.GET_SCOPES)))).pipe(
    filter((action) => action.payload.interactionKind === InteractionKind.POPUP),
    tap(() => console.log(`Attempting Popup Token Acquire`)),
    mergeMap((action) =>
      msalService.acquireTokenPopup({ scopes: ['profile', 'openid', ...action.payload.scopes] }).pipe(
        map((res) => {
          if (!res.accessToken) {
            throw { code: 'NoAccessToken' };
          }
          return setAccessToken(res.accessToken);
        }),
        catchError((err) => {
          const errString = String(err);
          if (errString.indexOf('popup_window_error') >= 0) {
            // Popup not OK - let's do it with a redirect instead.
            return of(getScopes(action.payload.scopes, InteractionKind.REDIRECT));
          }
          // Other error
          return throwError({ code: 'AcquireTokenFailed', message: err });
        })
      )
    )
  );

export const getScopesRedirectEpic: Epic<AuthAction, AuthAction, RootState> = (action$, state$) =>
  whenLoggedIn$(action$.pipe(filter(isOfType(AuthActionTypes.GET_SCOPES)))).pipe(
    map((action) => action),
    filter((action) => action.payload.interactionKind === InteractionKind.REDIRECT),
    tap(() => console.log(`Attempting Redirect Token Acquire`)),
    mergeMap((action) => {
      msalService.acquireTokenRedirect({ scopes: ['profile', 'openid', ...action.payload.scopes] });
      return EMPTY;
    }),
    catchError((err) => {
      // Other error
      return throwError({ code: 'AcquireTokenFailed', message: err });
    })
  );
