import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { Clause, Criteria } from '@cal/criteria';
import { TranslateService } from '@ngx-translate/core';
import { AlertHandlerService } from '@src/app/component-tools/alert/alert-handler.service';
import { ConfirmationModalComponent } from '@src/app/component-tools/confirmation-modal/confirmation-modal/confirmation-modal.component';
import { DataTableComponent } from '@src/app/component-tools/data-table/data-table.component';
import {
  DataColumn,
  DataLoadEvent,
  DataOrderDirection,
  DataRow
} from '@src/app/component-tools/data-table/models';
import { IdentityClaimsModel } from '@src/app/models/security.models';
import { SecurityService } from '@src/app/services/security.service';
import { StorageStateService } from '@src/app/services/storage-state.service';
import { keys } from '@src/keys';
import { OAuthService } from 'angular-oauth2-oidc';
import { AnotacionDto, ExpedienteAlumnoDto } from '../expediente-models';
import { ExpedienteService } from '../expediente.service';
import { ObservacionFormComponent } from '../observacion-form/observacion-form.component';

@Component({
  selector: 'app-observaciones',
  templateUrl: './observaciones.component.html',
  styleUrls: ['./observaciones.component.scss']
})
export class ObservacionesComponent implements OnInit {
  columns: DataColumn[];
  rows: DataRow[] = null;
  public totalElements = 0;
  public pageIndex = 1;
  public pageCount = 10;
  filters;
  cacheId = 'EXPEDIENTES_ANOTACIONES_FILTROS';
  private historial: Clause[] = [];
  clauses: Clause[];
  @Input() isReadOnly = false;
  @Input() dataExpedienteAlumno: ExpedienteAlumnoDto;
  @ViewChild('tableObservaciones', { static: true })
  tableObservaciones: DataTableComponent;
  @ViewChild('openForm', { static: false })
  form: ObservacionFormComponent;
  @ViewChild('tableObservaciones', { static: true }) table: DataTableComponent;
  @ViewChild('confirmModal', { static: false })
  confirmModal: ConfirmationModalComponent;
  constructor(
    private expedienteServiceService: ExpedienteService,
    private translate: TranslateService,
    private alertSvc: AlertHandlerService,
    private oauthService: OAuthService,
    private security: SecurityService,
    private storage: StorageStateService
  ) {
    this.loadPaginationCache();
  }

  ngOnInit(): void {
    this.headerDataTable();
    this.limpiar();
  }

  search(event: DataLoadEvent): void {
    this.storage.setState(`${this.cacheId}`, event);
    const data = [];
    this.tableObservaciones.block();
    const criteria = this.getCriteria(event);
    this.expedienteServiceService.getAnotaciones(criteria).subscribe((info) => {
      info.data.forEach((element: AnotacionDto) => {
        data.push(
          new DataRow(
            {
              resumen: element.resumen,
              nota: element.mensaje,
              fecha: element.fecha,
              usuario: element.nombreUsuario,
              ambito: ''
            },
            element
          )
        );
      });
      this.totalElements = info.meta.totalCount;
      this.tableObservaciones.unblock();
      this.rows = data;
    });
  }

  newObservacion(): void {
    const item = new AnotacionDto();
    item.expedienteAlumno = {
      id: this.dataExpedienteAlumno.id
    };
    this.form.open(item, () => this.onSuccess());
  }

  editObservacion(item: AnotacionDto): void {
    this.form.open(item, () => this.onSuccess());
  }

  deleteObservacion(item: AnotacionDto): void {
    this.confirmModal.show(
      () => {
        this.table.block();
        this.expedienteServiceService.deleteAnotacion(item.id).subscribe(() => {
          this.table.unblock();
          this.limpiar();
        });
      },
      this.translate.instant('pages.observations.deleteTitle'),
      this.translate.instant('pages.observations.deleteMessage'),
      this.translate.instant('common.aceptar')
    );
  }

  onSuccess(): void {
    this.alertSvc.success(this.translate.instant('messages.success'));
    this.limpiar();
  }

  getCriteria(event?: DataLoadEvent): Criteria {
    const criteria: Criteria = {
      offset: (event?.page - 1) * event?.count,
      limit: event?.count,
      order: '-id'
    };

    const clauses = this.buildClausesFromFilters();
    criteria.where = clauses;

    criteria.where.filter((element) =>
      element.value ? this.historial.push(element) : null
    );

    if (this.historial) {
      this.clauses = this.historial;
      criteria.where = this.historial;
      this.historial = [];
    }
    return criteria;
  }

  buscar(): void {
    const cache: DataLoadEvent = this.storage.getState(
      `${this.cacheId}`
    ) as DataLoadEvent;
    this.pageIndex = cache ? cache.page : 1;
    this.pageCount = cache ? cache.count : 10;
    this.search({
      page: this.pageIndex,
      count: this.pageCount,
      order: this.columns[1],
      direction: DataOrderDirection.Descending
    });
  }

  loadPaginationCache(): void {
    const cache: DataLoadEvent = this.storage.getState(
      `${this.cacheId}`
    ) as DataLoadEvent;
    this.pageIndex = cache ? cache.page : 1;
    this.pageCount = cache ? cache.count : 10;
  }

  limpiar(): void {
    this.filters = {};
    Object.keys(this.filters).forEach((key) => {
      this.filters[key] = null;
    });
    this.buscar();
  }

  private buildClausesFromFilters(): Clause[] {
    this.clauses = [];

    this.clauses.push({
      field: 'idExpedienteAlumno',
      value: this.dataExpedienteAlumno.id,
      key: 'idExpedienteAlumno'
    });

    this.clauses.push({
      field: 'texto',
      value: this.filters.texto,
      key: 'texto'
    });

    this.clauses.push({
      field: 'fechaDesde',
      value: this.filters.fechaDesde,
      key: 'fechaDesde'
    });

    this.clauses.push({
      field: 'fechaHasta',
      value: this.filters.fechaHasta,
      key: 'fechaHasta'
    });

    return this.clauses;
  }

  public get userId(): string | null {
    const claims = this.oauthService.getIdentityClaims() as IdentityClaimsModel;
    if (!claims) {
      return null;
    }
    return claims.sub;
  }

  public get esAdmin(): boolean {
    return this.security.userRoles().some((resp) => resp === keys.ADMIN_ROLE);
  }

  private headerDataTable(): void {
    this.columns = [
      {
        field: 'resumen',
        sortable: false,
        header: this.translate.instant('pages.observations.resumen')
      },
      {
        field: 'nota',
        sortable: false,
        header: this.translate.instant('pages.observations.nota')
      },
      {
        field: 'fecha',
        sortable: false,
        header: this.translate.instant('pages.observations.fecha')
      },
      {
        field: 'usuario',
        sortable: false,
        header: this.translate.instant('pages.observations.usuario')
      },
      {
        field: 'ambito',
        sortable: false,
        header: this.translate.instant('pages.observations.ambito')
      }
    ];
    if (!this.isReadOnly) {
      this.columns.push({
        field: 'acciones',
        sortable: false,
        header: this.translate.instant('common.opciones'),
        style: { width: '90px' }
      });
    }
  }
}
