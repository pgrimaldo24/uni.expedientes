import {
  Component,
  OnInit,
  Output,
  EventEmitter,
  ViewChild
} from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import {
  ComboboxItem,
  ConfigureCombobox
} from '@src/app/component-tools/combobox/models';
import { CustomSimpleListItemModel } from '@src/app/models/erp-academico';
import { keys } from '@src/keys';
import * as help from '@helpers/commons-helpers';
import {
  FormFiltroRequisitoControls,
  ModoSeleccion,
  TipoModoSeleccion
} from '../requesitos.models';
import { MultiselectProperty } from '@pages/expediente-alumno/expediente-models';
import { ComboboxComponent } from '@src/app/component-tools/combobox/combobox.component';
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
  public controls = FormFiltroRequisitoControls;
  public configComboEstadoExpediente: ConfigureCombobox;
  public configComboModoRequerimientoDocumentacion: ConfigureCombobox;
  public multiSelectProperty = MultiselectProperty;
  public filter = {};
  public tiposModoSeleccion: TipoModoSeleccion[];
  @Output() searchEvent = new EventEmitter();
  @ViewChild('cbxEstadoExpediente', { static: true })
  comboboxEstadoExpediente: ComboboxComponent;
  @ViewChild('cbxModoRequerimientoDocumentacion', { static: true })
  comboboxModoRequerimientoDocumentacion: ComboboxComponent;
  inputsSeleccionTodos = [
    FormFiltroRequisitoControls.estaVigente.toString(),
    FormFiltroRequisitoControls.requeridoTitulo.toString(),
    FormFiltroRequisitoControls.requiereMatricularse.toString(),
    FormFiltroRequisitoControls.requeridoPagoTasas.toString(),
    FormFiltroRequisitoControls.requiereDocumentacion.toString(),
    FormFiltroRequisitoControls.documentacionProtegida.toString()
  ];

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
      [FormFiltroRequisitoControls.condicionExpediente]: null,
      [FormFiltroRequisitoControls.estaVigente]: ModoSeleccion.Todos,
      [FormFiltroRequisitoControls.requeridoTitulo]: ModoSeleccion.Todos,
      [FormFiltroRequisitoControls.requiereMatricularse]: ModoSeleccion.Todos,
      [FormFiltroRequisitoControls.requeridoPagoTasas]: ModoSeleccion.Todos,
      [FormFiltroRequisitoControls.estadoExpediente]: null,
      [FormFiltroRequisitoControls.requiereDocumentacion]: ModoSeleccion.Todos,
      [FormFiltroRequisitoControls.documentacionProtegida]: ModoSeleccion.Todos,
      [FormFiltroRequisitoControls.modoRequerimientoDocumentacion]: null
    });
  }

  initializeFilter(): void {
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

  get loadTiposModoSeleccion(): TipoModoSeleccion[] {
    return [
      {
        id: ModoSeleccion.Todos,
        nombre: this.translateService.instant('pages.listadoRequisitos.todos')
      },
      {
        id: ModoSeleccion.Si,
        nombre: this.translateService.instant('pages.listadoRequisitos.si')
      },
      {
        id: ModoSeleccion.No,
        nombre: this.translateService.instant('pages.listadoRequisitos.no')
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
      [key]: this.inputsSeleccionTodos.includes(key)
        ? ModoSeleccion.Todos
        : null
    });
    this.applyFilter();
  }

  cleanFilters(): void {
    this.filterForm = this.createFilterForm();
    this.comboboxEstadoExpediente.dataItems = [];
    this.comboboxModoRequerimientoDocumentacion.dataItems = [];
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

    if (this.filter[FormFiltroRequisitoControls.condicionExpediente]) {
      this.setClauseFilter(
        'nombre',
        FormFiltroRequisitoControls.condicionExpediente
      );
    }

    if (this.filter[FormFiltroRequisitoControls.estaVigente]) {
      this.setClauseFilter(
        FormFiltroRequisitoControls.estaVigente,
        FormFiltroRequisitoControls.estaVigente,
        true
      );
    }

    if (this.filter[FormFiltroRequisitoControls.requeridoTitulo]) {
      this.setClauseFilter(
        FormFiltroRequisitoControls.requeridoTitulo,
        FormFiltroRequisitoControls.requeridoTitulo,
        true
      );
    }

    if (this.filter[FormFiltroRequisitoControls.requiereMatricularse]) {
      this.setClauseFilter(
        FormFiltroRequisitoControls.requiereMatricularse,
        FormFiltroRequisitoControls.requiereMatricularse,
        true
      );
    }

    if (this.filter[FormFiltroRequisitoControls.requeridoPagoTasas]) {
      this.setClauseFilter(
        'requeridoParaPago',
        FormFiltroRequisitoControls.requeridoPagoTasas,
        true
      );
    }

    if (this.filter[FormFiltroRequisitoControls.estadoExpediente]) {
      this.setClauseFilter(
        'idsEstadosExpedientes',
        FormFiltroRequisitoControls.estadoExpediente,
        false,
        true
      );
    }

    if (this.filter[FormFiltroRequisitoControls.requiereDocumentacion]) {
      this.setClauseFilter(
        FormFiltroRequisitoControls.requiereDocumentacion,
        FormFiltroRequisitoControls.requiereDocumentacion,
        true
      );
    }

    if (this.filter[FormFiltroRequisitoControls.documentacionProtegida]) {
      this.setClauseFilter(
        FormFiltroRequisitoControls.documentacionProtegida,
        FormFiltroRequisitoControls.documentacionProtegida,
        true
      );
    }

    if (
      this.filter[FormFiltroRequisitoControls.modoRequerimientoDocumentacion]
    ) {
      this.setClauseFilter(
        'idsModosRequerimientoDocumentacion',
        FormFiltroRequisitoControls.modoRequerimientoDocumentacion,
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
    let value: Array<unknown> | boolean;
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
      label: this.translateService.instant(`pages.listadoRequisitos.${key}`),
      text: isBoolean
        ? this.translateService.instant(`pages.listadoRequisitos.${texto}`)
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
