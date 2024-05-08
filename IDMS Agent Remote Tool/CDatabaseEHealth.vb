Public Class CDatabaseEHealth
    Inherits CDatabase

    Private Shared syncLocker As New Object()
    Private Shared singleDB As CDatabaseEHealth

    Public Sub New()
#If DEBUG Then
        DbServer = My.Resources.DBLocalServer
        DbUID = My.Resources.DbLocalUID
        DbPassword = My.Resources.DbLocalPassword
        
#Else
        DbServer = My.Resources.DBServer
        DbUID = My.Resources.DbUID
        DbPassword = My.Resources.DBPassword
        DBSSLFlag = False
#End If
        DbDatabase = My.Resources.DbEHealth
        DBPort = "3306"
        Init()
    End Sub

    Public Shared Function getInstanceEHealth() As CDatabaseEHealth
        If (singleDB Is Nothing) Then
            SyncLock (syncLocker)
                If (singleDB Is Nothing) Then
                    singleDB = New CDatabaseEHealth()
                End If
            End SyncLock
        End If
        Return singleDB
    End Function

    Public Function GetUsers() As DataTable
        Return GetData("users")
    End Function
End Class
