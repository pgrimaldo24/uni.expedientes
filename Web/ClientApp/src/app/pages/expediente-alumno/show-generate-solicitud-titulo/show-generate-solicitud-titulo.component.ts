import {
  Component,
  ElementRef,
  EventEmitter,
  OnInit,
  Output,
  ViewChild
} from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  Validators
} from '@angular/forms';
import { Clause } from '@cal/criteria';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfigureCombobox } from '@src/app/component-tools/combobox/models';
import { keys } from '@src/keys';
import { Guid } from 'guid-typescript';
import { BlockUIService } from 'ng-block-ui';
import { Data } from '../expediente-models';
import { ExpedienteService } from '../expediente.service';
import * as helpers from '@helpers/commons-helpers';
import {
  GenerateSolicitudTitulo,
  GenerateSolicitudTituloControl
} from './show-generate-solicitud-titulo.models';
import { CustomSimpleListItemModel } from '@src/app/models/erp-academico';

@Component({
  selector: 'app-show-generate-solicitud-titulo',
  templateUrl: './show-generate-solicitud-titulo.component.html',
  styleUrls: ['./show-generate-solicitud-titulo.component.scss']
})
export class ShowGenerateSolicitudTituloComponent implements OnInit {
  @ViewChild('content') modalContent: ElementRef;
  public roles = keys;
  public blockIdentity = Guid.create().toString();
  public controls = GenerateSolicitudTituloControl;
  public form: FormGroup;
  private onSuccess: () => void;
  public configComboTiposSolicitudes: ConfigureCombobox;
  private idsExpedientesSeleccionados: number[];
  private checkAll = false;
  private idUniversidad: number;
  private filtros: Clause[];
  @Output() loadingGenerar = new EventEmitter<boolean>();
  constructor(
    private modalService: NgbModal,
    private fb: FormBuilder,
    private blockUIService: BlockUIService,
    private expedienteService: ExpedienteService
  ) {}

  ngOnInit(): void {
    this.configurationTiposSolicitud();
    this.form = this.createForm();
  }

  private configurationTiposSolicitud(): void {
    this.configComboTiposSolicitudes = new ConfigureCombobox({
      url: `${keys.API_BASE_URL}/api/v1/integracion/expedicion-titulos/tipos-solicitudes`,
      data: {
        idsRefUniversidad: [String(this.idUniversidad)],
        filterDisplayName: '#SEARCH'
      },
      method: 'POST',
      transformData: (data: CustomSimpleListItemModel[]) =>
        helpers.getItems(data, 'displayName')
    });
  }

  public onSubmit(): void {
    this.form.markAllAsTouched();
    if (this.form.invalid) {
      return;
    }
    this.blockUIService.start(this.blockIdentity);
    const payload = this.obtenerDatosFormulario();
    this.loadingGenerar.emit(true);
    this.redireccionarServicio(payload);
  }

  private obtenerDatosFormulario(): GenerateSolicitudTitulo {
    const payload: GenerateSolicitudTitulo = {
      TipoSolicitud: this.form.value[
        GenerateSolicitudTituloControl.tipoSolicitud
      ].data?.id,
      FechaSolicitud: this.form.value[
        GenerateSolicitudTituloControl.fechaSolicitud
      ],
      FechaPago: this.form.value[GenerateSolicitudTituloControl.fechaPago],
      IdsExpedientes: []
    };
    return payload;
  }

  private redireccionarServicio(payload: GenerateSolicitudTitulo): void {
    if (this.checkAll) {
      this.generateSolicitudMasivo(payload);
    } else {
      this.generateSolicitudIndividual(payload);
    }
  }

  private generateSolicitudIndividual(payload: GenerateSolicitudTitulo): void {
    payload.IdsExpedientes = this.idsExpedientesSeleccionados;
    this.expedienteService
      .generateSolicitudTituloCertificado(payload)
      .subscribe({
        next: () => {
          this.blockUIService.stop(this.blockIdentity);
          this.onSuccessOperation();
        },
        complete: () => this.loadingGenerar.emit(false),
        error: () => this.loadingGenerar.emit(false)
      });
  }

  private generateSolicitudMasivo(payload: GenerateSolicitudTitulo): void {
    payload.where = this.filtros;
    this.expedienteService
      .generateSolicitudTituloCertificadoMasivo(payload)
      .subscribe({
        next: () => {
          this.blockUIService.stop(this.blockIdentity);
          this.onSuccessOperation();
        },
        complete: () => this.loadingGenerar.emit(false),
        error: () => this.loadingGenerar.emit(false)
      });
  }

  private onSuccessOperation(): void {
    this.blockUIService.stop(this.blockIdentity);
    this.modalService.dismissAll();
    if (this.onSuccess) {
      this.onSuccess();
    }
  }

  private createForm(): FormGroup {
    return this.fb.group({
      [GenerateSolicitudTituloControl.tipoSolicitud]: [
        null,
        [Validators.required]
      ],
      [GenerateSolicitudTituloControl.fechaSolicitud]: [
        new Date(),
        [Validators.required]
      ],
      [GenerateSolicitudTituloControl.fechaPago]: null
    });
  }

  public ctrlField(name: string): AbstractControl {
    return this.form.get(name);
  }

  public ctrlValid(name: string): boolean {
    const field = this.ctrlField(name);
    return field.valid && (field.touched || field.dirty);
  }

  public ctrlInvalid(name: string): boolean {
    const field = this.ctrlField(name);
    return field.invalid && (field.touched || field.dirty);
  }

  public open(
    checkAll: boolean,
    expedientes: Data[],
    idUniversidad: number,
    filtros: Clause[],
    onSuccess?: () => void
  ): void {
    this.checkAll = checkAll;
    this.idsExpedientesSeleccionados = expedientes.map((x) => x.id);
    this.idUniversidad = idUniversidad;
    this.filtros = filtros;
    this.ngOnInit();
    this.onSuccess = onSuccess;
    this.modalService.open(this.modalContent, {
      ariaLabelledBy: 'modal-basic-title',
      backdrop: 'static',
      size: 'md',
      centered: true
    });
  }

  public onChangeTipoSolicitud(): void {
    const tipoSeleccionado = this.form.value[
      GenerateSolicitudTituloControl.tipoSolicitud
    ].data;
    this.form.patchValue({
      [GenerateSolicitudTituloControl.fechaPago]: tipoSeleccionado.conFechaPago
        ? new Date()
        : null
    });
  }
}
