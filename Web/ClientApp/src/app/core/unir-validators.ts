import {
  AbstractControl,
  FormControl,
  FormGroup,
  ValidationErrors,
  ValidatorFn,
  Validators
} from '@angular/forms';

export class UnirValidators {
  static requiredAtLeastOne(controls: string[]): ValidatorFn {
    return (form: FormGroup): ValidationErrors | null => {
      let isInvalid = true;
      for (const controlKey of controls) {
        if (form.get(controlKey).value) {
          isInvalid = false;
          break;
        }
      }
      if (isInvalid) {
        return {
          requiredAtLeastOne: true
        };
      }
      return null;
    };
  }

  static unirEmail(control: AbstractControl): ValidationErrors | null {
    const format = /^[A-Za-z0-9._%-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,6}$/;
    const str = control.value;
    if (str?.match(format)) {
      return null;
    }
    return {
      unirEmail: true
    };
  }

  static noWhitespace(control: AbstractControl): ValidationErrors | null {
    if (Validators.required(control)) {
      return null;
    }
    if (control.value.replace(/\s/g, '').length > 0) {
      return null;
    }
    return { required: true };
  }

  static inputFileValidator(files: FileList, sizeMaxMB: number) {
    return function (control: FormControl): ValidationErrors | null {
      if (control.value === '' || !files.length) return null;
      const fileToUpload = files[0];
      const errors: string[] = [];
      const capacidad = 1024;
      const extensionNoEncontrada = -1;
      const validFormats = ['doc', 'docx', 'pdf', 'jpg', 'jpeg', 'png', 'gif'];
      const extension = fileToUpload.name.split('.').pop();
      if (
        extension === '' ||
        validFormats.indexOf(extension) === extensionNoEncontrada
      ) {
        errors.push('Extensión de archivo no permitido');
      }
      const fileSizeInMB = Math.round(
        fileToUpload.size / capacidad / capacidad
      );
      if (fileSizeInMB > sizeMaxMB) {
        errors.push(`El tamaño máximo es de ${sizeMaxMB} MB`);
      }
      return errors.length ? errors : null;
    };
  }
}
