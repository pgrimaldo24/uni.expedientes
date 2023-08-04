import { Component, OnInit, ViewChild } from '@angular/core';
import { Criteria } from '@cal/criteria';
import { TranslateService } from '@ngx-translate/core';
import { AlertHandlerService } from '@src/app/component-tools/alert/alert-handler.service';
import { AlertType } from '@src/app/component-tools/alert/models';
import { ConfirmationModalComponent } from '@src/app/component-tools/confirmation-modal/confirmation-modal/confirmation-modal.component';
import { DataTableComponent } from '@src/app/component-tools/data-table/data-table.component';
import {
  DataColumn,
  DataLoadEvent,
  DataOrderDirection,
  DataRow
} from '@src/app/component-tools/data-table/models';
import { StorageStateService } from '@src/app/services/storage-state.service';
import {
  ComportamientoExpedienteDto,
  ComportamientoExpedienteMasivoDto,
  FormFiltroComportamientoControls
} from '../comportamientos.models';
import { ComportamientosService } from '../comportamientos.service';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {
  totalElements = 0;
  pageIndex = 1;
  count = 10;
  rows: DataRow[];
  columns: DataColumn[];
  whereCriteria = [];
  cacheId = 'EXPEDIENTES_COMPORTAMIENTOS_FILTROS';
  @ViewChild('table', { static: true }) table: DataTableComponent;
  @ViewChild('confirmModal', { static: false })
  confirmModal: ConfirmationModalComponent;
  comportamientosSeleccionados: ComportamientoExpedienteDto[];

  constructor(
    private translateService: TranslateService,
    private comportamientoService: ComportamientosService,
    private storageService: StorageStateService,
    private alertService: AlertHandlerService
  ) {
    this.loadPaginationCache();
  }

  ngOnInit(): void {
    this.rows = [];
    this.comportamientosSeleccionados = [];
    this.initializeTableHeaders();
    this.buscar();
  }

  private initializeTableHeaders(): void {
    this.columns = [
      {
        field: 'seleccion',
        sortable: false,
        class: 'text-center',
        style: { width: '50px' }
      },
      {
        field: FormFiltroComportamientoControls.comportamientoExpediente,
        sortable: false,
        header: this.translateService.instant(
          'pages.listadoComportamientos.comportamientoExpediente'
        )
      },
      {
        field: FormFiltroComportamientoControls.estaVigente,
        sortable: false,
        header: this.translateService.instant(
          'pages.listadoComportamientos.estaVigente'
        )
      },
      {
        field: FormFiltroComportamientoControls.descripcion,
        sortable: false,
        header: this.translateService.instant(
          'pages.listadoComportamientos.descripcion'
        )
      },
      {
        field: 'opciones',
        sortable: false,
        header: this.translateService.instant('common.opciones'),
        style: { width: '100px' }
      }
    ];
  }

  applySearchFilter(whereCriteria: Criteria[]): void {
    this.whereCriteria = whereCriteria;
    this.table.update();
  }

  private getCriteria(event: DataLoadEvent): Criteria {
    const criteria = {
      offset: (event?.page - 1) * event?.count,
      limit: event?.count,
      order: '-id',
      where: this.whereCriteria
    };
    return criteria;
  }

  loadPaginationCache(): void {
    const cache: DataLoadEvent = this.storageService.getState(
      `${this.cacheId}`
    ) as DataLoadEvent;
    this.pageIndex = cache ? cache.page : 1;
    this.count = cache ? cache.count : 10;
  }

  buscar(): void {
    const cache: DataLoadEvent = this.storageService.getState(
      `${this.cacheId}`
    ) as DataLoadEvent;
    this.pageIndex = cache ? cache.page : 1;
    this.count = cache ? cache.count : 10;
    this.search({
      page: this.pageIndex,
      count: this.count,
      order: this.columns[2],
      direction: DataOrderDirection.Ascending
    });
  }

  search(event: DataLoadEvent): void {
    this.storageService.setState(`${this.cacheId}`, event);
    const data = [];
    this.table.block();
    const criteria = this.getCriteria(event);
    this.comportamientoService
      .getComportamientosExpedientes(criteria)
      .subscribe((info) => {
        info.data.forEach((element: ComportamientoExpedienteDto) => {
          data.push(
            new DataRow(
              {
                checkbox: '',
                comportamientoExpediente: element.nombre,
                estaVigente: this.showTextBoolean(element.estaVigente),
                descripcion: element.descripcion,
                opciones: ''
              },
              element
            )
          );
        });
        this.totalElements = info.meta.totalCount;
        this.table.unblock();
        this.rows = data;
      });
  }

  showTextBoolean(isTrue: boolean): string {
    const texto = isTrue ? 'si' : 'no';
    return this.translateService.instant(
      `pages.listadoComportamientos.${texto}`
    );
  }

  deleteComportamiento(comportamiento: ComportamientoExpedienteDto): void {
    this.confirmModal.show(
      () => {
        this.table.block();
        this.comportamientoService
          .deleteComportamiento(comportamiento.id)
          .subscribe(() => {
            this.alertService.success(
              this.translateService.instant('messages.success')
            );
            this.table.unblock();
            this.buscar();
          });
      },
      this.translateService.instant('pages.listadoComportamientos.deleteTitle'),
      this.translateService.instant(
        'pages.listadoComportamientos.deleteMessage'
      ),
      this.translateService.instant('common.aceptar')
    );
  }

  deleteMasive(): void {
    if (!this.comportamientosSeleccionados.length) {
      this.alertService.error(
        this.translateService.instant('validations.atLeastOneSelected')
      );
      return;
    }

    const comportamientosExpedientes = new ComportamientoExpedienteMasivoDto();
    comportamientosExpedientes.idsComportamientos = this.comportamientosSeleccionados.map(
      (r) => r.id
    );

    this.confirmModal.show(
      () => {
        this.table.block();
        this.comportamientoService
          .deleteComportamientoMasivo(comportamientosExpedientes)
          .subscribe((resp) => {
            if (resp.length) {
              this.alertService.custom({
                autoClose: false,
                messages: resp,
                type: AlertType.error
              });
            } else {
              this.alertService.success(
                this.translateService.instant('messages.success')
              );
            }
            this.table.unblock();
            this.buscar();
          });
      },
      this.translateService.instant(
        'pages.listadoComportamientos.deleteTitleMassive'
      ),
      this.translateService.instant(
        'pages.listadoComportamientos.deleteMessageMassive'
      ),
      this.translateService.instant('common.aceptar')
    );
  }

  isChecked(idComportamiento: number): boolean {
    return this.comportamientosSeleccionados.some(
      (r) => r.id === idComportamiento
    );
  }

  checkComportamiento(event: Event, data: ComportamientoExpedienteDto): void {
    const checked: boolean = (event.target as HTMLInputElement).checked;
    if (checked) {
      this.comportamientosSeleccionados.push(data);
    } else {
      this.comportamientosSeleccionados = this.comportamientosSeleccionados.filter(
        (s) => s.id !== data.id
      );
    }
  }

  isAllChecked(): boolean {
    return (
      this.rows.length &&
      this.rows.every((fila) =>
        this.comportamientosSeleccionados.some(
          (comportamiento) => comportamiento.id === fila.object.id
        )
      )
    );
  }

  checkComportamientosFiltered(event: Event): void {
    const checked: boolean = (event.target as HTMLInputElement).checked;
    this.checkUncheckAll(checked);
  }

  checkUncheckAll(checked: boolean): void {
    this.comportamientosSeleccionados = [];
    if (checked) {
      this.rows.forEach((row) => {
        this.comportamientosSeleccionados.push(row.object);
      });
    }
  }
}
