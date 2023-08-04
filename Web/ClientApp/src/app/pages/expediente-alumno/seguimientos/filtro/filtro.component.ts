import { Component, EventEmitter, OnInit, Output, Input } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Clause } from '@cal/criteria';
import { TranslateService } from '@ngx-translate/core';
import { FormFiltroExpedienteControls } from '@pages/expediente-alumno/expediente-models';
import {
  ComboboxItem,
  ConfigureCombobox
} from '@src/app/component-tools/combobox/models';
import { CustomSimpleListItemModel } from '@src/app/models/erp-academico';
import { keys } from '@src/keys';
import * as helpers from '@helpers/commons-helpers';

@Component({
  selector: 'app-filtro-seguimiento',
  templateUrl: './filtro.component.html',
  styleUrls: ['./filtro.component.scss']
})
export class FiltroComponent implements OnInit {
  public isCollapsed = false;
  public clauses: Clause[];
  public filterForm: FormGroup;
  public controls = FormFiltroExpedienteControls;
  public configComboTipoSeguimiento: ConfigureCombobox;
  @Input() idExpedienteAlumno: number;
  @Input() historial: Clause[];
  @Input() cacheHistorial: string;
  @Output() searchEvent = new EventEmitter();
  @Output() cleanEvent = new EventEmitter();

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
      [FormFiltroExpedienteControls.tipoSeguimiento]: null,
      [FormFiltroExpedienteControls.descripcionSeguimiento]: null,
      [FormFiltroExpedienteControls.fechaDesde]: null,
      [FormFiltroExpedienteControls.fechaHasta]: null
    });
  }

  private initializeFilter() {
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
  }

  private applyFilter(): void {
    this.clauses = [];
    Object.keys(this.filterForm.controls).forEach((key) => {
      if (!this.filterForm.controls[key].value) return;
      let labelTranslate: string;
      let nameParameter: string;
      let isSingleSelect: boolean;
      let isDate: boolean;
      switch (key) {
        case FormFiltroExpedienteControls.tipoSeguimiento:
          nameParameter = 'filterIdTipoSeguimiento';
          labelTranslate = 'listadoSeguimientos.tipo';
          isSingleSelect = true;
          break;
        case FormFiltroExpedienteControls.descripcionSeguimiento:
          nameParameter = 'filterDescripcion';
          labelTranslate = 'editarExpediente.seguimientos.descripcion';
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
        default:
          break;
      }
      this.setClauseFilter(
        nameParameter,
        labelTranslate,
        key,
        isSingleSelect,
        isDate
      );
    });

    this.searchEvent.emit(this.clauses);
  }

  private setClauseFilter(
    nameParameter: string,
    label: string,
    key: string,
    isSingleSelect?: boolean,
    isDate?: boolean
  ): void {
    const input = this.filterForm.controls[key].value;
    this.clauses.push({
      field: nameParameter,
      key: key,
      label: this.translateService.instant(`pages.${label}`),
      text: isSingleSelect
        ? input.text
        : isDate
        ? helpers.dateToString(input, 'DD/MM/YYYY')
        : input,
      value: isSingleSelect ? input.value : input
    });
  }

  private setHistoryFilters(): void {
    if (!this.historial?.length) return;

    this.clauses = this.historial;
    this.clauses.forEach((clause) => {
      switch (clause.key) {
        case FormFiltroExpedienteControls.descripcionSeguimiento:
          this.patchInputForm(clause.key, clause.value);
          break;
        case FormFiltroExpedienteControls.fechaDesde:
        case FormFiltroExpedienteControls.fechaHasta:
          this.patchInputForm(clause.key, new Date(clause.value as string));
          break;
        case FormFiltroExpedienteControls.tipoSeguimiento:
          const item = new ComboboxItem({
            value: clause.value,
            text: clause.text
          });
          this.patchInputForm(clause.key, item);
          break;
        default:
          break;
      }
    });
  }

  private patchInputForm(
    key: string,
    value: string | Date | ComboboxItem
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
  }

  public cleanFilters(): void {
    this.filterForm = this.createFilterForm();
    this.clauses = [];
    this.cleanEvent.emit();
  }

  public onCollapse(): void {
    this.isCollapsed = !this.isCollapsed;
    helpers.clearFocus();
  }
}
