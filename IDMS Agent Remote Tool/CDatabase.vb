Imports MySql.Data.MySqlClient

Public Enum FilterCompare
    EqualCompare
    LikeCompare
    DateCompare
    MonthCompare
    YearCompare
End Enum

Public Enum SortType
    ASC
    DESC
End Enum
Public Structure DataSort
    Public keyword As String
    Public type As SortType
End Structure
Public Structure Filters
    Public filtername As String
    Public filtervalue As String
    Public compare As FilterCompare
    Public casted As Boolean
End Structure
Public Structure WriteParam
    Public parameter As String
    Public value As Object
    Public caster As CDatabaseCast
End Structure

Public Structure FilterCast
    Public columnname As String
    Public caster As CDatabaseCast
End Structure

Public Class CDatabase
    Protected DbServer As String
    Protected DbUID As String
    Protected DbPassword As String
    Protected DbDatabase As String
    Protected DbDatabase2 As String
    Protected DTRTable As String
    Protected UsersTable As String
    Protected DBPort As String
    Protected DBSSLFlag As Boolean = True

    Dim myCommand As New MySqlCommand()
    Private conn As New MySqlConnection

    Private encryptor As New CEncryption
    Private Shared syncLocker As New Object()
    Private Shared singleDB As CDatabase

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
        DbDatabase = My.Resources.Db
        DBPort = "3306"
        Init()
    End Sub
    Protected Sub Init()
        conn.Close()

        Dim myConnectionString As String = "server=" & DbServer & ";uid=" & DbUID & ";pwd=" & DbPassword & ";database=" & DbDatabase & ";port=" & DBPort

        If Not DBSSLFlag Then
            myConnectionString += ";SSL Mode=None"
        End If

        Try
            conn.ConnectionString = myConnectionString
            conn.Open()
            myCommand.Connection = conn
        Catch ex As Exception
            'MessageBox.Show("Unable to connect to the server!", "Error Found", MessageBoxButtons.OK, MessageBoxIcon.Error)
            'Application.Exit()
        End Try
    End Sub

    Public Shared Function getInstance() As CDatabase
        If (singleDB Is Nothing) Then
            SyncLock (syncLocker)
                If (singleDB Is Nothing) Then
                    singleDB = New CDatabase()
                End If
            End SyncLock
        End If
        Return singleDB
    End Function

    Private Function GetConnection() As MySqlConnection
        Return conn
    End Function
    Protected Function GetDataTable(ByVal query As String) As DataTable
        Dim myadapter As New MySqlDataAdapter(query, GetConnection())
        Dim dtset As New DataSet

        Try
            myadapter.Fill(dtset)
            myadapter.Dispose()
        Catch ex As Exception
            myadapter.Dispose()

            Return Nothing
        End Try

        Return dtset.Tables(0)
    End Function
    Protected Function GenerateFilterString(ByVal filterset As Filters(), Optional whereTag As Boolean = False, Optional removecastcol As Boolean = False) As String
        Dim filterstring As String = ""

        For Each i In filterset
            If Not removecastcol Or Not i.casted Then
                If Not IsNothing(i.filtervalue) Then
                    If i.compare = FilterCompare.LikeCompare Then
                        filterstring = filterstring & i.filtername & " LIKE '%" & i.filtervalue & "%' AND "
                    ElseIf i.compare = FilterCompare.DateCompare Then
                        filterstring = filterstring & "DATE(" & i.filtername & ") = '" & i.filtervalue & "' AND "
                    ElseIf i.compare = FilterCompare.MonthCompare Then
                        filterstring = filterstring & "MONTH(" & i.filtername & ") = '" & i.filtervalue & "' AND "
                    ElseIf i.compare = FilterCompare.YearCompare Then
                        filterstring = filterstring & "YEAR(" & i.filtername & ") = '" & i.filtervalue & "' AND "
                    Else
                        filterstring = filterstring & i.filtername & " = '" & i.filtervalue & "' AND "
                    End If
                End If
            End If
        Next

        Dim MyChar As Char() = {"A", "N", "D"}
        filterstring = filterstring.Trim().TrimEnd(MyChar)

        Return IIf(whereTag AndAlso filterstring.Trim <> "", " WHERE ", "") & filterstring.Trim()
    End Function
    Public Function GetData(ByVal datatable As String, ByVal filterset As Filters()) As DataTable
        Dim sqlquery = "SELECT * FROM " & datatable & GenerateFilterString(filterset, True)

        Return GetDataTable(sqlquery)
    End Function

    Public Function GetData(ByVal datatable As String) As DataTable
        Dim sqlquery = "SELECT * FROM " & datatable

        Return GetDataTable(sqlquery)
    End Function

    Public Function GetData(ByVal datatable As String, ByVal filterset As Filters(), Optional datacast As List(Of FilterCast) = Nothing, Optional sort As DataSort = Nothing) As DataRow()
        Dim sqlquery = "SELECT * FROM " & datatable & GenerateFilterString(filterset, True, True)

        If sort.keyword IsNot Nothing Then
            sqlquery &= " ORDER BY " & sort.keyword & " " & IIf(sort.type = SortType.DESC, "DESC", "ASC")
        End If

        Dim table As DataTable = GetDataTable(sqlquery)

        If datacast IsNot Nothing Then
            For i As Integer = 0 To table.Rows.Count - 1
                For Each castcol As FilterCast In datacast
                    Try
                        Dim caster As New CDatabaseCast
                        If castcol.caster IsNot Nothing Then
                            table.Rows(i)(castcol.columnname) = castcol.caster.DeCast(table.Rows(i)(castcol.columnname))
                        Else
                            table.Rows(i)(castcol.columnname) = caster.DeCast(table.Rows(i)(castcol.columnname))
                        End If
                    Catch ex As Exception

                    End Try
                Next
            Next
        End If

        Return table.Select(GenerateFilterString(filterset))
    End Function
    Public Sub UpdateRow(ByVal datatable As String, ByVal param As WriteParam(), ByVal filterset As Filters())
        Dim paramString As String = ""
        Dim command As New MySqlCommand

        command.Connection = GetConnection()

        For Each parameters In param

            Dim placeholder As String = "@" & parameters.parameter
            paramString &= parameters.parameter & " = " & " " & placeholder & ", "

            command.Parameters.AddWithValue(placeholder, Cast(parameters))
        Next

        paramString = paramString.Trim().Trim(",")

        Dim query As String = "UPDATE " & datatable & " SET " & paramString & GenerateFilterString(filterset, True)

        command.CommandText = query
        command.ExecuteNonQuery()
    End Sub

    Public Sub UpdateRow(ByVal datatable As String, ByVal param As WriteParam(), ByVal filterset As Filters(), Optional datacast As List(Of FilterCast) = Nothing)
        Dim id As Integer = GetData(datatable, filterset, datacast)(0)("id")

        Dim filters(0) As Filters

        filters(0).filtername = "id"
        filters(0).filtervalue = id

        Dim paramString As String = ""
        Dim command As New MySqlCommand

        command.Connection = GetConnection()

        For Each parameters In param

            Dim placeholder As String = "@" & parameters.parameter
            paramString &= parameters.parameter & " = " & " " & placeholder & ", "

            command.Parameters.AddWithValue(placeholder, Cast(parameters))
        Next

        paramString = paramString.Trim().Trim(",")

        Dim query As String = "UPDATE " & datatable & " SET " & paramString & GenerateFilterString(filters, True)

        command.CommandText = query
        command.ExecuteNonQuery()
    End Sub

    Public Function InsertRow(ByVal datatable As String, ByVal param As WriteParam()) As Integer
        Dim paramString As String = "("
        Dim placeholderString As String = "("
        Dim command As New MySqlCommand

        command.Connection = GetConnection()

        For Each parameters In param
            paramString &= parameters.parameter & ", "

            Dim placeholder As String = "@" & parameters.parameter

            placeholderString &= placeholder & ", "
            command.Parameters.AddWithValue(placeholder, Cast(parameters))
        Next

        paramString = paramString.Trim().Trim(",")
        placeholderString = placeholderString.Trim().Trim(",")
        paramString &= ")"
        placeholderString &= ")"

        Dim query As String = "insert into " & datatable & " " & paramString & " values " & placeholderString & "; SELECT LAST_INSERT_ID();"

        command.CommandText = query
        Return command.ExecuteScalar()

    End Function
    Public Sub DeleteData(ByVal datatable As String, ByVal filterset As Filters())
        Dim command As New MySqlCommand

        command.Connection = GetConnection()

        Dim query As String = "DELETE FROM " & datatable & " WHERE " & GenerateFilterString(filterset)

        command.CommandText = query
        command.ExecuteNonQuery()
    End Sub

    Private Function Cast(ByRef param As WriteParam) As Object
        Dim caster As New CDatabaseCast

        If param.caster IsNot Nothing Then
            Return param.caster.Cast(param.value)
        Else
            Return caster.Cast(param.value)
        End If
    End Function

    Public Function CheckConnection() As Boolean
        If (conn.State = ConnectionState.Closed) Then
            Return False
        Else
            Return True
        End If
    End Function
    Protected Overrides Sub Finalize()
        conn.Close()
        MyBase.Finalize()
    End Sub

    Public Function AuthAdmin(ByVal password As String) As Boolean
        Dim filters(0) As Filters

        filters(0).filtername = "name"
        filters(0).filtervalue = "admin_password"

        Dim dbAdminPass As String = GetData("app_settings", filters, New List(Of FilterCast))(0)("value")

        Return CEncryption.CheckHash(dbAdminPass, password)
    End Function

    Public Sub setUser(ByVal id As Integer, ByVal user As Integer, ByVal location As String)
        Dim filters(0) As Filters

        filters(0).filtername = "id"
        filters(0).filtervalue = id

        Dim param(1) As WriteParam

        param(0).parameter = "user"
        param(0).value = user

        param(1).parameter = "location"
        param(1).value = location
        param(1).caster = New CDatabaseCastEncryption

        UpdateRow("pc_main", param, filters)
    End Sub

    Public Function getUser(ByVal id As Integer) As Integer
        Dim filters(0) As Filters

        filters(0).filtername = "id"
        filters(0).filtervalue = id

        Dim data As DataTable = GetData("pc_main", filters)

        If data.Rows.Count > 0 Then
            If Not IsDBNull(data(0)("user")) Then
                Return data(0)("user")
            End If
        End If

        Return 0
    End Function

    Public Function getLocation(ByVal id As Integer) As String
        Dim filters(0) As Filters

        filters(0).filtername = "id"
        filters(0).filtervalue = id

        Dim data As DataRow() = GetData("pc_main", filters, New List(Of FilterCast)(New FilterCast() {New FilterCast With {.columnname = "location", .caster = New CDatabaseCastEncryption}}))

        If data.Length > 0 Then
            If Not IsDBNull(data(0)("location")) Then
                Return data(0)("location")
            End If
        End If

        Return ""
    End Function
End Class
