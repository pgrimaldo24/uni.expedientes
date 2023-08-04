import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { DataTableComponent } from '@src/app/component-tools/data-table/data-table.component';
import {
  DataColumn,
  DataRow
} from '@src/app/component-tools/data-table/models';
import * as helpers from '@helpers/commons-helpers';
import { Router } from '@angular/router';
import { AlumnoService } from '../alumno.service';
import { AlumnoInfoDto } from '@pages/expediente-alumno/expediente-models';

@Component({
  selector: 'app-expedientes',
  templateUrl: './expedientes.component.html',
  styleUrls: ['./expedientes.component.scss']
})
export class ExpedientesComponent implements OnInit {
  columns: DataColumn[];
  rows: DataRow[];
  @ViewChild('table', { static: true }) table: DataTableComponent;
  @Input() alumno: AlumnoInfoDto;

  constructor(
    private translateService: TranslateService,
    private router: Router,
    private alumnoService: AlumnoService
  ) {
    this.translateService.setDefaultLang('es');
  }
  ngOnInit(): void {
    this.inicializarCabecerasTabla();
    this.table.update();
  }

  search(): void {
    const displayNameDocumentoIdentificacionAlumno = `${this.alumno.idIntegracionAlumno} - ${this.alumno.tipoDocumentoIdentificacionPais} ${this.alumno.nroDocIdentificacion}`;
    this.rows = this.alumno.expedientes
      .sort(function (expedienteA, expedienteB) {
        return expedienteA.id - expedienteB.id;
      })
      .map(
        (e) =>
          new DataRow(
            {
              id: e.id,
              displayNameNombreAlumno: this.alumno.displayName,
              displayNameDocumentoIdentificacionAlumno: displayNameDocumentoIdentificacionAlumno,
              fechaApertura:
                e.fechaApertura != null
                  ? helpers.dateToString(e.fechaApertura, 'DD/MM/YYYY')
                  : e.fechaFinalizacion != null
                  ? helpers.dateToString(e.fechaFinalizacion, 'DD/MM/YYYY')
                  : '',
              fechaFinalizacion:
                e.fechaFinalizacion != null
                  ? helpers.dateToString(e.fechaFinalizacion, 'DD/MM/YYYY')
                  : '',
              plan: e.nombrePlan ? e.nombrePlan : e.nombreEstudio
            },
            e
          )
      );
  }

  redirectToEditExpediente(idExpediente: number): void {
    this.router.navigate([`/ExpedienteAlumno/edit/${idExpediente}`]);
  }

  checkSolicitud(event: Event, idSolicitud: number): void {
    const checked: boolean = (event.target as HTMLInputElement).checked;
    if (checked) {
      this.alumnoService.idsExpedientes.push(idSolicitud);
    } else {
      this.alumnoService.idsExpedientes = this.alumnoService.idsExpedientes.filter(
        (id) => id !== idSolicitud
      );
    }
    this.alumnoService.setFlat(!this.alumnoService.flat);
    this.alumnoService.setRowExpediente();
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
        field: 'id',
        sortable: false,
        header: this.translateService.instant('pages.listadoExpediente.id')
      },
      {
        field: 'acronimoUniversidad',
        sortable: false,
        header: this.translateService.instant(
          'pages.listadoExpediente.universidad'
        )
      },
      {
        field: 'alumno',
        sortable: false,
        header: this.translateService.instant('pages.listadoExpediente.alumno'),
        style: { width: '400px' }
      },
      {
        field: 'plan',
        sortable: false,
        header: this.translateService.instant(
          'pages.listadoExpediente.columnEstudioPlan'
        )
      },
      {
        field: 'fechas',
        sortable: false,
        header: this.translateService.instant('pages.listadoExpediente.fechas'),
        style: { width: '200px' }
      },
      {
        field: 'acciones',
        sortable: false
      }
    ];
  }
}
