import { Component } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { NgbDate } from '@ng-bootstrap/ng-bootstrap';
import { DailyEntity } from 'src/DailyEntity';
import { NgbDateExt } from 'src/NgbDateExt';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})

export class AppComponent
{
  title = 'angular_app';

  loadedData: Map<string, DailyEntity>;
  loadedDays: Array<string>;

  selectedDate?: DailyEntity;

  constructor(
    private http: HttpClient, //http client for streaming wiki data from json file
  )
  {
    this.loadedDays = new Array<string>();
    this.loadedData = new Map<string, DailyEntity>();
  }

  onChangeMonth(ngbDate: NgbDate)
  {
    let date = new NgbDateExt(ngbDate.year, ngbDate.month, 2);
    let months = [date, NgbDateExt.AddMonths(date, 1), NgbDateExt.AddMonths(date, -1)];
    let currentMonth = NgbDateExt.Now();
    currentMonth.day = 1;

    months.forEach(month =>
    {
      var maxage = month.after(currentMonth) ? "max-age=3600" : "max-age=86400";
      const headers = new HttpHeaders().set('Cache-Control', ['public', maxage])
      try {
        this.http.get('/api/HttpGetDbData?month=' + month.toString(false), { headers }).subscribe(
          (response) => this.deserializeAPIResponse(response));
      }
      catch(error)
      {
        console.log(error);
      }
    });
  }

  onDayPick(ngbDate: NgbDate)
  {
    const date = new NgbDateExt(ngbDate.year, ngbDate.month, ngbDate.day);
    this.selectedDate = this.loadedData.get(date.toString()) || undefined;
  }

  public showAbout()
  {
    this.selectedDate = undefined;
  }

  public deserializeAPIResponse(response: { [index: string]: any })
  {
    Object.keys(response).filter(dateString => !this.loadedDays.includes(dateString)).forEach(dateString =>
    {
      const dateArray = dateString.split('-');
      const year = parseInt(dateArray[0]);
      const month = parseInt(dateArray[1]);
      const day = parseInt(dateArray[2]);
      if (!(dateArray.length == 3 && year && month && day)) {
        console.log(`Error parsing date from API response ${dateArray}`);
        return;
      }

      this.loadedData.set(dateString, new DailyEntity(new NgbDateExt(year, month, day), JSON.parse(response[dateString])));
      this.loadedDays = this.loadedDays.concat([dateString]);
    });
  }
}
