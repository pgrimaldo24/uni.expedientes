import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { GeolocationComponent } from './geolocation.component';
import { TranslateModule } from '@ngx-translate/core';
import { ComboboxModule } from '../combobox/combobox.module';

@NgModule({
  declarations: [GeolocationComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    TranslateModule,
    ComboboxModule
  ],
  exports: [GeolocationComponent]
})
export class GeolocationModule {}
