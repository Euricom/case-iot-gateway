import { Routes } from '@angular/router'

// components
import { UnauthorizedComponent } from './components/unauthorized/unauthorized.component'
import { PageNotFoundComponent } from './components/pageNotFound/pageNotFound.component'
import { LoginViewComponent } from './views/login/loginView.component'
import { SettingsViewComponent } from './views/settings/settingsView.component'
import { ZwaveViewComponent } from './views/zwave/zwaveView.component'
import { CameraViewComponent } from './views/camera/cameraView.component'
import { LazyBonesViewComponent } from './views/lazybone/lazyBoneView.component'
import { DanaLocksViewComponent } from './views/danalocks/danalocksView.component'
import { WallMountViewComponent } from './views/wallmount-switches/wallmountView.component'
import { LogViewComponent } from './views/log/logView.component'
import { OpenZWaveLogViewComponent } from './views/openzwavelog/openzwavelogView.component'

import {
  AuthGuardService
} from './services/authGuardService'
import { UsersViewComponent } from './views/users/usersView.component';
import { HomeViewComponent } from './views/home/homeView.component';
import { AccountViewComponent } from './views/account/accountView.component';

// resolvers: TODO add resolvers

// routing

export const routes: Routes = [
  { path: '', component: HomeViewComponent },
  { path: 'unauthorized', component: UnauthorizedComponent },
  { path: 'login', component: LoginViewComponent },
  { path: 'settings', component: SettingsViewComponent, canActivate: [AuthGuardService], data: ['Administrator'] },
  { path: 'account', component: AccountViewComponent, canActivate: [AuthGuardService], data: ['Administrator', 'Manager', 'User'] },
  { path: 'users', component: UsersViewComponent, canActivate: [AuthGuardService], data: ['Manager'] },
  { path: 'zwave', component: ZwaveViewComponent, canActivate: [AuthGuardService], data: ['Manager'] },
  { path: 'cameras', component: CameraViewComponent, canActivate: [AuthGuardService], data: ['Manager'] },
  { path: 'switches', component: LazyBonesViewComponent, canActivate: [AuthGuardService], data: ['Manager'] },
  { path: 'wallmounts', component: WallMountViewComponent, canActivate: [AuthGuardService], data: ['Manager'] },
  { path: 'danalocks', component: DanaLocksViewComponent, canActivate: [AuthGuardService], data: ['Manager'] },
  { path: 'log', component: LogViewComponent, canActivate: [AuthGuardService], data: ['Manager'] },
  { path: 'openzwavelog', component: OpenZWaveLogViewComponent, canActivate: [AuthGuardService], data: ['Manager'] },
  { path: '**', component: PageNotFoundComponent },
]
