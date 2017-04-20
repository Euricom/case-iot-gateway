import { Config } from '../../config'

export class LazyBone {
  DeviceId?: String
  Name?: String
  IsDimmer?: boolean
  Type?: String
  Address?: String
  Port?: Number
  PollingTime?: Number
  Enabled?: boolean

  constructor(resource) {
    Object.assign(this, resource)
  }

  isNew(): boolean {
    return !(this.DeviceId)
  }
}
