import { Directive, Input, TemplateRef, ViewContainerRef } from '@angular/core';
import { SecurityService } from '@services/security.service';

@Directive({
  selector: '[appHasRoles]'
})
export class RolesDirective {
  hasRoles: boolean;
  constructor(
    private viewContainer: ViewContainerRef,
    private securityService: SecurityService,
    private templateRef: TemplateRef<unknown>
  ) {}

  @Input() set appHasRoles(roles: string[]) {
    const userRoles = this.securityService.userRoles();
    const hasRoles = [];

    for (const userRole of userRoles) {
      const found = roles.find((rol) => rol === userRole);
      if (found) {
        hasRoles.push(found);
      }
    }

    if (hasRoles.length > 0) {
      this.hasRoles = true;
      this.viewContainer.clear();
      this.viewContainer.createEmbeddedView(this.templateRef);
    }
  }

  @Input() set appHasRolesElse(ref: TemplateRef<unknown>) {
    if (!this.hasRoles) {
      this.viewContainer.clear();
      this.viewContainer.createEmbeddedView(ref);
    }
  }
}
