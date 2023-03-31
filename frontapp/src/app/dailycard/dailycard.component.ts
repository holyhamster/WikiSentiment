import { Component, Input, OnChanges } from '@angular/core';
import { DailyEntity } from 'src/DailyEntity';
import { CountryFlags } from 'src/CountryFlags';


@Component({
  selector: 'app-dailycard',
  templateUrl: './dailycard.component.html',
  styleUrls: ['./dailycard.component.scss']
})

//card that displays daily and about data
export class DailycardComponent implements OnChanges
{
  @Input() selectedDate?: DailyEntity;
  tabKeys = new Array<string>();
  featured = "featured";
  tagStrings = CountryFlags;

  //tracks change to selected date
  ngOnChanges(changes: any)
  {
    if (!changes.selectedDate)
      return;
    this.tabKeys = this.selectedDate?.GetAllCodes() || [];
  }
}
