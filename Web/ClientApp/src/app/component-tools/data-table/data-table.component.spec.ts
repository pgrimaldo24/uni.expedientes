import { DataTableComponent } from './data-table.component';
import { TestBed, ComponentFixture } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA, DebugElement } from '@angular/core';
import { BlockUIService, BlockUIModule } from 'ng-block-ui';
import { Guid } from 'guid-typescript';
import { DataRow } from './models';
import { By } from '@angular/platform-browser';
import { waitForAsync } from '@angular/core/testing';

describe('DataTableComponent should ', () => {
  let component: DataTableComponent;
  let fixture: ComponentFixture<DataTableComponent>;
  let debug: DebugElement;
  let element: HTMLElement;

  beforeEach(
    waitForAsync(() => {
      const mockBlockService = {
        start: () => {},
        stop: () => {}
      };
      TestBed.configureTestingModule({
        imports: [BlockUIModule.forRoot()],
        declarations: [DataTableComponent],
        schemas: [NO_ERRORS_SCHEMA],
        providers: [{ provide: BlockUIService, use: mockBlockService }]
      })
        .compileComponents()
        .then(() => {
          fixture = TestBed.createComponent(DataTableComponent);
          component = fixture.componentInstance;
          debug = fixture.debugElement.query(By.css('.container-data-table'));
          element = debug.nativeElement;
        });
    })
  );

  it('deberia pintarse la tabla', () => {
    component.blockIdentity = Guid.create().toString();
    component.columns = [
      { field: 'column1', header: 'Column 1', sortable: false },
      { field: 'column2', header: 'Column 2', sortable: false }
    ];
    fixture.detectChanges();
    const table = element.getElementsByTagName('TABLE');
    expect(table.length).toEqual(1);
  });

  it('deberia pintarse 3 filas en la tabla', () => {
    component.blockIdentity = Guid.create().toString();
    component.columns = [
      { field: 'column1', header: 'Column 1', sortable: false },
      { field: 'column2', header: 'Column 2', sortable: false }
    ];
    component.rows = [
      new DataRow({ column1: 'value 1', column2: 'value 2' }),
      new DataRow({ column1: 'value 1', column2: 'value 2' }),
      new DataRow({ column1: 'value 1', column2: 'value 2' })
    ];
    fixture.detectChanges();
    const table = element.getElementsByTagName('TABLE');
    expect(table.length).toEqual(1);
    const rows = table
      .item(0)
      .getElementsByTagName('TBODY')
      .item(0)
      .getElementsByTagName('TR');
    expect(rows.length).toEqual(component.rows.length);
  });

  it('deberia poderse ordenar por la primera columna', () => {
    component.blockIdentity = Guid.create().toString();
    component.columns = [
      { field: 'column1', header: 'Column 1', sortable: true },
      { field: 'column2', header: 'Column 2', sortable: false }
    ];
    component.rows = [
      new DataRow({ column1: 'value 1', column2: 'value 2' }),
      new DataRow({ column1: 'value 1', column2: 'value 2' }),
      new DataRow({ column1: 'value 1', column2: 'value 2' })
    ];
    fixture.detectChanges();
    const table = element.getElementsByTagName('TABLE');
    expect(table.length).toEqual(1);
    const order = table
      .item(0)
      .getElementsByTagName('THEAD')
      .item(0)
      .getElementsByTagName('TR')
      .item(0)
      .getElementsByTagName('TH')
      .item(0)
      .getElementsByTagName('I');
    expect(order.length).toEqual(1);
  });
});
