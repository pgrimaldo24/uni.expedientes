import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { AbstractControl } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import {
  DocumentoRequisitoDto,
  DocumentoRequisitoMasivoModel,
  RequisitoDocumentoModel
} from '@pages/requisitos/requesitos.models';
import { RequisitosService } from '@pages/requisitos/requisitos.service';
import { AlertHandlerService } from '@src/app/component-tools/alert/alert-handler.service';
import { AlertType } from '@src/app/component-tools/alert/models';
import { ConfirmationModalComponent } from '@src/app/component-tools/confirmation-modal/confirmation-modal/confirmation-modal.component';
import { DataTableComponent } from '@src/app/component-tools/data-table/data-table.component';
import {
  DataColumn,
  DataRow
} from '@src/app/component-tools/data-table/models';
import { DocumentosRequisitosFormComponent } from '../form/documentos-requisitos-form.component';

@Component({
  selector: 'app-documentos-requisitos',
  templateUrl: './documentos-requisitos.component.html',
  styleUrls: ['./documentos-requisitos.component.scss']
})
export class DocumentosRequisitosComponent implements OnInit {
  documentosSeleccionados: DocumentoRequisitoDto[];
  filters;
  @Input() idRequisitoExpediente: number;
  @Input() disabledFilesTipes: boolean;
  @Input() documentacionProtegida: AbstractControl;
  columns: DataColumn[];
  rows: DataRow[] = [];
  public totalElements = 0;
  public pageIndex = 1;
  public pageCount = 10;

  @ViewChild('openForm', { static: false })
  form: DocumentosRequisitosFormComponent;
  @ViewChild('confirmModal', { static: false })
  confirmModal: ConfirmationModalComponent;
  @ViewChild('tableDocumentosRequisitos', { static: true })
  table: DataTableComponent;

  constructor(
    private translate: TranslateService,
    private alertSvc: AlertHandlerService,
    private requisitoSrv: RequisitosService
  ) {}

  ngOnInit(): void {
    this.documentosSeleccionados = [];
    this.headerDataTable();
    this.table.update();
  }

  newDocumento(): void {
    const item = new RequisitoDocumentoModel();
    item.idRequisitoExpediente = this.idRequisitoExpediente;
    item.documentoSecurizado = this.documentacionProtegida.value;
    this.form.open(item, () => this.onSuccess());
  }

  editDocumentoRequisito(item: RequisitoDocumentoModel): void {
    item.documentoSecurizado = this.documentacionProtegida.value;
    this.form.open(item, () => this.onSuccess());
  }

  deleteDocumentoRequisito(item: RequisitoDocumentoModel): void {
    this.confirmModal.show(
      () => {
        this.table.block();
        this.requisitoSrv.deleteDocumentoRequisito(item.id).subscribe(() => {
          this.table.unblock();
          this.search();
        });
      },
      this.translate.instant('pages.formularioRequisitos.deleteTitle'),
      this.translate.instant('pages.formularioRequisitos.deleteMessage'),
      this.translate.instant('common.aceptar')
    );
  }

  search(): void {
    this.table.block();
    this.requisitoSrv
      .getDocumentosRequisito(this.idRequisitoExpediente)
      .subscribe((response) => {
        this.table.unblock();
        this.rows = response.map((value) => new DataRow({ ...value }, value));
        if (this.rows.length == 0) this.requisitoSrv.hasDocumentos = false;
        else {
          this.requisitoSrv.hasDocumentos = true;
          this.requisitoSrv.setDocumentacionProtegida(
            response[0].documentoSecurizado
          );
        }
      });
  }

  private headerDataTable(): void {
    this.columns = [
      {
        field: 'seleccion',
        sortable: false,
        class: 'text-center',
        style: { width: '50px' }
      },
      {
        field: 'nombreDocumento',
        sortable: false,
        header: this.translate.instant('pages.formularioRequisitos.nombre')
      },
      {
        field: 'documentoEditable',
        sortable: false,
        header: this.translate.instant('pages.formularioRequisitos.editable')
      },
      {
        field: 'documentoSecurizado',
        sortable: false,
        header: this.translate.instant('pages.formularioRequisitos.securizado')
      },
      {
        field: 'documentoObligatorio',
        sortable: false,
        header: this.translate.instant('pages.formularioRequisitos.obligatorio')
      },
      {
        field: 'requiereAceptacionAlumno',
        sortable: false,
        header: this.translate.instant(
          'pages.formularioRequisitos.requiereAceptacionAlumno'
        )
      },
      {
        field: 'opciones',
        sortable: false,
        header: this.translate.instant('common.opciones')
      }
    ];
  }

  onSuccess(): void {
    this.alertSvc.success(this.translate.instant('messages.success'));
    this.search();
  }

  deleteMasive(): void {
    if (!this.documentosSeleccionados.length) {
      this.alertSvc.error(
        this.translate.instant('validations.atLeastOneSelected')
      );
      return;
    }

    const requisitosExpedientes = new DocumentoRequisitoMasivoModel();
    requisitosExpedientes.idsDocumentos = this.documentosSeleccionados.map(
      (r) => r.id
    );

    this.confirmModal.show(
      () => {
        this.table.block();
        this.requisitoSrv
          .deleteDocumentosRequisito(requisitosExpedientes)
          .subscribe((resp) => {
            if (resp.length) {
              this.alertSvc.custom({
                autoClose: false,
                messages: resp,
                type: AlertType.error
              });
            } else {
              this.alertSvc.success(this.translate.instant('messages.success'));
            }
            this.table.unblock();
            this.search();
          });
      },
      this.translate.instant('pages.formularioRequisitos.deleteTitleMassive'),
      this.translate.instant('pages.formularioRequisitos.deleteMessageMassive'),
      this.translate.instant('common.aceptar')
    );
  }

  isChecked(idRequisito: number): boolean {
    return this.documentosSeleccionados.some((r) => r.id === idRequisito);
  }

  checkRequisito(event: Event, data: DocumentoRequisitoDto): void {
    const checked: boolean = (event.target as HTMLInputElement).checked;
    if (checked) {
      this.documentosSeleccionados.push(data);
    } else {
      this.documentosSeleccionados = this.documentosSeleccionados.filter(
        (s) => s.id !== data.id
      );
    }
  }

  isAllChecked(): boolean {
    return (
      this.rows.length &&
      this.rows.every((fila) =>
        this.documentosSeleccionados.some(
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
    this.documentosSeleccionados = [];
    if (checked) {
      this.rows.forEach((row) => {
        this.documentosSeleccionados.push(row.object);
      });
    }
  }
}
