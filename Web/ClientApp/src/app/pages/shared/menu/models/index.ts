export class MenuModel {
  constructor(data: Partial<MenuModel>) {
    this.active = false;
    this.order = 0;
    this.iconClass = '';
    Object.assign(this, data);
  }
  title: string;
  route: string;
  roles: string[];
  iconClass?: string;
  active?: boolean;
  order?: number;
  subMenu?: SubMenuModel[];
}

export class SubMenuModel {
  constructor(data: Partial<SubMenuModel>) {
    this.active = false;
    this.iconClass = '';
    Object.assign(this, data);
  }
  roles: string[];
  title?: string;
  route?: string;
  iconClass?: string;
  active?: boolean;
}
