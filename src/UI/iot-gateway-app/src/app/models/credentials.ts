export class Credentials {
  Username?: String
  Password?: String

  constructor(resource) {
    Object.assign(this, resource)
  }
}
