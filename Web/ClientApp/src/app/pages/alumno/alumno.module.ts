import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ShowComponent } from './show/show.component';
import { AlumnoService } from './alumno.service';
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
import { AlumnoRoutingModule } from './alumno.routing.modules';
import { DatepickerModule } from '@src/app/component-tools/datepicker/datepicker.module';
import { CoreModule } from '@src/app/core/core.module';
import { ExpedientesComponent } from './expedientes/expedientes.component';
import { MatriculasComponent } from './matriculas/matriculas.component';
import { AlumnoComponent } from './alumno/alumno.component';
import { SharedModule } from '@pages/shared/shared.module';
import { DateStringPipe } from '@helpers/pipes-helpers';
import { CondicionesComponent } from './condiciones/condiciones.component';
import { TreeViewModule } from '@src/app/component-tools/tree-view/tree-view.module';
import { ShowSaldosComponent } from './show-saldos/show-saldos.component';
import { PipesModule } from '@src/app/core/pipes/pipes.module';
import { CustomCurrencyPipe } from '@src/app/core/pipes/CustomCurrencyPipe';

@NgModule({
  declarations: [
    ShowComponent,
    ExpedientesComponent,
    MatriculasComponent,
    AlumnoComponent,
    CondicionesComponent,
    ShowSaldosComponent,
    DateStringPipe
  ],
  imports: [
    AlumnoRoutingModule,
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
    BlockUIModule,
    CoreModule,
    SharedModule,
    TreeViewModule,
    PipesModule
  ],
  providers: [AlumnoService, CustomCurrencyPipe],
  exports: [AlumnoComponent, CondicionesComponent, DateStringPipe, PipesModule]
})
export class AlumnoModule {}
