import { MenuModel } from '@pages/shared/menu/models/index';
import { keys } from '@src/keys';
export class ApplicationMenu {
  menu: MenuModel[];
  config: MenuModel[];
  static initialize(): ApplicationMenu {
    const value = new ApplicationMenu();
    value.menu = [];
    value.config = [];
    return value;
  }
}
export const applicationMenu: ApplicationMenu = {
  menu: [
    new MenuModel({
      title: 'Expedientes',
      route: '/ExpedienteAlumno/List',
      roles: [keys.ADMIN_ROLE, keys.GESTOR_ROLE, keys.LECTOR_ROLE],
      order: 1
    }),
    new MenuModel({
      title: 'Seguimiento',
      route: '/SeguimientoExpediente/List',
      roles: [keys.ADMIN_ROLE, keys.GESTOR_ROLE],
      order: 2
    })
  ],
  config: [
    new MenuModel({
      title: '',
      route: '/configuracion',
      iconClass: 'fas fa-cog',
      order: 1,
      roles: [keys.ADMIN_ROLE, keys.GESTOR_ROLE],
      subMenu: [
        {
          title: 'Requisitos',
          route: '/requisitos',
          roles: [keys.ADMIN_ROLE, keys.GESTOR_ROLE]
        },
        {
          title: 'Comportamientos',
          route: '/comportamientos',
          roles: [keys.ADMIN_ROLE, keys.GESTOR_ROLE]
        }
      ]
    })
  ]
};
