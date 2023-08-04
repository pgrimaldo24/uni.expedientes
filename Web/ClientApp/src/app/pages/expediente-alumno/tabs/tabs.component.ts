import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BlockUIService } from 'ng-block-ui';
import { ExpedienteSection } from '../expediente-section-enum';
import { ExpedienteService } from '../expediente.service';
import { SecurityService } from '@services/security.service';
import { Guid } from '@src/app/component-tools/combobox/models';
import { keys } from '@src/keys';
import { ConfiguracionExpedienteUniversidadDto } from '../requisitos-tab/consolidar-requisito.models';
import {
  DatosErpResponse,
  ExpedienteAlumnoDto,
  ExpedienteDto
} from '../expediente-models';
import { GrafoDto, RequestNodoPlan } from '../resumen-tab/resumen.models';
import { CalificacionesService } from '../calificaciones-tab/calificaciones.service';
import { ReconocimientoClasificacionDto } from '../calificaciones-tab/calificaciones-model';

@Component({
  selector: 'app-tabs',
  templateUrl: './tabs.component.html',
  styleUrls: ['./tabs.component.scss']
})
export class TabsComponent implements OnInit {
  @Input() isReadOnly = false;
  @Input() section = 'expediente';
  @Input() idExpedienteAlumno: number;
  blockIdentity = Guid.create().toString();
  data = null;
  rolesUsuarios = null;
  dataExpedienteAlumno: ExpedienteAlumnoDto;
  configuracionUniversidad: ConfiguracionExpedienteUniversidadDto;
  grafoPlan: GrafoDto;
  reconocimientos: ReconocimientoClasificacionDto;
  public roles = keys;
  public expedienteSection = ExpedienteSection;
  public tabs = null;
  constructor(
    private securityService: SecurityService,
    private activatedRoute: ActivatedRoute,
    private expedienteService: ExpedienteService,
    private route: Router,
    private blockUIService: BlockUIService,
    private calificacionesService: CalificacionesService
  ) {}

  ngOnInit(): void {
    this.loadTabs();
    this.activatedRoute.params.subscribe(({ id }) => {
      if (id === undefined) {
        this.route.navigateByUrl('/');
        return;
      }
      this.getExpedienteAlumno(id);
      this.getConfiguracionExpedienteUniversidad(id);
    });
  }

  loadTabs(): void {
    this.rolesUsuarios = this.securityService.userRoles();
    this.tabs = [
      {
        label: 'pages.listadoExpediente.tabs.resumen',
        value: this.expedienteSection.RESUMEN,
        hidden: false
      },
      {
        label: 'pages.listadoExpediente.tabs.expediente',
        value: this.expedienteSection.EXPEDIENTE,
        hidden: false
      },
      {
        label: 'pages.listadoExpediente.tabs.requisitos',
        value: this.expedienteSection.REQUISITOS,
        hidden: false
      },
      {
        label: 'pages.listadoExpediente.tabs.calificaciones',
        value: this.expedienteSection.CALIFICACIONES,
        hidden: false
      },
      {
        label: 'pages.listadoExpediente.tabs.puedeTitular',
        value: this.expedienteSection.PUEDE_TITULARSE,
        hidden: false
      },
      {
        label: 'pages.listadoExpediente.tabs.anotaciones',
        value: this.expedienteSection.ANOTACIONES,
        hidden:
          this.rolesUsuarios.indexOf('admin_expediente') == -1 &&
          this.rolesUsuarios.indexOf('gestor_expediente') == -1
      },
      {
        label: 'pages.listadoExpediente.tabs.seguimientos',
        value: this.expedienteSection.SEGUIMIENTOS,
        hidden:
          this.rolesUsuarios.indexOf('admin_expediente') == -1 &&
          this.rolesUsuarios.indexOf('gestor_expediente') == -1
      }
    ];
  }

  getExpedienteAlumno(id: number): void {
    this.blockUIService.start(this.blockIdentity);
    this.expedienteService
      .getDatosErpAlumno(id)
      .subscribe((response: DatosErpResponse) => {
        this.data = response;
      });
    this.expedienteService
      .getExpedienteAlumno(id)
      .subscribe((response: ExpedienteAlumnoDto) => {
        this.dataExpedienteAlumno = response;
        this.setExpedienteToAlumno();
        this.getAsignaturasReconocimientos();
        this.getGrafoPlan(+response.idRefPlan);
        this.blockUIService.stop(this.blockIdentity);
      });
  }

  getConfiguracionExpedienteUniversidad(id: number): void {
    this.expedienteService
      .getConfiguracionExpedienteUniversidad(id)
      .subscribe((response: ConfiguracionExpedienteUniversidadDto) => {
        this.configuracionUniversidad = response;
      });
  }

  public changeTab(section: string, event: Event): void {
    event.preventDefault();
    this.section = section;
  }

  setExpedienteToAlumno(): void {
    const expediente: ExpedienteDto = {
      id: this.dataExpedienteAlumno.id,
      fechaApertura: this.dataExpedienteAlumno.fechaApertura,
      fechaFinalizacion: this.dataExpedienteAlumno.fechaFinalizacion,
      nombrePlan: this.dataExpedienteAlumno.nombrePlan,
      nombreEstudio: this.dataExpedienteAlumno.nombreEstudio,
      acronimoUniversidad: this.dataExpedienteAlumno.acronimoUniversidad,
      consolidacionesRequisitosExpedientes: this.dataExpedienteAlumno
        .consolidacionesRequisitosExpedientes
    };
    this.dataExpedienteAlumno.alumno.expedientes = [];
    this.dataExpedienteAlumno.alumno.expedientes.push(expediente);
  }

  getGrafoPlan(idRefPlan: number): void {
    const request = new RequestNodoPlan();
    request.filterIdPlan = idRefPlan;
    this.expedienteService
      .getGrafoPlan(request)
      .subscribe((grafo: GrafoDto) => {
        if (
          this.dataExpedienteAlumno.idRefViaAccesoPlan &&
          this.data?.viaAccesoPlan &&
          this.dataExpedienteAlumno.idRefViaAccesoPlan ==
            this.data?.viaAccesoPlan?.id
        ) {
          grafo.nodos = grafo.nodos.filter(
            (n) => n.id == this.data.viaAccesoPlan.nodo.id
          );
          grafo.esNodoInicial = true;
        }
        this.grafoPlan = grafo;
        this.expedienteService.hasInfoGrafo(grafo);
      });
  }

  getAsignaturasReconocimientos(): void {
    this.calificacionesService
      .getAsignaturasReconocimientos(
        this.dataExpedienteAlumno.idRefIntegracionAlumno,
        this.dataExpedienteAlumno.idRefPlan,
        this.dataExpedienteAlumno.idRefVersionPlan
      )
      .subscribe((response: ReconocimientoClasificacionDto) => {
        this.reconocimientos = response;
      });
  }
}
