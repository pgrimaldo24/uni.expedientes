import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {
  NgbAlertModule,
  NgbButtonsModule,
  NgbCollapseModule,
  NgbDropdownModule,
  NgbPopoverModule,
  NgbTooltipModule
} from '@ng-bootstrap/ng-bootstrap';
import { PuedeTitularseComponent } from '@pages/expediente-alumno/puede-titularse/puede-titularse.component';
import { ExpedienteRoutingModule } from '@pages/expediente-alumno/expediente.routing.modules';
import { DataTableModule } from '@src/app/component-tools/data-table/data-table.module';
import { ComboboxModule } from '@src/app/component-tools/combobox/combobox.module';
import { ListComponent } from '@pages/expediente-alumno/list/list.component';
import { EditComponent } from '@pages/expediente-alumno/edit/edit.component';
import { ShowComponent } from '@pages/expediente-alumno/show/show.component';
import { BlockUIModule } from 'ng-block-ui';
import { TranslateModule } from '@ngx-translate/core';
import { UnirTreeSelectModule } from '@tools/unir-tree-select/unir-tree-select.module';
import { TabsComponent } from './tabs/tabs.component';
import { SeguimientosComponent } from './seguimientos/seguimientos.component';
import { DatepickerModule } from '@src/app/component-tools/datepicker/datepicker.module';
import { NodosActualesComponent } from './nodos-actuales/nodos-actuales.component';
import { HitosObtenidosComponent } from './hitos-obtenidos/hitos-obtenidos.component';
import { AuthenticateGuard } from '@src/app/guards/authenticate.guard';
import { DateToLocalStringPipe } from '@helpers/pipes-helpers';
import { ConfirmationModalModule } from '@tools/confirmation-modal/confirmation-modal.module';
import { CoreModule } from '@src/app/core/core.module';
import { ExpedienteComponent } from './expediente/expediente.component';
import { InstitucionDocenteAddComponent } from './expediente/popup-add/institucion-docente-popup-add.component';
import { GeolocationModule } from '@src/app/component-tools/geolocation/geolocation.module';
import { ObservacionesComponent } from './observaciones/observaciones.component';
import { ObservacionFormComponent } from './observacion-form/observacion-form.component';
import { AngularEditorModule } from '@kolkov/angular-editor';
import { AlumnoModule } from '@pages/alumno/alumno.module';
import { ConsolidarRequisitoFormComponent } from './requisitos-tab/consolidar-requisito-form/consolidar-requisito-form.component';
import { RequisitosTabComponent } from './requisitos-tab/requisitos-tab.component';
import { CalificacionesTabComponent } from './calificaciones-tab/calificaciones-tab.component';
import { ListCalificacionesComponent } from './calificaciones-tab/list/list-calificaciones.component';
import { ResumenCreditosComponent } from './resumen-tab/resumen-creditos/resumen-creditos.component';
import { ResumenTabComponent } from './resumen-tab/resumen-tab.component';
import { ResumenSituacionComponent } from './resumen-tab/resumen-situacion/resumen-situacion.component';
import { ResumenCronologiaComponent } from './resumen-tab/resumen-cronologia/resumen-cronologia.component';
import { PopupCronologiaComponent } from './resumen-tab/resumen-cronologia/popup-cronologia/popup-cronologia.component';
import { ResumenPlanComponent } from './resumen-tab/resumen-plan/resumen-plan.component';
import { PopupCalificacionesComponent } from './resumen-tab/resumen-plan/popup-calificaciones/popup-calificaciones.component';
import { ComboTrayectoComponent } from './calificaciones-tab/combo-trayecto/combo-trayecto.component';
import { FiltroComponent } from './list/filtro/filtro.component';
import { ShowGenerateSolicitudTituloComponent } from './show-generate-solicitud-titulo/show-generate-solicitud-titulo.component';
import { FiltroComponent as FiltroSeguimientosComponent } from './seguimientos/filtro/filtro.component';

@NgModule({
  declarations: [
    EditComponent,
    ShowComponent,
    ListComponent,
    PuedeTitularseComponent,
    TabsComponent,
    SeguimientosComponent,
    NodosActualesComponent,
    HitosObtenidosComponent,
    DateToLocalStringPipe,
    ExpedienteComponent,
    InstitucionDocenteAddComponent,
    ObservacionesComponent,
    ObservacionFormComponent,
    RequisitosTabComponent,
    ConsolidarRequisitoFormComponent,
    CalificacionesTabComponent,
    ListCalificacionesComponent,
    ResumenCreditosComponent,
    ResumenTabComponent,
    ResumenSituacionComponent,
    ResumenCronologiaComponent,
    PopupCronologiaComponent,
    ResumenPlanComponent,
    PopupCalificacionesComponent,
    ComboTrayectoComponent,
    FiltroComponent,
    ShowGenerateSolicitudTituloComponent,
    FiltroSeguimientosComponent
  ],
  exports: [DateToLocalStringPipe, FiltroComponent],
  imports: [
    ExpedienteRoutingModule,
    CommonModule,
    NgbCollapseModule,
    NgbDropdownModule,
    TranslateModule,
    FormsModule,
    NgbButtonsModule,
    NgbPopoverModule,
    NgbTooltipModule,
    ReactiveFormsModule,
    NgbAlertModule,
    DataTableModule,
    ComboboxModule,
    DatepickerModule,
    BlockUIModule.forRoot(),
    UnirTreeSelectModule,
    ConfirmationModalModule,
    CoreModule,
    GeolocationModule,
    AngularEditorModule,
    AlumnoModule
  ],
  providers: [AuthenticateGuard]
})
export class ExpedienteModule {}
