Imports System.DirectoryServices


Public Class CDirectory
    Private groupMembers As List(Of DirectoryEntry)
    Private app_settings As New CAPPSettings
    Private db As CDatabase
    Private PCLogs As CPCLogs

    Public Sub New()
        Me.db = CDatabase.getInstance()
        PCLogs = New CPCLogs(db)
        Init()
    End Sub

    Private Sub Init()
        Dim dirEntry As New DirectoryEntry("WinNT://" & Environment.MachineName & ",computer")

        Dim admins As DirectoryEntry = GetAdminUsers(dirEntry)
        groupMembers = GetUsers(admins)
    End Sub

    Private Function GetAdminUsers(ByVal dir As DirectoryEntry) As DirectoryEntry
        Dim admins As DirectoryEntry = dir.Children.Find("administrators", "group")

        Return admins
    End Function

    Private Function GetUsers(ByVal dir As DirectoryEntry) As List(Of DirectoryEntry)
        Dim members As IEnumerable = dir.Invoke("members", Nothing)
        Dim newDir As New List(Of DirectoryEntry)

        For Each member As Object In members
            Dim users As DirectoryEntry = New DirectoryEntry(member)
            Dim path As String = users.Path
            Dim array As String() = path.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)

            If array.Length > 3 Then
                newDir.Add(users)
            End If

        Next

        Return newDir
    End Function

    Public Sub UpdatePassword(ByVal pc_id As Integer)
        Dim password As DataRow = GetLatestPassword()

        'check if updated password
        If Not IsPasswordUpdated(password("created_at")) Then
            For Each member As Object In groupMembers
                Dim user As DirectoryEntry = DirectCast(member, DirectoryEntry)
                If user.Properties("Name").Value.ToString <> "Administrator" Then

#If DEBUG Then
                    'bypass
                    If user.Properties("Name").Value.ToString = "DSWD" Then
                        Continue For
                    End If
#End If

                    Try
                        user.Invoke("SetPassword", password("password"))
                        user.CommitChanges()

                        app_settings.UpdateAPPSettings("Password Date", password("created_at"))
                        PCLogs.AddLogs(pc_id, user.Properties("Name").Value.ToString & ": Sucessfully updated password!")
                    Catch ex As Exception
                        PCLogs.AddLogs(pc_id, user.Properties("Name").Value.ToString & ": " & ex.InnerException.Message)
                    End Try
                End If
            Next
        End If
    End Sub

    Private Function IsPasswordUpdated(ByVal passwordDate As DateTime) As Boolean
        Dim curPasswordDate As DateTime = app_settings.GetSetting("Password Date")

        If curPasswordDate <> passwordDate Then
            Return False
        End If

        Return True
    End Function

    Private Function GetLatestPassword() As DataRow
        Dim filters(0) As Filters
        Dim sort As DataSort

        sort.keyword = "created_at"
        sort.type = SortType.DESC

        Dim passwords = db.GetData("pc_passwords", filters, New List(Of FilterCast)(New FilterCast() {New FilterCast With {.columnname = "password", .caster = New CDatabaseCastEncryption}}), sort)(0)

        Return passwords
    End Function
End Class
