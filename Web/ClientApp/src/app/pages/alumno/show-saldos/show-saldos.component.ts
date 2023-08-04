import { Component, OnInit, ViewChild } from '@angular/core';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
import {
  DataColumn,
  DataRow
} from '@src/app/component-tools/data-table/models';
import { CustomCurrencyPipe } from '@src/app/core/pipes/CustomCurrencyPipe';
import { DeudaClienteModel } from '../alumno-models';

@Component({
  selector: 'app-show-saldos',
  templateUrl: './show-saldos.component.html',
  styleUrls: ['./show-saldos.component.scss']
})
export class ShowSaldosComponent implements OnInit {
  deuda: DeudaClienteModel;
  columns: DataColumn[];
  rows: DataRow[];
  currencyUse: string;
  @ViewChild('content', { static: false }) modalRef: NgbModalRef;
  constructor(
    private modal: NgbModal,
    private translateService: TranslateService,
    private customCurrencyPipe: CustomCurrencyPipe
  ) {}

  ngOnInit(): void {
    this.inicializarCabecerasTabla();
  }

  open(deuda: DeudaClienteModel): void {
    this.deuda = deuda;
    this.search();
    this.modal.open(this.modalRef, {
      ariaLabelledBy: 'modal-basic-title',
      backdrop: 'static',
      size: 'xl',
      centered: true,
      backdropClass: 'light-blue-backdrop'
    });
  }

  dismissModal(): void {
    this.modal.dismissAll();
  }

  search(): void {
    this.setformatCurrencyAmounts();
    this.currencyUse = this.deuda.divisaEmpresa;
    this.rows = this.deuda.facturas?.map(
      (e) =>
        new DataRow(
          {
            numeroPedido: e.numeroPedido,
            tipoDocumento: e.tipoDocumento,
            numeroDocumento: e.numeroDocumento,
            fechaRegistro: e.fechaRegistro,
            idEntidadOperacion: e.idEntidadOperacion,
            importe: {
              value: e.importe,
              class: 'text-right'
            },
            importePendiente: {
              value: e.importePendiente,
              class: 'text-right'
            },
            metodoPago: e.metodoPago
          },
          e,
          e.importePendiente > 0 ? 'conSaldo' : ''
        )
    );
  }

  private inicializarCabecerasTabla() {
    this.columns = [
      {
        field: 'numeroPedido',
        sortable: false,
        header: this.translateService.instant('pages.alumno.deuda.numeroPedido')
      },
      {
        field: 'tipoDocumento',
        sortable: false,
        header: this.translateService.instant(
          'pages.alumno.deuda.tipoDocumento'
        )
      },
      {
        field: 'numeroDocumento',
        sortable: false,
        header: this.translateService.instant(
          'pages.alumno.deuda.numeroDocumento'
        )
      },
      {
        field: 'fechaRegistro',
        sortable: false,
        header: this.translateService.instant(
          'pages.alumno.deuda.fechaRegistro'
        )
      },
      {
        field: 'idEntidadOperacion',
        sortable: false,
        header: this.translateService.instant(
          'pages.alumno.deuda.idEntidadOperacion'
        )
      },
      {
        field: 'importe',
        sortable: false,
        header: this.translateService.instant('pages.alumno.deuda.importe')
      },
      {
        field: 'importePendiente',
        sortable: false,
        header: this.translateService.instant(
          'pages.alumno.deuda.importePendiente'
        ),
        style: { width: '20px' }
      },
      {
        field: 'metodoPago',
        sortable: false,
        header: this.translateService.instant('pages.alumno.deuda.metodoPago')
      }
    ];
  }

  private setformatCurrencyAmounts(): void {
    this.deuda.facturas?.map((item) => {
      const importe = this.customCurrencyPipe.transform(
        Number(item.importe),
        this.deuda.divisaEmpresa
      );
      const importePendiente = this.customCurrencyPipe.transform(
        Number(item.importePendiente),
        this.deuda.divisaEmpresa
      );
      item.importeSimbolo = importe;
      item.importePendienteSimbolo = importePendiente;
    });
  }
}
