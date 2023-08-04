import { UnirTreeViewItem } from '@tools/unir-tree-select/models/unir-tree-view-item';
export class ConfigureComboboxTree {
  constructor(data: Partial<ConfigureComboboxTree>) {
    Object.assign(this, data);
    this.method = this.method == null ? 'GET' : this.method;
    this.minLength = this.minLength == null ? 3 : this.minLength;
    this.delay = this.delay == null ? 1000 : this.delay;
    this.perPage = this.perPage == null ? 10 : this.perPage;
    this.querySearch = this.querySearch == null ? '' : this.querySearch;
    this.queryPage = this.queryPage == null ? '' : this.queryPage;
    this.queryAsc = this.queryAsc == null ? '' : this.queryAsc;
    this.queryPerPage = this.queryPerPage == null ? '' : this.queryPerPage;
    this.asc = this.asc == null ? true : this.asc;
    this.cancelServerData =
      this.cancelServerData == null ? false : this.cancelServerData;
  }
  url: string;
  method: string;
  querySearch: string;
  queryPage: string;
  queryAsc: string;
  queryPerPage: string;
  asc: boolean;
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  data?: any;
  cancelServerData?: boolean;

  perPage: number;
  minLength: number;
  delay: number;
  transformData?: (data: any) => UnirTreeViewItem[];
  readHeaders?: (headers: any) => void;
}
