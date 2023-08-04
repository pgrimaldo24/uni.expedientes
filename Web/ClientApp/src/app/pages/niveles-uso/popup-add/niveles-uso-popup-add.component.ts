import {
  FormNivelesUsoCtrls,
  TipoNivelUsoListItemDto,
  UniversidadModel,
  TipoEstudioModel,
  EstudioModel,
  PlanEstudioModel,
  AsignaturaPlanModel,
  TipoAsignaturaModel,
  Payload
} from './../niveles-uso.models';
import {
  Component,
  OnInit,
  Output,
  EventEmitter,
  ViewChild,
  ElementRef
} from '@angular/core';
import { NgbModalRef, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Guid } from 'guid-typescript';
import {
  FormGroup,
  AbstractControl,
  FormBuilder,
  Validators
} from '@angular/forms';
import {
  ConfigureCombobox,
  ComboboxItem
} from '@src/app/component-tools/combobox/models';
import { Subject } from 'rxjs';
import { keys } from '@src/keys';
import { CreateNivelUsoPayload } from '../niveles-uso.models';

@Component({
  selector: 'app-niveles-uso-popup-add',
  templateUrl: './niveles-uso-popup-add.component.html',
  styleUrls: ['./niveles-uso-popup-add.component.scss']
})
export class NivelesUsoPopupAddComponent implements OnInit {
  submitting = false;
  blockPopup = Guid.create().toString();
  unsubscribe$ = new Subject();
  form: FormGroup;
  ctrl = FormNivelesUsoCtrls;
  configTipoNivelUso: ConfigureCombobox;
  configUniversidad: ConfigureCombobox;
  configTipoEstudio: ConfigureCombobox;
  configEstudio: ConfigureCombobox;
  configPlanEstudio: ConfigureCombobox;
  configTipoAsignatura: ConfigureCombobox;
  configAsignaturaPlan: ConfigureCombobox;
  configAsignaturaOfertada: ConfigureCombobox;
  idVersionEscala: number;
  @Output() completed = new EventEmitter();
  @ViewChild('content') modalContent: ElementRef;
  constructor(
    private formBuilder: FormBuilder,
    private modalService: NgbModal
  ) {
    this.configTipoNivelUsoCombo();
  }

  ngOnInit(): void {}
  buildForm(): void {
    this.form = this.formBuilder.group({
      [FormNivelesUsoCtrls.tipoNivelUsoCombo]: null,
      [FormNivelesUsoCtrls.universidadCombo]: null,
      [FormNivelesUsoCtrls.tipoEstudioCombo]: null,
      [FormNivelesUsoCtrls.estudioCombo]: null,
      [FormNivelesUsoCtrls.planEstudioCombo]: null,
      [FormNivelesUsoCtrls.tipoAsignaturaCombo]: null,
      [FormNivelesUsoCtrls.asignaturaPlanCombo]: null,
      [FormNivelesUsoCtrls.idTipoNivelUso]: null
    });
    this.loadValueChangeListener();
  }
  configTipoNivelUsoCombo(): void {
    this.configTipoNivelUso = new ConfigureCombobox({
      url: `${keys.API_BASE_URL}/api/v1/comportamientos-expedientes/tipos-niveles-uso/query`,
      data: {
        where: [
          {
            field: 'filterNombre',
            value: '#SEARCH'
          }
        ],
        limit: '#PER_PAGE',
        offset: '#PAGE'
      },
      method: 'POST',
      transformData: (data: TipoNivelUsoListItemDto[]) =>
        data.map(
          (tipo) =>
            new ComboboxItem({
              value: tipo.id,
              text: tipo.nombre,
              data: tipo
            })
        )
    });
  }
  configComboUniversidad(): void {
    this.configUniversidad = new ConfigureCombobox({
      url: `${keys.API_BASE_URL}/api/v1/integracion/erp-academico/universidades`,
      perPage: 10,
      data: {
        search: '#SEARCH',
        count: '#PER_PAGE',
        index: '#PAGE'
      },
      method: 'POST',
      transformData: (data: UniversidadModel[]) =>
        data.map(
          (university) =>
            new ComboboxItem({
              value: university.id,
              text: university.displayName,
              data: university
            })
        )
    });
  }
  configComboTipoEstudio(): void {
    const payload: Payload = {
      search: '#SEARCH',
      count: '#PER_PAGE',
      index: '#PAGE'
    };
    if (this.getComboBoxValue(FormNivelesUsoCtrls.universidadCombo)) {
      payload.filterIdUniversidad = this.getComboBoxValue(
        FormNivelesUsoCtrls.universidadCombo
      );
    }
    this.configTipoEstudio = new ConfigureCombobox({
      url: `${keys.API_BASE_URL}/api/v1/integracion/erp-academico/tipos-estudios`,
      perPage: 10,
      data: {
        ...payload
      },
      method: 'POST',
      transformData: (data: TipoEstudioModel[]) =>
        data.map(
          (tipo) =>
            new ComboboxItem({
              value: tipo.id,
              text: tipo.displayName,
              data: tipo
            })
        )
    });
  }

  onChangeUniversidad(): void {
    this.form.patchValue({
      [FormNivelesUsoCtrls.tipoEstudioCombo]: null,
      [FormNivelesUsoCtrls.estudioCombo]: null,
      [FormNivelesUsoCtrls.planEstudioCombo]: null,
      [FormNivelesUsoCtrls.asignaturaPlanCombo]: null,
      [FormNivelesUsoCtrls.tipoAsignaturaCombo]: null
    });
    this.configComboTipoEstudio();
    this.configComboEstudio();
    this.configComboPlanEstudio();
    this.configComboAsignaturaPlan();
    this.configComboTipoAsignatua();
  }

  onChangeTipoEstudio(): void {
    this.form.patchValue({
      [FormNivelesUsoCtrls.estudioCombo]: null,
      [FormNivelesUsoCtrls.planEstudioCombo]: null,
      [FormNivelesUsoCtrls.asignaturaPlanCombo]: null,
      [FormNivelesUsoCtrls.tipoAsignaturaCombo]: null
    });
    this.configComboEstudio();
    this.configComboPlanEstudio();
    this.configComboAsignaturaPlan();
    this.configComboTipoAsignatua();
  }

  onChangeEstudio(): void {
    this.form.patchValue({
      [FormNivelesUsoCtrls.planEstudioCombo]: null,
      [FormNivelesUsoCtrls.asignaturaPlanCombo]: null,
      [FormNivelesUsoCtrls.tipoAsignaturaCombo]: null
    });
    this.configComboPlanEstudio();
    this.configComboAsignaturaPlan();
    this.configComboTipoAsignatua();
  }

  onChangePlanEstudio(): void {
    this.form.patchValue({
      [FormNivelesUsoCtrls.asignaturaPlanCombo]: null,
      [FormNivelesUsoCtrls.tipoAsignaturaCombo]: null
    });
    this.configComboAsignaturaPlan();
    this.configComboTipoAsignatua();
  }

  onChangeTipoAsignatura(): void {
    this.form.patchValue({
      [FormNivelesUsoCtrls.asignaturaPlanCombo]: null
    });
    this.configComboAsignaturaPlan();
  }

  onChangeAsignaturaPlan(): void {}

  getComboBoxValue(key: string): string {
    let value = null;
    if (this.form.value[key]) {
      value = this.form.value[key].value;
    }
    return value;
  }

  getComboBoxText(key: string): string {
    let value = null;
    if (this.form.value[key]) {
      value = this.form.value[key].text;
    }
    return value;
  }

  configComboEstudio(): void {
    const universidadId = this.getComboBoxValue(
      FormNivelesUsoCtrls.universidadCombo
    );
    const tipoEstudioId = this.getComboBoxValue(
      FormNivelesUsoCtrls.tipoEstudioCombo
    );
    this.configEstudio = new ConfigureCombobox({
      url: `${keys.API_BASE_URL}/api/v1/integracion/erp-academico/estudios`,
      perPage: 10,
      data: {
        search: '#SEARCH',
        count: '#PER_PAGE',
        index: '#PAGE',
        filterIdsUniversidades: universidadId ? [Number(universidadId)] : [],
        filterIdTipo: tipoEstudioId ? Number(tipoEstudioId) : null
      },
      method: 'POST',
      transformData: (data: EstudioModel[]) =>
        data.map(
          (estudio) =>
            new ComboboxItem({
              value: estudio.id,
              text: estudio.displayName,
              data: estudio
            })
        )
    });
  }
  configComboPlanEstudio(): void {
    const universidadId = this.getComboBoxValue(
      FormNivelesUsoCtrls.universidadCombo
    );
    const estudioId = this.getComboBoxValue(FormNivelesUsoCtrls.estudioCombo);
    const tipoEstudioId = this.getComboBoxValue(
      FormNivelesUsoCtrls.tipoEstudioCombo
    );
    this.configPlanEstudio = new ConfigureCombobox({
      url: `${keys.API_BASE_URL}/api/v1/integracion/erp-academico/planes-estudios`,
      perPage: 10,
      data: {
        search: '#SEARCH',
        count: '#PER_PAGE',
        index: '#PAGE',
        filterIdsUniversidades: universidadId ? [Number(universidadId)] : [],
        filterIdTipoEstudio: tipoEstudioId ? Number(tipoEstudioId) : null,
        filterIdsEstudios: estudioId ? [Number(estudioId)] : []
      },
      method: 'POST',
      transformData: (data: PlanEstudioModel[]) =>
        data.map(
          (plan) =>
            new ComboboxItem({
              value: plan.id,
              text: plan.displayName,
              data: plan
            })
        )
    });
  }

  configComboTipoAsignatua(): void {
    this.configTipoAsignatura = new ConfigureCombobox({
      url: `${keys.API_BASE_URL}/api/v1/integracion/erp-academico/tipos-asignaturas`,
      perPage: 10,
      data: {
        search: '#SEARCH',
        count: '#PER_PAGE',
        index: '#PAGE'
      },
      method: 'POST',
      transformData: (data: TipoAsignaturaModel[]) =>
        data.map(
          (tipoAsignatura) =>
            new ComboboxItem({
              value: tipoAsignatura.id,
              text: tipoAsignatura.nombre,
              data: tipoAsignatura
            })
        )
    });
  }

  configComboAsignaturaPlan(): void {
    const universidadId = this.getComboBoxValue(
      FormNivelesUsoCtrls.universidadCombo
    );
    const estudioId = this.getComboBoxValue(FormNivelesUsoCtrls.estudioCombo);
    const tipoId = this.getComboBoxValue(FormNivelesUsoCtrls.tipoEstudioCombo);
    const planId = this.getComboBoxValue(FormNivelesUsoCtrls.planEstudioCombo);
    const tipoAsignaruta = this.getComboBoxValue(
      FormNivelesUsoCtrls.tipoAsignaturaCombo
    );
    this.configAsignaturaPlan = new ConfigureCombobox({
      url: `${keys.API_BASE_URL}/api/v1/integracion/erp-academico/asignaturas-planes`,
      perPage: 10,
      data: {
        search: '#SEARCH',
        count: '#PER_PAGE',
        index: '#PAGE',
        filterIdUniversidad: universidadId ? Number(universidadId) : null,
        filterIdTipoEstudio: tipoId ? Number(tipoId) : null,
        filterIdEstudio: estudioId ? Number(estudioId) : null,
        filterIdsPlanes: planId ? [Number(planId)] : [],
        filterIdTipoAsignatura: tipoAsignaruta ? Number(tipoAsignaruta) : null
      },
      method: 'POST',
      transformData: (data: AsignaturaPlanModel[]) =>
        data.map(
          (asignaturaPlan) =>
            new ComboboxItem({
              value: asignaturaPlan.id,
              text: asignaturaPlan.displayNameNivelUso,
              data: asignaturaPlan
            })
        )
    });
  }

  loadValueChangeListener(): void {
    this.ctrlField(
      FormNivelesUsoCtrls.tipoNivelUsoCombo
    ).valueChanges.subscribe((selectedValue: ComboboxItem) => {
      this.resetFormCombo();
      if (!selectedValue) return;
      const tipoNivelUso = selectedValue.data as TipoNivelUsoListItemDto;
      this.ctrlField(FormNivelesUsoCtrls.idTipoNivelUso).patchValue(
        tipoNivelUso.id
      );
      if (tipoNivelUso.esUniversidad) {
        this.configComboUniversidad();
        this.updateValidationField(FormNivelesUsoCtrls.universidadCombo);
        this.ctrlField(FormNivelesUsoCtrls.universidadCombo).patchValue(null);
        this.ctrlField(
          FormNivelesUsoCtrls.universidadCombo
        ).updateValueAndValidity();
      }
      if (tipoNivelUso.esTipoEstudio) {
        this.configComboUniversidad();
        this.configComboTipoEstudio();
        this.updateValidationField(FormNivelesUsoCtrls.tipoEstudioCombo);
        this.ctrlField(FormNivelesUsoCtrls.tipoEstudioCombo).patchValue(null);
        this.ctrlField(
          FormNivelesUsoCtrls.tipoEstudioCombo
        ).updateValueAndValidity();
      }
      if (tipoNivelUso.esEstudio) {
        this.configComboUniversidad();
        this.configComboTipoEstudio();
        this.configComboEstudio();
        this.updateValidationField(FormNivelesUsoCtrls.estudioCombo);
        this.ctrlField(FormNivelesUsoCtrls.estudioCombo).patchValue(null);
        this.ctrlField(
          FormNivelesUsoCtrls.estudioCombo
        ).updateValueAndValidity();
      }
      if (tipoNivelUso.esPlanEstudio) {
        this.configComboUniversidad();
        this.configComboTipoEstudio();
        this.configComboEstudio();
        this.configComboPlanEstudio();
        this.updateValidationField(FormNivelesUsoCtrls.planEstudioCombo);
        this.ctrlField(FormNivelesUsoCtrls.planEstudioCombo).patchValue(null);
        this.ctrlField(
          FormNivelesUsoCtrls.planEstudioCombo
        ).updateValueAndValidity();
      }
      if (tipoNivelUso.esTipoAsignatura) {
        this.configComboUniversidad();
        this.configComboTipoEstudio();
        this.configComboEstudio();
        this.configComboPlanEstudio();
        this.configComboTipoAsignatua();
        this.updateValidationField(FormNivelesUsoCtrls.tipoAsignaturaCombo);
        this.ctrlField(FormNivelesUsoCtrls.tipoAsignaturaCombo).patchValue(
          null
        );
        this.ctrlField(
          FormNivelesUsoCtrls.tipoAsignaturaCombo
        ).updateValueAndValidity();
        this.form
          .get(FormNivelesUsoCtrls.universidadCombo)
          .setValidators([Validators.required]);
        this.ctrlField(FormNivelesUsoCtrls.universidadCombo).patchValue(null);
        this.ctrlField(
          FormNivelesUsoCtrls.universidadCombo
        ).updateValueAndValidity();
      }
      if (tipoNivelUso.esAsignaturaPlan) {
        this.configComboUniversidad();
        this.configComboTipoEstudio();
        this.configComboEstudio();
        this.configComboPlanEstudio();
        this.configComboTipoAsignatua();
        this.configComboAsignaturaPlan();
        this.updateValidationField(FormNivelesUsoCtrls.asignaturaPlanCombo);
        this.ctrlField(FormNivelesUsoCtrls.asignaturaPlanCombo).patchValue(
          null
        );
        this.ctrlField(
          FormNivelesUsoCtrls.asignaturaPlanCombo
        ).updateValueAndValidity();
      }
    });
  }

  private updateValidationField(field: string) {
    for (const fieldKey in this.form.controls) {
      this.form.get(fieldKey).clearValidators();
      if (field === fieldKey) {
        this.form.get(fieldKey).setValidators([Validators.required]);
      }
      this.form.get(field).updateValueAndValidity();
    }
  }

  private resetFormCombo(): void {
    this.form.patchValue({
      [FormNivelesUsoCtrls.universidadCombo]: null,
      [FormNivelesUsoCtrls.tipoEstudioCombo]: null,
      [FormNivelesUsoCtrls.estudioCombo]: null,
      [FormNivelesUsoCtrls.planEstudioCombo]: null,
      [FormNivelesUsoCtrls.tipoAsignaturaCombo]: null,
      [FormNivelesUsoCtrls.asignaturaPlanCombo]: null
    });
  }

  addNivelUso(): void {
    this.buildForm();
    this.modalService.open(this.modalContent, {
      ariaLabelledBy: 'modal-basic-title',
      size: 'xl',
      backdrop: 'static',
      centered: true
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

  guardarNivelUso(modal: NgbModalRef, evento: Event): void {
    evento.preventDefault();
    this.form.markAllAsTouched();
    if (this.form.invalid) {
      return;
    }
    const payload: CreateNivelUsoPayload = {
      ...this.form.value
    };
    this.setPropertiesIdRef(payload);
    this.completed.emit(payload);
    modal.close();
  }

  setPropertiesIdRef(payload: CreateNivelUsoPayload): void {
    const universidadId = this.getComboBoxValue(
      FormNivelesUsoCtrls.universidadCombo
    );
    const tipoEstudioId = this.getComboBoxValue(
      FormNivelesUsoCtrls.tipoEstudioCombo
    );
    const estudioId = this.getComboBoxValue(FormNivelesUsoCtrls.estudioCombo);
    const planEstudioId = this.getComboBoxValue(
      FormNivelesUsoCtrls.planEstudioCombo
    );
    const tipoAsignarutaId = this.getComboBoxValue(
      FormNivelesUsoCtrls.tipoAsignaturaCombo
    );
    const asignaturaPlanId = this.getComboBoxValue(
      FormNivelesUsoCtrls.asignaturaPlanCombo
    );
    const nombreNivelUso = this.getComboBoxText(
      FormNivelesUsoCtrls.tipoNivelUsoCombo
    );
    const acronimoUniversidad = this.getComboBoxText(
      FormNivelesUsoCtrls.universidadCombo
    );
    const nombreTipoEstudio = this.getComboBoxText(
      FormNivelesUsoCtrls.tipoEstudioCombo
    );
    const nombreEstudio = this.getComboBoxText(
      FormNivelesUsoCtrls.estudioCombo
    );
    const nombrePlan = this.getComboBoxText(
      FormNivelesUsoCtrls.planEstudioCombo
    );
    const nombreTipoAsignatura = this.getComboBoxText(
      FormNivelesUsoCtrls.tipoAsignaturaCombo
    );
    const nombreAsignatura = this.getComboBoxText(
      FormNivelesUsoCtrls.asignaturaPlanCombo
    );

    payload.idRefUniversidad = universidadId ? universidadId.toString() : null;
    payload.idRefTipoEstudio = tipoEstudioId ? tipoEstudioId.toString() : null;
    payload.idRefEstudio = estudioId ? estudioId.toString() : null;
    payload.idRefPlanEstudio = planEstudioId ? planEstudioId.toString() : null;
    payload.idRefTipoAsignatura = tipoAsignarutaId
      ? tipoAsignarutaId.toString()
      : null;
    payload.idRefAsignatura = asignaturaPlanId
      ? asignaturaPlanId.toString()
      : null;
    payload.nombreNivelUso = nombreNivelUso ? nombreNivelUso.toString() : null;
    payload.acronimoUniversidad = acronimoUniversidad
      ? acronimoUniversidad.toString()
      : null;
    payload.nombreTipoEstudio = nombreTipoEstudio
      ? nombreTipoEstudio.toString()
      : null;
    payload.nombreEstudio = nombreEstudio ? nombreEstudio.toString() : null;
    payload.nombrePlan = nombrePlan ? nombrePlan.toString() : null;
    payload.nombreTipoAsignatura = nombreTipoAsignatura
      ? nombreTipoAsignatura.toString()
      : null;
    payload.nombreAsignatura = nombreAsignatura
      ? nombreAsignatura.toString()
      : null;
    payload.esUniversidad = this.esUniversidad;
    payload.esTipoEstudio = this.esTipoEstudio;
    payload.esEstudio = this.esEstudio;
    payload.esPlanEstudio = this.esPlanEstudio;
    payload.esTipoAsignatura = this.esTipoAsignatura;
    payload.esAsignaturaPlan = this.esAsignaturaPlan;
  }

  cancelar(modal: NgbModalRef): void {
    this.form.reset();
    this.submitting = false;
    modal.close();
  }

  get selectedTipoNivelUso(): TipoNivelUsoListItemDto {
    return this.ctrlField(FormNivelesUsoCtrls.tipoNivelUsoCombo).value
      ?.data as TipoNivelUsoListItemDto;
  }

  get esUniversidad(): boolean {
    return this.selectedTipoNivelUso != null
      ? this.selectedTipoNivelUso.esUniversidad
      : false;
  }
  get esTipoEstudio(): boolean {
    return this.selectedTipoNivelUso != null
      ? this.selectedTipoNivelUso.esTipoEstudio
      : false;
  }
  get esEstudio(): boolean {
    return this.selectedTipoNivelUso != null
      ? this.selectedTipoNivelUso.esEstudio
      : false;
  }
  get esPlanEstudio(): boolean {
    return this.selectedTipoNivelUso != null
      ? this.selectedTipoNivelUso.esPlanEstudio
      : false;
  }
  get esTipoAsignatura(): boolean {
    return this.selectedTipoNivelUso != null
      ? this.selectedTipoNivelUso.esTipoAsignatura
      : false;
  }
  get esAsignaturaPlan(): boolean {
    return this.selectedTipoNivelUso != null
      ? this.selectedTipoNivelUso.esAsignaturaPlan
      : false;
  }
}
