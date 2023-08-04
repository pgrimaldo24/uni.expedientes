import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { MultiselectProperty } from '@pages/expediente-alumno/expediente-models';
import {
  ComboboxItem,
  ConfigureCombobox
} from '@src/app/component-tools/combobox/models';
import { CustomSimpleListItemModel } from '@src/app/models/erp-academico';
import { keys } from '@src/keys';
import * as help from '@helpers/commons-helpers';
import {
  FormFiltroComportamientoControls,
  ModoSeleccion,
  TipoModoSeleccion
} from '../comportamientos.models';
import { Clause } from '@cal/criteria';

@Component({
  selector: 'app-filtro',
  templateUrl: './filtro.component.html',
  styleUrls: ['./filtro.component.scss']
})
export class FiltroComponent implements OnInit {
  clauses: Clause[];
  public isCollapsed = false;
  public filterForm: FormGroup;
  public controls = FormFiltroComportamientoControls;
  public configComboCondiciones: ConfigureCombobox;
  public multiSelectProperty = MultiselectProperty;
  public filter = {};
  public tiposModoSeleccion: TipoModoSeleccion[];
  @Output() searchEvent = new EventEmitter();

  constructor(
    private fb: FormBuilder,
    private translateService: TranslateService
  ) {
    this.tiposModoSeleccion = this.loadTiposModoSeleccion;
    this.filterForm = this.createFilterForm();
  }

  ngOnInit(): void {
    this.initializeFilter();
  }

  createFilterForm(): FormGroup {
    return this.fb.group({
      [FormFiltroComportamientoControls.comportamientoExpediente]: null,
      [FormFiltroComportamientoControls.estaVigente]: ModoSeleccion.Todos,
      [FormFiltroComportamientoControls.nivelUso]: null,
      [FormFiltroComportamientoControls.condiciones]: null
    });
  }

  initializeFilter(): void {
    this.configComboCondiciones = new ConfigureCombobox({
      url: `${keys.API_BASE_URL}/api/v1/requisitos-expedientes/query-all`,
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
  }

  get loadTiposModoSeleccion(): TipoModoSeleccion[] {
    return [
      {
        id: ModoSeleccion.Todos,
        nombre: this.translateService.instant(
          'pages.listadoComportamientos.todos'
        )
      },
      {
        id: ModoSeleccion.Si,
        nombre: this.translateService.instant('pages.listadoComportamientos.si')
      },
      {
        id: ModoSeleccion.No,
        nombre: this.translateService.instant('pages.listadoComportamientos.no')
      }
    ];
  }

  onCollapse(): void {
    this.isCollapsed = !this.isCollapsed;
    help.clearFocus();
  }

  removeFilterField(evento: Event, key: string): void {
    evento.stopPropagation();
    this.filterForm.patchValue({
      [key]:
        key === FormFiltroComportamientoControls.estaVigente
          ? ModoSeleccion.Todos
          : null
    });
    this.applyFilter();
  }

  cleanFilter(): void {
    this.filterForm = this.createFilterForm();
    this.applyFilter();
  }

  onSubmit(): void {
    this.filterForm.markAllAsTouched();
    if (this.filterForm.invalid) {
      return;
    }
    this.applyFilter();
  }

  ctrlField(name: string): AbstractControl {
    return this.filterForm.get(name);
  }

  setModoSeleccion(input: string, modo: ModoSeleccion): void {
    this.ctrlField(input).patchValue(modo);
  }

  applyFilter(): void {
    this.filter = this.filterForm.value;
    this.clauses = [];

    if (
      this.filter[FormFiltroComportamientoControls.comportamientoExpediente]
    ) {
      this.setClauseFilter(
        'nombre',
        FormFiltroComportamientoControls.comportamientoExpediente
      );
    }

    if (this.filter[FormFiltroComportamientoControls.estaVigente]) {
      this.setClauseFilter(
        FormFiltroComportamientoControls.estaVigente,
        FormFiltroComportamientoControls.estaVigente,
        true
      );
    }

    if (this.filter[FormFiltroComportamientoControls.nivelUso]) {
      this.setClauseFilter(
        FormFiltroComportamientoControls.nivelUso,
        FormFiltroComportamientoControls.nivelUso
      );
    }
    if (this.filter[FormFiltroComportamientoControls.condiciones]) {
      this.setClauseFilter(
        'idsCondiciones',
        FormFiltroComportamientoControls.condiciones,
        false,
        true
      );
    }
    this.searchEvent.emit(this.clauses);
  }

  setClauseFilter(
    nameParameter: string,
    key: string,
    isBoolean?: boolean,
    isSelectMultiple?: boolean
  ): void {
    let texto: string | Array<unknown>;
    let value: boolean | Array<unknown>;
    if (isBoolean) {
      const isYes = this.filter[key];
      texto = isYes === 1 ? 'si' : 'no';
      value = isYes === 1;
    }

    if (isSelectMultiple) {
      texto = this.multiSelectconfig(
        this.filter[key],
        this.multiSelectProperty.text,
        this.multiSelectProperty.string
      );

      value = this.multiSelectconfig(
        this.filter[key],
        this.multiSelectProperty.value,
        this.multiSelectProperty.string
      );
    }

    this.clauses.push({
      field: nameParameter,
      key: key,
      label: this.translateService.instant(
        `pages.listadoComportamientos.${key}`
      ),
      text: isBoolean
        ? this.translateService.instant(`pages.listadoComportamientos.${texto}`)
        : isSelectMultiple
        ? [...texto]
        : this.filter[key],
      value: isBoolean
        ? value
        : isSelectMultiple
        ? [...(value as Array<unknown>).map((v) => Number(v))]
        : this.filter[key]
    });
  }

  multiSelectconfig(
    filter: ComboboxItem[],
    property: string,
    typeValue?: string
  ): Array<unknown> {
    const values = [];
    filter?.forEach((element) => {
      if (typeValue === this.multiSelectProperty.string)
        values.push(String(element[property]));
      else values.push(element[property]);
    });
    return values;
  }
}
