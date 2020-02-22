Imports System.Net.Http
Public Class GoogleSearchSuggestions

    ''' <summary>
    ''' The Google Search Suggestion URL
    ''' <remarks>
    ''' hl for language, q for search text
    ''' </remarks>
    ''' </summary>
    Private Const baseSearchURL _
    As String = "http://www.google.com/complete/search?output=toolbar&q={0}&hl={1}"
    Private Const baseSearchURLAlternate _
        As String = "http://suggestqueries.google.com/complete/search?client=toolbar&q={0}&hl={1}"
    'we can use gl for country, writing here for reference.
    ''' <summary>
    ''' Gets the search suggestions from Google.
    ''' </summary>
    ''' <param name="query">Query to perform</param>
    ''' <param name="language"> Chooses the language in which the search is being performed. 
    ''' <a href="Http://en.wikipedia.org/wiki/List_of_ISO_639-1_codes"/>
    ''' </param>
    ''' <param name="useAlternate">There is a alternate URL that can be used if default don't work</param>
    ''' <returns>A list of <see cref="GoogleSuggestion"/>s.</returns>
    Public Shared Async Function GetSearchSuggestions(query As String, Optional language As String = "en", Optional useAlternate As Boolean = False) As Task(Of List(Of GoogleSuggestion))
        If String.IsNullOrWhiteSpace(query) Then
            Throw New ArgumentNullException("Argument cannot be null or empty!", "query")
        End If
        Dim result As String = String.Empty
        Using client As New HttpClient
            If useAlternate Then
                result = Await client.GetStringAsync(String.Format(baseSearchURLAlternate, query, language))
            Else
                result = Await client.GetStringAsync(String.Format(baseSearchURL, query, language))
            End If

        End Using
        Dim doc As XDocument = XDocument.Parse(result)
        Dim suggestions = From suggestion In doc.Descendants("CompleteSuggestion")
                          Select New GoogleSuggestion With {
                              .Phrase = suggestion.Element("suggestion").Attribute("data").Value}
        Return suggestions.ToList()
    End Function
End Class
Public Class GoogleSuggestion
    ''' <summary>
    ''' Gets or sets the phrase.
    ''' </summary>
    ''' <value>The phrase.</value>
    Public Property Phrase As String
    ''' <summary>
    ''' Returns a <see cref="System.String"/> that represents this instance.
    ''' </summary>
    ''' <returns>A <see cref="System.String"/> that represents this instance.</returns>
    Public Overrides Function ToString() As String
        Return Me.Phrase
    End Function
End Class
