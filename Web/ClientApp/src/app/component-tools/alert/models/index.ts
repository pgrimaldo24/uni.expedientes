export interface AlertModel {
  id?: string;
  type: AlertType;
  message: string;
  messages: string[];
  autoClose?: boolean;
  delay?: number;
  title?: string;
}

export enum AlertType {
  error,
  info,
  success,
  warning
}

export interface AlertOptions {
  timeOut?: number;
  disableTimeOut?: boolean;
  enableHtml?: boolean;
}
