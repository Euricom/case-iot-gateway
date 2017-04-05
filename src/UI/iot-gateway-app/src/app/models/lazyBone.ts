import { Config } from '../../config'

export class LazyBone {
  DeviceId?: String
  Name?: String
  Type?: String
  Address?: String
  PollingTime?: Number
  Enabled?: boolean

  constructor(resource) {
    Object.assign(this, resource)
  }

  isNew(): boolean {
    return !(this.DeviceId)
  }
}
