//Language tag strings for the data table

export class CountryFlags
{
    static GetNameShort(tag: string): string
    {
        return LANGUAGE_TAGS[tag]?.toUpperCase() || "ERR";
    }

    static GetNameLong(tag: string): string
    {
        return LANGUAGE_FULL[tag] || "Error";
    }

    static GetFlagPath(tag: string): string
    {
        if (Object.keys(LANGUAGE_TAGS).includes(tag))
            return "../assets/country_flags/" + tag + ".svg";
        else
            return "../assets/country_flags/err.svg";
    }
}

const LANGUAGE_TAGS : any = {
    'en': "en", 'pl': "pl", ar: "ar", es: "es", de: "de", fr: "fr", it: "it", nl: "nl",
    ja: "ja", pt: "pt", sv: "sv", uk: "uk", vi: "vi", zh: "zh", ru: "ru"
};

const LANGUAGE_FULL : any = {
    en: "English", pl: "Polish", ar: "Arabic", es: "Spanish", de: "German", fr: "French", it: "Italian", nl: "Dutch",
    ja: "Japanese", pt: "Portuguese", sv: "Swedish", uk: "Ukranian", vi: "Vietnamese", zh: "Chinese", ru: "Russian"
};
