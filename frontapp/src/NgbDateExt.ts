import { NgbDate } from "@ng-bootstrap/ng-bootstrap";

//Provides date manipulations and conversions to Date
export class NgbDateExt extends NgbDate
{

    constructor(year: number, month: number, day: number)
    {
        super(year, month, day);
    }

    override toString(displayDay = true): string
    {
        let result = '' + this.year + '-' + NgbDateExt.numberToDD(this.month);
        if (displayDay)
            result += '-' + NgbDateExt.numberToDD(this.day);
        return result;
    }

    private toDate(): Date
    {
        return new Date(this.year, this.month - 1, this.day);
    }

    static BuildFromDate(date: Date): NgbDateExt
    {
        return new NgbDateExt(date.getFullYear(), date.getMonth() + 1, date.getDate());
    }

    static Now(): NgbDateExt
    {
        return this.BuildFromDate(new Date(Date.now()));
    }

    static AddMonths(ngDate: NgbDateExt, months: number): NgbDateExt
    {
        const date = ngDate.toDate();
        date.setMonth(date.getMonth() + months);
        return this.BuildFromDate(date);
    }
    static AddDays(ngDate: NgbDateExt, day: number): NgbDateExt
    {
        const date = ngDate.toDate();
        date.setDate(date.getDate() + day);
        return this.BuildFromDate(date);
    }

    private static numberToDD(i: number): string
    {
        return "" + (i < 10 ? "0" : "") + i;
    }
}