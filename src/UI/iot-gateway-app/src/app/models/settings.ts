import { Config } from '../../config'

export class Settings {
  Password?: String
  LogLevel?: String
  HistoryLog?: Number
  GatewayDeviceKey?: String
  AzureIotHubUri?: String
  AzureIotHubUriConnectionString?: String
  AzureAccountName?: String
  AzureStorageAccessKey?: String
  DropboxAccessToken?: String

  constructor(resource) {
    Object.assign(this, resource)
  }
}
