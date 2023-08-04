import { Component } from '@angular/core';
import { IdentityClaimsModel } from '@src/app/models/security.models';
import { OAuthService } from 'angular-oauth2-oidc';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {
  constructor(private oauthService: OAuthService) {}
  public get isLoggedIn(): boolean {
    return this.oauthService.hasValidAccessToken();
  }
  public get name(): string {
    const claims = this.oauthService.getIdentityClaims() as IdentityClaimsModel;
    if (!claims) {
      return null;
    }
    return claims.given_name;
  }
}
