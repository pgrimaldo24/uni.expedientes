import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ListComponent } from '@pages/seguimientos/list/list.component';
import { AuthenticateGuard } from '@src/app/guards/authenticate.guard';
import { keys } from '@src/keys';

const routes: Routes = [
  {
    path: 'List',
    component: ListComponent,
    canActivate: [AuthenticateGuard],
    data: {
      roles: [keys.ADMIN_ROLE, keys.GESTOR_ROLE]
    }
  },
  {
    path: 'List/:id',
    component: ListComponent,
    canActivate: [AuthenticateGuard],
    data: {
      roles: [keys.ADMIN_ROLE, keys.GESTOR_ROLE]
    }
  },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SeguimientosRoutingModule {}
