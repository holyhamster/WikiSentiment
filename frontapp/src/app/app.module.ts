import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule } from '@angular/forms';
import {  HttpClientModule } from '@angular/common/http';
import { CalendarComponent } from './calendar/calendar.component';
import { DailycardComponent } from './dailycard/dailycard.component';
import { DateEmitterService } from './date-emitter.service';
import { EntrytableComponent } from './entrytable/entrytable.component';

@NgModule({
  declarations: [
    AppComponent,
    CalendarComponent,
    DailycardComponent,
    EntrytableComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    NgbModule,
    HttpClientModule
  ],
  providers: [DateEmitterService],
  bootstrap: [AppComponent]
})
export class AppModule { }
