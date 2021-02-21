import { Configuration, ILoggerCallback, Logger, LogLevel } from '@azure/msal-browser';

const authClientId = process.env["NEXT_PUBLIC_AUTH_CLIENT_ID"];
const authTenant = process.env["NEXT_PUBLIC_AUTH_TENANT_NAME"];
const authPolicy = process.env["NEXT_PUBLIC_USER_FLOW"];

const loggerCallback : ILoggerCallback = (logLevel, message, containsPii) => {
  console.log(message);
}

// Config object to be passed to Msal on creation
export const msalConfig: Configuration = {
  auth: {
    clientId: authClientId || "",
    redirectUri: "/",
    postLogoutRedirectUri: "/",
    authority: `https://${authTenant}.b2clogin.com/${authTenant}.onmicrosoft.com/${authPolicy}`,
    knownAuthorities: [`${authTenant}.b2clogin.com`],
  },
  cache: {
    cacheLocation: "localStorage",
    storeAuthStateInCookie: false,
  },
  system: {
    loggerOptions: {
      logLevel: LogLevel.Warning,
      piiLoggingEnabled: false,
      loggerCallback: loggerCallback
    }
  }
};

// Add here scopes for id token to be used at MS Identity Platform endpoints.
export const loginRequest = {
  scopes: ["openid"]
};

// Add here the endpoints for MS Graph API services you would like to use.
export const graphConfig = {
  graphMeEndpoint: "https://graph.microsoft.com/v1.0/me"
};