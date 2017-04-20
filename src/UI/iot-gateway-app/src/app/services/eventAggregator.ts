import { Injectable } from '@angular/core'
import { Subject } from 'rxjs/Subject'
import { Subscription } from 'rxjs/Subscription'
import 'rxjs/add/operator/share'

export class Event {
  type: String
  data: any
}

@Injectable()
export class EventAggregator {
  subject: Subject<Event>

  constructor() {
    this.subject = new Subject<Event>()
  }

  publish(type: String, data: any) {
    this.subject.next({ type, data })
  }

  listen(type: String) {
    return this.subject
      .filter((event) => event.type === type)
      .map(event => event.data)
      .share()
  }

  unsubscribe() {
    this.subject.unsubscribe()
  }

}
