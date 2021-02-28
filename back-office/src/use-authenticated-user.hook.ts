import { AccountInfo, InteractionType } from "@azure/msal-browser";
import { MsalAuthenticationResult, useMsal, useMsalAuthentication } from "@azure/msal-react";
import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { authenticate, setAccessToken } from "../components/activities/store/auth.actions";
import { RootState } from "../store/store";

interface AuthenticatedUserResult extends MsalAuthenticationResult {
  token: string | null;
}

export function useAuthenticatedUser(scopes: string[]): AuthenticatedUserResult {

  const request = {
    scopes: ["profile", "openid", ...scopes]
  };

  const [token, setToken] = useState<string | null>(null);
  const { instance, inProgress, logger, accounts } = useMsal();
  const { login, result, error } = useMsalAuthentication(InteractionType.Popup);

  const dispatch = useDispatch();
  
  useEffect(() => {

    const getToken = async () => {
      // If we are not yet logged in there's no point in continuing
      if (!accounts[0])
        return;
      
      // Set the active account to the first one
      instance.setActiveAccount(accounts[0] as AccountInfo);

      // Try to get a token silently
      let res = await instance.acquireTokenSilent(request);
      if (!res.accessToken) {
        try {
          // Couldn't get the token silently - let's try a popup
          res = await instance.acquireTokenPopup(request);
        } catch (err) {
          // If we failed because of a blocked popup, let's 
          // try to do it with a redirect instead
          const errStr = String(err);

          logger.error('Popup was blocked. Trying redirect.');
          if (errStr.indexOf('popup_window_error') > -1) {
            await instance.acquireTokenRedirect(request);
          }
        }
      }
      // Have we got our access token? If so, let's set it
      if (res.accessToken) {
        setToken(res.accessToken);
        // Also store it in the Redux state
        dispatch(setAccessToken(res.accessToken));
      }
    };

    getToken();
  }, [accounts]);

  return {
    login,
    result,
    error,
    token
  };
}