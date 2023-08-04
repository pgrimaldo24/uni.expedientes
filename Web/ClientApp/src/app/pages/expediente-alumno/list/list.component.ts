import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { keys } from '@src/keys';
import * as helpers from '@helpers/commons-helpers';
import {
  DataColumn,
  DataLoadEvent,
  DataOrderDirection,
  DataRow
} from '@tools/data-table/models';
import { DataTableComponent } from '@tools/data-table/data-table.component';
import { Clause, Criteria } from '@cal/criteria';
import { ExpedienteService } from '@pages/expediente-alumno/expediente.service';
import {
  Data,
  ExpedienteAlumnoDto
} from '@pages/expediente-alumno/expediente-models';
import { StorageStateService } from '@src/app/services/storage-state.service';
import { SecurityService } from '@src/app/services/security.service';
import { ShowGenerateSolicitudTituloComponent } from '../show-generate-solicitud-titulo/show-generate-solicitud-titulo.component';
import { AlertHandlerService } from '@src/app/component-tools/alert/alert-handler.service';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {
  public historial: Clause[] = [];
  public cacheHistorial = 'EXPEDIENTES_LIST_HISTORIAL_FILTROS';
  private cacheDataLoadEvent = 'expediente-list';
  private clauses: Clause[] = [];
  public pageIndex = 1;
  public count = 10;
  public columns: DataColumn[];
  public rows: DataRow[] = [];
  public totalElements: number;
  public certificados: string[];
  public loadingCertificate = false;
  public roles = keys;
  public isAdminGestor: boolean;
  @ViewChild('table', { static: true }) table: DataTableComponent;
  public loadingGenerateSolicitudTitulo = false;
  public selectedAll = false;
  public totalMaximoSeleccionado = 100;
  public expedientesSeleccionadas: Data[] = [];
  private idUniversidadGenerarSolicitudTitulo: number;
  @ViewChild('modalShowGenerateSolicitudTitulo', { static: true })
  modalShowGenerateSolicitudTitulo: ShowGenerateSolicitudTituloComponent;
  constructor(
    private translateService: TranslateService,
    private storageService: StorageStateService,
    private expedienteService: ExpedienteService,
    private securityService: SecurityService,
    private alertService: AlertHandlerService
  ) {
    this.loadPaginationCache();
    this.historial =
      (this.storageService.getState(this.cacheHistorial) as Clause[]) || [];
    this.clauses = this.historial;
  }

  ngOnInit(): void {
    this.setAdminGestor();
    this.initializeTableHeaders();
  }

  private setAdminGestor(): void {
    const rolesUsuarios = this.securityService.userRoles();
    this.isAdminGestor =
      rolesUsuarios.indexOf(this.roles.ADMIN_ROLE) > -1 ||
      rolesUsuarios.indexOf(this.roles.GESTOR_ROLE) > -1;
  }

  private initializeTableHeaders() {
    this.columns = [];
    if (this.isAdminGestor) {
      this.columns = [
        {
          field: 'seleccion',
          sortable: false,
          class: 'text-center',
          style: { width: '10px' }
        }
      ];
    }

    this.columns = [
      ...this.columns,
      {
        field: 'expediente',
        sortable: false,
        header: this.translateService.instant(
          'pages.listadoExpediente.expediente'
        ),
        style: { width: '250px' }
      },
      {
        field: 'alumno',
        sortable: false,
        header: this.translateService.instant('pages.listadoExpediente.alumno'),
        style: { width: '400px' }
      },
      {
        field: 'plan',
        sortable: false,
        header: this.translateService.instant(
          'pages.listadoExpediente.columnEstudioPlan'
        )
      },
      {
        field: 'fechas',
        sortable: false,
        header: this.translateService.instant('pages.listadoExpediente.fechas'),
        style: { width: '200px' }
      },
      {
        field: 'acciones',
        sortable: false,
        header: this.translateService.instant(
          'pages.listadoExpediente.acciones'
        )
      }
    ];
  }

  private loadPaginationCache(): void {
    const cache: DataLoadEvent = this.storageService.getState(
      `${this.cacheHistorial}-${this.cacheDataLoadEvent}`
    ) as DataLoadEvent;
    this.pageIndex = cache ? cache.page : 1;
    this.count = cache ? cache.count : 10;
  }

  public reloadTable(): void {
    this.table.update();
  }

  public applySearchFilter(clauses: Clause[]): void {
    this.clauses = clauses;
    this.clearCheckTable();
    this.table.update();
  }

  public buscar(): void {
    this.loadPaginationCache();
    this.search({
      page: this.pageIndex,
      count: this.count,
      order: this.columns[1],
      direction: DataOrderDirection.Ascending
    });
  }

  public search(event: DataLoadEvent): void {
    this.rows = [];
    this.storageService.setState(
      `${this.cacheHistorial}-${this.cacheDataLoadEvent}`,
      event
    );
    const criteria = this.getCriteria(event);
    this.table.block();
    this.expedienteService.advancedSearch(criteria).subscribe((response) => {
      this.rows = response.data.map(
        (e) =>
          new DataRow(
            {
              id: e.id,
              alumno:
                e.displayNameDocumentoIdentificacionAlumno != null
                  ? e.displayNameDocumentoIdentificacionAlumno
                  : e.displayNameNombreAlumno,
              fechaApertura:
                e.fechaApertura != null
                  ? helpers.dateToString(e.fechaApertura, 'DD/MM/YYYY')
                  : e.fechaFinalizacion != null
                  ? helpers.dateToString(e.fechaFinalizacion, 'DD/MM/YYYY')
                  : '',
              fechaFinalizacion:
                e.fechaFinalizacion != null
                  ? helpers.dateToString(e.fechaFinalizacion, 'DD/MM/YYYY')
                  : '',
              plan: e.nombrePlan ? e.nombrePlan : e.nombreEstudio,
              planDeEstudio: e.planEstudioDisplayName
            },
            e
          )
      );
      this.totalElements = response.meta.totalCount;
      this.table.unblock();
    });
  }

  private getCriteria(event: DataLoadEvent): Criteria {
    const criteria = {
      offset: (event?.page - 1) * event?.count,
      limit: event?.count,
      order: '-id',
      where: this.clauses
    };

    this.historial = this.clauses;
    this.storageService.setState(this.cacheHistorial, this.historial);
    return criteria;
  }

  public getCertificados(expediente: ExpedienteAlumnoDto): void {
    this.loadingCertificate = true;
    this.expedienteService
      .getCertificados(expediente.idRefIntegracionAlumno, expediente.idRefPlan)
      .subscribe((response) => {
        this.certificados = response.map(
          (c) =>
            `${c.tipo.refCodigoTipoSolicitud} (${
              c.estado.nombre
            }, ${helpers.dateToString(c.fechaEstado, 'DD/MM/YYYY')})`
        );
        this.loadingCertificate = false;
      });
  }

  public cleanTable(): void {
    this.rows = [];
    this.totalElements = 0;
    this.historial = [];
    this.clauses = [];
    this.storageService.setState(this.cacheHistorial, []);
    this.clearCheckTable();
  }

  private validacionGenerarSolicitud(): boolean {
    if (this.expedientesSeleccionadas.length <= 0) {
      this.showWarningMessageSolicitud('seleccionarExpediente');
      return false;
    }

    if (this.expedientesSeleccionadas.length > this.totalMaximoSeleccionado) {
      this.showWarningMessageSolicitud('mensajeTotalMaximoExpedientes', {
        totalMaximo: this.totalMaximoSeleccionado
      });
      return false;
    }

    const idsUniversidades = this.expedientesSeleccionadas.map(
      (x) => x.idUniversidad
    );
    const tiposUniversidades = idsUniversidades.filter(
      (x, i, a) => a.indexOf(x) === i
    );

    if (tiposUniversidades.length > 1) {
      this.showWarningMessageSolicitud('seleccionarExpedienteMismaUniversidad');
      return false;
    }
    this.idUniversidadGenerarSolicitudTitulo = tiposUniversidades[0];
    return true;
  }

  private showWarningMessageSolicitud(
    keyTranslate: string,
    params?: unknown
  ): void {
    this.alertService.warning(
      this.translateService.instant(
        `pages.listadoExpediente.generateSolicitudTitulo.${keyTranslate}`,
        params
      )
    );
  }

  public generateSolicitudTituloCertificado(): void {
    if (!this.validacionGenerarSolicitud()) return;

    this.modalShowGenerateSolicitudTitulo.open(
      this.selectedAll,
      this.expedientesSeleccionadas,
      this.idUniversidadGenerarSolicitudTitulo,
      this.clauses,
      () => {
        this.alertService.success(
          this.translateService.instant(
            'pages.listadoExpediente.generateSolicitudTitulo.mensajeGeneracionCorrectamente'
          )
        );
        this.clearCheckTable();
      }
    );
  }

  public isAllChecked(): boolean {
    return (
      this.rows.length > 0 &&
      this.rows.every((fila) =>
        this.expedientesSeleccionadas.some(
          (solicitud) => solicitud.id === fila.object.id
        )
      )
    );
  }

  public selectAll(): void {
    this.selectedAll = true;
  }

  public checkExpedientesFiltered(event: Event): void {
    const checked: boolean = (event.target as HTMLInputElement).checked;
    this.checkUncheckAll(checked);
  }

  public checkUncheckAll(checked: boolean): void {
    if (checked) {
      this.expedientesSeleccionadas = [];
      this.rows.forEach((row) => {
        this.expedientesSeleccionadas.push(row.object);
      });
    } else {
      this.selectedAll = checked;
      this.expedientesSeleccionadas = [];
    }
  }

  public checkExpediente(event: Event, data: Data): void {
    const checked: boolean = (event.target as HTMLInputElement).checked;
    if (checked) {
      this.expedientesSeleccionadas.push(data);
    } else {
      const find = this.expedientesSeleccionadas.find((s) => s.id === data.id);
      if (find != null) {
        this.expedientesSeleccionadas = this.expedientesSeleccionadas.filter(
          (s) => s.id !== data.id
        );
      }
    }
  }

  public isChecked(idExpediente: number): boolean {
    return this.expedientesSeleccionadas.some((s) => s.id === idExpediente);
  }

  public getLoaderGenerarSolicitud(loader: boolean): void {
    this.loadingGenerateSolicitudTitulo = loader;
  }

  private clearCheckTable(): void {
    this.selectedAll = false;
    this.checkUncheckAll(false);
    this.loadingGenerateSolicitudTitulo = false;
    this.expedientesSeleccionadas = [];
  }
}
