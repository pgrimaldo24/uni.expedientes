import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormGroup, Validators } from '@angular/forms';
import {
  ComboboxItem,
  ConfigureCombobox
} from '@src/app/component-tools/combobox/models';
import { DataTableComponent } from '@src/app/component-tools/data-table/data-table.component';
import {
  DataColumn,
  DataRow
} from '@src/app/component-tools/data-table/models';
import { CustomSimpleListItemModel } from '@src/app/models/erp-academico';
import { keys } from '@src/keys';
import * as help from '@helpers/commons-helpers';
import { UnirValidators } from '@src/app/core/unir-validators';
import {
  ConfiguracionExpedienteUniversidadDto,
  ConsolidacionRequisitoExpedienteDocumentoDto,
  ConsolidacionRequisitoExpedienteModel,
  EstadoRequisitoExpediente,
  FormConsolidarRequisitoControls
} from '../consolidar-requisito.models';
import {
  AlumnoInfoDto,
  ConsolidacionRequisitoExpedienteDto,
  RequisitoExpedienteDocumentoDto
} from '@pages/expediente-alumno/expediente-models';
import { ConfirmationModalComponent } from '@src/app/component-tools/confirmation-modal/confirmation-modal/confirmation-modal.component';
import { TranslateService } from '@ngx-translate/core';
import { AlertHandlerService } from '@src/app/component-tools/alert/alert-handler.service';

import { DownloadFileService } from '@src/app/services/download-file.service';
import { BlockUIService } from 'ng-block-ui';
import { Guid } from 'guid-typescript';
import { SecurityService } from '@src/app/services/security.service';
import { AlumnoService } from '@pages/alumno/alumno.service';
import { ConsolidarRequisitoService } from './consolidar-requisito.service';

@Component({
  selector: 'app-consolidar-requisito-form',
  templateUrl: './consolidar-requisito-form.component.html',
  styleUrls: ['./consolidar-requisito-form.component.scss']
})
export class ConsolidarRequisitoFormComponent implements OnInit {
  @Input() requisitoForm: FormGroup;
  @Input() idExpediente: number;
  @Input() configuracionUniversidad: ConfiguracionExpedienteUniversidadDto;
  @Input() blockIdentityRequisito: string;
  @Input() alumno: AlumnoInfoDto;
  @ViewChild('table', { static: true }) table: DataTableComponent;
  @ViewChild('confirmModal', { static: false })
  confirmModal: ConfirmationModalComponent;
  public rows: DataRow[];
  public columns: DataColumn[];
  public comboIdioma: ConfigureCombobox;
  public comboCausaEstadoRequisito: ConfigureCombobox;
  public comboRequisitoDocumento: ConfigureCombobox;
  public formControls = FormConsolidarRequisitoControls;
  public consolidacion: ConsolidacionRequisitoExpedienteDto;
  public blockIdentity = Guid.create().toString();
  public documentosRequisitos: ComboboxItem[] = [];
  public ficheroTooltip = '';
  public isAdminGestor: boolean;
  private roles = keys;
  validFormats = ['doc', 'docx', 'pdf', 'jpg', 'jpeg', 'png', 'gif'];
  dataDocumentosItems = [];

  constructor(
    private translateService: TranslateService,
    private alertService: AlertHandlerService,
    private downloadService: DownloadFileService,
    private blockUIService: BlockUIService,
    private security: SecurityService,
    private alumnoService: AlumnoService,
    private consolidarRequisitoService: ConsolidarRequisitoService
  ) {}

  ngOnInit(): void {
    this.initializeTableHeaders();
    this.initializeCombobox();
    this.consolidarRequisitoService.consolidacionSubject.subscribe((value) => {
      if (!this.hasAccessRole(value)) {
        this.alertService.error(
          this.translateService.instant(
            'pages.expedienteTabs.requisitos.consolidarRequisito.errorAccessRole'
          )
        );
        return;
      }
      this.consolidacion = value;
      this.patchForm();
      this.setConfigInputs();
      this.search();
    });
  }

  hasAccessRole(consolidacion: ConsolidacionRequisitoExpedienteDto): boolean {
    const userRoles = this.security.userRoles();
    this.isAdminGestor =
      userRoles.indexOf(this.roles.ADMIN_ROLE) > -1 ||
      userRoles.indexOf(this.roles.GESTOR_ROLE) > -1;

    if (!consolidacion.requisitoExpediente.estaRestringida) return true;
    const requisitoRoles = consolidacion.requisitoExpediente.rolesRequisitosExpedientes.map(
      (rol) => rol.rol
    );
    const access = userRoles.some((rol) => requisitoRoles.includes(rol));
    return access;
  }

  search(): void {
    const data = [];
    this.blockUIService.start(this.blockIdentity);
    this.consolidarRequisitoService
      .getConsolidacionRequisitoDocumentosByIdConsolidacion(
        this.consolidacion.id
      )
      .subscribe((info) => {
        info.forEach(
          (element: ConsolidacionRequisitoExpedienteDocumentoDto) => {
            data.push(
              new DataRow(
                {
                  nombreFichero: element.fichero,
                  opciones: ''
                },
                element
              )
            );
          }
        );
        this.blockUIService.stop(this.blockIdentity);
        this.rows = data;
      });
  }

  patchForm(): void {
    this.requisitoForm.patchValue({
      [FormConsolidarRequisitoControls.esDocumentacionFisica]: this
        .consolidacion.esDocumentacionFisica,
      [FormConsolidarRequisitoControls.enviadaPorAlumno]: this.consolidacion
        .enviadaPorAlumno,
      [FormConsolidarRequisitoControls.texto]: this.consolidacion.texto,
      [FormConsolidarRequisitoControls.fecha]: this.consolidacion.fecha
        ? new Date(this.consolidacion.fecha)
        : null,
      [FormConsolidarRequisitoControls.idioma]: this.consolidacion.idRefIdioma
        ? {
            value: this.consolidacion.idRefIdioma,
            text: this.consolidacion.nombreIdioma,
            data: {
              siglas: this.consolidacion.siglasIdioma
            }
          }
        : null,
      [FormConsolidarRequisitoControls.nivelIdioma]: this.consolidacion
        .nivelIdioma,
      [FormConsolidarRequisitoControls.requisitoDocumento]: null
    });
  }

  setConfigInputs(): void {
    this.setValidatorToInput(
      FormConsolidarRequisitoControls.texto,
      this.consolidacion.requisitoExpediente.requiereTextoAdicional
    );
    this.setValidatorToInput(
      FormConsolidarRequisitoControls.idioma,
      this.consolidacion.requisitoExpediente.certificadoIdioma
    );
    this.setValidatorToInput(
      FormConsolidarRequisitoControls.nivelIdioma,
      this.consolidacion.requisitoExpediente.certificadoIdioma
    );

    this.configComboCausaEstadoRequisito(
      this.consolidacion.requisitoExpediente.id,
      this.consolidacion.estadoRequisitoExpediente?.id
    );
    this.documentosRequisitos = this.asignarRequisitosDocumentos(
      this.consolidacion.requisitoExpediente.requisitosExpedientesDocumentos
    );
    if (this.consolidacion.requisitoExpediente.requiereDocumentacion) {
      this.ctrlField(
        FormConsolidarRequisitoControls.esDocumentacionFisica
      ).enable();
      this.ctrlField(FormConsolidarRequisitoControls.enviadaPorAlumno).enable();
    } else {
      this.ctrlField(
        FormConsolidarRequisitoControls.esDocumentacionFisica
      ).disable();
      this.ctrlField(
        FormConsolidarRequisitoControls.enviadaPorAlumno
      ).disable();
    }
    if (this.configuracionUniversidad) {
      this.ficheroTooltip =
        this.translateService.instant(
          'pages.expedienteTabs.requisitos.consolidarRequisito.ficheroTooltip'
        ) + `${this.configuracionUniversidad.tamanyoMaximoFichero} MB`;
    }
  }

  get tieneCodigoClasificacionUniversidad(): boolean {
    return (
      this.configuracionUniversidad &&
      this.configuracionUniversidad.codigoDocumental !== ''
    );
  }

  setValidatorToInput(control: string, hasValidation: boolean): void {
    const field = this.ctrlField(control);
    field.setValidators(hasValidation ? Validators.required : null);
    field.updateValueAndValidity();
  }

  initializeTableHeaders(): void {
    this.columns = [
      {
        field: 'nombreFichero',
        sortable: false,
        header: 'Nombre'
      },
      {
        field: 'opciones',
        sortable: false,
        header: 'Opciones',
        style: { width: '100px' }
      }
    ];
  }

  initializeCombobox(): void {
    this.comboIdioma = new ConfigureCombobox({
      url: `${keys.API_BASE_URL}/api/v1/integracion/erp-academico/idiomas`,
      perPage: 10,
      data: {
        searchText: '#SEARCH'
      },
      method: 'POST',
      transformData: (data: CustomSimpleListItemModel[]) => help.getItems(data)
    });
  }

  configComboCausaEstadoRequisito(
    idRequisito: number,
    idEstadoConsolidacion: number
  ): void {
    this.comboCausaEstadoRequisito = new ConfigureCombobox({
      url: `${keys.API_BASE_URL}/api/v1/consolidaciones-requisitos-expedientes/causas-query`,
      perPage: 10,
      data: {
        offset: '#PAGE',
        limit: '#PER_PAGE',
        where: [
          {
            field: 'idRequisito',
            value: idRequisito
          },
          {
            field: 'idEstadoConsolidacion',
            value: idEstadoConsolidacion
          },
          {
            field: 'filterNombre',
            value: '#SEARCH'
          }
        ]
      },
      method: 'POST',
      transformData: (data: CustomSimpleListItemModel[]) => help.getItems(data)
    });
  }

  asignarRequisitosDocumentos(
    documentos: RequisitoExpedienteDocumentoDto[]
  ): ComboboxItem[] {
    const comboboxItems: ComboboxItem[] = [];

    documentos.forEach((documento) => {
      comboboxItems.push({
        id: documento.id.toString(),
        text: documento.nombreDocumento,
        value: documento.id
      });
    });
    return comboboxItems;
  }

  onFileInput(event: Event): void {
    if (
      !(event.target as HTMLInputElement).files.length ||
      this.consolidacion.isEstadoValidada
    )
      return;

    const fieldFileUpload = this.ctrlField(
      FormConsolidarRequisitoControls.fileUpload
    );
    fieldFileUpload.setValidators(
      UnirValidators.inputFileValidator(
        (event.target as HTMLInputElement).files,
        this.configuracionUniversidad.tamanyoMaximoFichero
      )
    );
    fieldFileUpload.updateValueAndValidity();
    if (fieldFileUpload.invalid) return;

    const requisitoDocumento = this.ctrlField(
      FormConsolidarRequisitoControls.requisitoDocumento
    ).value;
    if (requisitoDocumento) {
      this.saveConsolidacionRequisitoDocumento(event, requisitoDocumento.id);
      return;
    }

    this.alertService.error(
      this.translateService.instant(
        'pages.expedienteTabs.requisitos.consolidarRequisito.requisitoDocumentoRequerido'
      )
    );
  }

  saveConsolidacionRequisitoDocumento(
    event: Event,
    idRequisitoDocumento: number
  ): void {
    ((event.target as HTMLButtonElement)
      .parentElement as HTMLButtonElement).disabled = true;
    ((event.target as HTMLButtonElement)
      .previousSibling as HTMLButtonElement).className =
      'fa fa-spinner fa-pulse fa-fw';
    const fileToUpload = (event.target as HTMLInputElement).files.item(0);
    const formData: FormData = new FormData();
    formData.append('idRequisitoDocumento', idRequisitoDocumento.toString());
    formData.append('file', fileToUpload, fileToUpload.name);
    this.consolidarRequisitoService
      .uploadFileConsolidacionRequisitoDocumento(
        this.consolidacion.id,
        formData
      )
      .subscribe(
        () => {
          this.search();
          ((event.target as HTMLButtonElement)
            .parentElement as HTMLButtonElement).disabled = false;
          ((event.target as HTMLButtonElement)
            .previousSibling as HTMLButtonElement).className =
            'fas fa-plus fa-sm';
        },
        () => {
          ((event.target as HTMLButtonElement)
            .parentElement as HTMLButtonElement).disabled = false;
          ((event.target as HTMLButtonElement)
            .previousSibling as HTMLButtonElement).className =
            'fas fa-plus fa-sm';
        }
      );
  }

  ctrlField(name: string): AbstractControl {
    return this.requisitoForm.get(name);
  }

  ctrlInvalid(name: string): boolean {
    const field = this.ctrlField(name);
    return field.invalid;
  }

  cancelChanges(): void {
    this.confirmModal.show(
      () => {
        this.requisitoForm.reset();
        this.consolidacion = null;
        this.rows = [];
      },
      this.translateService.instant(
        'pages.expedienteTabs.requisitos.consolidarRequisito.cancelarCambios'
      ),
      this.translateService.instant(
        'pages.expedienteTabs.requisitos.consolidarRequisito.cancelarMessage'
      ),
      this.translateService.instant('common.aceptar')
    );
  }

  updateConsolidacionRequisitoDocumento(
    consolidacionDocumento: ConsolidacionRequisitoExpedienteDocumentoDto,
    button: Event
  ): void {
    if (this.consolidacion.isEstadoValidada) return;
    const payload: ConsolidacionRequisitoExpedienteDocumentoDto = {
      id: consolidacionDocumento.id,
      ficheroValidado: !consolidacionDocumento.ficheroValidado
    };
    ((button.target as HTMLButtonElement)
      .parentElement as HTMLButtonElement).disabled = true;
    (button.target as HTMLButtonElement).className =
      'fa fa-spinner fa-pulse fa-fw';
    this.consolidarRequisitoService
      .updateConsolidacionRequisitoDocumentosById(payload)
      .subscribe(
        () => {
          this.search();
          ((button.target as HTMLButtonElement)
            .parentElement as HTMLButtonElement).disabled = false;
          (button.target as HTMLButtonElement).className =
            'fas fa-trash-alt text-dark';
        },
        () => {
          ((button.target as HTMLButtonElement)
            .parentElement as HTMLButtonElement).disabled = false;
          (button.target as HTMLButtonElement).className =
            'fas fa-trash-alt text-dark';
        }
      );
  }

  downloadFicheroConsolidacionRequisitoDocumento(
    id: number,
    button: Event
  ): void {
    if (this.consolidacion.isEstadoValidada) return;
    ((button.target as HTMLButtonElement)
      .parentElement as HTMLButtonElement).disabled = true;
    (button.target as HTMLButtonElement).className =
      'fa fa-spinner fa-pulse fa-fw';
    this.consolidarRequisitoService
      .downloadFicheroConsolidacionRequisitoDocumento(id)
      .subscribe({
        next: (response) => {
          this.downloadService.downloadFile(
            response.body,
            response.headers.get('content-type'),
            response.headers.get('X-File-Name')
          );
          ((button.target as HTMLButtonElement)
            .parentElement as HTMLButtonElement).disabled = false;
          (button.target as HTMLButtonElement).className =
            'fas fa-download text-dark';
        },
        error: () => {
          ((button.target as HTMLButtonElement)
            .parentElement as HTMLButtonElement).disabled = false;
          (button.target as HTMLButtonElement).className =
            'fas fa-download text-dark';
        }
      });
  }

  deleteConsolidacionRequisitoDocumento(id: number, button: Event): void {
    if (this.consolidacion.isEstadoValidada) return;
    this.confirmModal.show(
      () => {
        ((button.target as HTMLButtonElement)
          .parentElement as HTMLButtonElement).disabled = true;
        (button.target as HTMLButtonElement).className =
          'fa fa-spinner fa-pulse fa-fw';
        this.consolidarRequisitoService
          .deleteConsolidacionRequisitoDocumentosById(id)
          .subscribe(
            () => {
              this.search();
              ((button.target as HTMLButtonElement)
                .parentElement as HTMLButtonElement).disabled = false;
              (button.target as HTMLButtonElement).className =
                'fas fa-trash-alt text-dark';
            },
            () => {
              ((button.target as HTMLButtonElement)
                .parentElement as HTMLButtonElement).disabled = false;
              (button.target as HTMLButtonElement).className =
                'fas fa-trash-alt text-dark';
            }
          );
      },
      this.translateService.instant(
        'pages.expedienteTabs.requisitos.consolidarRequisito.deleteFileTitle'
      ),
      this.translateService.instant(
        'pages.expedienteTabs.requisitos.consolidarRequisito.deleteFileMessage'
      ),
      this.translateService.instant('common.aceptar')
    );
  }

  updateConsolidacion(): void {
    this.setValidatorToInput(FormConsolidarRequisitoControls.fileUpload, false);
    this.setValidatorToInput(
      FormConsolidarRequisitoControls.requisitoAdicional,
      false
    );
    this.requisitoForm.markAllAsTouched();
    if (this.requisitoForm.invalid) return;
    if (this.consolidacion.isEstadoValidada) return;

    const payload = ConsolidacionRequisitoExpedienteModel.create(
      this.requisitoForm.value
    );
    payload.id = this.consolidacion.id;
    this.blockUIService.start(this.blockIdentity);
    this.consolidarRequisitoService
      .updateConsolidacionRequisitoExpediente(payload)
      .subscribe(() => {
        this.blockUIService.stop(this.blockIdentity);
        this.updatePropiedadesConsolidacion(
          payload,
          this.requisitoForm.value.fecha
        );
        this.alertService.success(
          this.translateService.instant('messages.success')
        );
      });
  }

  updatePropiedadesConsolidacion(
    payload: ConsolidacionRequisitoExpedienteModel,
    fecha: Date
  ): void {
    this.consolidacion.esDocumentacionFisica = payload.esDocumentacionFisica;
    this.consolidacion.enviadaPorAlumno = payload.enviadaPorAlumno;
    this.consolidacion.texto = payload.texto;
    this.consolidacion.fecha = fecha;
    this.consolidacion.idRefIdioma = payload.idRefIdioma;
    this.consolidacion.nombreIdioma = payload.nombreIdioma;
    this.consolidacion.siglasIdioma = payload.siglasIdioma;
    this.consolidacion.nivelIdioma = payload.nivelIdioma;
    this.consolidacion.isEstadoNoProcesada = false;
    this.consolidacion.isEstadoValidada = false;
    this.consolidacion.isEstadoRechazada = false;
    this.consolidacion.isEstadoPendiente = true;
    this.consolidacion.estadoRequisitoExpediente.nombre =
      EstadoRequisitoExpediente.Pendiente;
  }

  createConsolidacionRequisito(): void {
    this.setValidatorToInput(
      FormConsolidarRequisitoControls.requisitoAdicional,
      true
    );
    const requisitoExpediente = this.ctrlField(
      FormConsolidarRequisitoControls.requisitoAdicional
    );
    if (requisitoExpediente.invalid || !requisitoExpediente.value) return;

    const payload = new ConsolidacionRequisitoExpedienteModel();
    payload.idExpedienteAlumno = this.idExpediente;
    payload.idRequisitoExpediente = requisitoExpediente.value.value;
    this.blockUIService.start(this.blockIdentityRequisito);
    this.consolidarRequisitoService
      .createConsolidacionRequisitoExpediente(payload)
      .subscribe((value) => {
        this.getConsolidacionRequisitoExpediente(value);
        this.alertService.success(
          this.translateService.instant('messages.success')
        );
      });
  }

  deleteConsolidacionRequisito(): void {
    if (this.consolidacion.isEstadoValidada) return;

    this.confirmModal.show(
      () => {
        this.blockUIService.start(this.blockIdentity);
        this.consolidarRequisitoService
          .deleteConsolidacionRequisito(this.consolidacion.id)
          .subscribe(() => {
            const expedienteAlumno = this.alumno.expedientes?.find(
              (e) => e.id == this.idExpediente
            );
            expedienteAlumno.consolidacionesRequisitosExpedientes = expedienteAlumno.consolidacionesRequisitosExpedientes.filter(
              (item) => item.id !== this.consolidacion.id
            );
            this.requisitoForm.reset();
            this.consolidacion = null;
            this.rows = [];
            this.alumnoService.setFlat(!this.alumnoService.flat);
            this.blockUIService.stop(this.blockIdentity);
          });
      },
      this.translateService.instant(
        'pages.expedienteTabs.requisitos.consolidarRequisito.deleteConsolidacionRequisitoTitle'
      ),
      this.translateService.instant(
        'pages.expedienteTabs.requisitos.consolidarRequisito.deleteConsolidacionRequisitoMessage'
      ),
      this.translateService.instant('common.aceptar')
    );
  }

  rechazarConsolidacionRequisito(): void {
    if (
      !this.consolidacion.isEstadoPendiente &&
      !this.consolidacion.isEstadoRechazada
    )
      return;

    const payload = new ConsolidacionRequisitoExpedienteModel();
    payload.id = this.consolidacion.id;
    payload.idCausaEstadoRequisito = this.requisitoForm.value.causaEstadoRequisito?.value;

    this.blockUIService.start(this.blockIdentity);
    this.consolidarRequisitoService
      .rechazarConsolidacionRequisitoExpediente(payload)
      .subscribe(() => {
        this.consolidacion.isEstadoRechazada = true;
        this.consolidacion.isEstadoPendiente = false;
        this.consolidacion.estadoRequisitoExpediente.nombre =
          EstadoRequisitoExpediente.Rechazado;
        this.blockUIService.stop(this.blockIdentity);
        this.alertService.success(
          this.translateService.instant('messages.success')
        );
      });
  }

  validarConsolidacionRequisito(): void {
    if (
      !this.consolidacion.isEstadoPendiente &&
      !this.consolidacion.isEstadoRechazada
    )
      return;

    const payload = new ConsolidacionRequisitoExpedienteModel();
    payload.id = this.consolidacion.id;
    payload.idCausaEstadoRequisito = this.requisitoForm.value.causaEstadoRequisito?.value;

    this.blockUIService.start(this.blockIdentity);
    this.consolidarRequisitoService
      .validarConsolidacionRequisitoExpediente(payload)
      .subscribe(() => {
        this.consolidacion.isEstadoValidada = true;
        this.consolidacion.isEstadoRechazada = false;
        this.consolidacion.isEstadoPendiente = false;
        this.consolidacion.estadoRequisitoExpediente.nombre =
          EstadoRequisitoExpediente.Validado;
        this.blockUIService.stop(this.blockIdentity);
        this.alertService.success(
          this.translateService.instant('messages.success')
        );
      });
  }

  getConsolidacionRequisitoExpediente(id: number): void {
    this.consolidarRequisitoService
      .getConsolidacionRequisitoExpediente(id)
      .subscribe((value) => {
        const expedienteAlumno = this.alumno.expedientes?.find(
          (e) => e.id == this.idExpediente
        );
        expedienteAlumno.consolidacionesRequisitosExpedientes.push(value);
        this.alumnoService.setFlat(!this.alumnoService.flat);
        this.blockUIService.stop(this.blockIdentityRequisito);
      });
  }

  createConsolidacionesRequisitosExpediente(): void {
    this.blockUIService.start(this.blockIdentityRequisito);
    this.consolidarRequisitoService
      .createConsolidacionesRequisitosExpediente(this.idExpediente)
      .subscribe(() => {
        this.getConsolidacionesRequisitosExpediente();
      });
  }

  getConsolidacionesRequisitosExpediente(): void {
    this.consolidarRequisitoService
      .getConsolidacionesRequisitosByIdExpediente(this.idExpediente)
      .subscribe((value) => {
        const expedienteAlumno = this.alumno.expedientes?.find(
          (e) => e.id == this.idExpediente
        );
        const consolidaciones = value.filter(
          (val) =>
            !expedienteAlumno.consolidacionesRequisitosExpedientes.some(
              (cre) => cre.id === val.id
            )
        );
        if (consolidaciones.length) {
          expedienteAlumno.consolidacionesRequisitosExpedientes.push(
            ...consolidaciones
          );
          this.alumnoService.setFlat(!this.alumnoService.flat);
        }
        this.alertService.success(
          this.translateService.instant(
            consolidaciones.length
              ? 'messages.success'
              : 'pages.expedienteTabs.requisitos.consolidarRequisito.mensajeRequisitosActualizados'
          )
        );
        this.blockUIService.stop(this.blockIdentityRequisito);
      });
  }
}
