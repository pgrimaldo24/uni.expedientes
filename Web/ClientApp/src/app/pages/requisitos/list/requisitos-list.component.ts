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
import { AddRequisitoComponent } from '../add/add-requisito.component';
import {
  FormFiltroRequisitoControls,
  RequisitoExpedienteDto,
  RequisitoExpedienteMasivoDto
} from '../requesitos.models';
import { RequisitosService } from '../requisitos.service';

@Component({
  selector: 'app-requisitos-list',
  templateUrl: './requisitos-list.component.html',
  styleUrls: ['./requisitos-list.component.scss']
})
export class RequisitosListComponent implements OnInit {
  totalElements = 0;
  pageIndex = 1;
  count = 10;
  rows: DataRow[];
  columns: DataColumn[];
  whereCriteria = [];
  cacheId = 'EXPEDIENTES_REQUISITOS_FILTROS';
  private keyPage = 'REQUISITOS_FILTROS';
  @ViewChild('table', { static: true }) table: DataTableComponent;
  @ViewChild('confirmModal', { static: false })
  confirmModal: ConfirmationModalComponent;
  @ViewChild('openForm', { static: false })
  formAddRequisito: AddRequisitoComponent;
  requisitosSeleccionados: RequisitoExpedienteDto[];

  constructor(
    private translateService: TranslateService,
    private requisitoService: RequisitosService,
    private storageService: StorageStateService,
    private alertService: AlertHandlerService
  ) {
    this.loadPaginationCache();
  }

  ngOnInit(): void {
    this.rows = [];
    this.requisitosSeleccionados = [];
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
        field: FormFiltroRequisitoControls.condicionExpediente,
        sortable: false,
        header: this.translateService.instant(
          'pages.listadoRequisitos.condicionExpediente'
        )
      },
      {
        field: FormFiltroRequisitoControls.estaVigente,
        sortable: false,
        header: this.translateService.instant(
          'pages.listadoRequisitos.estaVigente'
        )
      },
      {
        field: FormFiltroRequisitoControls.requeridoTitulo,
        sortable: false,
        header: this.translateService.instant(
          'pages.listadoRequisitos.requeridoTitulo'
        )
      },
      {
        field: FormFiltroRequisitoControls.requiereMatricularse,
        sortable: false,
        header: this.translateService.instant(
          'pages.listadoRequisitos.requiereMatricularse'
        )
      },
      {
        field: FormFiltroRequisitoControls.requeridoPagoTasas,
        sortable: false,
        header: this.translateService.instant(
          'pages.listadoRequisitos.requeridoPagoTasas'
        )
      },
      {
        field: FormFiltroRequisitoControls.requiereDocumentacion,
        sortable: false,
        header: this.translateService.instant(
          'pages.listadoRequisitos.requiereDocumentacion'
        )
      },
      {
        field: FormFiltroRequisitoControls.modoRequerimientoDocumentacion,
        sortable: false,
        header: this.translateService.instant(
          'pages.listadoRequisitos.modoRequerimientoDocumentacion'
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

  applySearchFilter(whereCriteria: unknown[]): void {
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
      `${this.cacheId}-${this.keyPage}`
    ) as DataLoadEvent;
    this.pageIndex = cache ? cache.page : 1;
    this.count = cache ? cache.count : 10;
  }

  buscar(): void {
    const cache: DataLoadEvent = this.storageService.getState(
      `${this.cacheId}-${this.keyPage}`
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
    this.storageService.setState(`${this.cacheId}-${this.keyPage}`, event);
    const data = [];
    this.table.block();
    const criteria = this.getCriteria(event);
    this.requisitoService
      .getRequisitosExpedientes(criteria)
      .subscribe((info) => {
        info.data.forEach((element: RequisitoExpedienteDto) => {
          data.push(
            new DataRow(
              {
                checkbox: '',
                condicionExpediente: element.nombre,
                estaVigente: this.showTextBoolean(element.estaVigente),
                requeridoTitulo: this.showTextBoolean(
                  element.requeridaParaTitulo
                ),
                requiereMatricularse: this.showTextBoolean(
                  element.requiereMatricularse
                ),
                requeridoPagoTasas: this.showTextBoolean(
                  element.requeridaParaPago
                ),
                requiereDocumentacion: this.showTextBoolean(
                  element.requiereDocumentacion
                ),
                modoRequerimientoDocumentacion:
                  element.modoRequerimientoDocumentacion,
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
    return this.translateService.instant(`pages.listadoRequisitos.${texto}`);
  }

  deleteRequisito(requisito: RequisitoExpedienteDto): void {
    this.confirmModal.show(
      () => {
        this.table.block();
        this.requisitoService.deleteRequisito(requisito.id).subscribe(() => {
          this.alertService.success(
            this.translateService.instant('messages.success')
          );
          this.table.unblock();
          this.buscar();
        });
      },
      this.translateService.instant('pages.listadoRequisitos.deleteTitle'),
      this.translateService.instant('pages.listadoRequisitos.deleteMessage'),
      this.translateService.instant('common.aceptar')
    );
  }

  deleteMasive(): void {
    if (!this.requisitosSeleccionados.length) {
      this.alertService.error(
        this.translateService.instant('validations.atLeastOneSelected')
      );
      return;
    }

    const requisitosExpedientes = new RequisitoExpedienteMasivoDto();
    requisitosExpedientes.idsRequisitos = this.requisitosSeleccionados.map(
      (r) => r.id
    );

    this.confirmModal.show(
      () => {
        this.table.block();
        this.requisitoService
          .deleteRequisitoMasivo(requisitosExpedientes)
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
        'pages.listadoRequisitos.deleteTitleMassive'
      ),
      this.translateService.instant(
        'pages.listadoRequisitos.deleteMessageMassive'
      ),
      this.translateService.instant('common.aceptar')
    );
  }

  isChecked(idRequisito: number): boolean {
    return this.requisitosSeleccionados.some((r) => r.id === idRequisito);
  }

  checkRequisito(event: Event, data: RequisitoExpedienteDto): void {
    const checked: boolean = (event.target as HTMLInputElement).checked;
    if (checked) {
      this.requisitosSeleccionados.push(data);
    } else {
      this.requisitosSeleccionados = this.requisitosSeleccionados.filter(
        (s) => s.id !== data.id
      );
    }
  }

  isAllChecked(): boolean {
    return (
      this.rows.length &&
      this.rows.every((fila) =>
        this.requisitosSeleccionados.some(
          (requisito) => requisito.id === fila.object.id
        )
      )
    );
  }

  checkRequisitosFiltered(event: Event): void {
    const checked: boolean = (event.target as HTMLInputElement).checked;
    this.checkUncheckAll(checked);
  }

  checkUncheckAll(checked: boolean): void {
    this.requisitosSeleccionados = [];
    if (checked) {
      this.rows.forEach((row) => {
        this.requisitosSeleccionados.push(row.object);
      });
    }
  }

  addRequisito(): void {
    this.formAddRequisito.open();
  }
}
