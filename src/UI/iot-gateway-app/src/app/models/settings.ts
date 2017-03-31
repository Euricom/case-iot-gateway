import { Config } from '../../config'

export class Settings {
  historyLog?: Number
  azureIotHubUri?: String
  azureIotHubUriConnectionString?: String
  azureAccountName?: String
  azureStorageAccessKey?: String
  dropboxAccessToken?: String

  constructor(resource) {
    Object.assign(this, resource)
  }
}
