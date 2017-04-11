
export class DeviceType {
  Id?: String
  Name?: String

  constructor(resource) {
    Object.assign(this, resource)
  }
}
