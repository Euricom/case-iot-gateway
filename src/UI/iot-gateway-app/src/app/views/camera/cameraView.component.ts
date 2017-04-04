import { OnInit, Component } from '@angular/core'
import { ActivatedRoute, Router } from '@angular/router'
import { ToastsManager } from 'ng2-toastr/ng2-toastr'
import { Location } from '@angular/common'

import { Camera } from '../../models/camera'
import { CameraService } from '../../services/cameraService'

@Component({
  selector: 'overview',
  templateUrl: './cameraView.component.html',
})
export class CameraViewComponent implements OnInit {

  cameras: Camera[]
  camera: Camera = new Camera({})
  selectedRowIndex: Number = undefined

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private location: Location,
    // private formBuilder: FormBuilder,
    private cameraService: CameraService,
    private toastr: ToastsManager,
  ) {
    this.refresh()
  }

  onSubmit(): void {
    this.cameraService.update(this.camera.DeviceId, this.camera)
      .subscribe(
      (data) => {
        this.toastr.info('camera updated successfully')
        this.refresh()
      },
      (err) => {
        this.toastr.error('error occurred' + err)
      })
  }

  ngOnInit(): void {
    this.camera = new Camera({})
  }

  refresh() {
    this.cameraService.getAll()
      .subscribe(
      (data) => {
        this.cameras = data
      },
      (err) => {
        this.toastr.error('error occurred' + err)
      })
  }

  testConnection(camera: Camera) {
    if (!camera.Address) {
      this.toastr.error('Cannot test connection without valid ip address')
      return
    }
    this.cameraService.testConnection(camera.DeviceId)
    .subscribe(
      (data) => {
        this.toastr.info(data)
      },
      (err) => {
        this.toastr.error('error occurred' + err)
      })
  }

  setClickedRow(i: Number, camera: Camera) {
    this.selectedRowIndex = i
    this.camera = camera
  }
}
