import { AccountInfo, AuthenticationResult, Configuration, EventMessage, EventMessageUtils, InteractionStatus, PopupRequest, PublicClientApplication, RedirectRequest, SilentRequest, SsoSilentRequest } from "@azure/msal-browser";
import { from, Observable, ReplaySubject } from "rxjs";
import { filter, map, tap } from "rxjs/operators";
import { msalConfig } from "../auth-config";


class MsalService {
  private instance: PublicClientApplication;
  private eventSubject: ReplaySubject<EventMessage>;

  constructor(config: Configuration) {
    this.instance = new PublicClientApplication(config);
    this.eventSubject = new ReplaySubject<EventMessage>();
    this.instance.addEventCallback((evt) => this.handleCallback(evt));

    this.instance.handleRedirectPromise()
      .then(this.handleRedirectResponse)
      .catch((error) => {
        console.error(error);
      });
  }

  isInProgress$(): Observable<boolean> {
    return this.authEvents$().pipe(
      map(msg => EventMessageUtils.getInteractionStatusFromEvent(msg) !== InteractionStatus.None)
    );
  }

  currentAccount$(): Observable<AccountInfo | null> {
    return this.isInProgress$().pipe(
      filter(inProgress => !inProgress),
      map(() => this.ensureAccountSelected())
    );
  }

  authEvents$(): Observable<EventMessage> {
    return this.eventSubject.asObservable();
  }

  isLoggedIn$(): Observable<boolean> {
    return this.currentAccount$().pipe(
      map(acct => !!acct)
    );
  }

  handleRedirectResponse(resp: any) {
    console.log("Here's the redirect response", resp);
  }

  handleCallback(evt: EventMessage) {
    console.log("Publishing event", evt);
    this.eventSubject.next(evt);
  }

  ssoSilent(req: SsoSilentRequest): Observable<AuthenticationResult> {
    return from(this.instance.ssoSilent(req));
  }

  loginPopup(req: PopupRequest): Observable<AuthenticationResult> {
    return from(this.instance.loginPopup(req));
  }

  loginRedirect(req: RedirectRequest): Observable<void> {
    return from(this.instance.loginRedirect(req));
  }

  acquireTokenPopup(request: PopupRequest): Observable<AuthenticationResult> {
    return from(this.instance.acquireTokenPopup(request));
  }

  acquireTokenRedirect(request: RedirectRequest): Observable<void> {
    return from(this.instance.acquireTokenRedirect(request));
  }

  acquireTokenSilent(silentRequest: SilentRequest): Observable<AuthenticationResult> {
    return from(this.instance.acquireTokenSilent(silentRequest));
  }

  logout(): Observable<void> {
    return from(this.instance.logout());
  }

  private ensureAccountSelected(): AccountInfo | null {
    let currentAccount = this.instance.getActiveAccount() || null;
    if (!currentAccount) {
      const allAccounts = this.instance.getAllAccounts();
      const firstAccount = allAccounts && allAccounts[0];
      if (firstAccount) {
        currentAccount = firstAccount;
        this.instance.setActiveAccount(currentAccount);
      }
    }
    return currentAccount;
  }
}

export const msalService = new MsalService(msalConfig);

