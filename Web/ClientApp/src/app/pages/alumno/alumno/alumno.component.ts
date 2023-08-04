import { Component, Input, OnInit, ViewChild } from '@angular/core';
import {
  AlumnoInfoDto,
  ExpedienteAlumnoDto,
  Universities
} from '@pages/expediente-alumno/expediente-models';
import { ExpedienteService } from '@pages/expediente-alumno/expediente.service';
import { AppConfigService } from '@src/app/services/app-config.service';
import { CultureService } from '@src/app/services/culture.service';
import { Guid } from 'guid-typescript';
import { BlockUIService } from 'ng-block-ui';
import { DeudaClienteModel, DeudaClienteParameters } from '../alumno-models';
import { AlumnoService } from '../alumno.service';
import { ShowSaldosComponent } from '../show-saldos/show-saldos.component';

@Component({
  selector: 'app-alumno',
  templateUrl: './alumno.component.html',
  styleUrls: ['./alumno.component.scss']
})
export class AlumnoComponent implements OnInit {
  deuda: DeudaClienteModel;
  public blockIdentity = Guid.create().toString();
  @Input() alumno: AlumnoInfoDto;
  @Input() expediente: ExpedienteAlumnoDto;
  @ViewChild('openForm', { static: false })
  formSaldos: ShowSaldosComponent;

  constructor(
    private alumnoService: AlumnoService,
    private blockUI: BlockUIService,
    private appConfigService: AppConfigService,
    private cultureService: CultureService,
    private expedienteService: ExpedienteService
  ) {}

  ngOnInit(): void {
    this.getDeudaAlumno();
  }

  getDeudaAlumno(): void {
    this.blockUI.start(this.blockIdentity);
    const payload = new DeudaClienteParameters(
      this.alumno.idUniversidadIntegracion,
      this.alumno.idIntegracionAlumno
    );
    this.alumnoService.getSaldosAlumno(payload).subscribe((response) => {
      this.deuda = response;
      this.blockUI.stop(this.blockIdentity);
      this.setCultureAmount();
    });
  }

  get redirectToEditAlumno(): string {
    const config = this.appConfigService.getConfig();
    return `${config.urlErpAcademico}/Alumno/Edit/${this.alumno.idAlumno}`;
  }

  private setCultureAmount(): void {
    this.getCultureAlumno(this.alumno.idUniversidadIntegracion);
  }

  showSaldos(): void {
    this.formSaldos.open(this.deuda);
  }

  private getCultureAlumno(idIntegracion: string): void {
    this.expedienteService
      .getUniversities(idIntegracion)
      .subscribe((response: Universities) => {
        if (response.defaultCulture)
          this.cultureService.setCulture$ = response.defaultCulture;
      });
  }
}
