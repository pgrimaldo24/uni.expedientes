import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ListComponent } from './list/list.component';
import { SeguimientosService } from './seguimientos.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {
  NgbCollapseModule,
  NgbDropdownModule,
  NgbButtonsModule,
  NgbPopoverModule,
  NgbTooltipModule
} from '@ng-bootstrap/ng-bootstrap';
import { TranslateModule } from '@ngx-translate/core';
import { ComboboxModule } from '@src/app/component-tools/combobox/combobox.module';
import { DataTableModule } from '@src/app/component-tools/data-table/data-table.module';
import { BlockUIModule } from 'ng-block-ui';
import { SeguimientosRoutingModule } from './seguimientos.routing.modules';
import { DatepickerModule } from '@src/app/component-tools/datepicker/datepicker.module';
import { CoreModule } from '@src/app/core/core.module';
import { ExpedienteModule } from '@pages/expediente-alumno/expediente.module';

@NgModule({
  declarations: [ListComponent],
  imports: [
    SeguimientosRoutingModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    TranslateModule,
    DataTableModule,
    DatepickerModule,
    ComboboxModule,
    NgbCollapseModule,
    NgbDropdownModule,
    NgbButtonsModule,
    NgbTooltipModule,
    NgbPopoverModule,
    BlockUIModule.forRoot(),
    CoreModule,
    ExpedienteModule
  ],
  providers: [SeguimientosService]
})
export class SeguimientosModule {}
