import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  NgbAlertModule,
  NgbButtonsModule,
  NgbCollapseModule
} from '@ng-bootstrap/ng-bootstrap';
import { TranslateModule } from '@ngx-translate/core';
import { ComboboxModule } from '@src/app/component-tools/combobox/combobox.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CoreModule } from '@src/app/core/core.module';
import { DataTableModule } from '@src/app/component-tools/data-table/data-table.module';
import { ConfirmationModalModule } from '@src/app/component-tools/confirmation-modal/confirmation-modal.module';
import { BlockUIModule } from 'ng-block-ui';

import { ComportamientosRoutingModule } from './comportamientos-routing.module';
import { ListComponent } from './list/list.component';
import { FiltroComponent } from './filtro/filtro.component';
import { FormComponent } from './form/form.component';
import { TreeViewModule } from '@src/app/component-tools/tree-view/tree-view.module';
import { NivelesUsoModule } from '../niveles-uso/niveles-uso.module';
import { TableRequisitosComportamientosComponent } from './table-requisitos-comportamientos/table-requisitos-comportamientos.component';
import { NivelesUsoComportamientosComponent } from './niveles-uso-comportamientos/niveles-uso-comportamientos.component';

@NgModule({
  declarations: [
    ListComponent,
    FiltroComponent,
    FormComponent,
    TableRequisitosComportamientosComponent,
    NivelesUsoComportamientosComponent
  ],
  imports: [
    CommonModule,
    ComportamientosRoutingModule,
    NgbCollapseModule,
    TranslateModule,
    ComboboxModule,
    ReactiveFormsModule,
    FormsModule,
    NgbButtonsModule,
    CoreModule,
    DataTableModule,
    ConfirmationModalModule,
    NgbAlertModule,
    TreeViewModule,
    NivelesUsoModule,
    BlockUIModule.forRoot()
  ],
  exports: [TableRequisitosComportamientosComponent]
})
export class ComportamientosModule {}
