export const keys: AppConfiguration = {
  ADMIN_ROLE: 'admin_expediente',
  ADMIN_ROLE_NAME: 'Administrador de Expedientes',
  LECTOR_ROLE: 'lector_expediente',
  LECTOR_ROLE_NAME: 'Lector de Expedientes',
  GESTOR_ROLE: 'gestor_expediente',
  GESTOR_ROLE_NAME: 'Gestor de Expedientes',
  API_BASE_URL: window.location.origin
};

export interface AppConfiguration {
  ADMIN_ROLE: string;
  ADMIN_ROLE_NAME: string;
  LECTOR_ROLE: string;
  LECTOR_ROLE_NAME: string;
  GESTOR_ROLE: string;
  GESTOR_ROLE_NAME: string;
  API_BASE_URL: string;
}
