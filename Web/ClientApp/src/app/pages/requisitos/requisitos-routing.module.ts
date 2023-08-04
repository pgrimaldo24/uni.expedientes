import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthenticateGuard } from '@src/app/guards/authenticate.guard';
import { keys } from '@src/keys';
import { RequisitoFormComponent } from './form/requisito-form.component';
import { RequisitosListComponent } from './list/requisitos-list.component';

const routes: Routes = [
  {
    path: '',
    component: RequisitosListComponent,
    canActivate: [AuthenticateGuard],
    data: {
      roles: [keys.ADMIN_ROLE, keys.GESTOR_ROLE]
    }
  },
  {
    path: 'edit/:id',
    component: RequisitoFormComponent,
    canActivate: [AuthenticateGuard],
    data: {
      roles: [keys.ADMIN_ROLE, keys.GESTOR_ROLE]
    }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RequisitosRoutingModule {}
