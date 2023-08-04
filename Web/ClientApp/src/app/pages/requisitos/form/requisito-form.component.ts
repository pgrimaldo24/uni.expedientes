import { Component, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import {
  ComboboxItem,
  ConfigureCombobox
} from '@src/app/component-tools/combobox/models';
import { CustomSimpleListItemModel } from '@src/app/models/erp-academico';
import { keys } from '@src/keys';
import { Guid } from 'guid-typescript';
import { BlockUIService } from 'ng-block-ui';
import {
  EditRequisitoModel,
  FileTypeAcademicoModel,
  RequisitoDto,
  RolRequisitoExpedienteDto
} from '../requesitos.models';
import { RequisitosService } from '../requisitos.service';
import { RequisitoControl } from './requisito-control.model';
import * as help from '@helpers/commons-helpers';
import { TranslateService } from '@ngx-translate/core';
import { AlertHandlerService } from '@src/app/component-tools/alert/alert-handler.service';

@Component({
  selector: 'app-requisito-form',
  templateUrl: './requisito-form.component.html',
  styleUrls: ['./requisito-form.component.scss']
})
export class RequisitoFormComponent implements OnInit {
  idRequisito: number;
  public disabledFilesTipes: boolean;
  requisito: RequisitoDto;
  filesTypes: FileTypeAcademicoModel[] = [];
  public controls = RequisitoControl;
  public form: FormGroup;
  public blockIdentity = Guid.create().toString();
  public configComboTipoExpedienteAdicional: ConfigureCombobox;
  public configComboModoRequerimientoDocumentacion: ConfigureCombobox;
  public configComboEstadoExpediente: ConfigureCombobox;
  public roles: ComboboxItem[] = [];
  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private requisitoSrv: RequisitosService,
    private blockUI: BlockUIService,
    private translate: TranslateService,
    private alertSvc: AlertHandlerService
  ) {
    this.form = this.createForm();
    this.roles = [
      {
        value: keys.ADMIN_ROLE,
        text: keys.ADMIN_ROLE_NAME
      },
      {
        value: keys.GESTOR_ROLE,
        text: keys.GESTOR_ROLE_NAME
      }
    ];
  }

  ngOnInit(): void {
    this.requisitoSrv.documentacionProtegidaChanched$.subscribe((result) => {
      this.form.patchValue({
        [RequisitoControl.documentacionProtegida]: result
      });
    });
    this.initializeFilter();
    this.initializeEdit();
  }

  initializeEdit(): void {
    this.route.params.subscribe(({ id }) => {
      this.idRequisito = id;
      this.loadRequisito(id);
    });
  }

  onSubmit(): void {
    this.form.markAllAsTouched();
    if (this.form.invalid) {
      return;
    }
    const payload = EditRequisitoModel.create(this.form.value);
    payload.id = this.idRequisito;
    payload.idsFilesTypes = this.getFilesTypes();
    if (!this.infoDocumentosValida(payload)) return;
    payload.idRefUniversidad = '1';
    this.blockUI.start(this.blockIdentity);
    this.requisitoSrv.updateRequisito(payload).subscribe(() => {
      this.blockUI.stop(this.blockIdentity);
      this.alertSvc.success(this.translate.instant('messages.success'));
      this.router.navigate(['/requisitos']);
    });
  }

  infoDocumentosValida(payload: EditRequisitoModel): boolean {
    let result = true;
    if (
      this.ctrlField(RequisitoControl.requiereDocumentacion).value &&
      payload.idsFilesTypes.length == 0
    ) {
      this.alertSvc.error(
        this.translate.instant('pages.formularioRequisitos.errorFilesTypes')
      );
      result = false;
    }
    if (payload.requiereDocumentacion && !this.requisitoSrv.hasDocumentos) {
      this.alertSvc.error(
        this.translate.instant('pages.formularioRequisitos.errorSinDocumentos')
      );
      result = false;
    }
    return result;
  }

  createForm(): FormGroup {
    return this.fb.group({
      [RequisitoControl.nombre]: new FormControl(null, [Validators.required]),
      [RequisitoControl.orden]: new FormControl(null),
      [RequisitoControl.descripcion]: new FormControl(true),
      [RequisitoControl.estaVigente]: new FormControl(true),
      [RequisitoControl.requeridaParaTitulo]: new FormControl(false),
      [RequisitoControl.requiereDocumentacion]: new FormControl(false),
      [RequisitoControl.documentacionProtegida]: new FormControl(false),
      [RequisitoControl.enviarEmailAlumno]: new FormControl(false),
      [RequisitoControl.requeridaParaPago]: new FormControl(false),
      [RequisitoControl.estaRestringida]: new FormControl(false),
      [RequisitoControl.esCertificado]: new FormControl(false),
      [RequisitoControl.requiereTextoAdicional]: new FormControl(false),
      [RequisitoControl.requiereMatricularse]: new FormControl(false),
      [RequisitoControl.idRefModoRequerimientoDocumentacion]: new FormControl(
        false
      ),
      [RequisitoControl.idEstadoExpediente]: new FormControl(false),
      [RequisitoControl.tipoExpedienteAdicional]: new FormControl(null),
      [RequisitoControl.modoDeRequerimientoDeDocumentacion]: new FormControl(
        null
      ),
      [RequisitoControl.estadoExpediente]: new FormControl(null),
      [RequisitoControl.roles]: new FormControl(null),
      [RequisitoControl.modoDeRequerimientoDeDocumentacion]: new FormControl(
        null
      ),
      [RequisitoControl.extensionesPermitidasParaElDocumento]: new FormControl(
        null
      )
    });
  }

  loadRequisito(id: number): void {
    this.blockUI.start(this.blockIdentity);
    this.requisitoSrv.getRequisito(id).subscribe((value) => {
      this.blockUI.stop(this.blockIdentity);
      this.requisito = value;
      this.patchForm(value);
      this.loadFilesTypes();
      this.deshabilitarDocumentacionProtegida();
      this.changeObligatorio(
        RequisitoControl.enviarEmailAlumno,
        RequisitoControl.descripcion
      );
      this.deshabilitarRoles(
        RequisitoControl.estaRestringida,
        RequisitoControl.roles
      );
    });
  }

  patchForm(value: RequisitoDto): void {
    this.form.patchValue({
      [RequisitoControl.nombre]: value.nombre,
      [RequisitoControl.orden]: value.orden,
      [RequisitoControl.estaVigente]: value.estaVigente,
      [RequisitoControl.descripcion]: value.descripcion,
      [RequisitoControl.requeridaParaTitulo]: value.requeridaParaTitulo,
      [RequisitoControl.requiereDocumentacion]: value.requiereDocumentacion,
      [RequisitoControl.requeridaParaPago]: value.requeridaParaPago,
      [RequisitoControl.enviarEmailAlumno]: value.enviarEmailAlumno,
      [RequisitoControl.estaRestringida]: value.estaRestringida,
      [RequisitoControl.requiereTextoAdicional]: value.requiereTextoAdicional,
      [RequisitoControl.esCertificado]: value.esCertificado,
      [RequisitoControl.requiereMatricularse]:
        value.requisitosExpedientesRequerimientosTitulos.length > 0
          ? value.requisitosExpedientesRequerimientosTitulos[0]
              .requiereMatricularse
          : false,
      [RequisitoControl.tipoExpedienteAdicional]:
        value.requisitosExpedientesRequerimientosTitulos.length == 0
          ? null
          : {
              value:
                value.requisitosExpedientesRequerimientosTitulos[0]
                  .tipoRelacionExpediente.id,
              text:
                value.requisitosExpedientesRequerimientosTitulos[0]
                  .tipoRelacionExpediente.nombre
            },
      [RequisitoControl.estadoExpediente]: value.estadoExpediente
        ? {
            value: value.estadoExpediente.id,
            text: value.estadoExpediente.nombre
          }
        : null,
      [RequisitoControl.roles]: value.rolesRequisitosExpedientes
        ? this.asignarRolesMultiSelect(value.rolesRequisitosExpedientes)
        : null,
      [RequisitoControl.modoDeRequerimientoDeDocumentacion]: value.idRefModoRequerimientoDocumentacion
        ? {
            value: value.idRefModoRequerimientoDocumentacion,
            text: value.nombreModoRequerimientoDocumentacion
          }
        : null
    });
  }

  loadFilesTypes(): void {
    this.requisitoSrv.getFilesTypes().subscribe((value) => {
      this.filesTypes = value;
      this.requisito.requisitosExpedientesFilesType.forEach((fileType) => {
        const file = this.filesTypes.find(
          (f) => f.id == +fileType.idRefFileType
        );
        file.checked = true;
      });
    });
  }

  initializeFilter(): void {
    this.configComboTipoExpedienteAdicional = new ConfigureCombobox({
      url: `${keys.API_BASE_URL}/api/v1/expedientes-alumnos/tipos-relaciones/query`,
      perPage: 10,
      data: {
        offset: '#PAGE',
        limit: '#PER_PAGE',
        where: [
          {
            field: 'filterNombre',
            value: '#SEARCH'
          }
        ]
      },
      method: 'POST',
      transformData: (data: CustomSimpleListItemModel[]) => help.getItems(data)
    });

    this.configComboEstadoExpediente = new ConfigureCombobox({
      url: `${keys.API_BASE_URL}/api/v1/expedientes-alumnos/estado-expediente/query`,
      perPage: 10,
      data: {
        offset: '#PAGE',
        limit: '#PER_PAGE',
        where: [
          {
            field: 'filterNombre',
            value: '#SEARCH'
          }
        ]
      },
      method: 'POST',
      transformData: (data: CustomSimpleListItemModel[]) => help.getItems(data)
    });

    this.configComboModoRequerimientoDocumentacion = new ConfigureCombobox({
      url: `${keys.API_BASE_URL}/api/v1/integracion/erp-academico/modos-requerimiento-documentacion`,
      perPage: 10,
      data: {
        index: '#PAGE',
        count: '#PER_PAGE',
        search: '#SEARCH'
      },
      method: 'POST',
      transformData: (data: CustomSimpleListItemModel[]) => help.getItems(data)
    });
  }

  changeObligatorio(controlOrigen: string, controlDestino: string): void {
    const fieldOrigen = this.ctrlField(controlOrigen);
    const fieldDestino = this.ctrlField(controlDestino);
    if (fieldOrigen.value) fieldDestino.setValidators(Validators.required);
    else fieldDestino.setValidators(null);
    fieldDestino.updateValueAndValidity();
  }

  ctrlField(name: string): AbstractControl {
    return this.form.get(name);
  }

  ctrlInvalid(name: string): boolean {
    const field = this.ctrlField(name);
    return field.invalid && (field.touched || field.dirty);
  }

  asignarRolesMultiSelect(
    element: RolRequisitoExpedienteDto[]
  ): ComboboxItem[] {
    const multiSelectComboBox: ComboboxItem[] = [];

    element.forEach((ra) => {
      multiSelectComboBox.push({
        id: ra.rol,
        text: ra.rolName,
        value: ra.rol
      });
    });
    return multiSelectComboBox;
  }

  getFilesTypes(): string[] {
    return this.filesTypes
      .filter((f) => f.checked)
      .map((file) => file.id.toString());
  }

  deshabilitarDocumentacionProtegida(): void {
    this.disabledFilesTipes = true;
    if (this.ctrlField(RequisitoControl.requiereDocumentacion).value)
      this.disabledFilesTipes = false;
    this.deshabilitarField(
      RequisitoControl.requiereDocumentacion,
      RequisitoControl.documentacionProtegida
    );
    this.deshabilitarField(
      RequisitoControl.requiereDocumentacion,
      RequisitoControl.modoDeRequerimientoDeDocumentacion
    );
  }

  deshabilitarRoles(controlOrigen: string, controlDestino: string): void {
    this.deshabilitarField(controlOrigen, controlDestino);
    this.changeObligatorio(controlOrigen, controlDestino);
  }

  deshabilitarField(controlOrigen: string, controlDestino: string): void {
    this.form.get(controlDestino).disable();
    if (this.ctrlField(controlOrigen).value) {
      this.form.get(controlDestino).enable();
    }
  }
}
