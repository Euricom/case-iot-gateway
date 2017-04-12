import { Routes } from '@angular/router'

// components
import { UnauthorizedComponent } from './components/unauthorized/unauthorized.component'
import { PageNotFoundComponent } from './components/pageNotFound/pageNotFound.component'
import { LoginViewComponent } from './views/login/loginView.component'
import { SettingsViewComponent } from './views/settings/settingsView.component'
import { HardwareViewComponent } from './views/hardware/hardwareView.component'
import { CameraViewComponent } from './views/camera/cameraView.component'
import { LazyBonesViewComponent } from './views/lazybone/lazyBoneView.component'
import { DanaLocksViewComponent } from './views/danalocks/danalocksView.component'
import { LogViewComponent } from './views/log/logView.component'

import { AuthGuardService } from './services/authGuardService'

// resolvers: TODO add resolvers

// routing

export const routes: Routes = [
  { path: '', component: LoginViewComponent },
  { path: 'unauthorized', component: UnauthorizedComponent },
  { path: 'login', component: LoginViewComponent },
  { path: 'settings', component: SettingsViewComponent, canActivate: [AuthGuardService] },
  { path: 'hardware', component: HardwareViewComponent, canActivate: [AuthGuardService] },
  { path: 'cameras', component: CameraViewComponent, canActivate: [AuthGuardService] },
  { path: 'switches', component: LazyBonesViewComponent, canActivate: [AuthGuardService] },
  { path: 'danalocks', component: DanaLocksViewComponent, canActivate: [AuthGuardService] },
  { path: 'log', component: LogViewComponent, canActivate: [AuthGuardService] },
  { path: '**', component: PageNotFoundComponent },
]
