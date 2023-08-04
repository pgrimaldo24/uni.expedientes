import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { Clause } from '@cal/criteria';
import { TranslateService } from '@ngx-translate/core';
import { DataTableComponent } from '@src/app/component-tools/data-table/data-table.component';
import {
  DataColumn,
  DataRow
} from '@src/app/component-tools/data-table/models';

@Component({
  selector: 'app-hitos-obtenidos',
  templateUrl: './hitos-obtenidos.component.html',
  styleUrls: ['./hitos-obtenidos.component.scss']
})
export class HitosObtenidosComponent implements OnInit {
  columns: DataColumn[];
  totalElements: number;
  clauses: Clause;
  @ViewChild('table', { static: true })
  table: DataTableComponent;
  private _rows: DataRow[];

  constructor(private translateService: TranslateService) {}

  ngOnInit(): void {
    this.inicializarCabecerasTabla();
  }

  @Input()
  set rows(value: DataRow[]) {
    if (value) {
      this.table.unblock();
      this.totalElements = value.length;
      this._rows = value;
    }
  }

  get rows(): DataRow[] {
    return this._rows;
  }

  private inicializarCabecerasTabla() {
    this.columns = [
      {
        field: 'id',
        sortable: false,
        header: this.translateService.instant(
          'pages.puedeTitularse.hitosObtenidos.id'
        )
      },
      {
        field: 'nombre',
        sortable: false,
        header: this.translateService.instant(
          'pages.puedeTitularse.hitosObtenidos.nombre'
        ),
        style: { width: '400px' }
      },
      {
        field: 'otorga',
        sortable: false,
        header: this.translateService.instant(
          'pages.puedeTitularse.hitosObtenidos.otorga'
        )
      },
      {
        field: 'descripcion',
        sortable: false,
        header: this.translateService.instant(
          'pages.puedeTitularse.hitosObtenidos.descripcion'
        ),
        style: { width: '200px' }
      }
    ];
  }
}
