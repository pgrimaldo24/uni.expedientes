import { Injectable } from '@angular/core';
import {
  AlertModel,
  AlertOptions,
  AlertType
} from '@src/app/component-tools/alert/models';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class AlertHandlerService {
  defaultDelay = 10000;
  constructor(private toastrService: ToastrService) {}

  exception(): void {
    this.sendToast('', '', AlertType.error, {
      timeOut: this.defaultDelay
    });
  }
  unauthorize(): void {
    this.sendToast('', '', AlertType.error, {
      timeOut: this.defaultDelay
    });
  }
  error(message: string, autoClose = true): void {
    const options: AlertOptions = autoClose
      ? { timeOut: this.defaultDelay }
      : { disableTimeOut: true };
    this.sendToast(message, '', AlertType.error, options);
  }
  warning(message: string, autoClose = true): void {
    const options: AlertOptions = autoClose
      ? { timeOut: this.defaultDelay }
      : { disableTimeOut: true };
    this.sendToast(message, '', AlertType.warning, options);
  }
  info(message: string, autoClose = true): void {
    const options: AlertOptions = autoClose
      ? { timeOut: this.defaultDelay }
      : { disableTimeOut: true };
    this.sendToast(message, '', AlertType.info, options);
  }
  success(message: string, autoClose = true): void {
    const options: AlertOptions = autoClose
      ? { timeOut: this.defaultDelay }
      : { disableTimeOut: true };
    this.sendToast(message, '', AlertType.success, options);
  }
  custom(data: Partial<AlertModel>): void {
    const title = data.title || '';
    const options: AlertOptions = data.autoClose
      ? { timeOut: data.delay || this.defaultDelay }
      : { disableTimeOut: true };

    if (data.messages.length > 0) {
      // caso cuando hay array de mensajes
      const msg = data.messages.join('<br>');
      options.enableHtml = true;
      this.sendToast(msg, title, data.type, options);
    } else {
      // caso cuando no hay array de mensajes
      const msg = data.message || 'ERROR';
      this.sendToast(msg, title, data.type, options);
    }
  }

  private sendToast(
    _message: string,
    _title: string,
    _type: AlertType,
    _options: AlertOptions
  ): void {
    switch (_type) {
      case AlertType.error:
        _options['toastClass'] = 'myConfigToast';
        this.toastrService
          .error(_message, _title, _options)
          .onTap.subscribe(() => this.toasterClickedHandler(_message));
        break;
      case AlertType.info:
        _options['toastClass'] = 'myConfigToast';
        this.toastrService.info(_message, _title, _options);
        break;
      case AlertType.success:
        this.toastrService.success(_message, _title, _options);
        break;
      case AlertType.warning:
        _options['toastClass'] = 'myConfigToast';
        this.toastrService.warning(_message, _title, _options);
        break;
      default:
        this.toastrService.info(_message, _title, _options);
        break;
    }
  }

  toasterClickedHandler(message: string): void {
    navigator.clipboard.writeText(message);
  }

  clearAllToast(): void {
    this.toastrService.clear();
  }
}
