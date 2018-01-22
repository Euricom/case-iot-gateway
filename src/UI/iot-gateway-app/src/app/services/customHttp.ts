import { Injectable } from '@angular/core'
import { Observable } from 'rxjs/Observable'
import 'rxjs/add/operator/map'
import 'rxjs/add/operator/catch'
import 'rxjs/add/operator/do'
import 'rxjs/add/operator/finally'
import 'rxjs/add/observable/throw'
import { Http, XHRBackend, RequestOptions, Request, RequestOptionsArgs, Response, Headers } from '@angular/http'
import { EventAggregator } from './eventAggregator'
// declare var $: any

const mapMethods = {
    '0': 'GET',
    '1': 'POST',
    '2': 'PUT',
    '3': 'DELETE',
}

@Injectable()
export class CustomHttpService extends Http {
    public pendingRequests = 0
    public showLoading = false

    constructor(
        backend: XHRBackend,
        defaultOptions: RequestOptions,
        private eventAggregator: EventAggregator) {
        super(backend, defaultOptions)
    }

    request(request: Request, options?: RequestOptionsArgs): Observable<Response> {
        request.url = "http://192.168.40.185:8800" + request.url;
        console.info(`HTTP: ${mapMethods[request.method]}: ${request.url}`)
        return super.request(request, options)
            .catch((errorRes) => {
                console.error('ERROR: ', errorRes.statusText, errorRes.status)
                let errorMessage = `${errorRes.json().Message}`
                if (errorRes.status === 0) {
                    errorMessage = `Failed to connect to server. Bad connectivity or server down.`
                }
                this.eventAggregator.publish('ERROR', errorMessage)
                return Observable.throw(errorMessage)
            })
    }

    // intercept(observable: Observable<Response>): Observable<Response> {
    //   this.pendingRequests++
    //   return observable
    //     .catch(this.handleError)
    //     .do((res: Response) => {
    //       this.toastr.info('test')
    //     }, (err: any) => {
    //       // this.toastr.error(`Error: ${err}`)
    //     })
    //     .finally(() => {
    //     })
    // }

    // turnOnModal() {
    //   if (!this.showLoading) {
    //     this.showLoading = true
    //     $('body').spin('modal', '#FFFFFF', 'rgba(51, 51, 51, 0.1)')
    //     console.log('Turned on modal')
    //   }
    // }

    // private turnOffModal() {
    //   this.pendingRequests--
    //   if (this.pendingRequests <= 0) {
    //     if (this.showLoading) {
    //       $('body').spin('modal', '#FFFFFF', 'rgba(51, 51, 51, 0.1)')
    //     }
    //     this.showLoading = false
    //   }
    //   console.log('Turned off modal')
    // }

    // handleError(errorRes: Response, source: any): Observable<Response> {
    //   // alert(errorRes)
    //   //this.toastr.error(errorRes.statusText)
    //   return Observable.throw(errorRes)
    // }
}
