import { Pipe, PipeTransform } from '@angular/core';
import * as moment from 'moment';

@Pipe({
  name: 'dateToLocalString'
})
export class DateToLocalStringPipe implements PipeTransform {
  transform(date: string, format?: string): string {
    if (!date || date == null) return '-';
    if (date.substr(date.length - 1, 1) != 'Z') date += 'Z';
    return moment(date)
      .locale('es')
      .local()
      .format(format ? format : 'DD/MM/YYYY HH:mm:ss');
  }
}

@Pipe({
  name: 'dateString'
})
export class DateStringPipe implements PipeTransform {
  transform(date: string, format?: string): string {
    if (!date || date == null) return '-';
    if (date.substr(date.length - 1, 1) == 'Z') date = date.replace('Z', '');
    return moment(date)
      .locale('es')
      .local()
      .format(format ? format : 'DD/MM/YYYY HH:mm:ss');
  }
}
