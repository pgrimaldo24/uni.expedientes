import {
  Component,
  ElementRef,
  Input,
  OnDestroy,
  OnInit,
  ViewChild
} from '@angular/core';
import { FormGroup } from '@angular/forms';
import {
  AsignaturaExpedienteDto,
  ReconocimientoClasificacionDto,
  TrayectoSeleccionado
} from '@pages/expediente-alumno/calificaciones-tab/calificaciones-model';
import { CalificacionesService } from '@pages/expediente-alumno/calificaciones-tab/calificaciones.service';
import { ExpedienteAlumnoDto } from '@pages/expediente-alumno/expediente-models';
import { ExpedienteService } from '@pages/expediente-alumno/expediente.service';
import { Guid } from 'guid-typescript';
import { BlockUIService } from 'ng-block-ui';
import { Subscription } from 'rxjs';
import { GrafoColores, GrafoDto, NodoDto } from '../resumen.models';
import { PopupCalificacionesComponent } from './popup-calificaciones/popup-calificaciones.component';
import { ECOTree, IECONode, IECONodeData } from './resumen-plan-grafo.models';
@Component({
  selector: 'app-resumen-plan',
  templateUrl: './resumen-plan.component.html',
  styleUrls: ['./resumen-plan.component.scss']
})
export class ResumenPlanComponent implements OnInit, OnDestroy {
  @Input() resumenForm: FormGroup;
  @Input() expedienteAlumno: ExpedienteAlumnoDto;
  @Input() grafoPlan: GrafoDto;
  @Input() fullScreen: boolean;
  @Input() reconocimientos: ReconocimientoClasificacionDto;
  @ViewChild('divPlan', { static: true })
  divPlan: ElementRef;
  @ViewChild('modalCalificaciones')
  modalCalificaciones: PopupCalificacionesComponent;
  grafosNodes: ECOTree[] = [];
  parentWidth: number;
  parentHeight: number;
  subscription: Subscription;
  subscriptionTrayecto: Subscription;
  oneSecond: 1000;
  public blockIdentity = Guid.create().toString();
  public asignaturasExpediente: AsignaturaExpedienteDto[];

  constructor(
    private calificacionesService: CalificacionesService,
    private blockUIService: BlockUIService,
    private expedienteService: ExpedienteService
  ) {}

  ngOnInit(): void {
    this.getAsignaturasExpediente();
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
    this.subscriptionTrayecto?.unsubscribe();
  }

  getGrafoPlan(): void {
    if (!this.grafoPlan || !this.grafoPlan.nodos) return;
    this.grafoPlan.nodos.forEach((nodo) => {
      const dataGrafo: IECONode = {
        data: this.setDataNodo(nodo),
        linkColor: GrafoColores.gray,
        background: GrafoColores.green,
        color: GrafoColores.white,
        children: this.setChildren(nodo.hijos)
      };
      const grafoNode: ECOTree = new ECOTree();
      this.addNodes(grafoNode, dataGrafo);
      grafoNode.nDatabaseNodesPath = grafoNode.nDatabaseNodes.filter(
        (n) => n.nodeChildren.length > 0
      );
      grafoNode.UpdateTree();
      this.grafosNodes.push(grafoNode);
    });
    this.parentWidth = this.divPlan.nativeElement.offsetWidth;
    setTimeout(() => {
      this.parentHeight = this.divPlan.nativeElement.offsetHeight;
    }, this.oneSecond);
  }

  setChildren(nodos: NodoDto[]): IECONode[] {
    const childrens: IECONode[] = [];
    nodos.forEach((nodo) => {
      const children: IECONode = {
        data: this.setDataNodo(nodo),
        linkColor: GrafoColores.gray,
        background: GrafoColores.gray,
        color: GrafoColores.white,
        children: this.setChildren(nodo.hijos)
      };
      childrens.push(children);
    });
    return childrens;
  }

  setDataNodo(nodo: NodoDto): IECONodeData {
    const data: IECONodeData = {
      id: nodo.id,
      nombre: nodo.nombre,
      tipo: nodo.tipo,
      arcosSalientes: nodo.arcosSalientes,
      esNodoFinal: nodo.tipo.esFinal || nodo.hijos.some((n) => n.tipo.esFinal)
    };
    return data;
  }

  addNodes(tree: ECOTree, node: IECONode, parent: IECONode = null): void {
    parent = parent || {
      id: -1
    };
    node.color = node.color;
    node.background = node.background;
    node.linkColor = node.linkColor;
    node.id = tree.nDatabaseNodes.length;
    tree.add(
      node.id,
      parent.id,
      node.width,
      node.height,
      node.color,
      node.background,
      node.linkColor,
      node.data
    );
    if (node.children) {
      node.children.forEach((x: IECONode) => {
        this.addNodes(tree, x, node);
      });
    }
  }

  showCalificaciones(data: IECONodeData, dataParent?: IECONodeData): void {
    this.modalCalificaciones.loadModal(
      data,
      dataParent,
      this.asignaturasExpediente
    );
  }

  getAsignaturasExpediente(): void {
    this.blockUIService.start(this.blockIdentity);
    this.calificacionesService
      .getAsignaturasByIdExpediente(this.expedienteAlumno.id)
      .subscribe((asignaturas) => {
        this.asignaturasExpediente = asignaturas;
        this.changeInfoGrafo();
        this.blockUIService.stop(this.blockIdentity);
      });
  }

  changeInfoGrafo(): void {
    this.getGrafoPlan();
    this.subscription = this.expedienteService.hasInfoGrafo$.subscribe(
      (grafo: GrafoDto) => {
        this.grafoPlan = grafo;
        this.getGrafoPlan();
      }
    );

    this.subscriptionTrayecto = this.calificacionesService.changeTrayectoResumen$.subscribe(
      (trayecto: TrayectoSeleccionado) => {
        this.grafosNodes.forEach((g) => {
          g.nDatabaseNodes.forEach((dn) => {
            dn.data.seleccionado = trayecto.idsNodosTrayectos.some(
              (id) => id == dn.data.id
            );
            const arco = this.modalCalificaciones.loadAsignaturas(
              dn.data,
              dn.data.tipo.esFinal ? dn.nodeParent.data : null,
              this.asignaturasExpediente
            );
            dn.data.superado =
              arco.creditosObtenidos > 0 &&
              arco.creditosObtenidos >= arco.creditosRequeridos;
            dn.data.anyAsignaturaMatriculada = arco.anyAsignaturaMatriculada;
            dn.linkColor = dn.data.superado
              ? GrafoColores.green
              : dn.data.anyAsignaturaMatriculada
              ? GrafoColores.blue
              : GrafoColores.gray;
            if (!dn.data.tipo.esInicial) {
              const parentData = dn.nodeParent.data;
              dn.background = parentData.superado
                ? GrafoColores.green
                : parentData.anyAsignaturaMatriculada
                ? GrafoColores.blue
                : dn.data.seleccionado
                ? GrafoColores.orange
                : GrafoColores.gray;
            }
          });
        });
      }
    );
  }
}
