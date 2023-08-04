import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild
} from '@angular/core';
import { Clause } from '@cal/criteria';
import { TranslateService } from '@ngx-translate/core';
import {
  FormFiltroExpedienteControls,
  MultiselectProperty
} from '@pages/expediente-alumno/expediente-models';
import {
  ComboboxItem,
  ConfigureCombobox
} from '@src/app/component-tools/combobox/models';
import { keys } from '@src/keys';
import * as helpers from '@helpers/commons-helpers';
import { FormBuilder, FormGroup } from '@angular/forms';
import { CustomSimpleListItemModel } from '@src/app/models/erp-academico';
import { ComboboxComponent } from '@src/app/component-tools/combobox/combobox.component';
import { TrabajadorAcademicoResponse } from '@pages/seguimientos/seguimientos-models';

@Component({
  selector: 'app-filtro',
  templateUrl: './filtro.component.html',
  styleUrls: ['./filtro.component.scss']
})
export class FiltroComponent implements OnInit {
  private params = {
    search: '#SEARCH',
    count: '#PER_PAGE',
    index: '#PAGE'
  };
  public isCollapsed = false;
  public clauses: Clause[];
  public filterForm: FormGroup;
  public controls = FormFiltroExpedienteControls;
  public configComboUniversidad: ConfigureCombobox;
  public configComboPlanEstudio: ConfigureCombobox;
  public configComboTipoSeguimiento: ConfigureCombobox;
  public configComboUsuario: ConfigureCombobox;
  public multiSelectProperty = MultiselectProperty;
  @Input() historial: Clause[];
  @Input() cacheHistorial: string;
  @Input() isSeguimiento: boolean;
  @Output() searchEvent = new EventEmitter();
  @Output() cleanEvent = new EventEmitter();
  @ViewChild('comboPlanEstudio', { static: true })
  comboPlanEstudio: ComboboxComponent;
  constructor(
    private fb: FormBuilder,
    private translateService: TranslateService
  ) {
    this.filterForm = this.createFilterForm();
  }

  ngOnInit(): void {
    this.initializeFilter();
    this.setHistoryFilters();
  }

  private createFilterForm(): FormGroup {
    return this.fb.group({
      [FormFiltroExpedienteControls.idExpediente]: null,
      [FormFiltroExpedienteControls.universidad]: null,
      [FormFiltroExpedienteControls.planesEstudio]: null,
      [FormFiltroExpedienteControls.idRefPlan]: null,
      [FormFiltroExpedienteControls.nombreAlumno]: null,
      [FormFiltroExpedienteControls.primerApellido]: null,
      [FormFiltroExpedienteControls.segundoApellido]: null,
      [FormFiltroExpedienteControls.nroDocIdentificacion]: null,
      [FormFiltroExpedienteControls.idRefIntegracion]: null,
      [FormFiltroExpedienteControls.tipoSeguimiento]: null,
      [FormFiltroExpedienteControls.fechaDesde]: null,
      [FormFiltroExpedienteControls.fechaHasta]: null,
      [FormFiltroExpedienteControls.usuario]: null
    });
  }

  private initializeFilter() {
    this.configComboUniversidad = new ConfigureCombobox({
      url: `${keys.API_BASE_URL}/api/v1/integracion/erp-academico/universidades`,
      perPage: 10,
      data: { ...this.params },
      method: 'POST',
      transformData: (data: CustomSimpleListItemModel[]) =>
        helpers.getItems(data)
    });

    this.configComboPlanEstudio = new ConfigureCombobox({
      url: `${keys.API_BASE_URL}/api/v1/integracion/erp-academico/planes-estudios/combo-list`,
      perPage: 10,
      data: {
        filterDisplayName: '#SEARCH',
        index: '#PAGE'
      },
      method: 'POST',
      transformData: (data: CustomSimpleListItemModel[]) =>
        helpers.getItems(data, 'displayName')
    });

    if (!this.isSeguimiento) return;
    this.configComboTipoSeguimiento = new ConfigureCombobox({
      url: `${keys.API_BASE_URL}/api/v1/tipos-seguimientos-expedientes/query`,
      perPage: 10,
      data: {
        offset: '#PAGE',
        limit: '#PER_PAGE',
        where: [
          {
            field: 'nombre',
            value: '#SEARCH'
          }
        ]
      },
      method: 'POST',
      transformData: (data: CustomSimpleListItemModel[]) =>
        helpers.getItems(data)
    });

    this.configComboUsuario = new ConfigureCombobox({
      url: `${keys.API_BASE_URL}/api/v1/integracion/erp-academico/trabajadores`,
      perPage: 10,
      data: { ...this.params },
      method: 'POST',
      transformData: (data: TrabajadorAcademicoResponse[]) => {
        return data.map(
          (value) =>
            new ComboboxItem({
              value: value.persona.idSeguridad,
              text: value['displayName'],
              data: value
            })
        );
      }
    });
  }

  private applyFilter(): void {
    this.clauses = [];
    Object.keys(this.filterForm.controls).forEach((key) => {
      if (!this.filterForm.controls[key].value) return;
      let labelTranslate = `listadoExpediente.${key}`;
      let nameParameter: string;
      let isSingleSelect: boolean;
      let isMultipleSelect: boolean;
      let isDate: boolean;
      let values: string[];
      switch (key) {
        case FormFiltroExpedienteControls.idExpediente:
          nameParameter = 'filterIdExpedienteAlumno';
          labelTranslate = 'listadoExpediente.expediente';
          break;
        case FormFiltroExpedienteControls.universidad:
          nameParameter = 'filterIdRefUniversidad';
          isSingleSelect = true;
          break;
        case FormFiltroExpedienteControls.planesEstudio:
          nameParameter = 'filtersIdsRefPlan';
          labelTranslate = 'listadoExpediente.planesDeEstudio';
          isMultipleSelect = true;
          values = this.filterForm.controls[key].value.map((i) =>
            String(i.value)
          );
          break;
        case FormFiltroExpedienteControls.idRefPlan:
          nameParameter = 'filterIdRefPlan';
          break;
        case FormFiltroExpedienteControls.tipoSeguimiento:
          nameParameter = 'filterIdTipoSeguimiento';
          labelTranslate = 'listadoSeguimientos.tipo';
          isSingleSelect = true;
          break;
        case FormFiltroExpedienteControls.fechaDesde:
          nameParameter = 'filterFechaDesde';
          labelTranslate = 'listadoSeguimientos.desde';
          isDate = true;
          break;
        case FormFiltroExpedienteControls.fechaHasta:
          nameParameter = 'filterFechaHasta';
          labelTranslate = 'listadoSeguimientos.hasta';
          isDate = true;
          break;
        case FormFiltroExpedienteControls.nombreAlumno:
          nameParameter = 'filterNombreAlumno';
          break;
        case FormFiltroExpedienteControls.primerApellido:
          nameParameter = 'filterPrimerApellido';
          break;
        case FormFiltroExpedienteControls.segundoApellido:
          nameParameter = 'filterSegundoApellido';
          break;
        case FormFiltroExpedienteControls.nroDocIdentificacion:
          nameParameter = 'filterNroDocIdentificacion';
          break;
        case FormFiltroExpedienteControls.idRefIntegracion:
          nameParameter = 'filterIdRefIntegracionAlumno';
          break;
        case FormFiltroExpedienteControls.usuario:
          nameParameter = 'filterIdCuentaSeguridad';
          labelTranslate = 'listadoSeguimientos.usuario';
          isSingleSelect = true;
          break;
        default:
          break;
      }
      this.setClauseFilter(
        nameParameter,
        labelTranslate,
        key,
        isSingleSelect,
        isMultipleSelect,
        isDate,
        values
      );
    });

    this.searchEvent.emit(this.clauses);
  }

  private setClauseFilter(
    nameParameter: string,
    label: string,
    key: string,
    isSingleSelect?: boolean,
    isMultipleSelect?: boolean,
    isDate?: boolean,
    values?: string[]
  ): void {
    const input = this.filterForm.controls[key].value;
    this.clauses.push({
      field: nameParameter,
      key: key,
      label: this.translateService.instant(`pages.${label}`),
      text: isSingleSelect
        ? input.text
        : isMultipleSelect
        ? input.map((i) => i.text)
        : isDate
        ? helpers.dateToString(input, 'DD/MM/YYYY')
        : input,
      value: isSingleSelect ? input.value : isMultipleSelect ? values : input
    });
  }

  private setHistoryFilters(): void {
    if (!this.historial?.length) return;

    this.clauses = this.historial;
    this.clauses.forEach((clause) => {
      switch (clause.key) {
        case FormFiltroExpedienteControls.idExpediente:
        case FormFiltroExpedienteControls.idRefPlan:
        case FormFiltroExpedienteControls.nombreAlumno:
        case FormFiltroExpedienteControls.primerApellido:
        case FormFiltroExpedienteControls.segundoApellido:
        case FormFiltroExpedienteControls.nroDocIdentificacion:
        case FormFiltroExpedienteControls.idRefIntegracion:
          this.patchInputForm(clause.key, clause.value);
          break;
        case FormFiltroExpedienteControls.fechaDesde:
        case FormFiltroExpedienteControls.fechaHasta:
          this.patchInputForm(clause.key, new Date(clause.value as string));
          break;
        case FormFiltroExpedienteControls.universidad:
        case FormFiltroExpedienteControls.tipoSeguimiento:
        case FormFiltroExpedienteControls.usuario:
          const item = new ComboboxItem({
            value: clause.value,
            text: clause.text
          });
          this.patchInputForm(clause.key, item);
          break;
        case FormFiltroExpedienteControls.planesEstudio:
          const items: ComboboxItem[] = [];
          clause.value.forEach((value: string, index: number) => {
            items.push({
              text: clause.text[index],
              value: String(value)
            });
          });
          this.comboPlanEstudio.dataItems = items;
          this.patchInputForm(clause.key, items);
          break;
        default:
          break;
      }
    });
  }

  private patchInputForm(
    key: string,
    value: string | Date | ComboboxItem | ComboboxItem[]
  ): void {
    this.filterForm.patchValue({
      [key]: value
    });
  }

  public onSubmit(): void {
    this.filterForm.markAllAsTouched();
    if (this.filterForm.invalid) return;

    this.applyFilter();
  }

  public removeFilterField(key: string): void {
    this.filterForm.patchValue({
      [key]: null
    });
    this.clauses = this.clauses.filter((c) => c.key != key);
    this.historial = this.clauses;
    if (key == FormFiltroExpedienteControls.planesEstudio)
      this.comboPlanEstudio.dataItems = [];
  }

  public cleanFilters(): void {
    this.filterForm = this.createFilterForm();
    this.clauses = [];
    this.comboPlanEstudio.dataItems = [];
    this.cleanEvent.emit();
  }

  public onCollapse(): void {
    this.isCollapsed = !this.isCollapsed;
    helpers.clearFocus();
  }
}
