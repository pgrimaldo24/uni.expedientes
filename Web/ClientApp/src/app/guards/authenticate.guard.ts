import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  Router,
  RouterStateSnapshot
} from '@angular/router';

import { OAuthService } from 'angular-oauth2-oidc';
import { SecurityService } from '@services/security.service';

@Injectable()
export class AuthenticateGuard implements CanActivate {
  constructor(
    private securityOAuthService: OAuthService,
    private router: Router,
    private securityService: SecurityService
  ) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): boolean {
    const roles = route.data.roles;
    if (this.securityOAuthService.hasValidAccessToken()) {
      if (roles != null && roles.length > 0) {
        const hasRoles = this.hasRoles(roles);
        if (!hasRoles) {
          this.router.navigate(['/user-without-privileges']);
          return false;
        }
      }

      localStorage.setItem('returnUrl', state.url);
      return true;
    }
    this.router.navigate(['/user-without-privileges']);
    return false;
  }

  hasRoles(roles: string[]): boolean {
    const userRoles = this.securityService.userRoles();
    const hasRoles = [];

    for (const userRole of userRoles) {
      const found = roles.find((rol) => rol === userRole);
      if (found) {
        hasRoles.push(found);
      }
    }
    return hasRoles.length > 0;
  }
}
