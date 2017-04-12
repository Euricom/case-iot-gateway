import { OnInit, Component } from '@angular/core'
import { ActivatedRoute, Router } from '@angular/router'
import { ToastsManager } from 'ng2-toastr/ng2-toastr'
import { Location } from '@angular/common'
import { Settings } from '../../models/settings';
import { SettingsService } from '../../services/settingsService'
import { LogLevel } from '../../models/logLevel';

@Component({
  selector: 'overview',
  templateUrl: './settingsView.component.html',
})
export class SettingsViewComponent implements OnInit {

  settings: Settings
  logLevelOptions: string[]
  logLevelsEnum: typeof LogLevel = LogLevel

  constructor(private router: Router,
    private route: ActivatedRoute,
    private location: Location,
    private settingsService: SettingsService,
    private toastr: ToastsManager) {

  }

  onSubmit(): void {
    this.settingsService.saveSettings(this.settings)
      .subscribe(
      (data) => {
        this.toastr.info('Settings saved successfully')
        this.refresh()
      },
      (err) => {
        this.toastr.error('error occurred' + err)
      })
  }

  ngOnInit(): void {
    const logLevels = Object.keys(this.logLevelsEnum)
    this.logLevelOptions = logLevels.slice(logLevels.length / 2)
    this.settings = new Settings({})
    this.refresh()
  }

  refresh() {
    this.settingsService.getSettings()
      .subscribe(
      (data: any) => {
        this.settings = <Settings>data
        this.settings.LogLevel = this.logLevelsEnum[data.LogLevel]
      }//,
      // (err) => {
      //   this.toastr.error('error occurred' + err)
      // })
      )
  }

}
