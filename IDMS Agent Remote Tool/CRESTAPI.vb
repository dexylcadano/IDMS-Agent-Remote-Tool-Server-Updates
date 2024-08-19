Imports System.Net.Http
Imports System.Text
Imports System.Text.Json

Public Class APIReturn
    Public Property success As Boolean
    Public Property data As Object
End Class

Public Enum HTTPMethod
    GETREQUEST
    POSTREQUEST
    PATCHREQUEST
End Enum


Public Class CRESTAPI
#If DEBUG Then
    'Public Const agentBaseUrl = "http://public_html.test/api/idms-agent/"
    Public Const agentBaseUrl = "http://172.31.137.67/api/idms-agent/"
#Else
    Public Const agentBaseUrl = "https://idms-fo8-app.com/api/idms-agent/"
#End If
    Public Const maxRetry = 3

    Public Shared Function HTTPRequest(ByVal address As String, ByVal data As Object, Optional token As String = "", Optional headers As Dictionary(Of String, String) = Nothing, Optional method As HTTPMethod = HTTPMethod.POSTREQUEST, Optional baseUrl As String = agentBaseUrl) As Object
        Dim client As New HttpClient
        client.BaseAddress = New Uri(baseUrl)

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

        For retry As Integer = 1 To maxRetry
            Try
                Dim response As HttpResponseMessage

                Select Case method
                    Case HTTPMethod.GETREQUEST
                        response = client.GetAsync(address).Result
                    Case HTTPMethod.POSTREQUEST
                        response = client.PostAsync(address, content).Result
                    Case HTTPMethod.PATCHREQUEST
                        response = client.PatchAsync(address, content).Result
                    Case Else
                        Throw New Exception("HTTP Method Error")
                End Select

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
                End If
            End Try
        Next

        Throw New Exception("404: Not Found")
    End Function

End Class

Public Class CAPIExceptionErrorHandler
    Inherits Exception

    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub
End Class
