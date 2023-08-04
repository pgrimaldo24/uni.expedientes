import { Directive, ElementRef, HostListener, Renderer2 } from '@angular/core';
import { NgModel } from '@angular/forms';

@Directive({
  selector: '[UnirTrimLeftModel]'
})
export class UnirTrimLeftModelDirective {
  constructor(
    private renderer: Renderer2,
    private elementRef: ElementRef,
    private ngModel: NgModel
  ) {}

  @HostListener('input', ['$event.target.value'])
  onBlur(val: string): void {
    const value = val?.trimStart();
    if (value) {
      this.renderer.setProperty(this.elementRef.nativeElement, 'value', value);
      this.renderer.setAttribute(this.elementRef.nativeElement, 'value', value);
      this.ngModel.update.emit(value);
    } else {
      this.renderer.setProperty(this.elementRef.nativeElement, 'value', null);
      this.renderer.setAttribute(this.elementRef.nativeElement, 'value', null);
      this.ngModel.update.emit('');
    }
  }
}
