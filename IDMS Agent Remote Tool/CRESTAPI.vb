Imports System.Net.Http
Imports System.Text
Imports System.Text.Json

Public Class APIReturn
    Public Property success As Boolean
    Public Property data As Object
End Class

Public Class CRESTAPI
    Private Shared baseURL As String = "http://public_html.test/api/idms-agent/"

    Public Shared Function PostRequest(ByVal address As String, ByVal data As Object, Optional token As String = "", Optional headers As Dictionary(Of String, String) = Nothing) As Object
        Dim client As New HttpClient
        client.BaseAddress = New Uri(baseURL)

        Dim json = JsonSerializer.Serialize(data)
        Dim content = New StringContent(json, Encoding.UTF8, "application/json")

        If token <> "" Then
            client.DefaultRequestHeaders.Authorization = New Headers.AuthenticationHeaderValue("Bearer", token)
        End If

        If headers IsNot Nothing AndAlso headers.Count > 0 Then
            For Each header As KeyValuePair(Of String, String) In headers
                client.DefaultRequestHeaders.Add(header.Key, header.Value)
            Next
        End If

        Try
            Dim response = client.PostAsync(address, content).Result

            If response.IsSuccessStatusCode Then
                Dim resposeValue = response.Content.ReadAsStringAsync().Result
                Try
                    Return JsonSerializer.Deserialize(Of APIReturn)(resposeValue).data
                Catch ex As Exception
                    Throw New CAPIExceptionErrorHandler(response.StatusCode & ": " & response.ReasonPhrase)
                End Try
            Else
                Throw New CAPIExceptionErrorHandler(response.StatusCode & ": " & response.ReasonPhrase)
            End If
        Catch ex As Exception
            If ex.GetType() Is GetType(CAPIExceptionErrorHandler) Then
                Throw New Exception(ex.Message.ToString)
            Else
                Throw New Exception("404: Not Found")
            End If

        End Try
    End Function
End Class

Public Class CAPIExceptionErrorHandler
    Inherits Exception

    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub
End Class
