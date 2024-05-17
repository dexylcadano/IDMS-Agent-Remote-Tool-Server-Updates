Imports System.Text.Json

Public Enum APIRETURNRESULT
    SUCESSFULL
    UNAUTHORIZED
    FAILED
End Enum

Public Class CAgentAPI
    Private Shared syncLocker As New Object()
    Private Shared singleAPI As CAgentAPI

    Private token As String

    Public Sub New()
        token = GetToken()
    End Sub

    Public Shared Function getInstance() As CAgentAPI
        If (singleAPI Is Nothing) Then
            SyncLock (syncLocker)
                If (singleAPI Is Nothing) Then
                    singleAPI = New CAgentAPI()
                End If
            End SyncLock
        End If
        Return singleAPI
    End Function

    Private Function GetToken() As String
        Dim myData As New Dictionary(Of String, String)
        myData.Add("password", My.Resources.admin_password)

        Return JsonSerializer.Deserialize(Of Dictionary(Of String, String))(CRESTAPI.PostRequest("v1/auth/tokens", myData))("token").ToString()
    End Function

    Public Function UpdatePCMainInfo(ByVal info As Dictionary(Of String, String)) As Integer

        Return CInt(JsonSerializer.Deserialize(Of Dictionary(Of String, Object))(CRESTAPI.PostRequest("v1/pc-main", info, token))("id").ToString())
    End Function

    Public Sub UpdatePCInfo(ByVal info As Dictionary(Of String, Dictionary(Of String, String)), ByVal id As Integer)
        Dim headers As New Dictionary(Of String, String)
        headers.Add("id", id.ToString)

        CRESTAPI.PostRequest("v1/pc-info", info, token, headers)
    End Sub

    Public Sub UpdatePCPorts(ByVal info As Dictionary(Of String, String), ByVal id As Integer)
        Dim headers As New Dictionary(Of String, String)
        headers.Add("id", id.ToString)

        CRESTAPI.PostRequest("v1/pc-network-ports", info, token, headers)
    End Sub
End Class
