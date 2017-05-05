import { OnInit, Component, Input } from '@angular/core'
import { ActivatedRoute, Router } from '@angular/router'
import { ToastsManager } from 'ng2-toastr/ng2-toastr'
import { Location } from '@angular/common'
import { DatePickerOptions, DateModel } from 'ng2-datepicker'

import * as moment from 'moment'

import { Log } from 'app/models/log'
import { LogLine } from 'app/models/logLine'
import { LogService } from '../../services/logService'

@Component({
  selector: 'overview',
  templateUrl: './openzwavelogView.component.html',
})
export class OpenZWaveLogViewComponent implements OnInit {

  currentDate: DateModel
  options: DatePickerOptions

  filterText: string

  logLines: string[]
  copyOfLog: Log

  selectedLogLine: LogLine
  selectedRowIndex: Number = undefined

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private location: Location,
    private logService: LogService,
    private toastr: ToastsManager,
  ) {
  }

  ngOnInit(): void {

    this.filterText = ''
    this.logLines = []
    this.refresh()
  }

  refresh() {
    // Get data from service
    this.getLogs()
  }

  getLogs() {
    this.logService.getOpenZWaveLog()
      .subscribe(
      (data: string[]) => {
        this.logLines = data
        this.toastr.info('OpenZWave logs received')
      },
      (err) => {
        this.toastr.error('error occurred' + err)
      })
  }

  onChangeFilterText(text) {
  }

  setClickedRow(i: Number, logLine: LogLine) {
    this.selectedRowIndex = i
    this.selectedLogLine = logLine
  }

}
