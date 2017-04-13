export class User {
  Username?: String

  constructor(resource) {
    Object.assign(this, resource)
  }
}
