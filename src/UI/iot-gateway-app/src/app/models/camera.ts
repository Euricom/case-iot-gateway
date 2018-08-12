import { Config } from '../../config'

export class Camera {
  DeviceId?: String
  Name?: String
  MotionEyeIdentifier?: String
  Type?: String
  Address?: String
  PollingTime?: Number
  Enabled?: boolean

  constructor(resource) {
    Object.assign(this, resource)
  }
}
