import { OnInit, Component } from '@angular/core'
import { ActivatedRoute, Router } from '@angular/router'
import { ToastsManager } from 'ng2-toastr/ng2-toastr'
import { Location } from '@angular/common'

import { Node } from '../../models/node'
import { ZwaveService } from '../../services/zwaveService'

@Component({
  templateUrl: './zwaveView.component.html',
})
export class ZwaveViewComponent implements OnInit {

  node: Node = new Node({})

  formSubmitted = false
  // form: FormGroup

  nodes: Node[]

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private location: Location,
    // private formBuilder: FormBuilder,
    private zwaveService: ZwaveService,
    private toastr: ToastsManager,
  ) {
    this.refresh()
  }

  ngOnInit(): void {


  }

  refresh() {
    this.zwaveService.getAll()
      .subscribe(
      (data) => {
        this.nodes = data
      },
      (err) => {
        this.toastr.error('error occurred' + err)
      })
  }

  onClickCancel() {
    this.location.back()
  }
}
