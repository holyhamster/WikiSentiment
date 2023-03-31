import { NgbDateExt } from "./NgbDateExt";
import { TableEntry } from "./TableEntry";

//Collection of all data for a single day
export class DailyEntity
{
    private data: Map<string, Array<TableEntry>>;
    constructor(public date: NgbDateExt, collection: Object)
    {
        this.data = this.deserialize(collection);
    }

  deserialize(collection: { [index: string]: any })
    {
        const dailyData = collection["countrydailydict"];
        const tagMap = new Map<string, Array<TableEntry>>();
        tagMap.set("featured", new Array<TableEntry>());

        Object.keys(dailyData).forEach(tag =>
        {
            tagMap.set(tag, new Array<TableEntry>());
            const articleArray = dailyData[tag]["articles"];
          const totalViews = dailyData[tag]["totalviews"];
          articleArray.forEach((article: any) =>
            {
                const title = article["ttl"];
                const langlink = article["lngl"]["en"] || "";
                const views = article["vws"];
                tagMap.get(tag)?.push(new TableEntry(title, langlink, tag, views, totalViews));
            });
        });

        collection["featuredlist"].filter((tag:string) => tagMap.has(tag))
            .forEach((tag:string) =>
            {
                const topElement = (tagMap.get(tag) as TableEntry[])[0];
                (tagMap.get("featured") as Array<TableEntry>).push(topElement);
            });
        return tagMap;
    }

    GetArticles(tag: string): Array<TableEntry>
    {
        return this.data.get(tag) as Array<TableEntry> || [];
    }

    GetAllCodes()
    {
        return Array.from(this.data.keys());
    }
}
