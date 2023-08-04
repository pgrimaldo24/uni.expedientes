import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthenticateGuard } from '@src/app/guards/authenticate.guard';
import { keys } from '@src/keys';
import { ShowComponent } from './show/show.component';

const routes: Routes = [
  {
    path: 'show/:id',
    component: ShowComponent,
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
export class AlumnoRoutingModule {}
