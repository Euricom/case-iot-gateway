import { NgModule, ErrorHandler, OnInit, Injector } from '@angular/core'
import { Response } from '@angular/http'
import { Router } from '@angular/router'
import { ToastsManager } from 'ng2-toastr/ng2-toastr'

@NgModule({
  providers: [{ provide: CustomErrorHandler, useClass: CustomErrorHandler }],
})
export class CustomErrorHandler implements OnInit, ErrorHandler {

  router: Router

  constructor(
    private injector: Injector,
    private toastr: ToastsManager) {
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
        this.toastr.error(error.statusText)
    }
  }
}
