import { Component, OnDestroy, OnInit, Input } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { ExpedienteAlumnoDto } from '@pages/expediente-alumno/expediente-models';
import { AsignaturaDto } from '@pages/expediente-alumno/resumen-tab/resumen.models';
import {
  DataColumn,
  DataRow
} from '@src/app/component-tools/data-table/models';
import { Subscription } from 'rxjs';
import {
  AsignaturaExpedienteDto,
  AsignaturaReconocimientoDto,
  IdTipoAsignaturaReconocimiento,
  RequerimientoPlanDto,
  TipoAsignaturaRequerimientoDto
} from '../calificaciones-model';
import { CalificacionesService } from '../calificaciones.service';

@Component({
  selector: 'app-list-calificaciones',
  templateUrl: './list-calificaciones.component.html',
  styleUrls: ['./list-calificaciones.component.scss']
})
export class ListCalificacionesComponent implements OnInit, OnDestroy {
  @Input() expedienteAlumno: ExpedienteAlumnoDto;
  @Input() reconocimientos: AsignaturaReconocimientoDto[];
  public requerimientoPlan: RequerimientoPlanDto;
  public asignaturasExpediente: AsignaturaExpedienteDto[];
  public asignaturasTrayecto: AsignaturaDto[];
  public columns: DataColumn[];
  subscription: Subscription;
  private asignaturasReconocimientos: AsignaturaReconocimientoDto[];
  private reconocimientosTransversal: AsignaturaReconocimientoDto[];
  private reconocimientosSeminario: AsignaturaReconocimientoDto[];
  private reconocimientosUniversitaria: AsignaturaReconocimientoDto[];
  private labelCreditoRequerido: string;
  private labelCreditoObtenido: string;

  constructor(
    private translateService: TranslateService,
    private calificacionesService: CalificacionesService
  ) {}

  ngOnInit(): void {
    this.initializeTableHeaders();
    this.getTrayectoSelected();
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }

  getTrayectoSelected(): void {
    this.subscription = this.calificacionesService.changeTrayectoCalificaciones$.subscribe(
      (trayecto) => {
        this.requerimientoPlan = trayecto.requerimientoPlan;
        this.asignaturasTrayecto = trayecto.asignaturasTrayecto;
        this.asignaturasExpediente = trayecto.asignaturasExpediente;
        this.setReconocimientos();
        this.asignarAsignaturasEnTipoAsignatura();
      }
    );
  }

  initializeTableHeaders(): void {
    this.columns = [
      {
        field: 'asignatura',
        sortable: false,
        header: this.translateService.instant(
          'pages.expedienteTabs.calificaciones.asignatura'
        ),
        style: { width: '300px' }
      },
      {
        field: 'ects',
        sortable: false,
        header: this.translateService.instant(
          'pages.expedienteTabs.calificaciones.ects'
        ),
        style: { width: '50px' }
      },
      {
        field: 'calificacion',
        sortable: false,
        header: this.translateService.instant(
          'pages.expedienteTabs.calificaciones.calificacion'
        ),
        style: { width: '150px' }
      },
      {
        field: 'anioAcademico',
        sortable: false,
        header: this.translateService.instant(
          'pages.expedienteTabs.calificaciones.anioAcademico'
        ),
        style: { width: '150px' }
      },
      {
        field: 'curso',
        sortable: false,
        header: this.translateService.instant(
          'pages.expedienteTabs.calificaciones.curso'
        ),
        style: { width: '80px' }
      },
      {
        field: 'periodo',
        sortable: false,
        header: this.translateService.instant(
          'pages.expedienteTabs.calificaciones.periodo'
        ),
        style: { width: '150px' }
      },
      {
        field: 'idiomaImparticion',
        sortable: false,
        header: this.translateService.instant(
          'pages.expedienteTabs.calificaciones.idiomaImparticion'
        ),
        style: { width: '150px' }
      }
    ];
    this.labelCreditoRequerido = this.translateService.instant(
      'pages.expedienteTabs.calificaciones.creditosRequeridos'
    );
    this.labelCreditoObtenido = this.translateService.instant(
      'pages.expedienteTabs.calificaciones.creditosObtenidos'
    );
  }

  setReconocimientos(): void {
    if (!this.requerimientoPlan || !this.reconocimientos?.length) return;
    this.asignaturasReconocimientos = this.reconocimientos.filter(
      (ar) => ar.esAsignatura
    );
    this.reconocimientosTransversal = this.reconocimientos.filter(
      (ar) => ar.esTransversal
    );
    this.reconocimientosSeminario = this.reconocimientos.filter(
      (ar) => ar.esSeminario
    );
    this.reconocimientosUniversitaria = this.reconocimientos.filter(
      (ar) => ar.esExtensionUniversitaria
    );
    if (this.reconocimientosTransversal?.length) {
      this.setTiposAsignaturasReconocimientos(
        this.reconocimientosTransversal[0]
      );
    }

    if (this.reconocimientosSeminario?.length) {
      this.setTiposAsignaturasReconocimientos(this.reconocimientosSeminario[0]);
    }

    if (this.reconocimientosUniversitaria?.length) {
      this.setTiposAsignaturasReconocimientos(
        this.reconocimientosUniversitaria[0]
      );
    }
  }

  setTiposAsignaturasReconocimientos(
    reconocimiento: AsignaturaReconocimientoDto
  ): void {
    this.requerimientoPlan.tiposAsignaturasRequerimiento.push({
      minCreditos: 0,
      tipoAsignatura: {
        id: +reconocimiento.idRefTipoAsignatura,
        nombre: reconocimiento.nombreTipoAsignatura,
        esReconocimiento: true
      }
    });
  }

  asignarAsignaturasEnTipoAsignatura(): void {
    if (
      !this.requerimientoPlan ||
      !this.requerimientoPlan.tiposAsignaturasRequerimiento.length
    )
      return;
    this.requerimientoPlan.tiposAsignaturasRequerimiento.forEach((element) => {
      element.creditosAsignaturas = 0;
      const tipoAsignatura = element.tipoAsignatura;
      const asignaturasReconocimientos = tipoAsignatura.esReconocimiento
        ? this.getAsignaturasReconocimientosAMostrar(tipoAsignatura.id)
        : [];

      const asignaturasErp = this.asignaturasTrayecto.filter(
        (a) => a.tipoAsignatura.id == tipoAsignatura.id
      );
      if (
        asignaturasErp?.length ||
        (tipoAsignatura.esReconocimiento && asignaturasReconocimientos.length)
      ) {
        const asignaturas = tipoAsignatura.esReconocimiento
          ? asignaturasReconocimientos
          : this.getAsignaturasAMostrar(asignaturasErp);
        element.creditosAsignaturas = asignaturas.reduce(
          (acc, item) => acc + (item.ects ?? 0),
          0
        );
        element.rowsBloques = this.getAsignaturasDataRow(asignaturas);
      }
    });
    this.requerimientoPlan.creditosObtenidos = this.requerimientoPlan.tiposAsignaturasRequerimiento.reduce(
      (acc, item) => acc + (item.creditosAsignaturas ?? 0),
      0
    );
  }

  getAsignaturasAMostrar(
    asignaturasErp: AsignaturaDto[]
  ): AsignaturaExpedienteDto[] {
    const asignaturas: AsignaturaExpedienteDto[] = [];
    asignaturasErp.forEach((item) => {
      const asignaturaExpediente = this.asignaturasExpediente.find(
        (ae) =>
          +ae.idRefAsignaturaPlan === item.idAsignaturaPlan &&
          +ae.idRefTipoAsignatura === item.tipoAsignatura.id
      );
      if (asignaturaExpediente) {
        asignaturas.push(asignaturaExpediente);
      }
    });
    return asignaturas;
  }

  getAsignaturasReconocimientosAMostrar(
    idTipoAsignatura: number
  ): AsignaturaExpedienteDto[] {
    const asignaturasReconocimientos =
      idTipoAsignatura == IdTipoAsignaturaReconocimiento.Seminario
        ? this.reconocimientosSeminario
        : idTipoAsignatura == IdTipoAsignaturaReconocimiento.Universitaria
        ? this.reconocimientosUniversitaria
        : this.reconocimientosTransversal.filter(
            (rt) => rt.esTransversalPrincipal
          );

    const asignaturas: AsignaturaExpedienteDto[] = [];
    asignaturasReconocimientos.forEach((item) => {
      asignaturas.push({
        nombreAsignatura: item.nombreAsignatura,
        ects: item.ects,
        calificacion: item.calificacion,
        esReconocimientoTransversal:
          idTipoAsignatura == IdTipoAsignaturaReconocimiento.Transversal
      });
    });
    return asignaturas;
  }

  getAsignaturasDataRow(asignaturas: AsignaturaExpedienteDto[]): DataRow[] {
    const rows: DataRow[] = [];
    asignaturas.forEach((asignatura) => {
      const row = this.setRowAsignatura(asignatura);
      let reconocimientos = this.asignaturasReconocimientos?.filter(
        (ar) => ar.idAsignaturaPlanErp == +asignatura.idRefAsignaturaPlan
      );
      if (asignatura.esReconocimientoTransversal) {
        reconocimientos = this.reconocimientosTransversal.filter(
          (rt) => !rt.esTransversalPrincipal
        );
      }
      if (!reconocimientos || !reconocimientos.length) {
        rows.push(row);
        return;
      }
      this.setRowAsignaturaReconocimiento(
        reconocimientos,
        asignatura,
        row,
        rows
      );
    });
    return rows;
  }

  setRowAsignaturaReconocimiento(
    reconocimientos: AsignaturaReconocimientoDto[],
    asignatura: AsignaturaExpedienteDto,
    currentRow: DataRow,
    rows: DataRow[]
  ): void {
    if (!asignatura.esReconocimientoTransversal)
      currentRow.rowData.calificacion =
        reconocimientos[0].calificacionAsignaturaErp;

    rows.push(currentRow);
    let nombresAsignaturas = '';
    let ectsAsignaturas = '';
    let calificacionesAsignaturas = '';
    reconocimientos.forEach((r) => {
      nombresAsignaturas += `- ${r.nombreAsignatura} [${r.nombreTipoAsignatura}] </br>`;
      ectsAsignaturas += `${r.ects} </br> `;
      calificacionesAsignaturas += `${r.calificacion} </br>`;
    });
    rows.push(
      new DataRow(
        {
          asignatura: nombresAsignaturas,
          ects: ectsAsignaturas,
          calificacion: calificacionesAsignaturas,
          esReconocimiento: true
        },
        asignatura,
        'row-reconocimiento'
      )
    );
  }

  setRowAsignatura(asignatura: AsignaturaExpedienteDto): DataRow {
    return new DataRow(
      {
        asignatura: asignatura.codigoAsignatura
          ? `${asignatura.codigoAsignatura} - ${asignatura.nombreAsignatura}`
          : asignatura.nombreAsignatura,
        ects: asignatura.ects,
        calificacion: asignatura.calificacion,
        anioAcademico:
          asignatura.anyoAcademicoInicio && asignatura.anyoAcademicoFin
            ? `${asignatura.anyoAcademicoInicio}-${asignatura.anyoAcademicoFin}`
            : '',
        curso: asignatura.numeroCurso,
        periodo: asignatura.duracionPeriodo,
        idiomaImparticion: asignatura.simboloIdiomaImparticion
      },
      asignatura
    );
  }

  getCreditosRequeridosObtenidos(tipo: TipoAsignaturaRequerimientoDto): string {
    const creditoRequerido = `${tipo.minCreditos} ${this.labelCreditoRequerido}`;
    const creditoObtenido = `${tipo.creditosAsignaturas} ${this.labelCreditoObtenido}`;
    return tipo.tipoAsignatura.esReconocimiento
      ? creditoObtenido
      : `${creditoRequerido} / ${creditoObtenido}`;
  }
}
