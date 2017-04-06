import { Config } from '../../config'
import { LogLine } from './logLine'

export class Log {
  FileName?: String
  LogLines: LogLine[]

  constructor(resource) {
    Object.assign(this, resource)
  }
}
