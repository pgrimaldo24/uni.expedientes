import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { MenuModel } from './models';
import { Router } from '@angular/router';
import { SecurityService } from '@services/security.service';
import { ApplicationMenu } from '@pages/shared/menu/models/application-menu';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.scss']
})
export class MenuComponent implements OnInit {
  @Input() authorizedMenu: ApplicationMenu;
  @Input() menuPrincipal: string;
  dropDownMenuByRoute = '';
  navbarOpen = false;
  menu: MenuModel;
  @Output() readonly onChange = new EventEmitter<MenuModel>();
  @Output() onlogout = new EventEmitter<boolean>();
  constructor(private router: Router, public security: SecurityService) {}
  ngOnInit(): void {
    this.menu = new MenuModel({ route: this.router.url });
    this.changeMenu(this.menu);
  }

  toggleNavbar(): void {
    this.navbarOpen = !this.navbarOpen;
  }
  changeMenu(menu: MenuModel): void {
    this.authorizedMenu.menu.forEach((element) => {
      if (element.route === menu.route) {
        if (!element.active) {
          this.onChange.emit(menu);
          element.active = true;
        }
      } else {
        element.active = false;
      }
    });
  }
  isNullOrEmpty(str: string): boolean {
    return str == null || str === '';
  }

  toggleSubMenu(menuRoute: string): void {
    this.dropDownMenuByRoute = menuRoute;
  }

  unSelected(event: FocusEvent): void {
    if (event.relatedTarget === null) this.dropDownMenuByRoute = '';
  }

  get menus(): MenuModel[] {
    return this.authorizedMenu.menu;
  }

  get configMenus(): MenuModel[] {
    return this.authorizedMenu.config;
  }

  chosenOption(menu: MenuModel): void {
    const subMenuActive = document.querySelectorAll('.sub-menu-options');

    subMenuActive.forEach((element: HTMLElement) => {
      if (element.classList.contains('is-active')) {
        this.dropDownMenuByRoute = '';
        element.classList.remove('is-active');
      }
    });
    this.changeMenu(menu);
  }
  logoff(): void {
    this.onlogout.emit(true);
  }
}
