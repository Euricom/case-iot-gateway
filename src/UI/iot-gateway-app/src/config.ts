import { Injectable } from '@angular/core'

@Injectable()
export class Config {
  public baseUrl: any

  constructor() {
    this.InitDevConfig()
  }

  private InitDevConfig() {
    this.baseUrl = 'http://10.0.1.101:8800'
  }

  private InitProdConfig() {
    this.baseUrl = 'http://10.0.1.101:8800'
  }
}
