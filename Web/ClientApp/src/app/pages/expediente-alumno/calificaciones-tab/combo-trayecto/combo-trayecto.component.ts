import { Component, Input, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import {
  ExpedienteAlumnoDto,
  ExpedienteEspecializacionDto
} from '@pages/expediente-alumno/expediente-models';
import { ExpedienteService } from '@pages/expediente-alumno/expediente.service';
import {
  AsignaturaDto,
  GrafoDto,
  NodoDto,
  NodoTrayectoDto
} from '@pages/expediente-alumno/resumen-tab/resumen.models';
import { ComboboxItem } from '@src/app/component-tools/combobox/models';
import { Guid } from 'guid-typescript';
import { BlockUIService } from 'ng-block-ui';
import {
  AsignaturaExpedienteDto,
  FormCalificacionesControls,
  RequerimientoPlanDto,
  RequestRequerimientoPlan,
  TrayectoPlanDto,
  TrayectoSeleccionado
} from '../calificaciones-model';
import { CalificacionesService } from '../calificaciones.service';

@Component({
  selector: 'app-combo-trayecto',
  templateUrl: './combo-trayecto.component.html',
  styleUrls: ['./combo-trayecto.component.scss']
})
export class ComboTrayectoComponent implements OnInit {
  @Input() esResumen: boolean;
  @Input() grafoPlan: GrafoDto;
  @Input() expedienteAlumno: ExpedienteAlumnoDto;
  @Input() calificacionesForm: FormGroup;
  public requerimientosPlan: RequerimientoPlanDto[];
  public requerimientoPlan: RequerimientoPlanDto;
  public asignaturasExpediente: AsignaturaExpedienteDto[];
  public asignaturasTrayecto: AsignaturaDto[];
  public comboTrayectos: ComboboxItem[] = [];
  private trayectos: TrayectoPlanDto[] = [];
  public blockIdentity = Guid.create().toString();
  public formControls = FormCalificacionesControls;
  private nodeFound: boolean;
  private idsNodosDestino: number[] = [];
  private nodosTrayectos: NodoTrayectoDto[] = [];
  private idsNodosTrayectos: number[] = [];
  indexRequerimientos = 1;
  countRequerimientos = 20;
  constructor(
    private blockUIService: BlockUIService,
    private expedienteService: ExpedienteService,
    private calificacionesService: CalificacionesService
  ) {}

  ngOnInit(): void {
    this.getRequerimientosPlan();
  }

  getRequerimientosPlan(): void {
    const request = new RequestRequerimientoPlan();
    request.filterIdPlan = +this.expedienteAlumno.idRefPlan;
    request.index = this.indexRequerimientos;
    request.count = this.countRequerimientos;
    this.blockUIService.start(this.blockIdentity);
    this.calificacionesService
      .getRequerimientosPlan(request)
      .subscribe((requerimientos) => {
        this.requerimientosPlan = requerimientos;
        this.getEspecializacionesExpediente();
        this.getTrayectos();
      });
  }

  getTrayectos(): void {
    const comboboxItems: ComboboxItem[] = [];
    this.requerimientosPlan.forEach((requerimiento) => {
      requerimiento.trayectosPlanes.forEach((trayecto) => {
        if (
          this.grafoPlan?.esNodoInicial &&
          this.grafoPlan?.nodos[0]?.id != trayecto.nodoInicial.id
        )
          return;

        this.trayectos.push(trayecto);
        comboboxItems.push({
          id: trayecto.nodoFinal.id.toString(),
          text: trayecto.nodoFinal.nombreNodo,
          value: trayecto.nodoFinal.id
        });
      });
    });
    this.comboTrayectos = comboboxItems;
  }

  getEspecializacionesExpediente(): void {
    this.expedienteService
      .getEspecializacionesByIdExpediente(this.expedienteAlumno.id)
      .subscribe((especializaciones) => {
        this.getAsignaturasExpediente(especializaciones);
      });
  }

  getAsignaturasExpediente(
    especializaciones: ExpedienteEspecializacionDto[]
  ): void {
    this.calificacionesService
      .getAsignaturasByIdExpediente(this.expedienteAlumno.id)
      .subscribe((asignaturas) => {
        this.asignaturasExpediente = asignaturas;
        this.asignarTrayectoAlCombobox(especializaciones);
        this.blockUIService.stop(this.blockIdentity);
      });
  }

  asignarTrayectoAlCombobox(
    especializaciones: ExpedienteEspecializacionDto[]
  ): void {
    let trayecto: TrayectoPlanDto;
    if (especializaciones.length) {
      especializaciones.forEach((item) => {
        trayecto = this.trayectos.find((tp) =>
          tp.nodoFinal.hitos.find(
            (h) =>
              h.hitoEspecializacion &&
              h.hitoEspecializacion.especializacion.id ==
                item.idRefEspecializacion
          )
        );
        if (trayecto) return;
      });
    }

    if (!trayecto) {
      trayecto = this.trayectos.find((rp) => rp.esGenerico);
      if (!trayecto) return;
    }

    this.calificacionesForm.patchValue({
      [FormCalificacionesControls.trayecto]: {
        value: trayecto.nodoFinal.id,
        text: trayecto.nodoFinal.nombreNodo,
        data: trayecto
      }
    });

    this.requerimientoPlan = this.getRequerimiento(+trayecto.nodoFinal.id);
    this.asignaturasTrayecto = this.getAsignaturasTrayecto(
      +trayecto.nodoFinal.id
    );
    if (this.esResumen)
      this.calificacionesService.changeTrayectoResumen(
        this.setTrayectoSeleccionado()
      );
    else
      this.calificacionesService.changeTrayectoCalificaciones(
        this.setTrayectoSeleccionado()
      );
  }

  onChangeTrayecto(): void {
    const trayecto = this.calificacionesForm.get(
      FormCalificacionesControls.trayecto
    );
    this.idsNodosTrayectos = [];
    this.nodosTrayectos = [];
    this.idsNodosDestino = [];
    this.requerimientoPlan = trayecto.value
      ? this.getRequerimiento(+trayecto.value.id)
      : null;
    this.asignaturasTrayecto = trayecto.value
      ? this.getAsignaturasTrayecto(+trayecto.value.id)
      : [];
    if (this.esResumen)
      this.calificacionesService.changeTrayectoResumen(
        this.setTrayectoSeleccionado()
      );
    else
      this.calificacionesService.changeTrayectoCalificaciones(
        this.setTrayectoSeleccionado()
      );
  }

  getRequerimiento(idNodoFinal: number): RequerimientoPlanDto {
    return this.requerimientosPlan.find((rp) =>
      rp.trayectosPlanes.find((tp) => tp.nodoFinal.id === idNodoFinal)
    );
  }

  getAsignaturasTrayecto(idNodoFinal: number): AsignaturaDto[] {
    this.nodeFound = false;
    let asignaturasErp: AsignaturaDto[] = [];
    this.grafoPlan.nodos.forEach((nodo) => {
      if (this.nodeFound) return false;
      asignaturasErp = [];
      this.idsNodosDestino = [];
      this.idsNodosDestino.push(nodo.id);
      const nodoTrayecto: NodoTrayectoDto = {
        id: nodo.id,
        nodoDestino: []
      };
      nodo.arcosSalientes.forEach((as) => {
        as.bloques.forEach((b) => {
          b.asignaturas.map((a) => (a.idNodoDestino = as.nodoDestino.id));
          asignaturasErp.push(...b.asignaturas);
          b.subBloques.forEach((sb) => {
            sb.asignaturas.map((a) => (a.idNodoDestino = as.nodoDestino.id));
            asignaturasErp.push(...sb.asignaturas);
          });
        });
        nodoTrayecto.nodoDestino.push(as.nodoDestino);
        this.idsNodosDestino.push(as.nodoDestino.id);
      });
      this.nodosTrayectos.push(nodoTrayecto);
      asignaturasErp.push(...this.setAsignaturasErp(nodo.hijos, idNodoFinal));
    });
    const idsNodosFinales = this.trayectos.map((t) => t.nodoFinal.id);
    this.idsNodosDestino = this.idsNodosDestino.filter(
      (id) => !idsNodosFinales.includes(id)
    );
    this.idsNodosDestino.push(idNodoFinal);
    const asignaturasTrayecto: AsignaturaDto[] = [];
    this.idsNodosDestino.forEach((id) => {
      const asignaturas = asignaturasErp.filter((a) => a.idNodoDestino === id);
      asignaturasTrayecto.push(...asignaturas);
    });
    this.nodosTrayectos = this.nodosTrayectos.filter(
      (nt) => nt.nodoDestino.length > 0
    );
    this.setCurrentNodosTrayectos(idNodoFinal);
    this.idsNodosTrayectos.push(idNodoFinal);
    return asignaturasTrayecto;
  }

  setAsignaturasErp(nodos: NodoDto[], idNodoFinal: number): AsignaturaDto[] {
    const asignaturasErp: AsignaturaDto[] = [];
    nodos.forEach((nodo) => {
      const nodoTrayecto: NodoTrayectoDto = {
        id: nodo.id,
        nodoDestino: []
      };
      nodo.arcosSalientes.forEach((as) => {
        as.bloques.forEach((b) => {
          b.asignaturas.map((a) => (a.idNodoDestino = as.nodoDestino.id));
          asignaturasErp.push(...b.asignaturas);
          b.subBloques.forEach((sb) => {
            sb.asignaturas.map((a) => (a.idNodoDestino = as.nodoDestino.id));
            asignaturasErp.push(...sb.asignaturas);
          });
        });
        nodoTrayecto.nodoDestino.push(as.nodoDestino);
        this.idsNodosDestino.push(as.nodoDestino.id);
      });
      if (idNodoFinal === nodo.id) {
        this.nodeFound = true;
        return false;
      }
      this.nodosTrayectos.push(nodoTrayecto);
      asignaturasErp.push(...this.setAsignaturasErp(nodo.hijos, idNodoFinal));
    });
    return asignaturasErp;
  }

  setTrayectoSeleccionado(): TrayectoSeleccionado {
    const trayecto: TrayectoSeleccionado = {
      requerimientoPlan: this.requerimientoPlan,
      asignaturasTrayecto: this.asignaturasTrayecto,
      asignaturasExpediente: this.asignaturasExpediente,
      idsNodosTrayectos: this.idsNodosTrayectos
    };
    return trayecto;
  }

  setCurrentNodosTrayectos(idNodoFinal: number): void {
    const nodosTrayectos = this.nodosTrayectos.filter((nt) =>
      nt.nodoDestino.find((nd) => nd.id == idNodoFinal)
    );
    if (nodosTrayectos.length) {
      nodosTrayectos.forEach((nt) => {
        if (!this.idsNodosTrayectos.find((id) => id == nt.id)) {
          this.idsNodosTrayectos.push(nt.id);
        }
        this.setCurrentNodosTrayectos(nt.id);
      });
    }
  }
}
