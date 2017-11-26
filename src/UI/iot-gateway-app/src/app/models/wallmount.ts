import { Config } from '../../config'

export class Wallmount {
  DeviceId?: String
  Name?: String
  Type?: String
  NodeId?: Number
  PollingTime?: Number
  Enabled?: boolean

  constructor(resource) {
    Object.assign(this, resource)
  }
}
