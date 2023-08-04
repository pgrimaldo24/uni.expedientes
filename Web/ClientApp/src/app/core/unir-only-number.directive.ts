import {
  Directive,
  ElementRef,
  forwardRef,
  HostListener,
  Renderer2,
  StaticProvider
} from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

export const INPUT_NUMBER_CONTROL_VALUE_ACCESSOR: StaticProvider = {
  provide: NG_VALUE_ACCESSOR,
  useExisting: forwardRef(() => UnirOnlyNumberDirective),
  multi: true
};

@Directive({
  selector: '[UnirOnlyNumber]',
  providers: [INPUT_NUMBER_CONTROL_VALUE_ACCESSOR]
})
export class UnirOnlyNumberDirective implements ControlValueAccessor {
  private onChange: (val: string) => void;
  private onTouched: () => void;
  private value: string;

  constructor(private elementRef: ElementRef, private renderer: Renderer2) {}

  @HostListener('input', ['$event.target.value'])
  onInputChange(value: string): void {
    const filteredValue: string = this.filterValue(value);
    this.updateTextInput(filteredValue, this.value !== filteredValue);
  }

  @HostListener('blur')
  onBlur(): void {
    this.onTouched();
  }

  private updateTextInput(value: string, propagateChange: boolean) {
    this.renderer.setProperty(this.elementRef.nativeElement, 'value', value);
    if (propagateChange) {
      this.onChange(value);
    }
    this.value = value;
  }

  registerOnChange(fn: () => Record<string, never>): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => Record<string, never>): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.renderer.setProperty(
      this.elementRef.nativeElement,
      'disabled',
      isDisabled
    );
  }

  writeValue(value: string): void {
    value = value ? String(value) : '';
    this.updateTextInput(value, false);
  }

  filterValue(value: string): string {
    return value.replace(/[^0-9]*/g, '');
  }
}
