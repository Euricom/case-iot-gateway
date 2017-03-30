import { Injectable } from '@angular/core'

@Injectable()
export class Config {
  public baseUrl: any
  public logLevel: String

  constructor() {
    this.InitDevConfig()
  }

  private InitDevConfig() {
    //this.baseUrl = 'http://localhost:8800/'
    this.baseUrl = 'http://10.0.1.31:8800'
    this.logLevel = 'debug'
  }

  private InitProdConfig() {
    this.logLevel = 'info'
  }
}
