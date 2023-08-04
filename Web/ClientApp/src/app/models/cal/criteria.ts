export class Criteria {
  count?: boolean;
  order?: string;
  offset?: number;
  limit?: number;
  groupBy?: string;
  where?: Clause[];
  distict?: boolean;
  projection?: string;
}

export interface Clause {
  op?: string;
  field?: string;
  value?: any;
  key?: string;
  label?: string;
  text?: string;
  clauses?: Clause[];
}
