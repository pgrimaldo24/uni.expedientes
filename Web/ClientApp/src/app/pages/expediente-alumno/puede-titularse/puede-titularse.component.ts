import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Clause, Criteria } from '@cal/criteria';
import * as helpers from '@helpers/commons-helpers';
import { TranslateService } from '@ngx-translate/core';
import { Guid } from '@src/app/component-tools/combobox/models';
import { DataTableComponent } from '@src/app/component-tools/data-table/data-table.component';
import {
  DataColumn,
  DataRow
} from '@src/app/component-tools/data-table/models';
import { BlockUIService } from 'ng-block-ui';
import { ExpedienteService } from '../expediente.service';
@Component({
  selector: 'app-puede-titularse',
  templateUrl: './puede-titularse.component.html',
  styleUrls: ['./puede-titularse.component.scss']
})
export class PuedeTitularseComponent implements OnInit {
  @Input() dataExpedienteAlumno = null;
  blockIdentity = Guid.create().toString();
  colapsadoNodosActuales: boolean;
  colapsadoHitosObtenidos: boolean;
  idAlumno: number;
  data = null;
  expedienteAlumno = null;
  clauses: Clause[];
  filters;
  columnsNodosActuales: DataColumn[];
  rowsNodosActuales: DataRow[];
  totalElementsNodosActuales: number;
  columnsHitosObtenidos: DataColumn[];
  rowsHitosObtenidos: DataRow[];
  totalElementsHitosObtenidos: number;
  @ViewChild('tableNodosActuales', { static: true })
  tableNodosActuales: DataTableComponent;
  @ViewChild('tableHitosObtenidos', { static: true })
  tableHitosObtenidos: DataTableComponent;

  constructor(
    private activatedRoute: ActivatedRoute,
    private expedienteService: ExpedienteService,
    private route: Router,
    private blockUIService: BlockUIService,
    private translateService: TranslateService
  ) {}

  ngOnInit(): void {
    this.blockUIService.start(this.blockIdentity);
    this.activatedRoute.params.subscribe(({ id }) => {
      if (id === undefined) {
        this.route.navigateByUrl('/');
      }
      this.idAlumno = id;
      this.expedienteService.getDatosErpAlumno(id).subscribe((response) => {
        this.data = response;
        this.blockUIService.stop(this.blockIdentity);
        const criteria: Criteria = {};
        const clauses = this.buildClausesFromFilters();
        criteria.where = clauses;
        this.expedienteService
          .getAlumnoPuedeTitularseEnPlan(criteria)
          .subscribe((expedienteAlumno) => {
            this.expedienteAlumno = expedienteAlumno;
            this.fillNodosActuales(expedienteAlumno);
            this.fillHitosObtenidos(expedienteAlumno);
          });
      });
    });
  }

  showHideNodosActuales(): void {
    this.colapsadoNodosActuales = !this.colapsadoNodosActuales;
    helpers.clearFocus();
  }

  showHideHitosObtenidos(): void {
    this.colapsadoHitosObtenidos = !this.colapsadoHitosObtenidos;
    helpers.clearFocus();
  }

  private buildClausesFromFilters(): Clause[] {
    this.clauses = [];
    this.clauses.push({
      field: 'idRefIntegracionAlumno',
      value: this.data.idRefIntegracionAlumno
    });
    this.clauses.push({
      field: 'idRefPlan',
      value: this.data.idRefPlan
    });
    return this.clauses;
  }

  fillNodosActuales(expedienteAlumno: any): void {
    this.rowsNodosActuales = [];
    expedienteAlumno.esPlanSuperado.elementosSuperados?.nodosActuales.forEach(
      (nodoActual) => {
        const nodosAlcanzados = expedienteAlumno.esPlanSuperado.elementosSuperados.nodosAlcanzados.filter(
          (item) => item.id == nodoActual
        );
        if (nodosAlcanzados) {
          const dataRows = nodosAlcanzados.map(
            (e) =>
              new DataRow(
                {
                  id: e.id,
                  nombre: e.nombre,
                  tipo: e.tipo.nombre,
                  descripcion: e.descripcion != null ? e.descripcion : ''
                },
                e
              )
          );
          dataRows.forEach((element) => {
            this.rowsNodosActuales.push(element);
          });
        }
      }
    );
  }

  fillHitosObtenidos(expedienteAlumno: any): void {
    this.rowsHitosObtenidos = expedienteAlumno.esPlanSuperado.elementosSuperados?.hitosObtenidos.map(
      (e) =>
        new DataRow(
          {
            id: e.id,
            nombre: e.nombre,
            otorga: this.getOtorga(e),
            descripcion: e.Descripcion != null ? e.Descripcion : ''
          },
          e
        )
    );
  }

  getOtorga(rowData: any): string {
    let otorga = '';
    if (rowData.titulo == null && rowData.especializacion == null) {
      otorga = this.translateService.instant(
        'pages.puedeTitularse.hitosObtenidos.cellNoOtorga'
      );
    } else {
      if (!(rowData.titulo == null)) {
        otorga = this.translateService.instant(
          'pages.puedeTitularse.hitosObtenidos.cellTitulo'
        );
      }
      if (!(rowData.especializacion == null)) {
        otorga = this.translateService.instant(
          'pages.puedeTitularse.hitosObtenidos.cellEspecializacion'
        );
      }
    }
    return otorga;
  }
}
