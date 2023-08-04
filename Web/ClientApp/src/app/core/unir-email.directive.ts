import { Directive } from '@angular/core';
import {
  AbstractControl,
  NG_VALIDATORS,
  ValidationErrors,
  Validator,
  ValidatorFn
} from '@angular/forms';

@Directive({
  selector: '[unirEmail]',
  providers: [
    {
      provide: NG_VALIDATORS,
      useExisting: UnirEmailDirective,
      multi: true
    }
  ]
})
export class UnirEmailDirective implements Validator {
  private readonly EMAIL_PATTERN = /^[A-Za-z0-9._%-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,6}$/;
  validate(control: AbstractControl): ValidationErrors | null {
    return this.createUnirEmailValidator()(control);
  }

  createUnirEmailValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const value = control.value;
      if (!value) {
        return null;
      }
      if (value.match(this.EMAIL_PATTERN)) {
        return null;
      }
      return {
        unirEmail: true
      };
    };
  }
}
