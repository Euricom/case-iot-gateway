export class ChangePassword {
  New?: String
  Old?: String

  constructor(resource) {
    Object.assign(this, resource)
  }
}
