import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RequisitosRoutingModule } from './requisitos-routing.module';
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
import { RequisitosListComponent } from './list/requisitos-list.component';
import { FiltroComponent } from './filtro/filtro.component';
import { RequisitoFormComponent } from './form/requisito-form.component';
import { DocumentosRequisitosComponent } from './documentos/documentos-requisitos/documentos-requisitos.component';
import { DocumentosRequisitosFormComponent } from './documentos/form/documentos-requisitos-form.component';
import { AddRequisitoComponent } from './add/add-requisito.component';

@NgModule({
  declarations: [
    RequisitosListComponent,
    FiltroComponent,
    RequisitoFormComponent,
    DocumentosRequisitosComponent,
    DocumentosRequisitosFormComponent,
    AddRequisitoComponent
  ],
  imports: [
    CommonModule,
    RequisitosRoutingModule,
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
    BlockUIModule.forRoot()
  ]
})
export class RequisitosModule {}
