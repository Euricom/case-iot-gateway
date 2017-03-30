import { Config } from '../../config'

export class Device {
  guid?: String
  name?: String
  type?: String

  constructor(resource) {
    Object.assign(this, resource)
  }

  isNew(): boolean {
    return !(this.guid)
  }
}
