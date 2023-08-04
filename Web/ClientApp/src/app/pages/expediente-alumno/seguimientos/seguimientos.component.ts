import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import {
  DataColumn,
  DataLoadEvent,
  DataOrderDirection,
  DataRow
} from '@src/app/component-tools/data-table/models';
import { Clause, Criteria } from '@cal/criteria';
import { DataTableComponent } from '@src/app/component-tools/data-table/data-table.component';
import { SeguimientosService } from '@pages/seguimientos/seguimientos.service';
import { StorageStateService } from '@src/app/services/storage-state.service';
import { ExpedienteAlumnoDto } from '../expediente-models';

@Component({
  selector: 'app-seguimientos',
  templateUrl: './seguimientos.component.html',
  styleUrls: ['./seguimientos.component.scss']
})
export class SeguimientosComponent implements OnInit {
  public historial: Clause[] = [];
  public cacheHistorial = 'EXPEDIENTE_SEGUIMIENTOS_LIST_HISTORIAL_FILTROS';
  private cacheDataLoadEvent = 'expediente-seguimientos-list';
  private clauses: Clause[] = [];
  public pageIndex = 1;
  public count = 10;
  public columns: DataColumn[];
  public rows: DataRow[] = [];
  public totalElements: number;
  @Input() expedienteAlumno: ExpedienteAlumnoDto;
  @ViewChild('table', { static: true }) table: DataTableComponent;

  constructor(
    private translateService: TranslateService,
    private listadoService: SeguimientosService,
    private storageService: StorageStateService
  ) {
    this.loadPaginationCache();
  }

  ngOnInit(): void {
    this.initializeTableHeaders();
    this.cacheHistorial = `${this.expedienteAlumno.id}-${this.cacheHistorial}`;
    this.historial =
      (this.storageService.getState(this.cacheHistorial) as Clause[]) || [];
    this.clauses = this.historial;
  }

  private initializeTableHeaders() {
    this.columns = [
      {
        field: 'fecha',
        sortable: false,
        header: this.translateService.instant(
          'pages.editarExpediente.seguimientos.fecha'
        )
      },
      {
        field: 'tipo',
        sortable: false,
        header: this.translateService.instant(
          'pages.editarExpediente.seguimientos.tipo'
        ),
        style: { width: '400px' }
      },
      {
        field: 'usuario',
        sortable: false,
        header: this.translateService.instant(
          'pages.editarExpediente.seguimientos.usuario'
        )
      },
      {
        field: 'descripcion',
        sortable: false,
        header: this.translateService.instant(
          'pages.editarExpediente.seguimientos.descripcion'
        ),
        style: { width: '200px' }
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
      order: this.columns[0],
      direction: DataOrderDirection.Descending
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
    this.listadoService
      .getSeguimientosByIdExpedienteAlumno(criteria, this.expedienteAlumno.id)
      .subscribe((response) => {
        this.rows = response.data.map(
          (e) =>
            new DataRow(
              {
                fecha: e.fecha,
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
