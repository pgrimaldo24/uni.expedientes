import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ComboboxComponent } from './combobox.component';

@NgModule({
  declarations: [ComboboxComponent],
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  exports: [ComboboxComponent]
})
export class ComboboxModule {}
