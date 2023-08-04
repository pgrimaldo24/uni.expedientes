import {
  Component,
  OnInit,
  Output,
  EventEmitter,
  ViewChild,
  ElementRef,
  Input
} from '@angular/core';
import {
  FormGroup,
  AbstractControl,
  FormBuilder,
  Validators
} from '@angular/forms';
import { NgbModalRef, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Guid } from 'guid-typescript';

import { Subject } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
import {
  ComboboxItem,
  ConfigureCombobox
} from '@src/app/component-tools/combobox/models';
import {
  Country,
  EducationalInstitutions,
  FormInstitucionDocente
} from '@pages/expediente-alumno/expediente-models';
import { keys } from '@src/keys';

@Component({
  selector: 'app-institucion-docente-popup-add',
  templateUrl: './institucion-docente-popup-add.component.html',
  styleUrls: ['./institucion-docente-popup-add.component.scss']
})
export class InstitucionDocenteAddComponent implements OnInit {
  title: string;
  submitting = false;
  configPais: ConfigureCombobox;
  confCvInstituacionDocente: ConfigureCombobox;
  ctrl = FormInstitucionDocente;
  form: FormGroup;
  educationInstitutionselected: EducationalInstitutions;
  blockPopup = Guid.create().toString();
  unsubscribe$ = new Subject();
  @Input() educationInstitution: EducationalInstitutions;
  @Output() completed = new EventEmitter();
  @ViewChild('content') modalContent: ElementRef;
  @Output()
  institucionDocenteSeleccionado = new EventEmitter<EducationalInstitutions>();
  constructor(
    private formBuilder: FormBuilder,
    private modalService: NgbModal,
    private translate: TranslateService
  ) {}

  ngOnInit(): void {}

  buildForm(): void {
    this.form = this.formBuilder.group({
      [FormInstitucionDocente.paisInstitucionDocente]: [
        '',
        [Validators.required]
      ],
      [FormInstitucionDocente.institucionDocente]: ['', [Validators.required]],
      [FormInstitucionDocente.otro]: [''],
      [FormInstitucionDocente.detalleOtro]: ['', [Validators.required]]
    });
  }
  ctrlField(name: string): AbstractControl {
    return this.form.get(name);
  }
  get selectedCountry(): Country {
    return this.ctrlField(FormInstitucionDocente.paisInstitucionDocente).value
      ?.data as Country;
  }

  get tienePaisSeleccionado(): boolean {
    return this.selectedCountry != null;
  }

  get otrosChecked(): boolean {
    return this.ctrlField(FormInstitucionDocente.otro).value as boolean;
  }

  get selectedInstitucionDocente(): EducationalInstitutions {
    return this.ctrlField(FormInstitucionDocente.institucionDocente).value
      ?.data as EducationalInstitutions;
  }

  deleteValoresCombo(event: Event): void {
    const checked: boolean = (event.target as HTMLInputElement).checked;
    if (!checked) {
      this.putValidators(checked);
      this.form.patchValue({
        [FormInstitucionDocente.institucionDocente]: null
      });
      this.setCountry();
      if (
        !this.educationInstitution.countryCode ||
        this.educationInstitution.countryCode == null
      )
        this.form.patchValue({
          [FormInstitucionDocente.paisInstitucionDocente]: null
        });
    } else {
      this.putValidators(checked);
      this.form.patchValue({
        [FormInstitucionDocente.detalleOtro]: null
      });
    }
  }

  putValidators(isOtro: boolean): void {
    if (!isOtro) {
      this.form.get(FormInstitucionDocente.detalleOtro).clearValidators();
      this.form
        .get(FormInstitucionDocente.detalleOtro)
        .updateValueAndValidity();
      this.form
        .get(FormInstitucionDocente.institucionDocente)
        .setValidators(Validators.required);
      this.form
        .get(FormInstitucionDocente.institucionDocente)
        .updateValueAndValidity();
      this.form
        .get(FormInstitucionDocente.paisInstitucionDocente)
        .setValidators(Validators.required);
      this.form
        .get(FormInstitucionDocente.paisInstitucionDocente)
        .updateValueAndValidity();
    } else {
      this.form
        .get(FormInstitucionDocente.institucionDocente)
        .clearValidators();
      this.form
        .get(FormInstitucionDocente.paisInstitucionDocente)
        .clearValidators();
      this.form
        .get(FormInstitucionDocente.institucionDocente)
        .updateValueAndValidity();
      this.form
        .get(FormInstitucionDocente.paisInstitucionDocente)
        .updateValueAndValidity();
      this.form
        .get(FormInstitucionDocente.detalleOtro)
        .setValidators(Validators.required);
      this.form
        .get(FormInstitucionDocente.detalleOtro)
        .updateValueAndValidity();
    }
  }

  callParent(modal: NgbModalRef): void {
    this.form.markAllAsTouched();
    if (this.form.invalid) {
      return;
    }
    this.educationInstitutionselected = new EducationalInstitutions();
    if (this.otrosChecked) {
      this.educationInstitutionselected.code = '-1';
      this.educationInstitutionselected.name = this.ctrlField(
        FormInstitucionDocente.detalleOtro
      ).value as string;
    } else {
      this.educationInstitutionselected = this.selectedInstitucionDocente;
    }
    modal.close();
    this.institucionDocenteSeleccionado.next(this.educationInstitutionselected);
  }

  onChangeCountry(): void {
    if (this.tienePaisSeleccionado) {
      this.form.patchValue({
        [FormInstitucionDocente.institucionDocente]: null
      });
      this.configInstitucionDocente();
    } else {
      this.confCvInstituacionDocente = null;
    }
  }
  ctrlInvalid(name: string): boolean {
    const field = this.ctrlField(name);
    return field.invalid && (field.touched || field.dirty);
  }
  ctrlValid(name: string): boolean {
    const field = this.ctrlField(name);
    return field.valid && (field.touched || field.dirty);
  }
  ctrlTouched(name: string): boolean {
    const field = this.ctrlField(name);
    return field.touched || field.dirty;
  }
  configInstitucionDocente(): void {
    this.confCvInstituacionDocente = new ConfigureCombobox({
      url: `${keys.API_BASE_URL}/api/v1/integracion/commons/educational-institutions`,
      method: 'POST',
      perPage: 10,
      minLength: 0,
      data: {
        nombre: '#SEARCH',
        limit: '#PER_PAGE',
        indexPage: '#PAGE',
        codeCountry: this.selectedCountry?.isoCode
      },
      transformData: (data: EducationalInstitutions[]) =>
        this.getItemsInstituacionDocente(data)
    });
  }

  configPaisCombo(): void {
    this.configPais = new ConfigureCombobox({
      url: `${keys.API_BASE_URL}/api/v1/integracion/commons/countries`,
      method: 'POST',
      perPage: 10,
      minLength: 0,
      data: {
        nombre: '#SEARCH',
        limit: '#PER_PAGE',
        offset: '#PAGE'
      },
      transformData: (data: Country[]) =>
        data.map(
          (country) =>
            new ComboboxItem({
              value: country.isoCode,
              text: country.name,
              data: country
            })
        )
    });
  }

  getItemsInstituacionDocente(data: EducationalInstitutions[]): ComboboxItem[] {
    return data.map(
      (value) =>
        new ComboboxItem({
          value: value.code,
          text: value.name,
          data: value
        })
    );
  }

  cancelar(modal: NgbModalRef): void {
    this.submitting = false;
    modal.close();
  }

  selectInstitucionDocente(): void {
    this.title = this.translate.instant(
      'pages.editarExpediente.seleccionarInstitucionDocente'
    );
    this.configPais = null;
    this.confCvInstituacionDocente = null;
    this.buildForm();
    this.configPaisCombo();
    this.modalService.open(this.modalContent, {
      ariaLabelledBy: 'modal-basic-title',
      backdrop: 'static',
      centered: true
    });

    if (this.educationInstitution) {
      if (
        this.educationInstitution.code &&
        this.educationInstitution.code != '-1' &&
        this.educationInstitution.code != null
      ) {
        this.putValidators(false);
        this.setCountry();
      } else {
        this.putValidators(true);
        this.form.get(FormInstitucionDocente.otro).setValue(true);
        this.form
          .get(FormInstitucionDocente.detalleOtro)
          .setValue(this.educationInstitution.name);
      }
    } else {
      this.putValidators(false);
      this.setCountry();
    }
  }

  setCountry(): void {
    if (this.educationInstitution) {
      const country = new Country();
      country.name = this.educationInstitution.countryName;
      country.isoCode = this.educationInstitution.countryCode;
      this.form.get(FormInstitucionDocente.paisInstitucionDocente).setValue(
        new ComboboxItem({
          value: country.isoCode,
          text: country.name,
          data: country
        })
      );

      if (
        this.educationInstitution.code &&
        this.educationInstitution.code != '-1'
      )
        this.form.get(FormInstitucionDocente.institucionDocente).setValue(
          new ComboboxItem({
            value: this.educationInstitution.code,
            text: this.educationInstitution.name,
            data: this.educationInstitution
          })
        );
    }

    this.configInstitucionDocente();
  }
}
