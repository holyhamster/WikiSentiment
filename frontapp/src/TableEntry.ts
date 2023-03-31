//Formated data for a single table entry

export class TableEntry
{
    enlink: string;
    oglink: string;

    viewsString: string;
    totalviewsString: string;
    percentage: string;

    constructor(public ogtitle: string, public entitle: string, public country: string, public views: number, public totalviews: number)
    {
        this.ogtitle = ogtitle.replaceAll('_', ' ');
        this.oglink = this.getLink(country, ogtitle);

        this.entitle = entitle.replaceAll('_', ' ');
        this.enlink = this.getLink("en", entitle);
        const percents = 100 * views / totalviews;
        this.percentage = percents >= 10 ? percents.toFixed(1) : percents.toFixed(2);

        this.totalviewsString = totalviews.toLocaleString();
        this.viewsString = views.toLocaleString();
    }

    private getLink(code: string, title: string): string
    {
        return "https://" + code + ".wikipedia.org/wiki/" + title;
    }
}