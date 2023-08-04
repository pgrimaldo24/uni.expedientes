import { Component, ViewChild } from '@angular/core';
import { ComboboxItem } from '@tools/combobox/models';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators
} from '@angular/forms';
import { Guid } from 'guid-typescript';
import { OAuthService } from 'angular-oauth2-oidc';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { keys } from '@src/keys';
import { BlockUIService } from 'ng-block-ui';
import {
  Ambito,
  Observacion,
  ObservacionControl,
  RolAnotacion
} from './observacion-form.models';
import { IdentityClaimsModel } from '@src/app/models/security.models';
import { ComboboxComponent } from '@src/app/component-tools/combobox/combobox.component';
import { AngularEditorConfig } from '@kolkov/angular-editor';
import { ExpedienteService } from '../expediente.service';
import { AnotacionDto, RolAnotacionDto } from '../expediente-models';
import * as helpers from '@helpers/commons-helpers';
import { DateToLocalStringPipe } from '@helpers/pipes-helpers';

@Component({
  selector: 'app-observacion-form',
  templateUrl: './observacion-form.component.html',
  styleUrls: ['./observacion-form.component.scss']
})
export class ObservacionFormComponent {
  editorConfig: AngularEditorConfig = {
    editable: true,
    height: '200px',
    toolbarHiddenButtons: [['fontName'], ['fontSize']]
  };
  public ambito = 1;
  public fecha: string;
  public roles: ComboboxItem[] = [];
  public controls = ObservacionControl;
  public form: FormGroup;
  public blockIdentity = Guid.create().toString();
  public title = 'pages.observations.nuevaAnotacion';
  dateToLocalStringPipe = new DateToLocalStringPipe();
  idExpedienteAlumno: number;
  usuario = null;
  dataItems = [];
  @ViewChild('content', { static: false }) modalRef: NgbModalRef;
  @ViewChild('cbxRoles', { static: true }) comboboxRoles: ComboboxComponent;
  private onSuccess: () => void;
  private anotacion: AnotacionDto;
  constructor(
    private fb: FormBuilder,
    private modal: NgbModal,
    private expedienteServiceService: ExpedienteService,
    private blockUI: BlockUIService,
    private oauthService: OAuthService
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

  open(item: AnotacionDto, onSuccess = () => {}): void {
    this.onSuccess = onSuccess;
    this.title = 'pages.observations.nuevaAnotacion';
    this.form = this.createForm();
    this.modal.open(this.modalRef, {
      ariaLabelledBy: 'modal-basic-title',
      backdrop: 'static',
      size: 'xl',
      centered: true,
      backdropClass: 'light-blue-backdrop'
    });
    if (item.id) {
      this.title = 'pages.observations.editarAnotacion';
      this.blockUI.start(this.blockIdentity);
      this.ambito = item.esPublica
        ? Ambito.publica
        : item.esRestringida
        ? Ambito.restringida
        : Ambito.privada;
      this.expedienteServiceService
        .getAnotacionById(item.id)
        .subscribe((value) => {
          this.blockUI.stop(this.blockIdentity);
          this.idExpedienteAlumno = value.expedienteAlumno.id;
          this.anotacion = value;
          this.patchForm();
        });
    } else {
      this.fecha = helpers.dateToString(new Date());
      this.idExpedienteAlumno = item.expedienteAlumno.id;
      this.anotacion = new AnotacionDto();
    }
  }

  createForm(): FormGroup {
    return this.fb.group({
      [ObservacionControl.resumen]: new FormControl(null, [
        Validators.required
      ]),
      [ObservacionControl.mensaje]: new FormControl(null),
      [ObservacionControl.ambito]: new FormControl(true, [Validators.required]),
      [ObservacionControl.roles]: new FormControl(null)
    });
  }

  patchForm(): void {
    const values = { ...this.anotacion };
    this.fecha = this.dateToLocalStringPipe.transform(values.fecha.toString());
    this.usuario = values.nombreUsuario;
    let multiSelectComboBox = null;
    if (values.esRestringida) {
      multiSelectComboBox = this.asignarRolesMultiSelect(
        values.rolesAnotaciones
      );
      this.dataItems = multiSelectComboBox;
    }

    this.form.patchValue({
      [ObservacionControl.resumen]: values.resumen,
      [ObservacionControl.mensaje]: values.mensaje,
      [ObservacionControl.roles]: multiSelectComboBox
    });
  }

  ctrlField(name: string): AbstractControl {
    return this.form.get(name);
  }

  ctrlValid(name: string): boolean {
    const field = this.ctrlField(name);
    return field.valid && (field.touched || field.dirty);
  }

  ctrlInvalid(name: string): boolean {
    const field = this.ctrlField(name);
    return field.invalid && (field.touched || field.dirty);
  }

  dismissModal(): void {
    this.ambito = Ambito.publica;
    this.usuario = null;
    this.modal.dismissAll();
  }

  public onSubmit(): void {
    this.changeAmbito();
    this.form.markAllAsTouched();
    if (this.form.invalid) {
      return;
    }
    this.blockUI.start(this.blockIdentity);
    const payload = Observacion.create(this.form.value);
    payload.idExpedienteAlumno = this.idExpedienteAlumno;
    payload.esPublica = this.ambito == Ambito.publica;
    payload.esRestringida = this.ambito == Ambito.restringida;
    payload.rolesAnotaciones = payload.esRestringida ? this.getRoles() : null;
    payload.id = this.anotacion.id;
    if (!payload.id) {
      this.expedienteServiceService.saveAnotacion(payload).subscribe(() => {
        this.blockUI.stop(this.blockIdentity);
        this.dismissModal();
        this.onSuccess();
        this.ambito = Ambito.publica;
      });
    } else {
      this.expedienteServiceService.updateAnotacion(payload).subscribe(() => {
        this.blockUI.stop(this.blockIdentity);
        this.dismissModal();
        this.onSuccess();
        this.ambito = Ambito.publica;
      });
    }
  }

  getRoles(): RolAnotacionDto[] {
    const roles = this.form.value.roles.map((ra) => ra.value);
    return roles;
  }

  public get userName(): string | null {
    if (this.usuario != null) return this.usuario;
    const claims = this.oauthService.getIdentityClaims() as IdentityClaimsModel;
    if (!claims) {
      return null;
    }
    return claims.name;
  }

  changeAmbito(): void {
    this.form.get(ObservacionControl.roles).clearValidators();
    if (this.ambito == Ambito.restringida) {
      this.form
        .get(ObservacionControl.roles)
        .setValidators([Validators.required]);
    }
    this.form.get(ObservacionControl.roles).updateValueAndValidity();
  }

  asignarRolesMultiSelect(element: RolAnotacion[]): ComboboxItem[] {
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
}
