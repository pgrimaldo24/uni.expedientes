import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { DataTableComponent } from '@src/app/component-tools/data-table/data-table.component';
import {
  DataColumn,
  DataRow
} from '@src/app/component-tools/data-table/models';
import {
  AlumnoInfoDto,
  MatriculaDto
} from '@pages/expediente-alumno/expediente-models';
import { AppConfigService } from '@src/app/services/app-config.service';
import { AlumnoService } from '../alumno.service';

@Component({
  selector: 'app-matriculas',
  templateUrl: './matriculas.component.html',
  styleUrls: ['./matriculas.component.scss']
})
export class MatriculasComponent implements OnInit {
  columns: DataColumn[];
  rows: DataRow[] = [];
  @ViewChild('table', { static: true }) table: DataTableComponent;
  @Input() alumno: AlumnoInfoDto;

  constructor(
    private translateService: TranslateService,
    private appConfigService: AppConfigService,
    private alumnoService: AlumnoService
  ) {
    this.translateService.setDefaultLang('es');
  }

  ngOnInit(): void {
    this.inicializarCabecerasTabla();
    this.table.update();
    this.alumnoService.flatExpediente$.subscribe(() => {
      this.searchByExpedientes(this.alumnoService.idsExpedientes);
    });
  }

  searchByExpedientes(idsExpedientes: number[]): void {
    if (!idsExpedientes.length) {
      this.loadRowsMatriculas(this.alumno.matriculas);
      return;
    }
    const matriculas = this.alumno.matriculas.filter((m) =>
      idsExpedientes.includes(parseInt(m.idRefExpedienteAlumno))
    );
    this.loadRowsMatriculas(matriculas);
  }

  search(): void {
    this.loadRowsMatriculas(this.alumno.matriculas);
  }

  get redirectToEditMatricula(): string {
    const config = this.appConfigService.getConfig();
    return `${config.urlErpAcademico}/Matricula/Edit/`;
  }

  checkMatriculas(event: Event, idSolicitud: number): void {
    const checked: boolean = (event.target as HTMLInputElement).checked;
    if (checked) {
      this.alumnoService.idsMatriculas.push(idSolicitud);
    } else {
      this.alumnoService.idsMatriculas = this.alumnoService.idsMatriculas.filter(
        (id) => id !== idSolicitud
      );
    }
    this.alumnoService.setFlat(!this.alumnoService.flat);
  }

  private inicializarCabecerasTabla() {
    this.columns = [
      {
        field: 'seleccion',
        sortable: false,
        class: 'text-center',
        style: { width: '10px' }
      },
      {
        field: 'numero',
        sortable: false,
        header: this.translateService.instant('pages.alumno.matriculas.numero')
      },
      {
        field: 'alumno',
        sortable: false,
        header: this.translateService.instant('pages.alumno.matriculas.alumno'),
        style: { width: '300px' }
      },
      {
        field: 'anioAcademico',
        sortable: false,
        header: this.translateService.instant(
          'pages.alumno.matriculas.anioAcademico'
        )
      },
      {
        field: 'periodoAcademico',
        sortable: false,
        header: this.translateService.instant(
          'pages.alumno.matriculas.periodoAcademico'
        ),
        style: { width: '400px' }
      },
      {
        field: 'inicioPER',
        sortable: false,
        header: this.translateService.instant(
          'pages.alumno.matriculas.inicioPER'
        )
      },
      {
        field: 'planEstudio',
        sortable: false,
        header: this.translateService.instant(
          'pages.alumno.matriculas.planEstudio'
        ),
        style: { width: '400px' }
      },
      {
        field: 'tipo',
        sortable: false,
        header: this.translateService.instant('pages.alumno.matriculas.tipo')
      },
      {
        field: 'regionEstudio',
        sortable: false,
        header: this.translateService.instant(
          'pages.alumno.matriculas.regionEstudio'
        ),
        style: { width: '200px' }
      },
      {
        field: 'creditosTotales',
        sortable: false,
        header: this.translateService.instant(
          'pages.alumno.matriculas.creditosTotales'
        )
      },
      {
        field: 'estado',
        sortable: false,
        header: this.translateService.instant('pages.alumno.matriculas.estado')
      },
      {
        field: 'acciones',
        sortable: false
      }
    ];
  }

  loadRowsMatriculas(matriculas: MatriculaDto[]): void {
    if (!matriculas.length) return;
    this.rows = matriculas.map(
      (e) =>
        new DataRow(
          {
            idMatricula: e.id,
            numero: e.displayName,
            alumno: this.alumno.displayName,
            anioAcademico:
              e.planOfertado.periodoAcademico.anyoAcademico.displayName,
            periodoAcademico: e.planOfertado.periodoAcademico.displayName,
            inicioPER: e.planOfertado.periodoAcademico.fechaInicio,
            planEstudio: e.planOfertado.plan.displayName,
            tipo: e.tipo.displayName,
            regionEstudio: e.regionEstudio.displayName,
            creditosTotales: e.totalCreditosAsignaturasMatriculadasActivas,
            estado: e.estado.displayName
          },
          e
        )
    );
  }
}
