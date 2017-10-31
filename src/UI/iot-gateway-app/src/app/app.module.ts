import { BrowserModule } from '@angular/platform-browser'
import { FormsModule } from '@angular/forms'
import { NgModule } from '@angular/core'
import { RouterModule } from '@angular/router'
import { ToastModule } from 'ng2-toastr/ng2-toastr'
import { ToastOptions } from 'ng2-toastr'
import { ButtonsModule, TabsModule } from 'ng2-bootstrap'
import { HttpModule, Http, XHRBackend, RequestOptions } from '@angular/http'
import { DatePickerModule } from 'ng2-datepicker'
import { CollapseModule } from 'ng2-bootstrap/collapse'

import '../styles.less'
import { routes } from './app.routes'

import { Config } from '../config'

import { AuthService } from './services/authService'
import { ZwaveService } from './services/zwaveService'
import { SettingsService } from './services/settingsService'
import { CameraService } from './services/cameraService'
import { LazyBoneService } from './services/lazyBoneService'
import { DanaLockService } from './services/danaLockService'
import { WallmountService } from './services/wallmountService'
import { LogService } from './services/logService'

import { AppComponent } from './app.component'
import { UnauthorizedComponent } from './components/unauthorized/unauthorized.component'
import { PageNotFoundComponent } from './components/pageNotFound/pageNotFound.component'
import { NavigationComponent } from './components/navigation/navigation.component'

import { LoginViewComponent } from './views/login/loginView.component'
import { SettingsViewComponent } from './views/settings/settingsView.component'
import { ZwaveViewComponent } from './views/zwave/zwaveView.component'
import { CameraViewComponent } from './views/camera/cameraView.component'
import { LazyBonesViewComponent } from './views/lazybone/lazyBoneView.component'
import { DanaLocksViewComponent } from './views/danalocks/danalocksView.component'
import { WallMountViewComponent } from './views/wallmount-switches/wallmountView.component'
import { LogViewComponent } from './views/log/logView.component'
import { OpenZWaveLogViewComponent } from './views/openzwavelog/openzwavelogView.component'

import { AuthModule } from './app.auth.module'
import { CustomErrorHandler } from './app.error.module'
import { AuthGuardService } from './services/authGuardService'
import { CustomHttpService } from './services/customHttp'

import { EventAggregator } from './services/eventAggregator'

export function httpFactory(backend: XHRBackend, options, eventAggregator: EventAggregator) {
  return new CustomHttpService(backend, options, eventAggregator)
}

@NgModule({
  declarations: [
    AppComponent,
    UnauthorizedComponent,
    PageNotFoundComponent,
    NavigationComponent,
    LoginViewComponent,
    SettingsViewComponent,
    ZwaveViewComponent,
    CameraViewComponent,
    LazyBonesViewComponent,
    DanaLocksViewComponent,
    WallMountViewComponent,
    LogViewComponent,
    OpenZWaveLogViewComponent,
  ],
  imports: [
    BrowserModule,
    FormsModule,
    RouterModule.forRoot(routes),
    ButtonsModule.forRoot(),
    CollapseModule,
    TabsModule.forRoot(),
    ToastModule.forRoot(),
    HttpModule,
    DatePickerModule,
    AuthModule,
    CustomErrorHandler,
  ],
  providers: [
    Config,
    AuthService,
    AuthGuardService,
    ZwaveService,
    SettingsService,
    CameraService,
    LazyBoneService,
    DanaLockService,
    WallmountService,
    LogService,
    EventAggregator,
    { provide: CustomErrorHandler, useClass: CustomErrorHandler },
    {
      provide: Http,
      useFactory: httpFactory,
      deps: [XHRBackend, RequestOptions, EventAggregator],
    },
  ],
  bootstrap: [AppComponent],
})
export class AppModule { }
