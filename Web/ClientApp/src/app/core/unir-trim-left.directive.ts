import { Directive, ElementRef, HostListener, Input } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Directive({
  selector: '[UnirTrimLeft]'
})
export class UnirTrimLeftDirective {
  @Input() form: FormGroup;
  @Input() formControlName: string;
  constructor(private el: ElementRef) {}

  @HostListener('input', ['$event'])
  onInputTrim(): void {
    const targetValue = this.el.nativeElement.value?.trimStart();
    if (this.form) {
      this.form.controls[this.formControlName].setValue(targetValue);
    }
    this.el.nativeElement.value = targetValue;
  }
}
