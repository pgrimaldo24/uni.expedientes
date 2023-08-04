import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import * as helpers from '@helpers/commons-helpers';
import {
  ComboboxItem,
  ConfigureCombobox,
  Guid
} from '@src/app/component-tools/combobox/models';
import { CustomSimpleListItemModel } from '@src/app/models/erp-academico';
import { keys } from '@src/keys';
import { BlockUIService } from 'ng-block-ui';
import { ExpedienteService } from '../expediente.service';
import * as help from '@helpers/commons-helpers';
import {
  TreeItem,
  UnirTreeViewItem
} from '@src/app/component-tools/unir-tree-select/models/unir-tree-view-item';
import { ConfigureComboboxTree } from '@src/app/component-tools/unir-tree-select/models';
import {
  DatosErpResponse,
  EditExpedienteAlumno,
  EducationalInstitutions,
  TitulacionAccesoDto,
  VersionesPlane,
  FormTitulacionAcceso,
  ExpedienteAlumnoDto
} from '../expediente-models';
import { AlertHandlerService } from '@src/app/component-tools/alert/alert-handler.service';
import { TranslateService } from '@ngx-translate/core';
import { ConfirmationModalComponent } from '@tools/confirmation-modal/confirmation-modal/confirmation-modal.component';
import { InstitucionDocenteAddComponent } from './popup-add/institucion-docente-popup-add.component';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  Validators
} from '@angular/forms';
import { GeolocationComponent } from '@src/app/component-tools/geolocation/geolocation.component';
import {
  Country,
  LevelDivision
} from '@src/app/component-tools/geolocation/models';
import { GeolocationService } from '@src/app/component-tools/geolocation/geolocation.service';

@Component({
  selector: 'app-expediente',
  templateUrl: './expediente.component.html',
  styleUrls: ['./expediente.component.scss']
})
export class ExpedienteComponent implements OnInit {
  @Input() isReadOnly = false;
  @Input() data = null;
  @Input() expedienteAlumno: ExpedienteAlumnoDto;
  public confCbPlan: ConfigureCombobox;
  public confCbVersionesPlan: ConfigureCombobox;
  public confCvInstituacionDocente: ConfigureCombobox;
  public confCbAcceso: ConfigureComboboxTree;
  formTituacionAcceso: FormGroup;
  ctrl = FormTitulacionAcceso;
  blockIdentity = Guid.create().toString();
  colapsadoTitulacion: boolean;
  colapsadoTitulacionAcceso: boolean;
  colapsadoViaAccesoPlan: boolean;
  idAlumno: number;
  titulacionAcceso = null;
  tieneExpedienteBloqueado = null;
  tipoVinculacion = null;
  showTabs = true;
  combo = {
    versionPlan: null,
    acceso: null,
    institucionDocente: null
  };
  educationInstitution: EducationalInstitutions;
  countrySelected: Country;
  pathDivisionDefault: LevelDivision[];
  nameTerritorioInstitucionDocente: string = null;
  @ViewChild('confirmationModal') confirmationModal: ConfirmationModalComponent;
  @ViewChild('modalInstitucionDocente')
  modalInstitucionDocente: InstitucionDocenteAddComponent;
  @ViewChild('geolocationForm', { static: false })
  geolocationForm: GeolocationComponent;

  constructor(
    private activatedRoute: ActivatedRoute,
    private expedienteService: ExpedienteService,
    private route: Router,
    private blockUIService: BlockUIService,
    private alertSvc: AlertHandlerService,
    private translateService: TranslateService,
    private formBuilder: FormBuilder,
    private geolocationService: GeolocationService
  ) {}

  ngOnInit(): void {
    this.buildForm();
    this.translateService.setDefaultLang('es');
    this.activatedRoute.params.subscribe(({ id }) => {
      if (id === undefined) {
        this.route.navigateByUrl('/');
        return;
      }
      this.idAlumno = id;
      this.loadConfigComboBoxes();
      this.loadTipoVinculacion(
        this.expedienteAlumno.idRefTipoVinculacion,
        this.data
      );
      this.loadTitulacionAcceso(this.expedienteAlumno.titulacionAcceso);
      this.titulacionAcceso = this.expedienteAlumno.titulacionAcceso
        ? this.expedienteAlumno.titulacionAcceso
        : new TitulacionAccesoDto();
      if (this.titulacionAcceso.fechaInicioTitulo != undefined) {
        this.titulacionAcceso.fechaInicioTitulo = new Date(
          this.expedienteAlumno.titulacionAcceso.fechaInicioTitulo
        );
      }
      if (this.titulacionAcceso.fechafinTitulo != undefined) {
        this.titulacionAcceso.fechafinTitulo = new Date(
          this.expedienteAlumno.titulacionAcceso.fechafinTitulo
        );
      }
      this.expedienteService
        .tieneExpedienteBloqueado(id)
        .subscribe((response) => {
          this.tieneExpedienteBloqueado = response;
        });
    });
  }
  ctrlField(name: string): AbstractControl {
    return this.formTituacionAcceso.get(name);
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
  buildForm(): void {
    this.formTituacionAcceso = this.formBuilder.group({
      [FormTitulacionAcceso.titulo]: ['', [Validators.required]],
      [FormTitulacionAcceso.tipoEstudio]: null,
      [FormTitulacionAcceso.institucionDocente]: ['', [Validators.required]],
      [FormTitulacionAcceso.codeinstitucionDocente]: null,
      [FormTitulacionAcceso.idRefTerritorioInstitucionDocente]: null,
      [FormTitulacionAcceso.territorioInstitucionDocente]: null,
      [FormTitulacionAcceso.fechaInicioTitulo]: null,
      [FormTitulacionAcceso.fechafinTitulo]: null,
      [FormTitulacionAcceso.nroSemestreRealizados]: null,
      [FormTitulacionAcceso.codigoColegiadoProfesional]: null
    });
  }
  seleccionadoInstitucionDocente(data: EducationalInstitutions): void {
    this.ctrlField(FormTitulacionAcceso.codeinstitucionDocente).setValue(
      data.code
    );
    this.ctrlField(FormTitulacionAcceso.institucionDocente).setValue(data.name);
    this.educationInstitution = data;
    this.educationInstitution.countryName = data.countryName;
    this.educationInstitution.countryCode = data.code;

    if (
      this.ctrlField(FormTitulacionAcceso.idRefTerritorioInstitucionDocente)
        .value == null
    )
      this.setCountry(data);
  }

  setCountry(data: EducationalInstitutions): void {
    this.countrySelected = new Country();
    this.countrySelected.name = data.countryName;
    this.countrySelected.isoCode = data.countryCode;
  }

  loadTitulacionAcceso(data: TitulacionAccesoDto): void {
    if (data) {
      this.getPathUbicacion(data);
      this.ctrlField(FormTitulacionAcceso.titulo).setValue(data.titulo);
      this.ctrlField(FormTitulacionAcceso.tipoEstudio).setValue(
        data.tipoEstudio
      );
      this.ctrlField(FormTitulacionAcceso.codeinstitucionDocente).setValue(
        data.idRefInstitucionDocente
      );
      this.ctrlField(FormTitulacionAcceso.institucionDocente).setValue(
        data.institucionDocente
      );
      this.ctrlField(
        FormTitulacionAcceso.idRefTerritorioInstitucionDocente
      ).setValue(data.idRefTerritorioInstitucionDocente);
      if (data.fechaInicioTitulo != undefined) {
        this.ctrlField(FormTitulacionAcceso.fechaInicioTitulo).setValue(
          new Date(data.fechaInicioTitulo)
        );
      }
      if (data.fechafinTitulo != undefined) {
        this.ctrlField(FormTitulacionAcceso.fechafinTitulo).setValue(
          new Date(data.fechafinTitulo)
        );
      }
      this.ctrlField(FormTitulacionAcceso.nroSemestreRealizados).setValue(
        data.nroSemestreRealizados
      );
      this.ctrlField(FormTitulacionAcceso.codigoColegiadoProfesional).setValue(
        data.codigoColegiadoProfesional
      );
    }
  }

  loadModalInstitucionDocente(): void {
    this.modalInstitucionDocente.selectInstitucionDocente();
  }

  loadConfigComboBoxes(): void {
    if (this.data.idRefVersionPlan) {
      const versionPlan = this.getItemVersionPlan(
        this.data.plan.versionesPlanes.find(
          (x) => x.id == +this.data.idRefVersionPlan
        )
      );
      if (versionPlan) this.combo.versionPlan = versionPlan;
    }
    this.cargarViaAcceso();
    this.confCvInstituacionDocente = new ConfigureCombobox({
      url: `${keys.API_BASE_URL}/api/v1/integracion/commons/educational-institutions`,
      perPage: 10,
      minLength: 0,
      data: {
        search: '#SEARCH',
        limit: 10,
        offset: 1
      },
      method: 'POST',
      transformData: (data: EducationalInstitutions[]) =>
        this.getItemsInstituacionDocente(data)
    });

    this.confCbVersionesPlan = new ConfigureCombobox({
      url: `${keys.API_BASE_URL}/api/v1/integracion/erp-academico/versiones-planes`,
      perPage: 10,
      data: {
        index: 1,
        count: 10,
        filterIdPlan: this.data.idRefPlan
      },
      method: 'POST',
      transformData: (data: CustomSimpleListItemModel[]) =>
        help.getItems(data, 'nro')
    });

    this.confCbAcceso = new ConfigureComboboxTree({
      url: `${keys.API_BASE_URL}/api/v1/integracion/erp-academico/vias-acceso-tree-nodes`,
      perPage: 10,
      minLength: 0,
      data: {
        search: '',
        index: 0,
        count: 10,
        where: [
          {
            field: 'filterIdNodo',
            value: this.data?.viaAccesoPlan?.nodo?.id
          }
        ]
      },
      method: 'POST',
      transformData: (data: TreeItem[]) => this.getItems(data)
    });
  }

  getPathUbicacion(data: TitulacionAccesoDto): void {
    const ubicacion = data.codeCountryInstitucionDocente;
    if (data.idRefTerritorioInstitucionDocente) {
      this.geolocationService
        .getPathDivision(data.idRefTerritorioInstitucionDocente)
        .subscribe((path) => {
          if (path.length > 0) {
            this.pathDivisionDefault = path;
            const division = path.find(
              (d) => d.code == data.idRefTerritorioInstitucionDocente
            );
            if (division) {
              this.ctrlField(
                FormTitulacionAcceso.territorioInstitucionDocente
              ).setValue(`${division.name} - (${division.countryName})`);
            }
          }
        });
    }
    if (ubicacion == '') {
      this.setEducationInstitution(data, null);
      return;
    }
    this.geolocationService.getCountry(ubicacion).subscribe((country) => {
      if (
        country.name &&
        data.idRefTerritorioInstitucionDocente &&
        !this.pathDivisionDefault
      ) {
        this.ctrlField(
          FormTitulacionAcceso.territorioInstitucionDocente
        ).setValue(`${country.name}`);
      }
      this.countrySelected = country;
      this.setEducationInstitution(data, country);
    });
  }

  setEducationInstitution(data: TitulacionAccesoDto, country: Country): void {
    this.educationInstitution = new EducationalInstitutions();
    this.educationInstitution.code = data?.idRefInstitucionDocente;
    this.educationInstitution.name = data?.institucionDocente;
    this.educationInstitution.countryCode = country?.isoCode;
    this.educationInstitution.countryName = country?.name;
  }

  cargarViaAcceso(): void {
    const viaAcceso = this.getItemViaAcceso({
      id: this.data.viaAccesoPlan.viaAcceso.id,
      name: this.data.viaAccesoPlan.viaAcceso
        .displayNameClasificacionSuperViaAcceso
    });
    if (viaAcceso) this.combo.acceso = viaAcceso;
  }

  eliminarInstitucionDocente(): void {
    this.ctrlField(FormTitulacionAcceso.codeinstitucionDocente).setValue(null);
    this.ctrlField(FormTitulacionAcceso.institucionDocente).setValue(null);
    if (
      this.ctrlField(FormTitulacionAcceso.idRefTerritorioInstitucionDocente)
        .value == null
    ) {
      this.countrySelected = null;
      this.educationInstitution = null;
    } else this.setEducationInstitution(null, this.countrySelected);
  }

  eliminarUbicacion(): void {
    this.pathDivisionDefault = [];
    this.ctrlField(
      FormTitulacionAcceso.idRefTerritorioInstitucionDocente
    ).setValue(null);
    this.ctrlField(FormTitulacionAcceso.territorioInstitucionDocente).setValue(
      null
    );
    if (
      this.ctrlField(FormTitulacionAcceso.codeinstitucionDocente).value == null
    ) {
      this.countrySelected = null;
      this.educationInstitution = null;
    } else {
      this.countrySelected = null;
      this.setEducationInstitution(
        this.expedienteAlumno.titulacionAcceso,
        null
      );
    }
  }

  loadTipoVinculacion(
    idRefTipoVinculacion: string,
    dataExpediente: DatosErpResponse
  ): void {
    if (idRefTipoVinculacion != null && dataExpediente.plan != null) {
      this.expedienteService
        .getStudentUniversitesBindingTypes(
          dataExpediente.plan?.estudio?.areaAcademica?.centro?.universidad
            ?.idIntegracion
        )
        .subscribe((response) => {
          this.tipoVinculacion = response.find(
            (u) => u.code == idRefTipoVinculacion
          )?.name;
        });
    }
  }

  save(): void {
    if (this.combo.versionPlan != null) {
      const today = new Date();
      const fechaInicio = this.combo.versionPlan.data.fechaInicio
        ? new Date(this.combo.versionPlan.data.fechaInicio)
        : null;
      const fechaFin = this.combo.versionPlan.data.fechaFin
        ? new Date(this.combo.versionPlan.data.fechaFin)
        : null;
      if (today < fechaInicio || (fechaFin && today > fechaFin)) {
        this.confirmationModal.show(
          () => {
            this.editarExpediente();
          },
          this.translateService.instant(
            'pages.editarExpediente.editarExpedienteTitle'
          ),
          this.translateService.instant(
            'pages.editarExpediente.confirmEditarExpediente'
          )
        );
        return;
      }
    }
    this.editarExpediente();
  }
  ctrlInvalid(name: string): boolean {
    const field = this.ctrlField(name);
    return field.invalid && (field.touched || field.dirty);
  }
  ctrlValid(name: string): boolean {
    const field = this.ctrlField(name);
    return field.valid && (field.touched || field.dirty);
  }
  editarExpediente(): void {
    this.blockUIService.start(this.blockIdentity);
    this.showTabs = false;
    const payload = new EditExpedienteAlumno();
    payload.idExpedienteAlumno = +this.data.idIntegracion;
    payload.idRefVersionPlan = this.combo.versionPlan?.value.toString();
    payload.nroVersion = this.combo.versionPlan?.text;
    payload.idRefNodo = this.data.viaAccesoPlan?.nodo?.id.toString();
    payload.idRefViaAcceso = this.combo.acceso?.id.toString();
    this.formTituacionAcceso.markAllAsTouched();
    payload.titulacionAcceso = this.getTitulacionAcceso();
    this.expedienteService.updateExpediente(payload).subscribe(() => {
      this.alertSvc.success(this.translateService.instant('messages.success'));
      this.showTabs = true;
      this.expedienteService
        .getDatosErpAlumno(this.idAlumno)
        .subscribe((response) => {
          this.data = response;
          this.cargarViaAcceso();
          this.blockUIService.stop(this.blockIdentity);
        });
    });
  }

  getTitulacionAcceso(): TitulacionAccesoDto {
    if (
      (!this.ctrlField(FormTitulacionAcceso.titulo).value ||
        this.ctrlField(FormTitulacionAcceso.titulo).value === '') &&
      (!this.ctrlField(FormTitulacionAcceso.codeinstitucionDocente).value ||
        this.ctrlField(FormTitulacionAcceso.codeinstitucionDocente).value ===
          '') &&
      (!this.ctrlField(FormTitulacionAcceso.nroSemestreRealizados).value ||
        this.ctrlField(FormTitulacionAcceso.nroSemestreRealizados).value ===
          '') &&
      (!this.ctrlField(FormTitulacionAcceso.tipoEstudio).value ||
        this.ctrlField(FormTitulacionAcceso.tipoEstudio).value === '') &&
      (!this.ctrlField(FormTitulacionAcceso.institucionDocente).value ||
        this.ctrlField(FormTitulacionAcceso.institucionDocente).value === '') &&
      (!this.ctrlField(FormTitulacionAcceso.idRefTerritorioInstitucionDocente)
        .value ||
        this.ctrlField(FormTitulacionAcceso.idRefTerritorioInstitucionDocente)
          .value === '') &&
      (!this.ctrlField(FormTitulacionAcceso.fechaInicioTitulo).value ||
        this.ctrlField(FormTitulacionAcceso.fechaInicioTitulo).value === '') &&
      (!this.ctrlField(FormTitulacionAcceso.fechafinTitulo).value ||
        this.ctrlField(FormTitulacionAcceso.fechafinTitulo).value === '') &&
      (!this.ctrlField(FormTitulacionAcceso.codigoColegiadoProfesional).value ||
        this.ctrlField(FormTitulacionAcceso.codigoColegiadoProfesional)
          .value === '')
    )
      return null;

    const result = new TitulacionAccesoDto();
    result.titulo = this.ctrlField(FormTitulacionAcceso.titulo).value;
    result.tipoEstudio = this.ctrlField(FormTitulacionAcceso.tipoEstudio).value;
    result.idRefInstitucionDocente = this.ctrlField(
      FormTitulacionAcceso.codeinstitucionDocente
    ).value;
    result.institucionDocente = this.ctrlField(
      FormTitulacionAcceso.institucionDocente
    ).value;
    result.idRefTerritorioInstitucionDocente = this.ctrlField(
      FormTitulacionAcceso.idRefTerritorioInstitucionDocente
    ).value;
    result.fechaInicioTitulo = this.ctrlField(
      FormTitulacionAcceso.fechaInicioTitulo
    ).value;
    result.fechafinTitulo = this.ctrlField(
      FormTitulacionAcceso.fechafinTitulo
    ).value;
    result.codigoColegiadoProfesional = this.ctrlField(
      FormTitulacionAcceso.codigoColegiadoProfesional
    ).value;
    result.nroSemestreRealizados =
      this.ctrlField(FormTitulacionAcceso.nroSemestreRealizados).value != ''
        ? this.ctrlField(FormTitulacionAcceso.nroSemestreRealizados).value
        : null;
    return result;
  }

  showHideTitulacion(): void {
    this.colapsadoTitulacion = !this.colapsadoTitulacion;
    helpers.clearFocus();
  }

  showHideViaAccesoPlan(): void {
    this.colapsadoViaAccesoPlan = !this.colapsadoViaAccesoPlan;
    helpers.clearFocus();
  }

  showHideTitulacionAcceso(): void {
    this.colapsadoTitulacionAcceso = !this.colapsadoTitulacionAcceso;
    helpers.clearFocus();
  }

  getItemVersionPlan(data: VersionesPlane): ComboboxItem {
    return new ComboboxItem({
      value: data.id,
      text: data.nro.toString(),
      data: data
    });
  }

  getItemViaAcceso(data: TreeItem): UnirTreeViewItem {
    return new UnirTreeViewItem(data);
  }

  getItems(data: TreeItem[]): UnirTreeViewItem[] {
    return data.map((value) => new UnirTreeViewItem(value));
  }

  loadModalGeolocation(): void {
    this.geolocationForm.open((result) => this.onSuccess(result));
  }

  onSuccess(country: Country): void {
    this.countrySelected = country;
    this.pathDivisionDefault = country.divisions;
    let displayUbicacion = null;
    let idRefTerritorioInstitucionDocente = null;
    let level = 0;

    country.divisions.forEach((element) => {
      if (element.divisionLevel > level) {
        displayUbicacion = `${element.name} - (${country.name})`;
        idRefTerritorioInstitucionDocente = element.code;
        level = element.divisionLevel;
      }
    });

    if (country.divisions.length == 0) {
      idRefTerritorioInstitucionDocente = country.isoCode;
      displayUbicacion = country.name;
    }

    this.ctrlField(FormTitulacionAcceso.territorioInstitucionDocente).setValue(
      displayUbicacion
    );
    this.ctrlField(
      FormTitulacionAcceso.idRefTerritorioInstitucionDocente
    ).setValue(idRefTerritorioInstitucionDocente);
    if (
      this.ctrlField(FormTitulacionAcceso.codeinstitucionDocente).value == null
    )
      this.setEducationInstitution(null, this.countrySelected);
    else {
      if (
        this.ctrlField(FormTitulacionAcceso.codeinstitucionDocente).value ==
        '-1'
      ) {
        this.educationInstitution.countryCode = this.countrySelected?.isoCode;
        this.educationInstitution.countryName = this.countrySelected?.name;
      }
    }
  }
}
