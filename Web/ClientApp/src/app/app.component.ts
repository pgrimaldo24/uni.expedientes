import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { Title } from '@angular/platform-browser';
import { TranslateService } from '@ngx-translate/core';
import { UserInfo } from 'angular-oauth2-oidc';
import { SecurityService } from '@services/security.service';
import { IdentityClaimsModel } from '@models/security.models';
import { AppConfigService } from '@services/app-config.service';
import { HostListener } from '@angular/core';
import {
  ApplicationMenu,
  applicationMenu
} from '@pages/shared/menu/models/application-menu';
import { SubMenuModel } from '@pages/shared/menu/models';
import { registerLocaleData } from '@angular/common';
import { CultureService } from './services/culture.service';
import { NavegatorService } from './services/navegator.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title: string;
  cssResource: SafeResourceUrl;
  authorizedMenu: ApplicationMenu = ApplicationMenu.initialize();
  isloading = true;
  version: string;

  showMainHeaderLogoUp = true;
  fixedMainHeader = false;

  constructor(
    private securitySvc: SecurityService,
    private titleService: Title,
    private router: Router,
    private translate: TranslateService,
    private sanitizer: DomSanitizer,
    private appService: AppConfigService,

    private cultureService: CultureService,
    private navegatorService: NavegatorService
  ) {
    this.translate.setDefaultLang('es');
    this.cssResource = this.sanitizer.bypassSecurityTrustResourceUrl(
      'assets/themes/unir.css'
    );
  }

  @HostListener('window:scroll', ['$event']) onScroll(): void {
    this.fixedMainHeader = this.checkScrollPosition();
  }

  ngOnInit(): void {
    this.configure();
    this.getVersion();
    this.listenCultureStudent();
    this.loadCultureNavegator();
  }

  checkScrollPosition(): boolean {
    return window.scrollY > 57;
  }

  private configure() {
    this.translate.get('common.appName').subscribe((title: string) => {
      this.title = title;
      this.titleService.setTitle(title);
    });
    this.securitySvc.configureSecurity(this.onConfigureSecurity);
  }

  onConfigureSecurity = (user: UserInfo): void => {
    this.removeLoading();
    this.loadMenuWithRole(user);
  };

  removeLoading(): void {
    document.body.className = '';
    this.isloading = false;
  }

  public get isLoggedIn(): boolean {
    return this.securitySvc.isLoggedIn;
  }
  public logoff(): void {
    this.isloading = true;
    this.securitySvc.logOut();
    this.authorizedMenu = ApplicationMenu.initialize();
    this.router.navigateByUrl('/');
  }

  loadMenuWithRole(user: UserInfo): void {
    this.authorizedMenu = ApplicationMenu.initialize();
    const userinfo = user as IdentityClaimsModel;
    const roles = [...userinfo.info.roles];
    this.loadMenuLeftSide(roles);
    this.loadConfigMenu(roles);
  }

  private loadConfigMenu(roles: string[]): void {
    for (const item of applicationMenu.config) {
      if (this.hasAtLeastOneRole(item.roles, roles)) {
        const menu = { ...item };
        menu.subMenu = [];
        for (const subMenu of item.subMenu) {
          if (this.hasAtLeastOneRole(subMenu.roles, roles)) {
            menu.subMenu.push(subMenu);
          }
        }
        if (this.hasSubMenus(item.subMenu, menu.subMenu)) {
          this.authorizedMenu.config.push(menu);
        }
      }
    }
  }

  private loadMenuLeftSide(roles: string[]): void {
    for (const item of applicationMenu.menu) {
      if (this.hasAtLeastOneRole(item.roles, roles)) {
        this.authorizedMenu.menu.push(item);
      }
    }
  }

  private hasSubMenus(om: SubMenuModel[], am: SubMenuModel[]): boolean {
    return om.length > 0 && am.length > 0;
  }

  private hasAtLeastOneRole(items: string[], roles: string[]): boolean {
    return items.some((value) => roles.includes(value));
  }

  updateMenu(): void {
    this.isloading = true;
    setTimeout(() => (this.isloading = false), 1);
  }

  getVersion(): void {
    this.appService.getVersion().subscribe((response) => {
      this.version = response;
    });
  }

  private loadCultureNavegator(): void {
    this.navegatorService.localeNavegator = NavegatorService.getNavegatorCulture();
  }

  private listenCultureStudent(): void {
    this.cultureService.getCulture$.subscribe((value: string) => {
      this.registerCulture(value);
    });
  }

  private registerCulture(culture: string): void {
    this.cultureService.locale = culture;
    if (this.cultureService.locale) {
      this.localeInitializer(
        CultureService.toLowerCase(this.cultureService.locale)
      );
    }
  }

  private localeInitializer(localeId: string): Promise<unknown> {
    return import(
      /* webpackExclude: /\.d\.ts$/ */
      `node_modules/@angular/common/locales/${CultureService.replaceCultureDefault(
        localeId
      )}.mjs`
    )
      .then((module) => registerLocaleData(module.default, 'es'))
      .catch(() => {
        console.error(
          `Not Found Culture => ${localeId}, by default was replace => es`
        );
        this.registerCulture('es');
      });
  }
}
