import { Config } from '../../config'

export class LogLine {
  TimeStamp?: Date
  Level?: string
  MessageTemplate?: string
  Exception?: string

  constructor(resource) {
    Object.assign(this, resource)
  }
}
