
export class DeviceType {
  id?: String
  name?: String

  constructor(resource) {
    Object.assign(this, resource)
  }
}
