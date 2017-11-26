import { Config } from '../../config'

export class Camera {
  DeviceId?: String
  Name?: String
  Type?: String
  Address?: String
  PollingTime?: Number
  MaximumDaysDropbox?: Number
  MaximumStorageDropbox?: Number
  MaximumDaysAzureBlobStorage?: Number
  Enabled?: boolean

  constructor(resource) {
    Object.assign(this, resource)
  }
}
