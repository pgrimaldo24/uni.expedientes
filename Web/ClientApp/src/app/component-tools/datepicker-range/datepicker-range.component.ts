import { Component, forwardRef } from '@angular/core';
import { NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';

import {
  NgbDate,
  NgbCalendar,
  NgbDateParserFormatter,
  NgbDatepickerI18n
} from '@ng-bootstrap/ng-bootstrap';

import { DateRangeModel } from './models/index';
import {
  CustomDateParserFormatter,
  CustomDatepickerI18n,
  I18n
} from './datepicker-range.service';

@Component({
  selector: 'app-datepicker-range',
  templateUrl: './datepicker-range.component.html',
  styleUrls: ['./datepicker-range.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => DatepickerRangeComponent),
      multi: true
    },
    I18n,
    { provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter },
    { provide: NgbDatepickerI18n, useClass: CustomDatepickerI18n }
  ]
})
export class DatepickerRangeComponent implements ControlValueAccessor {
  hoveredDate: NgbDate | null = null;

  fromDate: NgbDate | null;
  toDate: NgbDate | null;

  onChange = Function.prototype;
  onTouched = Function.prototype;
  isDisabled: boolean;
  currentValue: DateRangeModel;

  constructor(
    private calendar: NgbCalendar,
    public formatter: NgbDateParserFormatter
  ) {
    this.currentValue = null;
    this.fromDate = null;
    this.toDate = null;
  }
  writeValue(value: DateRangeModel): void {
    this.currentValue = value;
  }
  registerOnChange(fn: () => Record<string, never>): void {
    this.onChange = fn;
  }
  registerOnTouched(fn: () => Record<string, never>): void {
    this.onTouched = fn;
  }
  setDisabledState?(state: boolean): void {
    this.isDisabled = state;
  }

  onDateSelection(date: NgbDate): void {
    if (!this.fromDate && !this.toDate) {
      this.fromDate = date;
    } else if (
      this.fromDate &&
      !this.toDate &&
      date &&
      date.after(this.fromDate)
    ) {
      this.toDate = date;
    } else {
      this.toDate = null;
      this.fromDate = date;
    }
    this.writeValue({
      from: this.parseToDate(this.fromDate),
      to: this.parseToDate(this.toDate)
    });
    this.onChange(this.currentValue);
    this.onTouched();
  }

  parseToDate(date: NgbDate | null): Date | null {
    if (date == null) {
      return null;
    }
    return new Date(date.year, date.month - 1, date.day);
  }

  isHovered(date: NgbDate): boolean {
    return (
      this.fromDate &&
      !this.toDate &&
      this.hoveredDate &&
      date.after(this.fromDate) &&
      date.before(this.hoveredDate)
    );
  }

  isInside(date: NgbDate): boolean {
    return this.toDate && date.after(this.fromDate) && date.before(this.toDate);
  }

  isRange(date: NgbDate): boolean {
    return (
      date.equals(this.fromDate) ||
      (this.toDate && date.equals(this.toDate)) ||
      this.isInside(date) ||
      this.isHovered(date)
    );
  }

  validateInput(currentValue: NgbDate | null, input: string): NgbDate | null {
    const parsed = this.formatter.parse(input);
    return parsed && this.calendar.isValid(NgbDate.from(parsed))
      ? NgbDate.from(parsed)
      : currentValue;
  }
}
