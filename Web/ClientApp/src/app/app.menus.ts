import { MenuModel } from '@pages/shared/menu/models';
import { AppRoutesRoot } from './app-routing.module';

export const menuApp: { [key: string]: MenuModel } = {
  [AppRoutesRoot.expedientes]: new MenuModel({
    title: 'Expedientes',
    route: AppRoutesRoot.expedientes
  })
};
