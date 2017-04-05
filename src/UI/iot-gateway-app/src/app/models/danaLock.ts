import { Config } from '../../config'

export class DanaLock {
  DeviceId?: String
  Name?: String
  Type?: String
  NodeId?: Number
  Enabled?: boolean
  PollingTime?: Number

  constructor(resource) {
    Object.assign(this, resource)
  }

  isNew(): boolean {
    return !(this.DeviceId)
  }
}
