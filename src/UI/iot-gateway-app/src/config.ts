import { Injectable } from '@angular/core'

@Injectable()
export class Config {
  public baseUrl: any
  public logLevel: String

  constructor() {
    this.InitDevConfig()
  }

  private InitDevConfig() {
    this.baseUrl = 'http://10.0.1.124:8800'
    this.logLevel = 'debug'
  }

  private InitProdConfig() {
    this.baseUrl = 'http://10.0.1.124:8800'
    this.logLevel = 'info'
  }
}
