import { BlockUIService } from 'ng-block-ui';
import { Guid } from 'guid-typescript';
import {
  Component,
  Input,
  TemplateRef,
  Output,
  EventEmitter,
  ViewChild,
  ElementRef
} from '@angular/core';
import {
  DataColumn,
  DataOrderDirection,
  DataLoadEvent,
  DataRow,
  DataRowSelected
} from './models';
import { CdkDragDrop } from '@angular/cdk/drag-drop';

@Component({
  selector: 'app-data-table',
  templateUrl: './data-table.component.html',
  styleUrls: ['./data-table.component.scss']
})
export class DataTableComponent {
  @ViewChild('dynamic') dynamic: ElementRef;
  @ViewChild('colDynamic') colDynamic: ElementRef;
  blockIdentity = Guid.create().toString();
  @Input() rows: DataRow[] = [];
  // Paginación
  @Input() page = 1;
  @Input() total = 0;
  @Input() count = 10;
  private columnsField: DataColumn[] = [];
  private countsField: number[] = [10, 30, 50, 100];
  // Ordenación
  @Input() order: DataColumn;
  @Input() isScroll = false;
  @Input() selectable = false;
  @Input() direction: DataOrderDirection = DataOrderDirection.Ascending;
  @Input() cellsTemplate: { [key: string]: TemplateRef<unknown> } = {};
  @Input() columnsTemplate: { [key: string]: TemplateRef<unknown> } = {};
  @Input() pageLengthOption = true;
  // drag and drop
  @Input() disableDropList = true;
  @Output()
  readonly onDrop: EventEmitter<CdkDragDrop<string[]>> = new EventEmitter<
    CdkDragDrop<string[]>
  >();
  // tslint:disable-next-line: no-output-on-prefix
  @Output()
  readonly onLoad: EventEmitter<DataLoadEvent> = new EventEmitter<DataLoadEvent>();
  @Output() onSelect = new EventEmitter<DataRowSelected>();
  constructor(private blockUIService: BlockUIService) {}
  block(): void {
    this.blockUIService.start(this.blockIdentity);
  }
  unblock(): void {
    this.blockUIService.stop(this.blockIdentity);
  }
  @Input() set columns(value: DataColumn[]) {
    this.columnsField = value;
    this.order = value.find((c) => c.sortable);
  }
  get columns(): DataColumn[] {
    return this.columnsField;
  }
  @Input() set counts(value: number[]) {
    this.countsField = value;
    this.count = value[0];
  }
  get counts(): number[] {
    return this.countsField;
  }

  update(): void {
    this.fireLoadEvent();
  }
  empty(): void {
    this.rows = [];
    this.total = 0;
    this.page = 1;
  }
  countChanged(): void {
    this.fireLoadEvent();
  }
  pageChange(): void {
    this.fireLoadEvent();
  }
  orderChange(event: Event, column: DataColumn): void {
    if (!column.sortable) {
      return;
    }

    if (this.order.field === column.field) {
      if (this.direction === DataOrderDirection.Ascending) {
        this.direction = DataOrderDirection.Descending;
      } else {
        this.direction = DataOrderDirection.Ascending;
      }
    } else {
      this.order = column;
      this.direction = DataOrderDirection.Ascending;
    }

    this.fireLoadEvent();
  }

  private fireLoadEvent(): void {
    const data = {
      page: this.page,
      count: this.count,
      order: this.order,
      direction: this.direction
    };
    this.onLoad.emit(data);
  }

  public selectRow(data: DataRow): void {
    if (!this.selectable) return;
    let selected = false;
    this.rows.map((value) => {
      if (value === data) {
        value.selected = !value.selected;
        selected = value.selected;
      } else {
        value.selected = false;
      }
      return value;
    });
    this.onSelect.emit({
      selected: selected,
      object: data.object
    });
  }

  public onDropRow(event: CdkDragDrop<string[]>): void {
    this.onDrop.emit(event);
  }
}
