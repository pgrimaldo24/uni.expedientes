import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import {
  NgbPaginationModule,
  NgbTooltipModule
} from '@ng-bootstrap/ng-bootstrap';
import { BlockUIModule } from 'ng-block-ui';

import { DataTableComponent } from './data-table.component';
import { DragDropModule } from '@angular/cdk/drag-drop';

@NgModule({
  declarations: [DataTableComponent],
  imports: [
    CommonModule,
    NgbPaginationModule,
    NgbTooltipModule,
    FormsModule,
    ReactiveFormsModule,
    BlockUIModule.forRoot(),
    DragDropModule
  ],
  exports: [DataTableComponent]
})
export class DataTableModule {}
