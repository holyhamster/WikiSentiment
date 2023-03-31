import { Component, Output, Input, OnChanges } from '@angular/core';
import { NgbDate } from '@ng-bootstrap/ng-bootstrap';
import { NgbDateExt } from 'src/NgbDateExt';
import { NgbDatepickerNavigateEvent } from '@ng-bootstrap/ng-bootstrap';
import { EventEmitter } from '@angular/core';

@Component({
  selector: 'app-calendar',
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.scss']
})

//calendar that lets user select the date
export class CalendarComponent implements OnChanges
{
  @Output() monthChangeEvent = new EventEmitter<NgbDate>();
  @Output() dayPickEvent = new EventEmitter<NgbDate>();
  @Input() datesLoaded = new Array<string>();
  maxDate;
  minDate;
  model: any;

  isDateBlocked: (date: NgbDate) => boolean;

  constructor()
  {
    this.maxDate = NgbDateExt.AddDays(NgbDateExt.Now(), -1);
    this.minDate = new NgbDateExt(2015, 9, 1);
    this.isDateBlocked = () => true;
  }

  //tracks change to loaded dates
  ngOnChanges(changes: any)
  {
    if (changes?.datesLoaded?.currentValue?.length > 0) {
      this.isDateBlocked = (date: NgbDate) =>
        !this.datesLoaded.includes(new NgbDateExt(date.year, date.month, date.day).toString());
    }
  }

  selectDate(d: NgbDate)
  {
    this.dayPickEvent.emit(d);
  }

  selectMonth(d: NgbDatepickerNavigateEvent)
  {
    this.monthChangeEvent.emit(new NgbDate(d.next.year, d.next.month, 1));
  }
}
