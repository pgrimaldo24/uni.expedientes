import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbDatepickerModule } from '@ng-bootstrap/ng-bootstrap';

import { DatepickerRangeComponent } from './datepicker-range.component';

@NgModule({
  declarations: [DatepickerRangeComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    NgbDatepickerModule
  ],
  exports: [DatepickerRangeComponent]
})
export class DatepickerRangeModule {}
