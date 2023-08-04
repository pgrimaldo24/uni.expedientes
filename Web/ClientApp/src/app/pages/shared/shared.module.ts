import { RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MenuComponent } from './menu/menu.component';
import { LoadingComponent } from './loading/loading.component';
import { HomeComponent } from './home/home.component';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
  declarations: [HomeComponent, LoadingComponent, MenuComponent],
  imports: [CommonModule, RouterModule, TranslateModule],
  exports: [HomeComponent, LoadingComponent, MenuComponent]
})
export class SharedModule {}
