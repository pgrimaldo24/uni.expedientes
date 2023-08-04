import { Component, forwardRef, Input, OnInit, ViewChild } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import {
  NgbCalendar,
  NgbDateParserFormatter,
  NgbDatepickerI18n,
  NgbDateStruct,
  NgbInputDatepicker
} from '@ng-bootstrap/ng-bootstrap';
import {
  CustomDateParserFormatter,
  CustomDatepickerI18n,
  I18n
} from './datepicker.service';

@Component({
  selector: 'app-datepicker',
  templateUrl: './datepicker.component.html',
  styleUrls: ['./datepicker.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => DatepickerComponent),
      multi: true
    },
    I18n,
    { provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter },
    { provide: NgbDatepickerI18n, useClass: CustomDatepickerI18n }
  ]
})
export class DatepickerComponent implements OnInit, ControlValueAccessor {
  selectedDate: NgbDateStruct | null;
  hoy = this.calendar.getToday();

  onChange = Function.prototype;
  onTouched = Function.prototype;
  isDisabled: boolean;
  currentValue: Date | null;
  @Input() max: NgbDateStruct;
  @Input() min: NgbDateStruct;
  @Input() disable = false;
  @ViewChild('d') datepicker: NgbInputDatepicker;
  constructor(private calendar: NgbCalendar) {}
  ngOnInit(): void {}
  writeValue(value: Date | null): void {
    this.currentValue = value;
    if (value) {
      this.selectedDate = {
        day: value.getDate(),
        month: value.getMonth() + 1,
        year: value.getFullYear()
      };
    } else {
      this.selectedDate = null;
    }
  }

  get maxDate(): NgbDateStruct {
    return this.max;
  }

  get minDate(): NgbDateStruct {
    return this.min;
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

  parseToDate(date: NgbDateStruct | null): Date | null {
    if (date == null) {
      return null;
    }
    return new Date(date.year, date.month - 1, date.day);
  }

  dateSelected(): void {
    this.writeValue(this.parseToDate(this.selectedDate));
    this.onChange(this.currentValue);
    this.onTouched();
  }

  setToday(): void {
    this.selectedDate = this.hoy;
    this.dateSelected();
    this.datepicker.close();
  }

  clear(): void {
    this.selectedDate = null;
    this.dateSelected();
    this.datepicker.close();
  }
}
