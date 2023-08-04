/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable @typescript-eslint/explicit-module-boundary-types */
import * as moment from 'moment';
import { formatDate } from '@angular/common';
import { CustomSimpleListItemModel } from '@models/erp-academico';
import { ComboboxItem } from '@tools/combobox/models';
import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';

export function dateToString(date: Date, format?: string) {
  if (format) {
    return moment(date).local().format(format);
  }
  return moment(date).local().format('DD/MM/YYYY HH:mm:ss');
}
export function dateToInputValue(date: Date): string {
  if (date == null) {
    return null;
  }
  return formatDate(date, 'yyyy-MM-dd', 'en');
}
export function dateToIsoString(date?: Date) {
  if (date) {
    return moment(date).toISOString();
  }
  return null;
}
export function isNullOrWhiteSpace(text: string): boolean {
  const clean = (text == null || text === undefined ? '' : text)
    .toString()
    .trim();
  return clean.length === 0;
}

export function clearFocus() {
  if (document.activeElement instanceof HTMLElement) {
    document.activeElement.blur();
  }
}

export function setProperty(
  path: string | string[],
  obj: any,
  value: any,
  separator = '.'
) {
  const properties = Array.isArray(path) ? path : path.split(separator);
  return properties.reduce(
    (o, p, i) => (o[p] = properties.length === ++i ? value : o[p] || {}),
    obj
  );
}

export function getItems(data: CustomSimpleListItemModel[], label = 'nombre') {
  return data.map(
    (value) =>
      new ComboboxItem({
        value: value.id,
        text: value[label],
        data: value
      })
  );
}

export function toDateStruct(date: Date, increment = 0): NgbDateStruct {
  return {
    year: date.getFullYear() + increment,
    month: date.getMonth() + 1,
    day: date.getDate()
  };
}
