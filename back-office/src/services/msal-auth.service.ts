import {
  AccountInfo,
  AuthenticationResult,
  Configuration,
  EventMessage,
  EventMessageUtils,
  InteractionStatus,
  PopupRequest,
  PublicClientApplication,
  RedirectRequest,
  SilentRequest,
  SsoSilentRequest,
} from '@azure/msal-browser';
import { from, Observable, ReplaySubject } from 'rxjs';
import { distinctUntilChanged, filter, map, tap } from 'rxjs/operators';

import { msalConfig } from '../auth-config';

class MsalService {
  private instance: PublicClientApplication;
  private eventSubject: ReplaySubject<EventMessage>;

  private authEvent$?: Observable<EventMessage>;

  constructor(config: Configuration) {
    this.instance = new PublicClientApplication(config);
    this.eventSubject = new ReplaySubject<EventMessage>();
    this.instance.addEventCallback((evt) => this.handleCallback(evt));

    this.instance.handleRedirectPromise().catch((error) => {
      console.error(error);
    });
  }

  isInProgress$(): Observable<boolean> {
    return this.authEvents$().pipe(map((msg) => msg.eventType.indexOf('Start') >= 0));
  }

  currentAccount$(): Observable<AccountInfo | null> {
    return this.isInProgress$().pipe(
      filter((inProgress) => !inProgress),
      map(() => this.ensureAccountSelected()),
      distinctUntilChanged((acct1, acct2) => acct1?.username === acct2?.username)
    );
  }

  authEvents$(): Observable<EventMessage> {
    if (this.authEvent$) return this.authEvent$;
    return (this.authEvent$ = this.eventSubject.asObservable());
  }

  isLoggedIn$(): Observable<boolean> {
    return this.currentAccount$().pipe(map((acct) => !!acct));
  }

  handleCallback(evt: EventMessage) {
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
