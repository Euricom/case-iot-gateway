import { Injectable } from '@angular/core'
import { Observable } from 'rxjs/Observable'
import 'rxjs/add/operator/map'
import 'rxjs/add/operator/catch'
import 'rxjs/add/operator/do'
import 'rxjs/add/operator/finally'
import 'rxjs/add/observable/throw'
import { Http, XHRBackend, RequestOptions, Request, RequestOptionsArgs, Response, Headers } from '@angular/http'
import { EventAggregator } from './eventAggregator'
import { AuthService } from './authService';
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
        //request.url = "http://192.168.40.184:8800" + request.url;
        console.info(`HTTP: ${mapMethods[request.method]}: ${request.url}`)
        return super.request(request, options)
            .catch((response) => {
                return Observable.throw(response)
            })
    }
}
