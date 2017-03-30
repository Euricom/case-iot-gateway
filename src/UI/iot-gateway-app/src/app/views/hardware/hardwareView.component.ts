import { OnInit, Component } from '@angular/core'
import { ActivatedRoute, Router } from '@angular/router'
// import { FormGroup, FormBuilder, FormControl } from '@angular/forms'
import { Location } from '@angular/common'
import { Device } from '../../models/device'
import { DeviceType } from '../../models/deviceType';
import { HardwareService } from '../../services/hardwareService'

@Component({
  templateUrl: './hardwareView.component.html',
})
export class HardwareViewComponent implements OnInit {

  device: Device = new Device({ guid: 'ff', name: 'test1', type: 'camera' })
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
  ) {
    hardwareService.getAll().subscribe(devices => this.devices = devices)
  }

  onSubmit(): void {

    this.hardwareService.create(this.device)
      .subscribe((res) => {
        this.router.navigate(['/hardware'])
      })
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

  onClickCancel() {
    this.location.back()
  }
}
