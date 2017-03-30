import { BrowserModule } from '@angular/platform-browser'
import { FormsModule } from '@angular/forms'
import { NgModule } from '@angular/core'
import { RouterModule } from '@angular/router'
import { ToastModule } from 'ng2-toastr/ng2-toastr'
import { ToastOptions } from 'ng2-toastr'
import { ButtonsModule, TabsModule } from 'ng2-bootstrap'
import { HttpModule, Http, XHRBackend, RequestOptions } from '@angular/http'

import '../styles.less'
import { routes } from './app.routes'

import { AppComponent } from './app.component'
import { PageNotFoundComponent } from './components/pageNotFound/pageNotFound.component'
import { NavigationComponent } from './components/navigation/navigation.component'

import { SettingsViewComponent } from './views/settings/settingsView.component'
import { HardwareViewComponent } from './views/hardware/hardwareView.component'
import { CameraViewComponent } from './views/camera/cameraView.component'
import { LazyBonesViewComponent } from './views/lazybone/lazyBoneView.component'
import { DanaLocksViewComponent } from './views/danalocks/danalocksView.component'
import { LogViewComponent } from './views/log/logView.component'

import { CollapseModule } from 'ng2-bootstrap/collapse'

@NgModule({
  declarations: [
    AppComponent,
    PageNotFoundComponent,
    NavigationComponent,
    SettingsViewComponent,
    HardwareViewComponent,
    CameraViewComponent,
    LazyBonesViewComponent,
    DanaLocksViewComponent,
    LogViewComponent,
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
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
