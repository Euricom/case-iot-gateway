import { NgModule, ErrorHandler, OnInit, Injector } from '@angular/core'
import { Response } from '@angular/http'
import { Router } from '@angular/router'

@NgModule({
  providers: [{ provide: ErrorHandler, useClass: MyErrorHandler }],
})
export class MyErrorHandler implements OnInit, ErrorHandler {

  router: Router

  constructor(private injector: Injector) {
    this.router = this.injector.get(Router)
  }

  ngOnInit() {

  }

  handleError(error: Response) {
    switch (error.status) {
      case 401:
        if (this.router) {
          this.router.navigateByUrl('/unauthorized')
        }
        break
      default:
        alert('error: ' + error)
    }
  }
}
