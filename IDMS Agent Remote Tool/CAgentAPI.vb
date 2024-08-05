Imports System.Text.Json

Public Enum APIRETURNRESULT
    SUCESSFULL
    UNAUTHORIZED
    FAILED
End Enum

Public Class User
    Public Property user_id As String
    Public Property name As String
End Class
Public Class Office
    Public Property office_id As String
    Public Property office_name As String
End Class
Public Class CAgentAPI

#If DEBUG Then
    Public Const eHealthBaseUrl = "http://public_html.test/api/"
#Else
    Public Const eHealthBaseUrl = "https://idms-fo8-app.com/api/"
#End If


    Private Shared ReadOnly syncLocker As New Object()
    Private Shared singleAPI As CAgentAPI

    Private ReadOnly token As String

    Public Sub New()
        Dim myData As New Dictionary(Of String, String) From {{"password", My.Resources.admin_password}}

        token = GetToken(myData)
    End Sub

    Public Shared Function GetInstance() As CAgentAPI
        If (singleAPI Is Nothing) Then
            SyncLock (syncLocker)
                If (singleAPI Is Nothing) Then
                    singleAPI = New CAgentAPI()
                End If
            End SyncLock
        End If
        Return singleAPI
    End Function

    Public Function GetToken(ByVal info As Dictionary(Of String, String)) As String
        Return JsonSerializer.Deserialize(Of Dictionary(Of String, String))(CRESTAPI.HTTPRequest("v1/auth/tokens", info))("token").ToString()
    End Function

    Public Function UpdatePCMainInfo(ByVal info As Dictionary(Of String, String)) As Integer
        Return CInt(JsonSerializer.Deserialize(Of Dictionary(Of String, Object))(CRESTAPI.HTTPRequest("v1/pc-main", info, token))("id").ToString())
    End Function

    Public Sub UpdatePCInfo(ByVal info As Dictionary(Of String, Dictionary(Of String, String)), ByVal id As Integer)
        Dim headers As New Dictionary(Of String, String) From {{"id", id.ToString}}

        CRESTAPI.HTTPRequest("v1/pc-info", info, token, headers)
    End Sub

    Public Sub UpdatePCPorts(ByVal info As List(Of Dictionary(Of String, String)), ByVal id As Integer)
        Dim headers As New Dictionary(Of String, String) From {{"id", id.ToString}}

        CRESTAPI.HTTPRequest("v1/pc-network-ports", info, token, headers)
    End Sub

    Public Function GetAllUsers(ByVal newToken As String) As List(Of User)
        Dim users As New List(Of Dictionary(Of String, String))
        Return JsonSerializer.Deserialize(Of List(Of User))(CRESTAPI.HTTPRequest("v1/users?formatting=for_drop_down", Nothing, newToken, Nothing, HTTPMethod.GETREQUEST))
    End Function

    Public Function GetAllOfficialOffices(ByVal newToken As String) As List(Of Office)
        Dim offices As New List(Of Dictionary(Of String, String))
        Return JsonSerializer.Deserialize(Of List(Of Office))(CRESTAPI.HTTPRequest("v1/offices?formatting=for_drop_down", Nothing, newToken, Nothing, HTTPMethod.GETREQUEST))
    End Function

    Public Function GetUser(ByVal pcID As Integer, ByVal newToken As String) As Dictionary(Of String, Object)
        Return JsonSerializer.Deserialize(Of Dictionary(Of String, Object))(CRESTAPI.HTTPRequest("v1/pc-main/" & pcID.ToString, Nothing, newToken, Nothing, HTTPMethod.GETREQUEST))
    End Function

    Public Sub SetUser(ByVal newToken As String, ByVal pcID As Integer, ByVal info As Dictionary(Of String, String))
        CRESTAPI.HTTPRequest("v1/pc-main/" & pcID.ToString, info, newToken, Nothing, HTTPMethod.PATCHREQUEST)
    End Sub
End Class
