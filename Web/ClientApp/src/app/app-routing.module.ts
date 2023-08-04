import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { QuicklinkStrategy } from 'ngx-quicklink';

import { HomeComponent } from '@pages/shared/home/home.component';
import { NoPrivilegesComponent } from '@pages/shared/no-privileges/no-privileges.component';

export enum AppRoutesRoot {
  home = '',
  userNotPrivileges = 'user-without-privileges',
  expedienteAlumno = 'ExpedienteAlumno',
  seguimientoExpediente = 'SeguimientoExpediente',
  alumno = 'Alumno',
  requisitos = 'requisitos',
  comportamientos = 'comportamientos'
}

const routes: Routes = [
  { path: AppRoutesRoot.home, component: HomeComponent, pathMatch: 'full' },
  { path: AppRoutesRoot.userNotPrivileges, component: NoPrivilegesComponent },
  {
    path: AppRoutesRoot.expedienteAlumno,
    loadChildren: () =>
      import('./pages/expediente-alumno/expediente.module').then(
        (m) => m.ExpedienteModule
      )
  },
  {
    path: AppRoutesRoot.seguimientoExpediente,
    loadChildren: () =>
      import('./pages/seguimientos/seguimientos.module').then(
        (m) => m.SeguimientosModule
      )
  },
  {
    path: AppRoutesRoot.alumno,
    loadChildren: () =>
      import('./pages/alumno/alumno.module').then((m) => m.AlumnoModule)
  },
  {
    path: AppRoutesRoot.requisitos,
    loadChildren: () =>
      import('@pages/requisitos/requisitos.module').then(
        (m) => m.RequisitosModule
      )
  },
  {
    path: AppRoutesRoot.comportamientos,
    loadChildren: () =>
      import('@pages/comportamientos/comportamientos.module').then(
        (m) => m.ComportamientosModule
      )
  },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes, {
      preloadingStrategy: QuicklinkStrategy,
      relativeLinkResolution: 'legacy'
    })
  ],
  exports: [RouterModule]
})
export class AppRoutingModule {}
