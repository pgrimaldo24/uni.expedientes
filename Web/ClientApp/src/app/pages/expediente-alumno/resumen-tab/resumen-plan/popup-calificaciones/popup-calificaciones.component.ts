import { Component, ElementRef, Input, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
import {
  ArcoSalienteDto,
  AsignaturaExpedienteDto,
  AsignaturaReconocimientoDto,
  BloqueArcoDto,
  IdTipoAsignaturaReconocimiento,
  TipoAsignaturaRequerimientoDto
} from '@pages/expediente-alumno/calificaciones-tab/calificaciones-model';
import {
  DataColumn,
  DataRow
} from '@src/app/component-tools/data-table/models';
import { AsignaturaDto, TipoAsignaturaDto } from '../../resumen.models';
import { IECONodeData } from '../resumen-plan-grafo.models';

@Component({
  selector: 'app-popup-calificaciones',
  templateUrl: './popup-calificaciones.component.html',
  styleUrls: ['./popup-calificaciones.component.scss']
})
export class PopupCalificacionesComponent {
  @Input() resumenForm: FormGroup;
  @Input() reconocimientos: AsignaturaReconocimientoDto[];
  @ViewChild('content') modalContent: ElementRef;
  public asignaturasExpediente: AsignaturaExpedienteDto[];
  public arcoSeleccionado: ArcoSalienteDto;
  public titulo: string;
  public columns: DataColumn[];
  private labelCreditoRequerido: string;
  private labelCreditoObtenido: string;
  private asignaturasReconocimientos: AsignaturaReconocimientoDto[];
  private reconocimientosTransversal: AsignaturaReconocimientoDto[];
  private reconocimientosSeminario: AsignaturaReconocimientoDto[];
  private reconocimientosUniversitaria: AsignaturaReconocimientoDto[];
  bloqueReconocimiento: BloqueArcoDto;

  constructor(
    private modalService: NgbModal,
    private translateService: TranslateService
  ) {}

  ngOnInit(): void {
    this.initializeTableHeaders();
    this.setReconocimientos();
  }

  loadModal(
    data: IECONodeData,
    dataParent: IECONodeData,
    asignaturasExpediente: AsignaturaExpedienteDto[]
  ): void {
    this.modalService.open(this.modalContent, {
      ariaLabelledBy: 'modal-basic-title',
      backdrop: 'static',
      centered: true,
      size: 'xl'
    });
    this.titulo = data.nombre;
    this.loadAsignaturas(data, dataParent, asignaturasExpediente);
  }

  loadAsignaturas(
    data: IECONodeData,
    dataParent: IECONodeData,
    asignaturasExpediente: AsignaturaExpedienteDto[]
  ): ArcoSalienteDto {
    this.asignaturasExpediente = asignaturasExpediente;
    let arcos = data.arcosSalientes;
    let esNodoFinal = data.esNodoFinal;
    if (data.tipo.esFinal || dataParent?.arcosSalientes.length > 1) {
      arcos = dataParent.arcosSalientes.filter(
        (a) => a.nodoDestino.id == data.id
      );
      esNodoFinal = dataParent.esNodoFinal;
    }

    this.arcoSeleccionado = {
      creditosRequeridos: 0,
      creditosObtenidos: 0,
      anyAsignaturaMatriculada: false,
      bloques: []
    };
    arcos.forEach((arco) => {
      arco.bloques.forEach((bloque, index) => {
        this.arcoSeleccionado.bloques.push({
          nombre: bloque.nombre,
          asignaturas: bloque.asignaturas,
          tiposAsignaturas: [],
          subBloques: []
        });
        const tiposAsignaturasBloques: TipoAsignaturaDto[] = [];
        const tiposAsignaturasSubBloques: TipoAsignaturaDto[] = [];
        bloque.asignaturas.forEach((asignatura) => {
          const tipoAsignatura = tiposAsignaturasBloques.find(
            (ta) => ta.id == asignatura.tipoAsignatura.id
          );
          if (!tipoAsignatura) {
            tiposAsignaturasBloques.push(asignatura.tipoAsignatura);
            this.arcoSeleccionado.bloques[index].tiposAsignaturas.push({
              minCreditos: bloque.minCreditos,
              tipoAsignatura: asignatura.tipoAsignatura
            });
          }
        });
        bloque.subBloques.forEach((sb, sbIndex) => {
          this.arcoSeleccionado.bloques[index].subBloques.push({
            nombre: sb.nombre,
            asignaturas: sb.asignaturas,
            tiposAsignaturas: [],
            subBloques: []
          });
          sb.asignaturas.forEach((asignatura) => {
            const tipoAsignatura = tiposAsignaturasSubBloques.find(
              (ta) => ta.id == asignatura.tipoAsignatura.id
            );
            if (!tipoAsignatura) {
              tiposAsignaturasSubBloques.push(asignatura.tipoAsignatura);
              this.arcoSeleccionado.bloques[index].subBloques[
                sbIndex
              ].tiposAsignaturas.push({
                minCreditos: sb.minCreditos,
                tipoAsignatura: asignatura.tipoAsignatura
              });
            }
          });
        });
      });
    });
    this.asignarAsignaturasEnTipoAsignatura(esNodoFinal);
    return this.arcoSeleccionado;
  }

  closeModal(modal: NgbModalRef): void {
    this.arcoSeleccionado = null;
    modal.close();
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

  asignarAsignaturasEnTipoAsignatura(esArcoFinal: boolean): void {
    let anyAsignaturaMatriculada = false;
    if (esArcoFinal && this.bloqueReconocimiento) {
      this.arcoSeleccionado.bloques.push(this.bloqueReconocimiento);
    }
    if (!this.arcoSeleccionado.bloques.length) return;

    this.arcoSeleccionado.bloques.forEach((bloque) => {
      bloque.tiposAsignaturas.forEach((tipoAsignatura) => {
        this.setBloquesAndSubBloques(tipoAsignatura, bloque.asignaturas);
        const exist = bloque.asignaturas.some((a) => a.found);
        if (!anyAsignaturaMatriculada && exist) {
          anyAsignaturaMatriculada = true;
        }
      });
      bloque.subBloques.forEach((sb) => {
        sb.tiposAsignaturas.forEach((sbta) => {
          this.setBloquesAndSubBloques(sbta, sb.asignaturas);
          const exist = bloque.asignaturas.some((a) => a.found);
          if (!anyAsignaturaMatriculada && exist) {
            anyAsignaturaMatriculada = true;
          }
        });
      });
    });
    this.arcoSeleccionado.anyAsignaturaMatriculada = anyAsignaturaMatriculada;
  }

  setBloquesAndSubBloques(
    tipoAsignaturaRequerimiento: TipoAsignaturaRequerimientoDto,
    asignaturasTrayecto: AsignaturaDto[]
  ): void {
    tipoAsignaturaRequerimiento.creditosAsignaturas = 0;
    const tipoAsignatura = tipoAsignaturaRequerimiento.tipoAsignatura;
    const asignaturasReconocimientos = tipoAsignatura.esReconocimiento
      ? this.getAsignaturasReconocimientosAMostrar(tipoAsignatura.id)
      : [];

    const asignaturasErp = asignaturasTrayecto.filter(
      (a) => a.tipoAsignatura.id == tipoAsignatura.id
    );
    if (
      asignaturasErp?.length ||
      (tipoAsignatura.esReconocimiento && asignaturasReconocimientos.length)
    ) {
      const asignaturas = tipoAsignatura.esReconocimiento
        ? asignaturasReconocimientos
        : this.getAsignaturasAMostrar(asignaturasErp);
      tipoAsignaturaRequerimiento.creditosAsignaturas = asignaturas.reduce(
        (acc, item) => acc + (item.notFound ? 0 : item.ects ?? 0),
        0
      );
      tipoAsignaturaRequerimiento.rowsBloques = this.getAsignaturasDataRow(
        asignaturas
      );
      if (tipoAsignatura.esReconocimiento) return;

      this.arcoSeleccionado.creditosObtenidos +=
        tipoAsignaturaRequerimiento.creditosAsignaturas;
      this.arcoSeleccionado.creditosRequeridos +=
        tipoAsignaturaRequerimiento.minCreditos;
    }
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
        item.found = true;
        asignaturas.push(asignatura);
      } else {
        item.found = false;
        asignaturas.push(this.setAsignaturaExpediente(item));
      }
    });
    return asignaturas;
  }

  setAsignaturaExpediente(asignatura: AsignaturaDto): AsignaturaExpedienteDto {
    const asignaturaExpediente: AsignaturaExpedienteDto = {
      id: asignatura.id,
      idRefAsignaturaPlan: asignatura.idAsignaturaPlan.toString(),
      nombreAsignatura: asignatura.nombre,
      codigoAsignatura: asignatura.codigo,
      idRefTipoAsignatura: asignatura.tipoAsignatura.id.toString(),
      ects: asignatura.creditos,
      notFound: true
    };
    return asignaturaExpediente;
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

      if (reconocimientos?.length) {
        if (!asignatura.esReconocimientoTransversal)
          row.rowData.calificacion =
            reconocimientos[0].calificacionAsignaturaErp;

        rows.push(row);
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
    });
    return rows;
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
      asignatura,
      asignatura.notFound ? 'not-found' : ''
    );
  }

  getCreditosRequeridosObtenidos(tipo: TipoAsignaturaRequerimientoDto): string {
    const creditoRequerido = `${tipo.minCreditos} ${this.labelCreditoRequerido}`;
    const creditoObtenido = `${tipo.creditosAsignaturas} ${this.labelCreditoObtenido}`;
    return tipo.tipoAsignatura.esReconocimiento
      ? creditoObtenido
      : `${creditoRequerido} / ${creditoObtenido}`;
  }

  setReconocimientos(): void {
    if (!this.reconocimientos?.length) return;
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

    if (
      !this.reconocimientosTransversal?.length &&
      !this.reconocimientosSeminario?.length &&
      !this.reconocimientosUniversitaria?.length
    ) {
      return;
    }

    this.bloqueReconocimiento = {
      nombre: this.translateService.instant(
        'pages.expedienteTabs.calificaciones.reconocimientoConOrigen'
      ),
      asignaturas: [],
      tiposAsignaturas: [],
      subBloques: []
    };
    if (this.reconocimientosTransversal?.length) {
      this.setTiposAsignaturasReconocimientos(
        this.reconocimientosTransversal[0],
        this.bloqueReconocimiento.tiposAsignaturas
      );
    }

    if (this.reconocimientosSeminario?.length) {
      this.setTiposAsignaturasReconocimientos(
        this.reconocimientosSeminario[0],
        this.bloqueReconocimiento.tiposAsignaturas
      );
    }

    if (this.reconocimientosUniversitaria?.length) {
      this.setTiposAsignaturasReconocimientos(
        this.reconocimientosUniversitaria[0],
        this.bloqueReconocimiento.tiposAsignaturas
      );
    }
  }

  setTiposAsignaturasReconocimientos(
    reconocimiento: AsignaturaReconocimientoDto,
    tiposAsignaturasRequerimiento: TipoAsignaturaRequerimientoDto[]
  ): void {
    tiposAsignaturasRequerimiento.push({
      minCreditos: 0,
      tipoAsignatura: {
        id: +reconocimiento.idRefTipoAsignatura,
        nombre: reconocimiento.nombreTipoAsignatura,
        esReconocimiento: true
      }
    });
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
}
