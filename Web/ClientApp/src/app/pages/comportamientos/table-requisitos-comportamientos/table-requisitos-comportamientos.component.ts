import {
  Component,
  OnInit,
  Input,
  Output,
  ViewChild,
  EventEmitter
} from '@angular/core';
import { keys } from '@src/keys';
import { RequisitoDto } from '../../requisitos/requesitos.models';
import {
  ComboboxItem,
  ConfigureCombobox
} from '@src/app/component-tools/combobox/models';
import {
  ComportamientoExpedienteDto,
  RequisitoComportamientoExpedienteDto,
  TipoRequisitoExpedienteDto,
  RequisitoComportamientoExpedienteField,
  RequisitoComportamientoExpedienteMasivoDto,
  EditRequisitoComportamientoExpedienteDto,
  CreateRequisitoComportamientoExpedienteDto
} from '../comportamientos.models';
import {
  DataColumn,
  DataRow
} from '@src/app/component-tools/data-table/models';
import { DataTableComponent } from '@src/app/component-tools/data-table/data-table.component';
import { TranslateService } from '@ngx-translate/core';
import { AlertHandlerService } from '@src/app/component-tools/alert/alert-handler.service';
import { ConfirmationModalComponent } from '@src/app/component-tools/confirmation-modal/confirmation-modal/confirmation-modal.component';
import { ComportamientosService } from '../comportamientos.service';

@Component({
  selector: 'app-table-requisitos-comportamientos',
  templateUrl: './table-requisitos-comportamientos.component.html',
  styleUrls: ['./table-requisitos-comportamientos.component.scss']
})
export class TableRequisitosComportamientosComponent implements OnInit {
  @Input() comportamiento?: ComportamientoExpedienteDto;
  public totalElements = 0;
  public pageIndex = 1;
  public pageCount = 10;
  rows: DataRow[];
  columns: DataColumn[];
  @ViewChild('tableRequisitos', { static: true })
  tableRequisitos: DataTableComponent;
  requisitosSeleccionados: RequisitoComportamientoExpedienteDto[] = [];
  public confCbRequisitoExpediente: ConfigureCombobox;
  public confCbTipoRequisitoExpediente: ConfigureCombobox;
  @ViewChild('confirmModal', { static: false })
  confirmModal: ConfirmationModalComponent;
  @Output() onChange = new EventEmitter<DataRow[]>();

  constructor(
    private translateService: TranslateService,
    private alertService: AlertHandlerService,
    private comportamientoService: ComportamientosService
  ) {}

  ngOnInit(): void {
    this.rows = [];
    this.initializetableRequisitosHeaders();
  }

  private initializetableRequisitosHeaders(): void {
    this.columns = [
      {
        field: 'seleccion',
        sortable: false,
        class: 'text-center',
        style: { width: '50px' }
      },
      {
        field: RequisitoComportamientoExpedienteField.nombre,
        sortable: false,
        header: this.translateService.instant(
          'pages.formularioComportamientos.condicionExpediente'
        )
      },
      {
        field: RequisitoComportamientoExpedienteField.obligatoria,
        sortable: false,
        header: this.translateService.instant(
          'pages.formularioComportamientos.tipoRequisito'
        )
      },
      {
        field: RequisitoComportamientoExpedienteField.estaVigente,
        sortable: false,
        header: this.translateService.instant(
          'pages.formularioComportamientos.vigente'
        )
      },
      {
        field: RequisitoComportamientoExpedienteField.requeridaParaTitulo,
        sortable: false,
        header: this.translateService.instant(
          'pages.formularioComportamientos.requeridaParaTitulo'
        )
      },
      {
        field: RequisitoComportamientoExpedienteField.requeridaParaPago,
        sortable: false,
        header: this.translateService.instant(
          'pages.formularioComportamientos.requeridaParaPago'
        )
      },
      {
        field: RequisitoComportamientoExpedienteField.requiereDocumentacion,
        sortable: false,
        header: this.translateService.instant(
          'pages.formularioComportamientos.requiereDocumentacion'
        )
      },
      {
        field:
          RequisitoComportamientoExpedienteField.modoRequerimientoDocumentacion,
        sortable: false,
        header: this.translateService.instant(
          'pages.formularioComportamientos.modoRequerimientoDocumentacion'
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

  loadRequisitos(): void {
    if (this.comportamiento) {
      this.tableRequisitos.block();
      const data = [];
      this.comportamiento.requisitosComportamientosExpedientes.forEach(
        (element: RequisitoComportamientoExpedienteDto, index: number) => {
          data.push(this.createRowDataRequisitos(element, index));
        }
      );
      this.totalElements = data.length;
      this.rows = data;
      this.tableRequisitos.unblock();
    }
  }

  createRowDataRequisitos(
    element: RequisitoComportamientoExpedienteDto,
    index: number
  ): DataRow {
    return new DataRow(
      {
        index: index,
        checkbox: '',
        nombre: '',
        obligatoria: '',
        estaVigente: '',
        requeridaParaTitulo: '',
        requeridaParaPago: '',
        requiereDocumentacion: '',
        modoRequerimientoDocumentacion:
          element.requisitoExpediente.modoRequerimientoDocumentacion,
        opciones: ''
      },
      element
    );
  }

  loadComboboxRequisitoExpediente(): void {
    this.confCbRequisitoExpediente = new ConfigureCombobox({
      url: `${keys.API_BASE_URL}/api/v1/requisitos-expedientes/query-all`,
      data: {
        limit: '#PER_PAGE',
        offset: '#PAGE',
        where: [
          {
            field: 'filterNombre',
            value: '#SEARCH'
          }
        ]
      },
      method: 'POST',
      transformData: (elems: any) => {
        const dataFilter = elems.filter(
          (requisito) => !this.existeRequisitoTabla(requisito.id)
        );
        return dataFilter.map((requisito) => {
          return new ComboboxItem({
            value: requisito.id,
            text: requisito.nombre,
            data: requisito
          });
        });
      }
    });
  }

  loadComboboxTipoRequisitoExpediente(): void {
    this.confCbTipoRequisitoExpediente = new ConfigureCombobox({
      url: `${keys.API_BASE_URL}/api/v1/requisitos-expedientes/tipos-requisitos/query`,
      data: {
        limit: '#PER_PAGE',
        offset: '#PAGE',
        where: [
          {
            field: 'filterNombre',
            value: '#SEARCH'
          }
        ]
      },
      method: 'POST',
      transformData: (data: any) => {
        return data.map(
          (tipoRequisito) =>
            new ComboboxItem({
              value: tipoRequisito.id,
              text: tipoRequisito.nombre,
              data: tipoRequisito
            })
        );
      }
    });
  }

  showTextBoolean(isTrue: boolean): string {
    const texto = isTrue ? 'si' : 'no';
    return this.translateService.instant(
      `pages.listadoComportamientos.${texto}`
    );
  }

  esObligatoriotipoRequisito(
    tipoRequisitoExpediente: TipoRequisitoExpedienteDto
  ): boolean {
    const texto = tipoRequisitoExpediente.id == 1 ? 'obligatoria' : 'opcional';
    return this.translateService.instant(
      `pages.formularioComportamientos.${texto}`
    );
  }

  isCheckedRequisito(idRequisitoExpediente: number): boolean {
    return this.requisitosSeleccionados.some(
      (r) => r.requisitoExpediente.id === idRequisitoExpediente
    );
  }

  checkRequisito(
    event: Event,
    data: RequisitoComportamientoExpedienteDto
  ): void {
    const checked: boolean = (event.target as HTMLInputElement).checked;
    if (checked) {
      this.requisitosSeleccionados.push(data);
    } else {
      this.requisitosSeleccionados = this.requisitosSeleccionados.filter(
        (s) => s.requisitoExpediente.id !== data.requisitoExpediente.id
      );
    }
  }

  isAllCheckedRequisito(): boolean {
    return (
      this.rows.length &&
      this.rows.every((fila) =>
        this.requisitosSeleccionados.some(
          (comportamiento) =>
            comportamiento.requisitoExpediente.id ===
            fila.object.requisitoExpediente.id
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

  newRequisitoExpediente(): void {
    const newElement: RequisitoComportamientoExpedienteDto = {
      id: null,
      requisitoExpediente: {} as RequisitoDto,
      tipoRequisitoExpediente: {} as TipoRequisitoExpedienteDto
    };
    const newRow = this.createRowDataRequisitos(newElement, this.rows.length);
    this.editRequisito(newRow);
    this.rows.push(newRow);
  }

  deleteMasive(): void {
    if (!this.requisitosSeleccionados.length) {
      this.alertService.error(
        this.translateService.instant('validations.atLeastOneSelected')
      );
      return;
    }
    if (this.rows.length == this.requisitosSeleccionados.length) {
      this.alertService.error(
        this.translateService.instant(
          'pages.formularioComportamientos.validations.RequisitoatLeastOneExist'
        )
      );
      return;
    }
    this.confirmModal.show(
      () => {
        this.tableRequisitos.block();
        if (this.comportamiento) {
          const requisitosComportamientosExpedientes = new RequisitoComportamientoExpedienteMasivoDto();
          requisitosComportamientosExpedientes.IdsRequisitosComportamientos = this.requisitosSeleccionados.map(
            (r) => r.id
          );
          this.comportamientoService
            .deleteRequisitoComportamientoMasivo(
              requisitosComportamientosExpedientes
            )
            .subscribe(() => {
              this.alertService.success(
                this.translateService.instant('messages.success')
              );
              this.tableRequisitos.unblock();
              this.onChange.emit(this.rows);
            });
        } else {
          this.requisitosSeleccionados.forEach((requisitoSeleccionado) => {
            const dataRow = this.rows.find(
              (r) =>
                r.object.requisitoExpediente.id ==
                requisitoSeleccionado.requisitoExpediente.id
            );
            this.removeRequisito(dataRow);
            this.onChange.emit(this.rows);
          });
          this.tableRequisitos.unblock();
        }
      },
      this.translateService.instant(
        'pages.formularioComportamientos.deleteRequisitoComportamientoTitleMassive'
      ),
      this.translateService.instant(
        'pages.formularioComportamientos.deleteRequisitoComportamientoMessageMassive'
      ),
      this.translateService.instant('common.aceptar')
    );
  }

  editRequisito(data: DataRow): void {
    data.rowData.esEdicion = true;
    this.loadComboboxRequisitoExpediente();
    if (data.object.requisitoExpediente.id)
      data.rowData.requisitoExpedienteEditar = new ComboboxItem({
        value: data.object.requisitoExpediente.id,
        text: data.object.requisitoExpediente.nombre,
        data: data.object.requisitoExpediente
      });
    this.loadComboboxTipoRequisitoExpediente();
    if (data.object.tipoRequisitoExpediente.id)
      data.rowData.tipoRequisitoExpedienteEditar = new ComboboxItem({
        value: data.object.tipoRequisitoExpediente.id,
        text: data.object.tipoRequisitoExpediente.nombre,
        data: data.object.tipoRequisitoExpediente
      });
  }

  deleteRequisito(data: DataRow): void {
    if (this.rows.length == 1) {
      this.alertService.error(
        this.translateService.instant(
          'pages.formularioComportamientos.validations.RequisitoatLeastOneExist'
        )
      );
      return;
    }
    this.confirmModal.show(
      () => {
        this.tableRequisitos.block();
        if (data.object.id != null) {
          this.comportamientoService
            .deleteRequisitoComportamiento(data.object.id)
            .subscribe(() => {
              this.alertService.success(
                this.translateService.instant('messages.success')
              );
              this.tableRequisitos.unblock();
              this.onChange.emit(this.rows);
            });
        } else {
          this.removeRequisito(data);
          this.tableRequisitos.unblock();
          this.onChange.emit(this.rows);
        }
      },
      this.translateService.instant(
        'pages.formularioComportamientos.deleteRequisitoComportamientoTitle'
      ),
      this.translateService.instant(
        'pages.formularioComportamientos.deleteRequisitoComportamientoMessage'
      ),
      this.translateService.instant('common.aceptar')
    );
  }

  removeRequisito(data: DataRow): void {
    this.rows.splice(data.rowData.index, 1);
    this.rows = this.rows.map((r: DataRow, index: number) => {
      r.rowData.index = index;
      return r;
    });
  }

  existeRequisitoTabla(idRequisito: number): boolean {
    return this.rows.some(
      (r) => r.object.requisitoExpediente.id === idRequisito
    );
  }

  saveEditRequisito(data: DataRow): void {
    if (!data.rowData.requisitoExpedienteEditar) {
      this.alertService.error(
        this.translateService.instant(
          'pages.formularioComportamientos.validations.RequisitoatLeastOneSelected'
        )
      );
      return;
    }
    if (!data.rowData.tipoRequisitoExpedienteEditar) {
      this.alertService.error(
        this.translateService.instant(
          'pages.formularioComportamientos.validations.TipoRequisitoatLeastOneSelected'
        )
      );
      return;
    }
    const element: RequisitoComportamientoExpedienteDto = data.object;
    element.requisitoExpediente = data.rowData.requisitoExpedienteEditar.data;
    element.tipoRequisitoExpediente =
      data.rowData.tipoRequisitoExpedienteEditar.data;
    this.tableRequisitos.block();
    if (element.id == null) {
      this.rows[data.rowData.index] = this.createRowDataRequisitos(
        element,
        data.rowData.index
      );
      if (this.comportamiento) {
        const payload = CreateRequisitoComportamientoExpedienteDto.create(
          element,
          this.comportamiento.id
        );
        this.comportamientoService
          .createRequisitoComportamiento(payload)
          .subscribe(() => {
            this.alertService.success(
              this.translateService.instant('messages.success')
            );
            this.tableRequisitos.unblock();
            this.onChange.emit(this.rows);
          });
      } else {
        this.tableRequisitos.unblock();
        this.onChange.emit(this.rows);
      }
    } else {
      const payload = EditRequisitoComportamientoExpedienteDto.create(element);
      this.comportamientoService
        .updateRequisitoComportamiento(payload)
        .subscribe(() => {
          this.alertService.success(
            this.translateService.instant('messages.success')
          );
          this.tableRequisitos.unblock();
          this.onChange.emit(this.rows);
        });
    }
  }

  cancelEditRequisito(data: DataRow): void {
    if (!data.object.requisitoExpediente.id) {
      this.removeRequisito(data);
    } else {
      data.rowData.esEdicion = false;
    }
  }

  update(comportamiento: ComportamientoExpedienteDto): void {
    this.comportamiento = comportamiento;
    this.tableRequisitos.update();
  }

  block(): void {
    this.tableRequisitos.block();
  }
  unblock(): void {
    this.tableRequisitos.unblock();
  }
}
