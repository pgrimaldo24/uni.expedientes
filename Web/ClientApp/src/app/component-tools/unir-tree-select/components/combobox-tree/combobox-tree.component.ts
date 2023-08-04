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
  TemplateRef,
  ViewChild
} from '@angular/core';
import {
  ControlValueAccessor,
  FormControl,
  NG_VALUE_ACCESSOR
} from '@angular/forms';
import { Observable, Subject } from 'rxjs';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { debounceTime, filter, finalize, takeUntil } from 'rxjs/operators';
import { ConfigureComboboxTree } from '@tools/unir-tree-select/models';
import {
  TreeItem,
  UnirTreeViewItem
} from '@tools/unir-tree-select/models/unir-tree-view-item';

@Component({
  selector: 'unir-combobox-tree',
  templateUrl: './combobox-tree.component.html',
  styleUrls: ['./combobox-tree.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ComboboxTreeComponent),
      multi: true
    }
  ]
})
export class ComboboxTreeComponent
  implements OnInit, ControlValueAccessor, OnDestroy {
  values: number[];
  loading = false;
  expanded: boolean;
  currentPage: number;
  changeSearch: string;
  filtered: UnirTreeViewItem[] | undefined = [];
  unsubscribe$ = new Subject();
  input: FormControl = new FormControl('');
  onChange = Function.prototype;
  onTouched = Function.prototype;
  currentValue: UnirTreeViewItem | null = null;
  dataItems = [];
  @ViewChild('search', { static: false }) inputChild: ElementRef;
  @Input() collapseIcon: TemplateRef<unknown>;

  @Output() readonly selected = new EventEmitter<
    UnirTreeViewItem | UnirTreeViewItem[] | null
  >();

  @Input() preLoaded: () => boolean;
  @Input() preSelected: (
    oldValue: UnirTreeViewItem | null,
    newValue: UnirTreeViewItem | null
  ) => boolean;
  @Input() items: UnirTreeViewItem[] | null = [];
  @Input() config: ConfigureComboboxTree;
  @Input() serverSide = false;
  @Input() disable: boolean;
  private itemsClone: UnirTreeViewItem[] | null = [];

  constructor(private http: HttpClient, private elementRef: ElementRef) {
    this.expanded = false;
    this.currentPage = 1;
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  writeValue(value: UnirTreeViewItem | null): void {
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
      this.config = new ConfigureComboboxTree({});
    }
    if (this.serverSide) {
      this.items = [];
    }
    if (this.serverSide) {
      this.input.valueChanges
        .pipe(
          takeUntil(this.unsubscribe$),
          filter((value: string) => value.length >= this.config.minLength),
          debounceTime(700)
        )
        .subscribe(() => {
          this.serverClient();
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

  private filterTextOffline(query: string): void {
    if (query !== '') {
      const filterItems: UnirTreeViewItem[] = [];
      const filterText = query.toLowerCase();
      [...this.itemsClone].forEach((item) => {
        const newItem = this.filterItem(item, filterText);
        if (newItem) {
          newItem.collapsed = false;
          filterItems.push(newItem);
        }
      });
      this.items = filterItems;
    } else {
      this.items = [...this.itemsClone];
    }
  }

  private filterItem(item: UnirTreeViewItem, text: string): UnirTreeViewItem {
    const isMatch = item.name.toLowerCase().includes(text);
    if (isMatch) {
      return item;
    }
    if (item.nodes) {
      const children: UnirTreeViewItem[] = [];
      item.nodes.forEach((child) => {
        const newChild = this.filterItem(child, text);
        if (newChild) {
          newChild.collapsed = false;
          children.push(newChild);
        }
      });
      if (children.length > 0) {
        const newItem = new UnirTreeViewItem(item);
        newItem.collapsed = false;
        newItem.nodes = children;
        return newItem;
      }
    }
    return undefined;
  }

  expandCollapse(event: Event): void {
    if (
      (event.target as Element).tagName === 'I' &&
      (event.target as Element).className === 'fas fa-times'
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
        if (this.changeSearch !== search || this.items?.length === 0) {
          this.currentPage = 1;
          this.items = [];
          this.serverData();
        }
      }, 100);
    } else {
      this.cleanSearAndArray();
    }
  }

  clickItem(item: UnirTreeViewItem): void {
    if (
      this.preSelected &&
      !this.preSelected(this.currentValue as UnirTreeViewItem | null, item)
    ) {
      return;
    }
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const value: any = this.currentValue;
    const oldValue = { ...value };
    this.expanded = false;
    if (oldValue !== item.id) {
      this.writeValue(item);
      this.onChange(this.currentValue);
      this.onTouched();
      this.selected.emit(item);
    }
    this.cleanSearAndArray();
  }

  clearItemSelect(item: UnirTreeViewItem | []): void {
    this.dataItems.forEach((element, index) => {
      if (element === item) this.dataItems.splice(index, 1);
    });

    if (this.dataItems.length === 0) this.currentValue = null;
  }

  cleanSearAndArray(): void {
    if (this.serverSide) {
      this.items = [];
    } else {
      this.filtered = [];
    }
    this.input.setValue('');
  }

  serverClient(): void {
    if (this.preLoaded && !this.preLoaded()) {
      return;
    }
    this.filterTextOffline(this.input.value);
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
            this.items = [...values];
            this.itemsClone = [...values];
          } else {
            const items = response.body as TreeItem[];
            const values = items.map((value) => new UnirTreeViewItem(value));
            this.items = [...values];
            this.itemsClone = [...values];
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
    const offsetHeight: number = (event.target as HTMLDivElement).offsetHeight;
    const scrollTop: number = (event.target as HTMLDivElement).scrollTop;
    const scrollHeight: number = (event.target as HTMLDivElement).scrollHeight;
    if (offsetHeight + scrollTop >= scrollHeight) {
      this.currentPage = this.currentPage + 1;
      this.serverData();
    }
  }

  clearSelected(event: Event): void {
    event.preventDefault();
    this.dataItems = [];
    if (
      this.preSelected &&
      !this.preSelected(this.currentValue as UnirTreeViewItem | null, null)
    ) {
      return;
    }

    this.input.setValue('');
    this.writeValue(null);
    this.onChange(this.currentValue);
    this.onTouched();
    this.selected.emit(null);
  }

  empty(): void {
    this.items = [];
    this.input.setValue('');

    this.writeValue(null);
    this.onChange(this.currentValue);
    this.onTouched();
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
          this.items = [];
        } else {
          this.filtered = [];
        }
        this.input.setValue('');
        this.onTouched();
      }
      this.expanded = false;
    }
  }

  selectedChange(value: UnirTreeViewItem): void {
    this.clickItem(value);
  }
}
