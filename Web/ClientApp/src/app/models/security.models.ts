export class IdentityClaimsModel {
  name: string;
  sub: string;
  // tslint:disable-next-line: variable-name
  given_name: string;
  roles: [string];
  info: Info;
}

export class Info {
  name: string;
  sub: string;
  // tslint:disable-next-line: variable-name
  given_name: string;
  roles: [string];
}
