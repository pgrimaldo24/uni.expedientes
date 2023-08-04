import { Injectable } from '@angular/core';

import {
  NgbDateParserFormatter,
  NgbDatepickerI18n,
  NgbDateStruct
} from '@ng-bootstrap/ng-bootstrap';

@Injectable()
export class CustomDateParserFormatter extends NgbDateParserFormatter {
  readonly DELIMITER = '/';

  parse(value: string): NgbDateStruct | null {
    if (value) {
      const date = value.split(this.DELIMITER);
      return {
        day: parseInt(date[0], 10),
        month: parseInt(date[1], 10),
        year: parseInt(date[2], 10)
      };
    }
    return null;
  }

  format(date: NgbDateStruct | null): string {
    return date
      ? date.day + this.DELIMITER + date.month + this.DELIMITER + date.year
      : '';
  }
}

const I18N_VALUES = {
  es: {
    weekDays: ['Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sa', 'Do'],
    months: [
      'Enero',
      'Febrero',
      'Marzo',
      'Abril',
      'Mayo',
      'Junio',
      'Julio',
      'Agosto',
      'Septiembre',
      'Octubre',
      'Noviembre',
      'Diciembre'
    ],
    shortNameMonths: [
      'Ene',
      'Feb',
      'Mar',
      'Abr',
      'May',
      'Jun',
      'Jul',
      'Ago',
      'Sep',
      'Oct',
      'Nov',
      'Dic'
    ]
  }
};

@Injectable()
export class I18n {
  language = 'es';
}

@Injectable()
export class CustomDatepickerI18n extends NgbDatepickerI18n {
  constructor(private i18n: I18n) {
    super();
  }

  getWeekdayShortName(weekday: number): string {
    return I18N_VALUES[this.i18n.language].weekDays[weekday - 1];
  }
  getMonthShortName(month: number): string {
    return I18N_VALUES[this.i18n.language].shortNameMonths[month - 1];
  }
  getMonthFullName(month: number): string {
    return I18N_VALUES[this.i18n.language].months[month - 1];
  }

  getDayAriaLabel(date: NgbDateStruct): string {
    return `${date.day}-${date.month}-${date.year}`;
  }
}
