import { Routes } from '@angular/router'

// components
import { PageNotFoundComponent } from './components/pageNotFound/pageNotFound.component'
import { SettingsViewComponent } from './views/settings/settingsView.component'
import { HardwareViewComponent } from './views/hardware/hardwareView.component'
import { CameraViewComponent } from './views/camera/cameraView.component'
import { LazyBonesViewComponent } from './views/lazybone/lazyBoneView.component'
import { DanaLocksViewComponent } from './views/danalocks/danalocksView.component'
import { LogViewComponent } from './views/log/logView.component'

// resolvers

export const routes: Routes = [
  { path: 'settings', component: SettingsViewComponent },
  { path: 'hardware', component: HardwareViewComponent },
  { path: 'cameras', component: CameraViewComponent },
  { path: 'switches', component: LazyBonesViewComponent },
  { path: 'danalocks', component: DanaLocksViewComponent },
  { path: 'log', component: LogViewComponent },
  { path: '**', component: PageNotFoundComponent },
]
