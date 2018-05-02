export class User {
  Username?: String;
  AccessToken?: String;

  Roles: string[];

  constructor(resource) {
    Object.assign(this, resource)
  }
}
