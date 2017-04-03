import { Config } from '../../config'

export class Camera {
  DeviceId?: String
  Name?: String
  Type?: String
  Address?: String
  Username?: String
  Password?: String
  Enabled?: boolean

  constructor(resource) {
    Object.assign(this, resource)
  }

  isNew(): boolean {
    return !(this.DeviceId)
  }
}
