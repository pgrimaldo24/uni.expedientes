import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthenticateGuard } from '@src/app/guards/authenticate.guard';
import { keys } from '@src/keys';
import { FormComponent } from './form/form.component';
import { ListComponent } from './list/list.component';

const routes: Routes = [
  {
    path: '',
    component: ListComponent,
    canActivate: [AuthenticateGuard],
    data: {
      roles: [keys.ADMIN_ROLE, keys.GESTOR_ROLE]
    }
  },
  {
    path: 'add',
    component: FormComponent,
    canActivate: [AuthenticateGuard],
    data: {
      roles: [keys.ADMIN_ROLE, keys.GESTOR_ROLE]
    }
  },
  {
    path: 'edit/:id',
    component: FormComponent,
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
export class ComportamientosRoutingModule {}
