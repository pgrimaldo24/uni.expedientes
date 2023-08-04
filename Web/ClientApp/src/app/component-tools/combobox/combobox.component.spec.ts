import { TestBed, ComponentFixture } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA, DebugElement } from '@angular/core';
import { By } from '@angular/platform-browser';
import { async } from '@angular/core/testing';
import { ComboboxComponent } from './combobox.component';
import { HttpClientModule } from '@angular/common/http';
import { ComboboxItem } from './models';

describe('ComboboxComponent should ', () => {
  let component: ComboboxComponent;
  let fixture: ComponentFixture<ComboboxComponent>;
  let debug: DebugElement;
  let element: HTMLElement;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientModule],
      declarations: [ComboboxComponent],
      schemas: [NO_ERRORS_SCHEMA],
      providers: []
    })
      .compileComponents()
      .then(() => {
        fixture = TestBed.createComponent(ComboboxComponent);
        component = fixture.componentInstance;
        debug = fixture.debugElement.query(By.css('.combobox-container-main'));
        element = debug.nativeElement;
      });
  }));

  it('deberia pintarse el componente', () => {
    fixture.detectChanges();
    const container = element.getElementsByClassName('container-selected');
    expect(container.length).toEqual(1);
  });
  it('deberia estar expandido', () => {
    component.serverSide = false;
    component.elements = [
      new ComboboxItem({
        text: 'Text 1',
        value: 'Value 1'
      })
    ];
    component.expanded = true;
    fixture.detectChanges();
    const container = element.getElementsByClassName('container-search-items');
    expect(container.length).toEqual(1);
  });

  it('deberia tener elementos', () => {
    component.serverSide = false;
    component.elements = [
      new ComboboxItem({ text: 'Text 1', value: 'Value 1' }),
      new ComboboxItem({ text: 'Text 2', value: 'Value 2' }),
      new ComboboxItem({ text: 'Text 3', value: 'Value 3' })
    ];
    component.expanded = true;
    fixture.detectChanges();
    const items = element
      .getElementsByClassName('container-search-items')
      .item(0)
      .getElementsByTagName('LI');
    expect(items.length).toEqual(component.elements.length);
  });

  it('deberia cambiar seleccionado al hacer click', () => {
    component.serverSide = false;
    component.elements = [
      new ComboboxItem({ text: 'Text 1', value: '1' }),
      new ComboboxItem({ text: 'Text 2', value: '2' })
    ];
    component.expanded = true;
    fixture.detectChanges();
    const first = element
      .getElementsByClassName('container-search-items')
      .item(0)
      .querySelector('li');

    expect(first).not.toBeNull();
    first.click();
    expect(component.currentPage).not.toBeNull();
    expect(component.currentPage).toEqual(component.elements[0].value);
  });
});
