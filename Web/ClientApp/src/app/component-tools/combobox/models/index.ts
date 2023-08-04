export class ComboboxItem {
  constructor(data: Partial<ComboboxItem>) {
    Object.assign(this, data);
    this.id = Guid.create().toString();
  }
  id?: string;
  value: string | number;
  text: string;
  active?: boolean;
  data?: any;
}
export class ConfigureCombobox {
  constructor(data: Partial<ConfigureCombobox>) {
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
  transformData?: (data: any) => ComboboxItem[];
  readHeaders?: (headers: any) => void;
}

export class Guid {
  public static validator = new RegExp(
    '^[a-z0-9]{8}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{12}$',
    'i'
  );

  public static empty = '00000000-0000-0000-0000-000000000000';
  private value: string;
  constructor(guid: string) {
    if (!guid) {
      throw new TypeError('Invalid argument; `value` has no value.');
    }

    this.value = Guid.empty;

    if (guid && Guid.isGuid(guid)) {
      this.value = guid;
    }
  }

  public static isGuid(guid: any): boolean {
    const value: string = guid.toString();
    return guid && (guid instanceof Guid || Guid.validator.test(value));
  }

  public static create(): Guid {
    return new Guid(
      [Guid.gen(2), Guid.gen(1), Guid.gen(1), Guid.gen(1), Guid.gen(3)].join(
        '-'
      )
    );
  }

  public static createEmpty(): Guid {
    return new Guid('emptyguid');
  }

  public static parse(guid: string): Guid {
    return new Guid(guid);
  }

  public static raw(): string {
    return [
      Guid.gen(2),
      Guid.gen(1),
      Guid.gen(1),
      Guid.gen(1),
      Guid.gen(3)
    ].join('-');
  }

  private static gen(count: number): string {
    let out = '';
    for (let i = 0; i < count; i++) {
      // eslint-disable-next-line no-bitwise
      out += (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    }
    return out;
  }

  public equals(other: Guid): boolean {
    return Guid.isGuid(other) && this.value === other.toString();
  }

  public isEmpty(): boolean {
    return this.value === Guid.empty;
  }

  public toString(): string {
    return this.value;
  }

  public toJSON(): unknown {
    return {
      value: this.value
    };
  }
}
