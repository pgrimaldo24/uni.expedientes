import { AuthConfig, OAuthService, UserInfo } from 'angular-oauth2-oidc';
import { JwksValidationHandler } from 'angular-oauth2-oidc-jwks';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

import { AppConfigService } from './app-config.service';
import { IdentityClaimsModel } from '@models/security.models';

@Injectable({
  providedIn: 'root'
})
export class SecurityService {
  constructor(
    private oauthService: OAuthService,
    private appConfigService: AppConfigService,
    private router: Router
  ) {}

  get getProfile(): IdentityClaimsModel | null {
    const claims = this.oauthService.getIdentityClaims() as IdentityClaimsModel;
    if (!claims) {
      return null;
    }
    return claims;
  }

  public configureSecurity(fn: (user: UserInfo) => void): void {
    const config = this.appConfigService.getConfig();
    const authConfig: AuthConfig = {
      issuer: config.securityUri,
      loginUrl: config.securityUri + '/authorize',
      logoutUrl: config.securityUri + '/external-logout',
      redirectUri: window.location.origin + '/index.html',
      clientId: config.clientId,
      scope: 'openid',
      silentRefreshRedirectUri: window.location.origin + '/silent_refresh.html'
    };
    this.oauthService.configure(authConfig);
    this.oauthService.tokenValidationHandler = new JwksValidationHandler();
    this.oauthService.setupAutomaticSilentRefresh();
    this.oauthService.events.subscribe((value) => {
      if (value.type === 'silently_refreshed') {
        this.oauthService.loadUserProfile().then((user: UserInfo) => {
          fn(user);
        });
      }
    });

    this.oauthService.loadDiscoveryDocumentAndLogin().then(() => {
      this.oauthService.tryLogin().then(() => {
        if (!this.oauthService.hasValidAccessToken()) {
          this.oauthService.initImplicitFlow();
        } else {
          this.oauthService.loadUserProfile().then((user: UserInfo) => {
            fn(user);
            const returnUrl = localStorage.getItem('returnUrl');
            if (returnUrl) {
              localStorage.removeItem('returnUrl');
              this.router.navigateByUrl(returnUrl);
            }
          });
        }
      });
    });
  }

  logOut(): void {
    this.oauthService.logOut();
  }

  public get isLoggedIn(): boolean {
    return this.oauthService.hasValidAccessToken();
  }

  public userRoles(): string[] {
    const claims = this.oauthService.getIdentityClaims() as IdentityClaimsModel;
    if (!claims) {
      return [];
    }
    return claims.roles == null ? [] : claims.roles;
  }
}
