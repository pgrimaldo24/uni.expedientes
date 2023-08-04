import { HalLinkObject, HasHalLinks } from './hal';

export interface PagedListResponseMessage<T> extends HasHalLinks {
  meta: MetaObject;
  data: T[];
  _links: HalLinkObject;
}

export interface MetaObject {
  totalCount?: number;
  offset?: number;
  limit?: number;
  messages?: string[];
}

export interface ResponseError {
  code: string;
  message: string;
  details: string[];
}
