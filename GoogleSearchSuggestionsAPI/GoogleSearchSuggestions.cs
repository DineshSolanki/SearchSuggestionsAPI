using System.Xml.Linq;

namespace GoogleSearchSuggestionsAPI;

public class GoogleSearchSuggestions
{
    /// <summary>
    ///     ''' The Google Search Suggestion URL
    ///     ''' <remarks>
    ///     ''' hl for language, q for search text
    ///     ''' </remarks>
    ///     ''' </summary>
    private const string BaseSearchUrl = "https://www.google.com/complete/search?output=toolbar&q={0}&hl={1}";

    private const string BaseSearchUrlAlternate = "https://suggestqueries.google.com/complete/search?client=toolbar&q={0}&hl={1}";
    // we can use gl for country, writing here for reference.
    /// <summary>
    ///     ''' Gets the search suggestions from Google.
    ///     ''' </summary>
    ///     ''' <param name="query">Query to perform</param>
    ///     ''' <param name="language"> Chooses the language in which the search is being performed. 
    ///     ''' <a href="Http://en.wikipedia.org/wiki/List_of_ISO_639-1_codes"/>
    ///     ''' </param>
    ///     ''' <param name="useAlternate">There is a alternate URL that can be used if default don't work</param>
    ///     ''' <returns>A list of <see cref="GoogleSuggestion"/>s.</returns>
    public static async Task<List<GoogleSuggestion>> GetSearchSuggestions(string query, string language = "en", bool useAlternate = false)
    {
        if (string.IsNullOrWhiteSpace(query))
            throw new ArgumentNullException(nameof(query), @"Argument cannot be null or empty!");
        string result;
        using (var client = new HttpClient())
        {
            if (useAlternate)
                result = await client.GetStringAsync(string.Format(BaseSearchUrlAlternate, query, language));
            else
                result = await client.GetStringAsync(string.Format(BaseSearchUrl, query, language));
        }
        var doc = XDocument.Parse(result);
        var suggestions = from suggestion in doc.Descendants("CompleteSuggestion")
                          select new GoogleSuggestion() { Phrase = suggestion.Element("suggestion").Attribute("data").Value };
        return suggestions.ToList();
    }
}

public class GoogleSuggestion
{
    /// <summary>
    ///     ''' Gets or sets the phrase.
    ///     ''' </summary>
    ///     ''' <value>The phrase.</value>
    public string? Phrase { get; set; }

    /// <summary>
    ///     ''' Returns a <see cref="System.String"/> that represents this instance.
    ///     ''' </summary>
    ///     ''' <returns>A <see cref="System.String"/> that represents this instance.</returns>
    public override string? ToString()
    {
        return Phrase;
    }
}