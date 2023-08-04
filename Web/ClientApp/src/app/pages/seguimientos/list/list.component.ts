import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import {
  DataColumn,
  DataLoadEvent,
  DataOrderDirection,
  DataRow
} from '@tools/data-table/models';
import { DataTableComponent } from '@tools/data-table/data-table.component';
import { Clause, Criteria } from '@cal/criteria';
import { SeguimientosService } from '../seguimientos.service';
import { StorageStateService } from '@src/app/services/storage-state.service';
import { ActivatedRoute } from '@angular/router';
import { FormFiltroExpedienteControls } from '@pages/expediente-alumno/expediente-models';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {
  public historial: Clause[] = [];
  public cacheHistorial = 'SEGUIMIENTOS_LIST_HISTORIAL_FILTROS';
  private cacheDataLoadEvent = 'seguimientos-list';
  private clauses: Clause[] = [];
  public pageIndex = 1;
  public count = 10;
  public columns: DataColumn[];
  public rows: DataRow[] = [];
  public totalElements: number;
  @ViewChild('table', { static: true }) table: DataTableComponent;

  constructor(
    private translateService: TranslateService,
    private storageService: StorageStateService,
    private seguimientosService: SeguimientosService,
    private activatedRoute: ActivatedRoute
  ) {
    this.loadPaginationCache();
    this.historial =
      (this.storageService.getState(this.cacheHistorial) as Clause[]) || [];
    this.clauses = this.historial;
  }

  ngOnInit(): void {
    this.initializeTableHeaders();
    this.getSeguimientosByExpediente();
  }

  private getSeguimientosByExpediente() {
    this.activatedRoute.params.subscribe(({ id }) => {
      if (!id) return;
      this.clauses = this.clauses.filter(
        (c) => c.key != FormFiltroExpedienteControls.idExpediente
      );
      this.clauses.push({
        field: 'filterIdExpedienteAlumno',
        key: FormFiltroExpedienteControls.idExpediente,
        label: this.translateService.instant(
          `pages.listadoExpediente.expediente`
        ),
        text: id,
        value: id
      });
      this.historial = this.clauses;
      this.buscar();
    });
  }

  private initializeTableHeaders() {
    this.columns = [
      {
        field: 'fecha',
        sortable: false,
        header: this.translateService.instant(
          'pages.listadoSeguimientos.fecha'
        ),
        style: { width: '175px' }
      },
      {
        field: 'expediente',
        sortable: false,
        header: this.translateService.instant(
          'pages.listadoSeguimientos.expediente'
        )
      },
      {
        field: 'alumno',
        sortable: false,
        header: this.translateService.instant(
          'pages.listadoSeguimientos.alumno'
        ),
        style: { width: '280px' }
      },
      {
        field: 'planDeEstudio',
        sortable: false,
        header: this.translateService.instant(
          'pages.listadoSeguimientos.planDeEstudio'
        )
      },
      {
        field: 'tipo',
        sortable: false,
        header: this.translateService.instant('pages.listadoSeguimientos.tipo')
      },
      {
        field: 'usuario',
        sortable: false,
        header: this.translateService.instant(
          'pages.listadoSeguimientos.usuario'
        ),
        style: { width: '300px' }
      },
      {
        field: 'descripcion',
        sortable: false,
        header: this.translateService.instant(
          'pages.listadoSeguimientos.descripcion'
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

  public applySearchFilter(clauses: Clause[]): void {
    this.clauses = clauses;
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
    this.seguimientosService.advancedSearch(criteria).subscribe((response) => {
      this.rows = response.data.map(
        (e) =>
          new DataRow(
            {
              fecha: e.fecha,
              expediente: e.expedienteAlumno.id,
              alumno: e.expedienteAlumno.alumnoDisplayName,
              planDeEstudio: e.expedienteAlumno.nombrePlan,
              tipo: e.tipoSeguimiento.nombre,
              usuario: e.nombreTrabajador,
              descripcion: e.descripcion
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

  public cleanTable(): void {
    this.rows = [];
    this.totalElements = 0;
    this.historial = [];
    this.clauses = [];
    this.storageService.setState(this.cacheHistorial, []);
  }
}
