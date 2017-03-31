import { Config } from '../../config'

export class Device {
  DeviceId?: String
  Name?: String
  Type?: String

  constructor(resource) {
    Object.assign(this, resource)
  }

  isNew(): boolean {
    return !(this.DeviceId)
  }
}
