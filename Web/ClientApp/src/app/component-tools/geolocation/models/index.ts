import { ConfigureCombobox } from '../../combobox/models';

export interface Division {
  code: string;
  countryCode: string;
  isPreferred: boolean;
  level: number;
  divisionLevel: number;
  name: string;
  value: any;
  divisionConfig: ConfigureCombobox;
}

export class Country {
  isoCode: string;
  name: string;
  iso3166Alpha3: string;
  iso3166Numeric: string;
  phoneCode: string;
  preferredDivisions: string[];
  divisions: LevelDivision[];
}

export interface LevelDivision {
  code: string;
  countryCode: string;
  countryName: string;
  divisionCode: string;
  divisionLevel: number;
  divisionName: string;
  isPreferredDivision: boolean;
  isoCode: string;
  localCode: string;
  name: string;
  parentEntityCode: string;
}

export enum GeolocationControl {
  country = 'country',
  divisions = 'divisions',
  value = 'value'
}
