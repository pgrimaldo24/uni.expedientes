import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ExpedienteAlumnoDto } from '@pages/expediente-alumno/expediente-models';
import { AsignaturaDto, GrafoDto } from '../resumen.models';
import { CalificacionesService } from '@pages/expediente-alumno/calificaciones-tab/calificaciones.service';
import {
  AsignaturaExpedienteDto,
  AsignaturaReconocimientoDto,
  RequerimientoPlanDto
} from '@pages/expediente-alumno/calificaciones-tab/calificaciones-model';
import {
  DataColumn,
  DataRow
} from '@src/app/component-tools/data-table/models';
import { TranslateService } from '@ngx-translate/core';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-resumen-creditos',
  templateUrl: './resumen-creditos.component.html',
  styleUrls: ['./resumen-creditos.component.scss']
})
export class ResumenCreditosComponent implements OnInit, OnDestroy {
  @Input() expedienteAlumno: ExpedienteAlumnoDto;
  @Input() grafoPlan: GrafoDto;
  @Input() resumenForm: FormGroup;
  @Input() reconocimientos: AsignaturaReconocimientoDto[];
  public requerimientoPlan: RequerimientoPlanDto;
  public asignaturasExpediente: AsignaturaExpedienteDto[];
  public asignaturasTrayecto: AsignaturaDto[];
  public columns: DataColumn[];
  public rows: DataRow[];
  subscription: Subscription;
  idTipoAsignaturaOptativa = 6;
  nombreTipoAsignaturaOptativa = 'Optativa';

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
    this.subscription = this.calificacionesService.changeTrayectoResumen$.subscribe(
      (trayecto) => {
        this.requerimientoPlan = trayecto.requerimientoPlan;
        this.asignaturasTrayecto = trayecto.asignaturasTrayecto;
        this.asignaturasExpediente = trayecto.asignaturasExpediente;
        this.setTipoAsignaturaOptativa();
        this.asignarTiposAsignaturas();
      }
    );
  }

  setTipoAsignaturaOptativa(): void {
    if (!this.requerimientoPlan || !this.reconocimientos?.length) return;

    const existeTipoOptativa = this.requerimientoPlan.tiposAsignaturasRequerimiento.some(
      (tar) => tar.tipoAsignatura.id == this.idTipoAsignaturaOptativa
    );
    if (existeTipoOptativa) return;

    this.requerimientoPlan.tiposAsignaturasRequerimiento.push({
      minCreditos: 0,
      tipoAsignatura: {
        id: this.idTipoAsignaturaOptativa,
        nombre: this.nombreTipoAsignaturaOptativa,
        esReconocimiento: true
      }
    });
  }

  asignarTiposAsignaturas(): void {
    this.rows = [];
    if (
      !this.requerimientoPlan ||
      !this.requerimientoPlan.tiposAsignaturasRequerimiento.length
    )
      return;

    this.requerimientoPlan.tiposAsignaturasRequerimiento.forEach((item) => {
      item.creditosAsignaturas = 0;
      const tipoAsignatura = item.tipoAsignatura;
      let asignaturasReconocimientos: AsignaturaExpedienteDto[] = [];
      if (
        this.idTipoAsignaturaOptativa == tipoAsignatura.id &&
        this.reconocimientos
      ) {
        asignaturasReconocimientos = this.getAsignaturasReconocimientosAMostrar(
          this.reconocimientos
        );
      }

      const asignaturasErp = this.asignaturasTrayecto.filter(
        (a) => a.tipoAsignatura.id == tipoAsignatura.id
      );
      if (asignaturasErp?.length || asignaturasReconocimientos.length) {
        const asignaturas = this.getAsignaturasAMostrar(asignaturasErp);
        asignaturas.push(...asignaturasReconocimientos);
        item.creditosAsignaturas = asignaturas.reduce(
          (acc, item) => acc + (item.ects ?? 0),
          0
        );
      }
      this.rows.push(
        new DataRow(
          {
            tipoAsignatura: tipoAsignatura.nombre,
            requeridos: item.minCreditos,
            obtenidos: item.creditosAsignaturas,
            icono: true
          },
          item
        )
      );
    });
    this.setDataRowTotal();
  }

  setDataRowTotal(): void {
    this.rows.push(
      new DataRow(
        {
          tipoAsignatura: this.translateService.instant(
            'pages.expedienteTabs.resumen.total'
          ),
          requeridos: this.requerimientoPlan.creditosRequeridos,
          obtenidos: this.requerimientoPlan.tiposAsignaturasRequerimiento.reduce(
            (acc, item) => acc + (item.creditosAsignaturas ?? 0),
            0
          ),
          icono: false
        },
        {}
      )
    );
  }

  getAsignaturasAMostrar(
    asignaturasErp: AsignaturaDto[]
  ): AsignaturaExpedienteDto[] {
    const asignaturas: AsignaturaExpedienteDto[] = [];
    asignaturasErp.forEach((item) => {
      const asignatura = this.asignaturasExpediente.find(
        (ae) =>
          +ae.idRefAsignaturaPlan === item.idAsignaturaPlan &&
          +ae.idRefTipoAsignatura === item.tipoAsignatura.id
      );
      if (asignatura) {
        asignaturas.push(asignatura);
      }
    });
    return asignaturas;
  }

  getAsignaturasReconocimientosAMostrar(
    asignaturasReconocimientos: AsignaturaReconocimientoDto[]
  ): AsignaturaExpedienteDto[] {
    const asignaturas: AsignaturaExpedienteDto[] = [];
    const reconocimientos = asignaturasReconocimientos.filter(
      (r) =>
        r.esExtensionUniversitaria || r.esSeminario || r.esTransversalPrincipal
    );
    reconocimientos.forEach((item) => {
      asignaturas.push({
        nombreAsignatura: item.nombreAsignatura,
        ects: item.ects,
        calificacion: item.calificacion
      });
    });
    return asignaturas;
  }

  initializeTableHeaders(): void {
    this.columns = [
      {
        field: 'tipoAsignatura',
        sortable: false,
        header: this.translateService.instant(
          'pages.expedienteTabs.resumen.tipoAsignatura'
        )
      },
      {
        field: 'requeridos',
        sortable: false,
        header: this.translateService.instant(
          'pages.expedienteTabs.resumen.requeridos'
        )
      },
      {
        field: 'obtenidos',
        sortable: false,
        header: this.translateService.instant(
          'pages.expedienteTabs.resumen.obtenidos'
        )
      }
    ];
  }
}
