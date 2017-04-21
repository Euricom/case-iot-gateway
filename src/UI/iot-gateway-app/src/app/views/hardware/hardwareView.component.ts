import { OnInit, Component } from '@angular/core'
import { ActivatedRoute, Router } from '@angular/router'
import { ToastsManager } from 'ng2-toastr/ng2-toastr'
import { Location } from '@angular/common'

import { Device } from '../../models/device'
import { DeviceType } from '../../models/deviceType'
import { HardwareService } from '../../services/hardwareService'

@Component({
  templateUrl: './hardwareView.component.html',
})
export class HardwareViewComponent implements OnInit {

  device: Device = new Device({ })
  deviceType: DeviceType

  formSubmitted = false
  // form: FormGroup

  devices: Device[]
  deviceTypes: DeviceType[]

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private location: Location,
    // private formBuilder: FormBuilder,
    private hardwareService: HardwareService,
    private toastr: ToastsManager,
  ) {
    this.refresh()
  }

  onSubmit(): void {
    this.hardwareService.create(this.device)
      .subscribe(
      (data) => {
        this.toastr.info('device created successfully')
        this.refresh()
      },
      )
  }

  ngOnInit(): void {

    this.device = new Device({})

    this.deviceTypes = [
      new DeviceType({
        id: 'camera',
        name: 'Raspberry Pi Camera',
      }),
      new DeviceType({
        id: 'danalock',
        name: 'DanaLock',
      }),
      new DeviceType({
        id: 'lazybone',
        name: 'LazyBone Switch',
      }),
    ]
  }

  deleteDevice(device: Device) {
    this.hardwareService.delete(device.Name)
      .subscribe(
      (data) => {
        this.toastr.info('device deleted successfully')
        this.refresh()
      },
      )
  }

  refresh() {
    this.hardwareService.getAll()
      .subscribe(
      (data) => {
        this.devices = data
      },
      (err) => {
        this.toastr.error('error occurred' + err)
      })
  }

  onClickCancel() {
    this.location.back()
  }
}
