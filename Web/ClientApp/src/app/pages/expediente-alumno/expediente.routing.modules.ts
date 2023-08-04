import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PuedeTitularseComponent } from '@pages/expediente-alumno/puede-titularse/puede-titularse.component';
import { ListComponent } from '@pages/expediente-alumno/list/list.component';
import { EditComponent } from '@pages/expediente-alumno/edit/edit.component';
import { ShowComponent } from '@pages/expediente-alumno/show/show.component';
import { AuthenticateGuard } from '@src/app/guards/authenticate.guard';
import { keys } from '@src/keys';

const routes: Routes = [
  {
    path: 'edit/:id',
    component: EditComponent,
    canActivate: [AuthenticateGuard],
    data: {
      roles: [keys.ADMIN_ROLE, keys.GESTOR_ROLE]
    }
  },
  {
    path: 'show/:id',
    component: ShowComponent,
    canActivate: [AuthenticateGuard],
    data: {
      roles: [keys.ADMIN_ROLE, keys.GESTOR_ROLE, keys.LECTOR_ROLE]
    }
  },
  {
    path: 'PuedeTitularse/:id',
    component: PuedeTitularseComponent,
    canActivate: [AuthenticateGuard],
    data: {
      roles: [keys.ADMIN_ROLE, keys.GESTOR_ROLE, keys.LECTOR_ROLE]
    }
  },
  {
    path: 'List',
    component: ListComponent,
    canActivate: [AuthenticateGuard],
    data: {
      roles: [keys.ADMIN_ROLE, keys.GESTOR_ROLE, keys.LECTOR_ROLE]
    }
  },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ExpedienteRoutingModule {}
