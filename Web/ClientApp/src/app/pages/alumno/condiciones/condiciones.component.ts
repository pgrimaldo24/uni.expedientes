import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { AlumnoInfoDto } from '@pages/expediente-alumno/expediente-models';
import { ConsolidarRequisitoService } from '@pages/expediente-alumno/requisitos-tab/consolidar-requisito-form/consolidar-requisito.service';
import {
  TreeNode,
  TreeNodeOptionSelected
} from '@src/app/component-tools/tree-view/model/tree-node';
import { TreeHandleService } from '@src/app/component-tools/tree-view/tree-handle.service';
import { AppConfigService } from '@src/app/services/app-config.service';
import { SecurityService } from '@src/app/services/security.service';
import { keys } from '@src/keys';
import { Subject, Subscription } from 'rxjs';
import {
  EntityType,
  ViaAccesoPlanAcademicoModel,
  PeriodoAcademicoMatriculaModel,
  CondicionConsolidadaModel,
  DocumentoAlumnoModel,
  ExpedienteAcademicoModel
} from '../alumno-models';
import { AlumnoService } from '../alumno.service';

@Component({
  selector: 'app-condiciones',
  templateUrl: './condiciones.component.html',
  styleUrls: ['./condiciones.component.scss']
})
export class CondicionesComponent implements OnInit, OnDestroy {
  @Input() tree: TreeNode;
  @Input() alumno: AlumnoInfoDto;
  @Input() idExpediente: number;
  unsubscribe$ = new Subject();
  subscription: Subscription;
  private documentoAlumno: DocumentoAlumnoModel;
  private isAdminGestor: boolean;
  private roles = keys;

  constructor(
    private alumnoService: AlumnoService,
    private translateService: TranslateService,
    private treeHandleSvc: TreeHandleService,
    private appConfigService: AppConfigService,
    private consolidarRequisitoService: ConsolidarRequisitoService,
    private security: SecurityService
  ) {
    this.treeHandleSubscription();
    this.translateService.setDefaultLang('es');
  }
  ngOnInit(): void {
    const userRoles = this.security.userRoles();
    this.isAdminGestor =
      userRoles.indexOf(this.roles.ADMIN_ROLE) > -1 ||
      userRoles.indexOf(this.roles.GESTOR_ROLE) > -1;

    this.alumnoService.flatChanched$.subscribe(() => {
      this.loadTree();
    });
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
    this.subscription.unsubscribe();
  }

  loadTree(): void {
    this.tree = new TreeNode(this.alumno.displayName);
    this.tree.nodes = [];
    this.getNodoFoto();
    this.getNodoDocumentosIdentificacion();
    if (this.documentoAlumno) {
      this.getNodoExpedientes();
      this.getNodoOtrosDocumentos();
    } else {
      this.getDocumentosAlumno();
    }
  }

  getNodoFoto(): void {
    if (this.alumno.foto) {
      const foto = new TreeNode('Foto');
      foto.entity = EntityType.FILE;
      foto.opciones = [];
      foto.opciones.push({
        type: 'link',
        entity: 'foto'
      });
      this.tree.nodes.push(foto);
    }
  }

  getNodoDocumentosIdentificacion(): void {
    const documentosIdentificacion = this.newNodo(
      this.translateService.instant(
        'pages.alumno.condiciones.documentosDeIdentificaion'
      ),
      EntityType.FOLDER
    );
    this.alumno.documentosIdentificacion.forEach((documento) => {
      const documentoTreeNode = this.newNodo(
        documento.displayName,
        EntityType.FILE
      );
      documentoTreeNode.opciones = [];
      documentoTreeNode.opciones.push({
        type: 'link',
        entity: 'documentoIdentificacion'
      });
      documentosIdentificacion.nodes.push(documentoTreeNode);
    });
    this.tree.nodes.push(documentosIdentificacion);
  }

  getNodoOtrosDocumentos(): void {
    const documentosAlumno = this.newNodo(
      this.translateService.instant('pages.alumno.condiciones.otrosDocumentos'),
      EntityType.FOLDER
    );
    this.alumno.documentosAlumno.forEach((documento) => {
      const documentoTreeNode = this.newNodo(
        documento.descripcion,
        EntityType.FILE
      );
      documentoTreeNode.opciones = [];
      documentoTreeNode.opciones.push({
        type: 'link',
        entity: 'otrosDocumentos'
      });
      documentosAlumno.nodes.push(documentoTreeNode);
    });
    this.tree.nodes.push(documentosAlumno);
  }

  getDocumentosAlumno(): void {
    this.alumnoService
      .getDocumentosAlumno(this.alumno.idAlumno)
      .subscribe((result) => {
        this.documentoAlumno = result;
        this.getNodoExpedientes();
        this.getNodoOtrosDocumentos();
      });
  }

  getNodoExpedientes(): void {
    const expedientes = this.newNodo(
      this.translateService.instant('pages.alumno.condiciones.expedientes'),
      EntityType.FOLDER
    );

    if (this.idExpediente) {
      this.documentoAlumno.documentosAcademicos = this.documentoAlumno.documentosAcademicos.filter(
        (da) => da.expediente.idIntegracion === this.idExpediente.toString()
      );
    }

    this.documentoAlumno.documentosAcademicos
      .sort(function (a, b) {
        return +a.expediente.idIntegracion - +b.expediente.idIntegracion;
      })
      .forEach((documentoAcademico) => {
        const find = this.alumnoService.idsExpedientes.find(
          (id) => id == +documentoAcademico.expediente.idIntegracion
        );
        if (this.alumnoService.idsExpedientes.length > 0 && !find) return;
        const nodoArray = [];
        nodoArray.push(
          this.getNodoViaAcceso(documentoAcademico.expediente.viaAccesoPlan)
        );
        this.getConsolidadoRequisitosExpediente(
          documentoAcademico.expediente,
          nodoArray
        );
        nodoArray.push(
          this.getNodoMatriculas(documentoAcademico.periodoAcademicoMatriculas)
        );
        expedientes.nodes.push(
          this.newNodo(
            documentoAcademico.expediente.idIntegracion +
              ' - ' +
              documentoAcademico.displayNamePlan,
            EntityType.FOLDER,
            nodoArray
          )
        );
      });
    this.tree.nodes.push(expedientes);
  }

  getNodoMatriculas(
    periodosAcademicosMatriculas: PeriodoAcademicoMatriculaModel[]
  ): TreeNode {
    const matriculas = this.newNodo(
      this.translateService.instant('pages.alumno.condiciones.matriculas'),
      EntityType.FOLDER
    );
    periodosAcademicosMatriculas.forEach((periodoAcademicoMatricula) => {
      const periodoNode = this.newNodo(
        periodoAcademicoMatricula.displayNamePeriodoAcademico,
        EntityType.FOLDER
      );
      periodoAcademicoMatricula.matriculas.forEach((matricula) => {
        const find = this.alumnoService.idsMatriculas.find(
          (id) => id === matricula.idMatricula
        );
        if (this.alumnoService.idsMatriculas.length > 0 && !find) return;
        let nombreNodoMatricula = matricula.displayNameMatricula;
        if (matricula.condicionesConsolidadas.length == 0) {
          nombreNodoMatricula += this.translateService.instant(
            'pages.alumno.condiciones.sinCondiciones'
          );
        }
        const nodoMatricula = this.newNodo(
          nombreNodoMatricula,
          EntityType.FOLDER,
          this.getCondicionesMatriculas(
            matricula.condicionesConsolidadas,
            matricula.idMatricula
          )
        );
        periodoNode.nodes.push(nodoMatricula);
      });
      matriculas.nodes.push(periodoNode);
    });

    return matriculas;
  }

  getCondicionesMatriculas(
    condicionesConsolidadas: CondicionConsolidadaModel[],
    idMatricula: number
  ): TreeNode[] {
    const nodoCondiciones = [];
    if (condicionesConsolidadas) {
      condicionesConsolidadas.forEach((condicion) => {
        const nodo = new TreeNode(
          condicion.condicionMatricula.nombre,
          null,
          null,
          EntityType.FILE,
          condicion
        );
        nodo.opciones = [];
        nodo.opciones.push({
          type: 'link',
          entity: 'matricula',
          value: idMatricula
        });
        nodoCondiciones.push(nodo);
      });
    }
    return nodoCondiciones;
  }

  getNodoViaAcceso(
    viaAccesoPlanAcademicoModel: ViaAccesoPlanAcademicoModel
  ): TreeNode {
    return new TreeNode(
      this.translateService.instant('pages.alumno.condiciones.viaDeAcceso'),
      null,
      [
        this.newNodo(
          viaAccesoPlanAcademicoModel.viaAcceso
            .displayNameClasificacionSuperViaAcceso,
          EntityType.FILE
        )
      ],
      EntityType.FOLDER
    );
  }

  getConsolidadoRequisitosExpediente(
    expediente: ExpedienteAcademicoModel,
    nodoArray: TreeNode[]
  ): void {
    const expedienteAlumno = this.alumno.expedientes?.find(
      (e) => e.id == +expediente.idIntegracion
    );
    if (expedienteAlumno) {
      expedienteAlumno.consolidacionesRequisitosExpedientes.forEach(
        (condicion) => {
          const nodo = new TreeNode(
            condicion.requisitoExpediente.nombre,
            null,
            null,
            EntityType.FILE,
            condicion
          );
          nodo.id = condicion.id.toString();
          nodo.opciones = [];
          nodo.opciones.push({
            type: '',
            entity: 'requisito',
            value: condicion.requisitoExpediente.id
          });
          nodoArray.push(nodo);
        }
      );
    }
  }

  newNodo(
    nameNodo: string,
    entityType: EntityType,
    nodo: TreeNode[] = []
  ): TreeNode {
    return new TreeNode(nameNodo, null, nodo, entityType);
  }

  getNodeIcon(node: TreeNode): string {
    let icon = 'icon fas fa-lg fa-folder';
    if (node?.entity === EntityType.FILE) {
      icon = 'far fa-lg fa-file-alt';
    }
    if (node?.entity === EntityType.FOLDER) {
      icon = 'icon fas fa-lg fa-folder';
      if (node?.showNodes) {
        icon = 'icon fas fa-lg fa-folder-open';
      }
    }
    return icon;
  }

  redirectToErpAcademico(idAlumno: number, url: string): void {
    const config = this.appConfigService.getConfig();
    window.open(`${config.urlErpAcademico}${url}${idAlumno}`, '_blank');
  }

  treeHandleSubscription(): void {
    this.subscription = this.treeHandleSvc
      .getOptions()
      .subscribe((option: TreeNodeOptionSelected) => {
        switch (option.option.entity) {
          case 'documentoIdentificacion':
          case 'foto':
            this.redirectToErpAcademico(
              this.alumno.idAlumno,
              `/Alumno/${this.isAdminGestor ? 'Edit' : 'Show'}/`
            );
            break;
          case 'otrosDocumentos':
            this.redirectToErpAcademico(
              this.alumno.idAlumno,
              '/Alumno/ShowDocumentosCondiciones/'
            );
            break;
          case 'matricula':
            this.redirectToErpAcademico(
              option.option.value,
              `/Matricula/${this.isAdminGestor ? 'Edit' : 'Show'}/`
            );
        }
      });

    this.treeHandleSvc.selectedNode().subscribe((value) => {
      if (value) {
        this.consolidarRequisitoService.loadConsolidacion(value.data);
      }
    });
  }
}
