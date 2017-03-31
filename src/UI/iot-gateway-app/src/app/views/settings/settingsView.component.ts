import { OnInit, Component } from '@angular/core'
import { ActivatedRoute, Router } from '@angular/router'
import { ToastsManager } from 'ng2-toastr/ng2-toastr'
import { Location } from '@angular/common'
import { Settings } from '../../models/settings';
import { SettingsService } from '../../services/settingsService'

@Component({
  selector: 'overview',
  templateUrl: './settingsView.component.html',
})
export class SettingsViewComponent implements OnInit {

  settings: Settings

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
    this.settings = new Settings({})
    this.refresh()
  }

  refresh() {
    this.settingsService.getSettings()
      .subscribe(
      (data: any) => {
        this.settings.azureAccountName = data.AzureAccountName
        this.settings.azureIotHubUri = data.AzureIotHubUri
        this.settings.azureIotHubUriConnectionString = data.AzureIotHubUriConnectionString
        this.settings.azureStorageAccessKey = data.AzureStorageAccessKey
        this.settings.dropboxAccessToken = data.DropboxAccessToken
        this.settings.historyLog = data.HistoryLog
      },
      (err) => {
        this.toastr.error('error occurred' + err)
      })
  }

}
