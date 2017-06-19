import { OnInit, Component, Input } from '@angular/core'
import { ActivatedRoute, Router } from '@angular/router'
import { ToastsManager } from 'ng2-toastr/ng2-toastr'
import { Location } from '@angular/common'
import { DatePickerOptions, DateModel } from 'ng2-datepicker'

import * as moment from 'moment'

import { Log } from 'app/models/log'
import { LogLine } from 'app/models/logLine'
import { LogService } from '../../services/logService'
import { LogLevel } from '../../models/logLevel'

@Component({
  selector: 'overview',
  templateUrl: './logView.component.html',
})
export class LogViewComponent implements OnInit {

  currentDate: DateModel
  options: DatePickerOptions

  filterText: string

  log: Log
  copyOfLog: Log

  selectedLogLine: LogLine
  selectedRowIndex: Number = undefined

  logLevel: string
  logLevelOptions: String[]
  logLevelsEnum: typeof LogLevel = LogLevel

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
    const logLevels = Object.keys(this.logLevelsEnum)
    this.logLevelOptions = logLevels.slice(logLevels.length / 2)

    this.log = new Log({})
    this.log.LogLines = new Array()

    const day = moment().date()
    const month = moment().month() + 1
    const year = moment().year()

    this.currentDate = new DateModel()
    this.currentDate.day = day.toString()
    this.currentDate.month = month.toString()
    this.currentDate.year = year.toString()

    this.options = new DatePickerOptions()
    this.options.initialDate = new Date()
  }


  onChangeDate(event): void {

    const dayFormatted = moment(event.formatted).format('YYYYMMDD')

    // Get data from service
    this.getLogsForSelectedDay(dayFormatted)
  }

  onChangeLogLevel(level) {
    let logLines: LogLine[] = this.cloneLogLines(this.copyOfLog)
    if (level) {
      logLines = this.filterLogByLogLevel(logLines, level)
    }
    if (this.filterText) {
      logLines = this.filterLogByText(logLines, this.filterText)
    }
    this.log.LogLines = logLines
  }

  onChangeFilterText(text) {
    let logLines: LogLine[] = this.cloneLogLines(this.copyOfLog)
    if (text) {
      logLines = this.filterLogByText(logLines, text)
    }
    if (this.logLevel) {
      logLines = this.filterLogByLogLevel(logLines, this.logLevel)
    }
    this.log.LogLines = logLines
  }

  filterLogByText(logLines: LogLine[], text: string) {
    const filteredLogLines = logLines.filter((logLine) => {
      return this.hasText(logLine, text.toLowerCase())
    })
    return filteredLogLines
  }

  filterLogByLogLevel(logLines: LogLine[], level: string) {
    const filteredLogLines = logLines.filter((logLine) => {
      return level === logLine.Level
    })
    return filteredLogLines
  }

  hasText(logLine: LogLine, text: string): boolean {
    if (logLine && logLine.Exception && logLine.Exception.toLowerCase().includes(text)) {
      return true
    }
    if (logLine && logLine.Level && logLine.Level.toLowerCase().includes(text)) {
      return true
    }
    if (logLine && logLine.MessageTemplate && logLine.MessageTemplate.toLowerCase().includes(text)) {
      return true
    }
    if (logLine && logLine.MessageTemplate && logLine.MessageTemplate.toLowerCase().includes(text)) {
      return true
    }
    return false
  }

  cloneLogLines(log: Log) {
    // const cloneLog = new Log({})
    // const cloneLogLines = log.LogLines.slice(0)
    // cloneLog.FileName = log.FileName
    // cloneLog.LogLines = cloneLogLines
    // return cloneLog
    return [...log.LogLines] // will clone the array
  }

  getLogsForSelectedDay(dayFormatted: string) {
    console.log('getting log for day: ' + dayFormatted)
    this.logService.getLog(dayFormatted)
      .subscribe(
      (data: Log) => {
        this.log = data
        this.copyOfLog = new Log({})
        this.copyOfLog.FileName = data.FileName
        this.copyOfLog.LogLines = this.cloneLogLines(this.log)
        this.toastr.info('Logs received for ' + moment(dayFormatted).format('DD/MM/YYYY'))
      },
      (err) => {
        this.log = new Log({})
        this.log.LogLines = []
        this.toastr.error('error occurred' + err)
      })
  }

  refresh() {
    const dateString = this.currentDate.year + '-' + this.currentDate.month + '-' + this.currentDate.day
    const dayFormatted = moment(dateString).format('YYYYMMDD')

    // Get data from service
    this.getLogsForSelectedDay(dayFormatted)
  }

  setClickedRow(i: Number, logLine: LogLine) {
    this.selectedRowIndex = i
    this.selectedLogLine = logLine
  }

  getLogLevelClass(level: String) {
    return level.toLowerCase()
  }

}
