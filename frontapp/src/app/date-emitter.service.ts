import { EventEmitter, Injectable, Output } from '@angular/core';
import { Subscription } from 'rxjs/internal/Subscription';

@Injectable({
  providedIn: 'root'
})

export class DateEmitterService {

  invokeDatePicker = new EventEmitter();
  invokeMonthChange = new EventEmitter();
  dateSubs: Subscription = Subscription.EMPTY;
  monthSubs: Subscription = Subscription.EMPTY;

  constructor() { }

  onDatePick(d:Object){
    this.invokeDatePicker.emit(d);
  }

  onMonthChange(d:Object){
    this.invokeMonthChange.emit(d)
  }
}
