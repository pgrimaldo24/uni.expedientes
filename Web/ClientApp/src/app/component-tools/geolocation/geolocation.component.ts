import { Component, forwardRef, Input, OnInit, ViewChild } from '@angular/core';
import {
  AbstractControl,
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  NG_VALUE_ACCESSOR,
  Validators
} from '@angular/forms';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { keys } from '@src/keys';
import { ComboboxItem, ConfigureCombobox } from '../combobox/models';
import { GeolocationService } from './geolocation.service';
import { Country, Division, GeolocationControl, LevelDivision } from './models';

@Component({
  selector: 'unir-geolocation',
  templateUrl: './geolocation.component.html',
  styleUrls: ['./geolocation.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => GeolocationComponent),
      multi: true
    }
  ]
})
export class GeolocationComponent implements OnInit {
  public form: FormGroup;
  configPais: ConfigureCombobox;
  divisions: Division[];
  private onSuccess: (result) => void;
  public controls = GeolocationControl;

  @Input() countrySelected: Country;
  @Input() pathDivisionDefault: LevelDivision[];
  @ViewChild('content', { static: false }) modalRef: NgbModalRef;

  constructor(
    private fb: FormBuilder,
    private modal: NgbModal,
    private geolocationService: GeolocationService
  ) {
    this.form = this.createForm();
    this.configPaisCombo();
  }

  ngOnInit(): void {}

  onSubmit(modal: NgbModalRef): void {
    this.form.markAllAsTouched();
    if (this.form.invalid) {
      return;
    }
    this.modal.dismissAll();
    this.onSuccess(this.getResultGeolocation(this.form.value));
  }

  getResultGeolocation(value: any): Country {
    let country = new Country();
    country = value.country.data;
    country.divisions = value.divisions
      .filter((d) => d.value != null)
      .map((value: Division) => value.value.data);
    return country;
  }

  createForm(): FormGroup {
    return this.fb.group({
      [GeolocationControl.country]: new FormControl(null, [
        Validators.required
      ]),
      [GeolocationControl.divisions]: this.fb.array([])
    });
  }

  setCountry(): ComboboxItem {
    if (!this.countrySelected) return null;

    this.form.get(GeolocationControl.country).setValue(
      new ComboboxItem({
        value: this.countrySelected.isoCode,
        text: this.countrySelected.name,
        data: this.countrySelected
      })
    );
    this.onChangeCountry(false);
  }

  open(onSuccess = (result) => {}): void {
    this.onSuccess = onSuccess;
    this.form = this.createForm();
    this.modal.open(this.modalRef, {
      ariaLabelledBy: 'modal-basic-title',
      backdrop: 'static',
      size: 'xs',
      centered: true,
      backdropClass: 'light-blue-backdrop'
    });
    if (this.countrySelected) this.setCountry();
  }

  onChangeCountry(fromView: boolean): void {
    const clearFormArray = (formArray: FormArray) => {
      while (formArray.length !== 0) {
        formArray.removeAt(0);
      }
    };
    clearFormArray(this.getDivisions);
    if (!this.selectedCountry) return;
    let lastSelected = null;
    this.geolocationService
      .getDivisionsCountry(this.selectedCountry.isoCode)
      .subscribe((response) => {
        response.forEach((element) => {
          let value = null;
          if (this.pathDivisionDefault) {
            const division = this.pathDivisionDefault.find(
              (r) => r.divisionLevel == element.level
            );
            if (division && !fromView)
              value = this.getItemLevelComboBox(division);
          }
          element.value = value;
          element.divisionConfig = this.getConfigDivision(
            element,
            null,
            lastSelected
          );
          lastSelected = value?.value ? value?.value : lastSelected;
          this.getDivisions.push(this.addDivision(element));
        });
      });
  }

  onChangeDivision(division: ComboboxItem, index: number): void {
    if (division != null) this.getNivelesSuperiores(division, index);

    let nextDivision = this.getDivisions.controls[index];
    if (!nextDivision) return;

    while (nextDivision) {
      let config = null;
      if (division == null)
        config = this.getConfigDivision(null, nextDivision, null);
      else config = this.getConfigDivision(division.data, nextDivision, null);

      nextDivision.get('value').setValue(null);
      nextDivision.get('divisionConfig').setValue(config);
      index++;
      nextDivision = this.getDivisions.controls[index];
    }
  }

  getItemLevelComboBox(level: LevelDivision): ComboboxItem {
    return new ComboboxItem({
      value: level.code,
      text: level.name,
      data: level
    });
  }

  getNivelesSuperiores(division: ComboboxItem, index: number): void {
    this.getPathDivision(division.data.code, index);
  }

  getPathDivision(code: string, level: number): void {
    this.geolocationService.getPathDivision(code).subscribe((response) => {
      this.getDivisions.controls.forEach((element: FormGroup) => {
        if (element.value.level < level) {
          const division = response.find(
            (r) => r.divisionLevel == element.value.level
          );
          element.get('value').setValue(this.getItemLevelComboBox(division));
        }
      });
    });
  }

  getConfigDivision(
    division: any,
    nextDivision: any,
    lastSelected: string
  ): ConfigureCombobox {
    return new ConfigureCombobox({
      url: `${keys.API_BASE_URL}/api/v1/integracion/commons/countries/division`,
      method: 'POST',
      perPage: 10,
      minLength: 0,
      data: {
        name: '#SEARCH',
        limit: '#PER_PAGE',
        offset: '#PAGE',
        isoCode:
          nextDivision != null
            ? nextDivision.controls.countryCode.value
            : division.countryCode,
        levelCode:
          nextDivision != null
            ? nextDivision.controls.code.value
            : division.code,
        superEntityCode: division?.divisionCode ? division.code : lastSelected
      },
      transformData: (data: LevelDivision[]) =>
        data.map(
          (level) =>
            new ComboboxItem({
              value: level.code,
              text: level.name,
              data: level
            })
        )
    });
  }

  addDivision(division: Division): FormGroup {
    return this.fb.group(division);
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

  cancelar(modal: NgbModalRef): void {
    modal.close();
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

  ctrlFieldDivisions(formGroup: FormGroup, name: string): AbstractControl {
    return formGroup.get(name);
  }

  ctrlValidDivisions(formGroup: FormGroup, name: string): boolean {
    const field = formGroup.get(name);
    return field.valid && (field.touched || field.dirty);
  }

  ctrlInvalidDivisions(formGroup: FormGroup, name: string): boolean {
    const field = formGroup.get(name);
    return field.invalid && (field.touched || field.dirty);
  }

  dismissModal(): void {
    this.modal.dismissAll();
  }

  get selectedCountry(): Country {
    return this.ctrlField(GeolocationControl.country).value?.data as Country;
  }

  get getDivisions(): FormArray {
    return this.ctrlField(GeolocationControl.divisions) as FormArray;
  }
}
