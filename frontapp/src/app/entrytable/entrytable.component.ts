import { Component, Input } from '@angular/core';
import { TableEntry } from 'src/TableEntry';
import { NgbDateExt } from 'src/NgbDateExt';
import { CountryFlags } from 'src/CountryFlags';

@Component({
  selector: 'app-entrytable',
  templateUrl: './entrytable.component.html',
  styleUrls: ['./entrytable.component.scss']
})

//table listing all daily data
export class EntrytableComponent {
  @Input() entryArray: Array<TableEntry>;
  @Input() tag: string;
  @Input() featured: boolean;
  @Input() date:NgbDateExt;

  tagStrings = CountryFlags;
}
