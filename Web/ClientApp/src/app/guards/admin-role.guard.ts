import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  Router,
  RouterStateSnapshot
} from '@angular/router';

import { SecurityService } from '@services/security.service';
import { keys } from '@src/keys';

@Injectable({
  providedIn: 'root'
})
export class AdminRoleGuard implements CanActivate {
  constructor(
    private securityService: SecurityService,
    private router: Router
  ) {}

  redirectPrivileges(permisos: boolean): void {
    if (!permisos) {
      this.router.navigate(['/user-without-privileges']);
    }
  }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): boolean {
    const roles = this.securityService.userRoles();
    const permisos = roles.some((resp) => resp === keys.ADMIN_ROLE);
    this.redirectPrivileges(permisos);
    localStorage.setItem('returnUrl', state.url);
    return permisos;
  }
}
