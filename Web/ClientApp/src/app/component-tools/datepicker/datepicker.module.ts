import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { NgbDatepickerModule } from '@ng-bootstrap/ng-bootstrap';

import { DatepickerComponent } from './datepicker.component';

@NgModule({
  declarations: [DatepickerComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    NgbDatepickerModule
  ],
  exports: [DatepickerComponent]
})
export class DatepickerModule {}
