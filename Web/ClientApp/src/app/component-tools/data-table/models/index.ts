/* eslint-disable @typescript-eslint/explicit-module-boundary-types */
/* eslint-disable @typescript-eslint/no-explicit-any */
export enum DataOrderDirection {
  Ascending = 1,
  Descending = 2
}

export interface DataCell {
  value: any;
  class?: string;
  style?: any;
}

export class DataColumn {
  field: string;
  header?: string;
  sortable: boolean;
  class?: string;
  style?: any;
}

export class DataRow {
  cells: DataCell[];
  object: any;
  rowData: any;
  selected: boolean;
  class?: string;
  constructor(rowData: any, object?: any, rowClass?: string) {
    this.cells = [];
    this.rowData = rowData;
    this.object = object;
    this.selected = false;
    this.class = rowClass;
    Object.keys(rowData).forEach((key) => {
      const dataCell = rowData[key];
      if (dataCell == null) {
        this.cells[key] = { value: dataCell };
      } else if (dataCell.class != null || dataCell.style != null) {
        this.cells[key] = {
          value: dataCell.value,
          class: dataCell.class,
          style: dataCell.style
        };
      } else {
        this.cells[key] = { value: dataCell };
      }
    });
  }
}

export class DataLoadEvent {
  page: number;
  count: number;

  order: DataColumn;
  direction: DataOrderDirection;
}

export interface DataRowSelected {
  selected: boolean;
  object: any;
}
