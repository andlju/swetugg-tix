import { AccountInfo, AuthenticationResult, Configuration, EventMessage, EventMessageUtils, InteractionStatus, PopupRequest, PublicClientApplication, RedirectRequest } from "@azure/msal-browser";
import { from, Observable, ReplaySubject } from "rxjs";
import { filter, map, tap } from "rxjs/operators";
import { msalConfig } from "../auth-config";


class MsalService {
  private msalObj: PublicClientApplication;
  private eventSubject: ReplaySubject<EventMessage>;

  constructor(config: Configuration) {
    this.msalObj = new PublicClientApplication(config);
    this.eventSubject = new ReplaySubject<EventMessage>();
    this.msalObj.addEventCallback((evt) => this.handleCallback(evt));

    this.msalObj.handleRedirectPromise()
      .then(this.handleRedirectResponse)
      .catch((error) => {
        console.error(error);
      });
  }

  isInProgress$() : Observable<boolean> {
    return this.authEvents$().pipe(
      tap(msg => console.log(`Message ${msg.eventType} translates to ${EventMessageUtils.getInteractionStatusFromEvent(msg)}`)),
      map(msg => EventMessageUtils.getInteractionStatusFromEvent(msg) !== InteractionStatus.None)
    );
  }

  currentAccount$() : Observable<AccountInfo | null> {
    return this.isInProgress$().pipe(
      filter(inProgress => !inProgress),
      map(() => this.ensureAccountSelected())
    );
  }

  authEvents$() : Observable<EventMessage> {
    return this.eventSubject.asObservable();
  }

  isLoggedIn$() : Observable<boolean> {
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

  loginPopup(req: PopupRequest): Observable<AuthenticationResult> {
    return from(this.msalObj.loginPopup(req));
  }

  loginRedirect(req: RedirectRequest): Observable<void> {
    return from(this.msalObj.loginRedirect(req));
  }

  private ensureAccountSelected(): AccountInfo | null {
    let currentAccount = this.msalObj.getActiveAccount() || null;
    if (!currentAccount) {
      const allAccounts = this.msalObj.getAllAccounts();
      const firstAccount = allAccounts && allAccounts[0];
      if (firstAccount) {
        currentAccount = firstAccount;
        this.msalObj.setActiveAccount(currentAccount);
      }
    }
    return currentAccount;
  }
}

export const msalService = new MsalService(msalConfig);

