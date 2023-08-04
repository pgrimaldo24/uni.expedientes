import { HttpClient, HttpResponse } from '@angular/common/http';
import {
  Component,
  ElementRef,
  EventEmitter,
  forwardRef,
  HostListener,
  Input,
  OnDestroy,
  OnInit,
  Output,
  ViewChild
} from '@angular/core';
import {
  ControlValueAccessor,
  FormControl,
  NG_VALUE_ACCESSOR
} from '@angular/forms';
import { Observable, Subject } from 'rxjs';
import { debounceTime, filter, finalize, takeUntil } from 'rxjs/operators';
import { ComboboxItem, ConfigureCombobox } from '@tools/combobox/models/index';

@Component({
  selector: 'unir-combobox',
  templateUrl: './combobox.component.html',
  styleUrls: ['./combobox.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ComboboxComponent),
      multi: true
    }
  ]
})
export class ComboboxComponent
  implements OnInit, ControlValueAccessor, OnDestroy {
  loading = false;
  expanded: boolean;
  currentPage: number;
  changeSearch: string;
  filtered: ComboboxItem[] | undefined = [];
  unsubscribe$ = new Subject();
  input: FormControl = new FormControl('');
  onChange = Function.prototype;
  onTouched = Function.prototype;
  currentValue: any | null = null;
  dataItems = [];
  @ViewChild('search', { static: false }) inputChild: ElementRef;

  @Output() readonly selected = new EventEmitter<
    ComboboxItem | ComboboxItem[] | null
  >();

  @Input() preLoaded: () => boolean;
  @Input() preSelected: (
    oldValue: ComboboxItem | null,
    newValue: ComboboxItem | null
  ) => boolean;
  @Input() elements: ComboboxItem[] | null = [];
  @Input() config: ConfigureCombobox;
  @Input() serverSide = false;
  @Input() disable: boolean;
  @Input() multiSelect = false;

  constructor(private http: HttpClient, private elementRef: ElementRef) {
    this.expanded = false;
    this.currentPage = 1;
  }
  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }
  public get hasValueInput(): boolean {
    const text: string = this.input.value;
    return text.length > 0;
  }

  public get selectElements(): boolean {
    return this.currentValue?.length === 0 || this.currentValue === null;
  }

  writeValue(value: ComboboxItem | ComboboxItem[] | null): void {
    this.currentValue = value;
  }
  registerOnChange(fn: () => Record<string, never>): void {
    this.onChange = fn;
  }
  registerOnTouched(fn: () => Record<string, never>): void {
    this.onTouched = fn;
  }
  setDisabledState?(state: boolean): void {
    this.disable = state;
  }

  ngOnInit(): void {
    if (this.config == null) {
      this.config = new ConfigureCombobox({});
    }
    if (this.serverSide) {
      this.elements = [];
    }
    if (this.serverSide) {
      this.input.valueChanges
        .pipe(
          takeUntil(this.unsubscribe$),
          filter((value: string) => value.length >= this.config.minLength),
          debounceTime(700)
        )
        .subscribe(() => {
          this.elements = [];
          this.currentPage = 1;
          this.serverData();
        });
    } else {
      this.input.valueChanges
        .pipe(takeUntil(this.unsubscribe$), debounceTime(0))
        .subscribe((value: string) => {
          if (value.length > 0) {
            this.serverClient();
          }
        });
    }
  }

  expandCollapse(event: Event): void {
    if (
      (event.target as HTMLInputElement).tagName === 'I' &&
      (event.target as HTMLInputElement).className === 'fas fa-times'
    ) {
      return;
    }
    this.expanded = !this.expanded;
    if (this.expanded) {
      this.onTouched();
    }
    if (this.expanded) {
      setTimeout(() => {
        if (this.inputChild != null) {
          this.inputChild.nativeElement.focus();
        }
        if (!this.serverSide) {
          return;
        }
        const search: string = this.input.value;
        if (this.changeSearch !== search || this.elements?.length === 0) {
          this.currentPage = 1;
          this.elements = [];
          this.serverData();
        }
      }, 100);
    } else {
      this.cleanSearAndArray();
    }
  }
  clickItem(item: ComboboxItem): void {
    if (this.multiSelect) {
      const itemSelect = this.dataItems.find(
        (element: ComboboxItem) => element.value === item.value
      );

      if (!itemSelect) {
        this.dataItems.unshift(item);
        this.writeValue(this.dataItems);
        this.onChange(this.currentValue);
        this.onTouched();
        this.selected.emit(this.dataItems);
      } else {
        this.clearItemSelect(itemSelect);
      }
    } else {
      if (this.preSelected && !this.preSelected(this.currentValue, item)) {
        return;
      }
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      const value: any = this.currentValue?.value;
      const oldValue = { ...value };
      this.expanded = false;
      if (oldValue !== item.value) {
        this.writeValue(item);
        this.onChange(this.currentValue);
        this.onTouched();
        this.selected.emit(item);
      }
      this.cleanSearAndArray();
    }
  }

  clearItemSelect(item: ComboboxItem | []): void {
    this.dataItems.forEach((element, index) => {
      if (element === item) this.dataItems.splice(index, 1);
    });

    if (this.dataItems.length === 0) this.currentValue = null;
  }

  cleanSearAndArray(): void {
    if (this.serverSide) {
      this.elements = [];
    } else {
      this.filtered = [];
    }
    this.input.setValue('');
  }
  serverClient(): void {
    if (this.preLoaded && !this.preLoaded()) {
      return;
    }
    const search: string = this.input.value;
    let term: string;
    this.filtered = this.elements?.filter((elem) => {
      term = this.stripSpecialChars(search).toLowerCase();
      return this.stripSpecialChars(elem.text).toLowerCase().includes(term);
    });
  }
  private stripSpecialChars(text: string): string {
    text = text == null ? '' : text;
    return text.normalize('NFD').replace(/[\u0300-\u036f]/g, '');
  }
  serverData(): void {
    if (!this.serverSide || this.config.cancelServerData) {
      this.serverClient();
      return;
    }
    if (this.preLoaded && !this.preLoaded()) {
      return;
    }
    const method = this.config.method.toUpperCase();
    const queries: string[] = [];
    if (this.config.querySearch !== '') {
      const search: string = this.input.value;
      queries.push(`${this.config.querySearch}=${search}`);
    }
    if (this.config.queryPage !== '') {
      queries.push(`${this.config.queryPage}=${this.currentPage}`);
    }
    if (this.config.queryPerPage !== '') {
      queries.push(`${this.config.queryPerPage}=${this.config.perPage}`);
    }
    if (this.config.queryAsc !== '') {
      queries.push(
        `${this.config.queryAsc}=${String(this.config.asc).toLowerCase()}`
      );
    }
    let queryString = '';
    if (queries.length > 0) {
      queryString =
        (this.config.url.includes('?') ? '&' : '?') + `${queries.join('&')}`;
    }
    let payload = {};

    if (this.config.data != null) {
      const search: string = this.input.value;
      const json = JSON.stringify(this.config.data)
        .replace('#SEARCH', search)
        .replace('"#ASC"', String(this.config.asc).toLowerCase())
        .replace('"#PER_PAGE"', this.config.perPage.toString())
        .replace('"#PAGE"', this.currentPage.toString());
      payload = JSON.parse(json);
    }
    // eslint-disable-next-line @typescript-eslint/ban-types
    let request: Observable<HttpResponse<Object>> | null = null;
    switch (method) {
      case 'GET':
        request = this.http.get(`${this.config.url}${queryString}`, {
          observe: 'response'
        });
        break;
      case 'POST':
        request = this.http.post(`${this.config.url}${queryString}`, payload, {
          observe: 'response'
        });
        break;
      case 'PUT':
        request = this.http.put(`${this.config.url}${queryString}`, payload, {
          observe: 'response'
        });
        break;
    }
    if (request != null) {
      this.loading = true;
      request
        .pipe(
          finalize(() => {
            this.loading = false;
          })
        )
        .toPromise()
        .then((response) => {
          this.changeSearch = this.input.value;
          if (this.config.transformData) {
            const values = this.config.transformData(response.body);
            const elements = this.elements as ComboboxItem[];
            this.elements = [...elements, ...values];
          }
          if (this.config.readHeaders) {
            this.config.readHeaders(response.headers);
          }
        });
    }
  }

  scroll(event: Event): void {
    if (!this.serverSide) {
      return;
    }
    const offsetHeight: number = (event.target as HTMLInputElement)
      .offsetHeight;
    const scrollTop: number = (event.target as HTMLInputElement).scrollTop;
    const scrollHeight: number = (event.target as HTMLInputElement)
      .scrollHeight;
    if (offsetHeight + scrollTop >= scrollHeight) {
      this.currentPage = this.currentPage + 1;
      this.serverData();
    }
  }
  clearSelected(event: Event): void {
    event.preventDefault();
    this.dataItems = [];
    if (this.preSelected && !this.preSelected(this.currentValue, null)) {
      return;
    }

    this.input.setValue('');
    this.writeValue(null);
    this.onChange(this.currentValue);
    this.onTouched();
    this.selected.emit(null);
  }
  empty(): void {
    this.elements = [];
    this.input.setValue('');

    this.writeValue(null);
    this.onChange(this.currentValue);
    this.onTouched();
  }
  onBlur(event: { sourceCapabilities: unknown }): void {
    if (event.sourceCapabilities == null) {
      if (this.expanded) {
        this.expanded = false;
      }
    }
  }
  @HostListener('document:click', ['$event'])
  clickout(event: { target: unknown }): void {
    if (this.elementRef.nativeElement.contains(event.target)) {
    } else {
      if (this.expanded) {
        if (this.serverSide) {
          if (this.currentValue == null) {
            this.empty();
          }
          this.elements = [];
        } else {
          this.filtered = [];
        }
        this.input.setValue('');
        this.onTouched();
      }
      this.expanded = false;
    }
  }
}
