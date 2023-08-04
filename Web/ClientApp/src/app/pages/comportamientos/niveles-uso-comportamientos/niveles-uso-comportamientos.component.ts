import {
  Component,
  OnInit,
  ViewChild,
  Input,
  Output,
  EventEmitter,
  OnDestroy
} from '@angular/core';
import {
  EntityType,
  NivelUsoComportamientoExpedienteDto,
  TipoNivelUsoDto,
  CreateNivelUsoComportamientoExpedienteDto,
  ComportamientoExpedienteDto,
  GetNivelUsoComportamientoExpedienteDto
} from '../comportamientos.models';
import { NivelesUsoPopupAddComponent } from '@pages/niveles-uso/popup-add/niveles-uso-popup-add.component';
import { CreateNivelUsoPayload } from '../../niveles-uso/niveles-uso.models';
import { BlockUIService } from 'ng-block-ui';
import { Guid } from 'guid-typescript';
import { ComportamientosService } from '../comportamientos.service';
import { AlertHandlerService } from '@src/app/component-tools/alert/alert-handler.service';
import { TranslateService } from '@ngx-translate/core';
import {
  TreeNode,
  TreeNodeOptionSelected
} from '@src/app/component-tools/tree-view/model/tree-node';
import { TreeHandleService } from '@tools/tree-view/tree-handle.service';
import { TreeViewComponent } from '@tools/tree-view/tree-view.component';
import { Subscription } from 'rxjs';
import { ConfirmationModalComponent } from '@src/app/component-tools/confirmation-modal/confirmation-modal/confirmation-modal.component';

@Component({
  selector: 'app-niveles-uso-comportamientos',
  templateUrl: './niveles-uso-comportamientos.component.html',
  styleUrls: ['./niveles-uso-comportamientos.component.scss']
})
export class NivelesUsoComportamientosComponent implements OnInit, OnDestroy {
  @Input() comportamiento?: ComportamientoExpedienteDto;
  @ViewChild('modalNivelUso')
  modalNivelUso: NivelesUsoPopupAddComponent;
  public blockIdentity = Guid.create().toString();
  nivelesUso: NivelUsoComportamientoExpedienteDto[] = [];
  @Input() tree: TreeNode;
  subscription: Subscription;
  @ViewChild('confirmModalNivelUso', { static: false })
  confirmModal: ConfirmationModalComponent;
  @Output() onChange = new EventEmitter<
    NivelUsoComportamientoExpedienteDto[]
  >();
  @ViewChild('treeViewnivelesUso', { static: false })
  treeViewnivelesUso: TreeViewComponent;

  constructor(
    private blockUI: BlockUIService,
    private comportamientoService: ComportamientosService,
    private alertService: AlertHandlerService,
    private translateService: TranslateService,
    private treeHandleSvc: TreeHandleService
  ) {
    this.treeHandleSubscription();
  }

  ngOnInit(): void {}

  addNivelUso(): void {
    this.modalNivelUso.addNivelUso();
  }

  saveNivelUso(payload: CreateNivelUsoPayload): void {
    const nivelUsoNew = {
      idRefUniversidad: payload.idRefUniversidad,
      idRefTipoEstudio: payload.idRefTipoEstudio,
      idRefEstudio: payload.idRefEstudio,
      idRefPlan: payload.idRefPlanEstudio,
      idRefTipoAsignatura: payload.idRefTipoAsignatura,
      idRefAsignaturaPlan: payload.idRefAsignatura,
      acronimoUniversidad: payload.acronimoUniversidad,
      nombreTipoEstudio: payload.nombreTipoEstudio,
      nombreEstudio: payload.nombreEstudio,
      nombrePlan: payload.nombrePlan,
      nombreTipoAsignatura: payload.nombreTipoAsignatura,
      nombreAsignatura: payload.nombreAsignatura,
      tipoNivelUso: {
        id: payload.idTipoNivelUso,
        nombre: payload.nombreNivelUso,
        esUniversidad: payload.esUniversidad,
        esTipoEstudio: payload.esTipoEstudio,
        esEstudio: payload.esEstudio,
        esPlanEstudio: payload.esPlanEstudio,
        esTipoAsignatura: payload.esTipoAsignatura,
        esAsignaturaPlan: payload.esAsignaturaPlan
      } as TipoNivelUsoDto
    } as NivelUsoComportamientoExpedienteDto;

    if (this.comportamiento) {
      const createNivelUsoDto = new CreateNivelUsoComportamientoExpedienteDto();
      createNivelUsoDto.idComportamiento = this.comportamiento.id;
      createNivelUsoDto.nivelUsoComportamientoExpediente = nivelUsoNew;
      this.blockUI.start(this.blockIdentity);
      this.comportamientoService
        .createNivelUsoComportamiento(createNivelUsoDto)
        .subscribe(() => {
          this.blockUI.stop(this.blockIdentity);
          this.alertService.success(
            this.translateService.instant('messages.success')
          );
          this.onChange.emit(this.nivelesUso);
        });
    } else {
      this.getNivelUsoDisplayNameComportamiento(nivelUsoNew);
    }
  }

  update(comportamiento: ComportamientoExpedienteDto): void {
    this.comportamiento = comportamiento;
    this.nivelesUso = this.comportamiento.nivelesUsoComportamientosExpedientes;
    this.loadNivelesUso();
  }

  block(): void {
    this.blockUI.start(this.blockIdentity);
  }
  unblock(): void {
    this.blockUI.stop(this.blockIdentity);
  }

  loadNivelesUso(): void {
    this.tree =
      this.tree ??
      new TreeNode(
        this.translateService.instant(
          'pages.formularioComportamientos.nivelesUso'
        )
      );
    this.tree.nodes = [];
    this.nivelesUso = this.nivelesUso.map((nu, index) => {
      nu.index = index;
      return nu;
    });
    this.loadNodeNivelUso(
      this.nivelesUso.filter((nu) => nu.tipoNivelUso.esUniversidad)
    );
    this.loadNodeNivelUso(
      this.nivelesUso.filter((nu) => nu.tipoNivelUso.esTipoEstudio)
    );
    this.loadNodeNivelUso(
      this.nivelesUso.filter((nu) => nu.tipoNivelUso.esEstudio)
    );
    this.loadNodeNivelUso(
      this.nivelesUso.filter((nu) => nu.tipoNivelUso.esPlanEstudio)
    );
    this.loadNodeNivelUso(
      this.nivelesUso.filter((nu) => nu.tipoNivelUso.esTipoAsignatura)
    );
    this.loadNodeNivelUso(
      this.nivelesUso.filter((nu) => nu.tipoNivelUso.esAsignaturaPlan)
    );
  }

  loadNodeNivelUso(nivelesUso: NivelUsoComportamientoExpedienteDto[]): void {
    if (nivelesUso.length == 0) return;
    const nivelRoot = this.newNodo(
      nivelesUso[0].tipoNivelUso.nombre,
      EntityType.FOLDER
    );
    nivelesUso.forEach((nivelUso: NivelUsoComportamientoExpedienteDto) => {
      const nivelChild = this.newNodo(nivelUso.displayName, EntityType.FILE);
      nivelChild.id = nivelUso.index.toString();
      nivelChild.opciones = [{ entity: 'nivelesUso', type: 'delete' }];
      nivelRoot.nodes.push(nivelChild);
    });
    this.tree.nodes.push(nivelRoot);
  }

  getNivelUsoDisplayNameComportamiento(
    nivelUso: NivelUsoComportamientoExpedienteDto
  ): void {
    this.blockUI.stop(this.blockIdentity);
    const getNivelUsoComportamientoExpedienteDto = {
      nivelUsoComportamientoExpediente: nivelUso
    } as GetNivelUsoComportamientoExpedienteDto;
    this.comportamientoService
      .getNivelUsoDisplayNameComportamiento(
        getNivelUsoComportamientoExpedienteDto
      )
      .subscribe((value) => {
        nivelUso.displayName = value;
        this.blockUI.stop(this.blockIdentity);
        this.nivelesUso.push(nivelUso);
        this.loadNivelesUso();
        this.onChange.emit(this.nivelesUso);
      });
  }

  newNodo(
    nameNodo: string,
    entityType: EntityType,
    nodo: TreeNode[] = []
  ): TreeNode {
    return new TreeNode(nameNodo, null, nodo, entityType);
  }

  removeNivelUso(idNivelUso: number): void {
    this.nivelesUso.splice(idNivelUso, 1);
    this.loadNivelesUso();
    this.onChange.emit(this.nivelesUso);
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

  treeHandleSubscription(): void {
    this.subscription = this.treeHandleSvc
      .getOptions()
      .subscribe((option: TreeNodeOptionSelected) => {
        switch (option.option.entity) {
          case 'nivelesUso':
            switch (option.option.type) {
              case 'delete':
                this.openConfirmDeleteNivelUso(option.id);
                break;
            }
            break;
        }
      });
  }

  openConfirmDeleteNivelUso(idxNivelUso: number): void {
    if (this.nivelesUso.length == 1) {
      this.alertService.error(
        this.translateService.instant(
          'pages.formularioComportamientos.validations.NivelUsoatLeastOneExist'
        )
      );
      setTimeout(() => {
        this.treeViewnivelesUso.clearSelected();
      });
      return;
    }
    this.confirmModal.show(
      () => {
        if (this.comportamiento) {
          this.blockUI.start(this.blockIdentity);
          this.comportamientoService
            .deleteNivelUsoComportamiento(this.nivelesUso[idxNivelUso].id)
            .subscribe(() => {
              this.blockUI.stop(this.blockIdentity);
              this.alertService.success(
                this.translateService.instant('messages.success')
              );
              this.removeNivelUso(idxNivelUso);
            });
        } else {
          this.removeNivelUso(idxNivelUso);
          this.blockUI.stop(this.blockIdentity);
        }
      },
      this.translateService.instant(
        'pages.formularioComportamientos.deleteNivelUsoComportamientoTitle'
      ),
      this.translateService.instant(
        'pages.formularioComportamientos.deleteNivelUsoComportamientoMessage'
      ),
      this.translateService.instant('common.aceptar')
    );
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
