import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup } from '@angular/forms';
import { AlumnoService } from '@pages/alumno/alumno.service';
import { ConfigureCombobox } from '@src/app/component-tools/combobox/models';
import { CustomSimpleListItemModel } from '@src/app/models/erp-academico';
import { keys } from '@src/keys';
import * as help from '@helpers/commons-helpers';
import {
  AlumnoInfoDto,
  ExpedienteAlumnoDto
} from '@pages/expediente-alumno/expediente-models';
import { ConsolidarRequisitoFormComponent } from './consolidar-requisito-form/consolidar-requisito-form.component';
import { Guid } from 'guid-typescript';
import {
  ConfiguracionExpedienteUniversidadDto,
  FormConsolidarRequisitoControls
} from './consolidar-requisito.models';
import { SecurityService } from '@src/app/services/security.service';

@Component({
  selector: 'app-requisitos-tab',
  templateUrl: './requisitos-tab.component.html',
  styleUrls: ['./requisitos-tab.component.scss']
})
export class RequisitosTabComponent implements OnInit {
  @Input() alumno: AlumnoInfoDto;
  @Input() expedienteAlumno: ExpedienteAlumnoDto;
  @Input() configuracionUniversidad: ConfiguracionExpedienteUniversidadDto;
  public requisitoForm: FormGroup;
  public formControls = FormConsolidarRequisitoControls;
  public configComboRequisito: ConfigureCombobox;
  public blockIdentityRequisito = Guid.create().toString();
  @ViewChild('consolidarRequisitoComponent', { static: true })
  consolidarRequisitoComponent: ConsolidarRequisitoFormComponent;
  public isAdminGestor: boolean;
  private roles = keys;

  constructor(
    public alumnoService: AlumnoService,
    private fb: FormBuilder,
    private security: SecurityService
  ) {
    this.requisitoForm = this.createRequisitoForm();
  }

  ngOnInit(): void {
    this.initializeCombobox();
    const userRoles = this.security.userRoles();
    this.isAdminGestor =
      userRoles.indexOf(this.roles.ADMIN_ROLE) > -1 ||
      userRoles.indexOf(this.roles.GESTOR_ROLE) > -1;
  }

  createRequisitoForm(): FormGroup {
    return this.fb.group({
      [FormConsolidarRequisitoControls.requisitoAdicional]: null,
      [FormConsolidarRequisitoControls.fileUpload]: null,
      [FormConsolidarRequisitoControls.esDocumentacionFisica]: null,
      [FormConsolidarRequisitoControls.enviadaPorAlumno]: null,
      [FormConsolidarRequisitoControls.texto]: null,
      [FormConsolidarRequisitoControls.fecha]: null,
      [FormConsolidarRequisitoControls.idioma]: null,
      [FormConsolidarRequisitoControls.nivelIdioma]: null,
      [FormConsolidarRequisitoControls.causaEstadoRequisito]: null,
      [FormConsolidarRequisitoControls.requisitoDocumento]: null
    });
  }

  initializeCombobox(): void {
    this.configComboRequisito = new ConfigureCombobox({
      url: `${keys.API_BASE_URL}/api/v1/requisitos-expedientes/query-bloqueados`,
      perPage: 10,
      data: {
        offset: '#PAGE',
        limit: '#PER_PAGE',
        where: [
          {
            field: 'filterNombre',
            value: '#SEARCH'
          },
          {
            field: 'filterIdRefUniversidad',
            value: this.expedienteAlumno.idRefUniversidad
          }
        ]
      },
      method: 'POST',
      transformData: (data: CustomSimpleListItemModel[]) => help.getItems(data)
    });
  }

  ctrlField(name: string): AbstractControl {
    return this.requisitoForm.get(name);
  }

  ctrlInvalid(name: string): boolean {
    const field = this.ctrlField(name);
    return field.invalid;
  }

  createConsolidacionRequisito(): void {
    this.consolidarRequisitoComponent.createConsolidacionRequisito();
  }

  createConsolidacionesRequisitosExpediente(): void {
    this.consolidarRequisitoComponent.createConsolidacionesRequisitosExpediente();
  }
}
