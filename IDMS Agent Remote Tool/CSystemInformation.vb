
Imports System.Management
Imports System.Net.Sockets
Imports System.Threading

'https://www.codeguru.com/visual-basic/obtaining-computer-information-with-visual-basic-net/

Public Class CSystemInformation
    Dim db As CDatabase
    Dim activeThread As Boolean = True

    'Public Function GetInfo() As Integer
    '    Dim PC_id As Integer = AddMainPC(GetListSysInfo())
    '    SupplyPCInfo(GetListSysInfo("Win32_Processor"), PC_id, 1)
    '    SupplyPCInfo(GetListSysInfo("Win32_NetworkAdapter"), PC_id, 2)
    '    SupplyPCInfo(GetListSysInfo("Win32_PhysicalMemory"), PC_id, 3)
    '    SupplyPCInfo(GetListSysInfo("Win32_BaseBoard"), PC_id, 4)
    '    SupplyPCInfo(GetListSysInfo("Win32_VideoController"), PC_id, 5)

    '    Return PC_id
    'End Function

    Public Function GetPCInfo() As Dictionary(Of String, Dictionary(Of String, String))
        Dim allInfo As New Dictionary(Of String, Dictionary(Of String, String))

        allInfo.Add("1", GetListSysInfo("Win32_Processor"))
        allInfo.Add("2", GetListSysInfo("Win32_NetworkAdapter"))
        allInfo.Add("3", GetListSysInfo("Win32_PhysicalMemory"))
        allInfo.Add("4", GetListSysInfo("Win32_BaseBoard"))
        allInfo.Add("5", GetListSysInfo("Win32_VideoController"))
        ' allInfo.Add("Ports", CheckPorts())
        Return allInfo
    End Function

    'Public Sub GetPorts(ByVal pc_id As Object)
    '    Dim PortsThread As Thread = New Thread(AddressOf CheckPorts)
    '    PortsThread.Start(pc_id)
    'End Sub

    Public Function CheckPorts() As Dictionary(Of String, String)
        Dim portsInfo As New Dictionary(Of String, String)

        'For i As Integer = 1 To 65535
        For i As Integer = 1 To 65535

            portsInfo.Add("Port " & i, GetOpenPorts(i))
        Next

        Return portsInfo
    End Function
    'Private Sub CheckPorts(ByVal pc_id As Object)
    '    'Dim filters(0) As Filters
    '    Dim dbThread As CDatabase = New CDatabase()
    '    'filters(0).filtername = "pc_main_id"
    '    'filters(0).filtervalue = pc_id

    '    'db.DeleteData("pc_network_ports", filters)

    '    For i As Integer = 1 To 65535
    '        If Not activeThread Then
    '            Exit For
    '        End If

    '        Dim filters_b(1) As Filters

    '        filters_b(0).filtername = "port_no"
    '        ' filters_b(0).filtervalue = encryptor.Encrypt(i)
    '        filters_b(0).filtervalue = i
    '        filters_b(0).casted = True

    '        filters_b(1).filtername = "pc_main_id"
    '        filters_b(1).filtervalue = pc_id

    '        Dim search_port = dbThread.GetData("pc_network_ports", filters_b, New List(Of FilterCast)(New FilterCast() {New FilterCast With {.columnname = "port_no", .caster = New CDatabaseCastEncryption}}))

    '        If GetOpenPorts(i) Then
    '            If search_port IsNot Nothing AndAlso search_port.Count = 0 Then
    '                Dim param(3) As WriteParam

    '                param(0).parameter = "port_no"
    '                ' param(0).value = encryptor.Encrypt(i)
    '                param(0).value = i
    '                param(0).caster = New CDatabaseCastEncryption

    '                param(1).parameter = "pc_main_id"
    '                param(1).value = pc_id

    '                param(2).parameter = "updated_at"
    '                param(2).value = Now

    '                param(3).parameter = "status"
    '                param(3).value = "Active"

    '                dbThread.InsertRow("pc_network_ports", param)
    '            Else
    '                If search_port(0)("status") <> "Active" Then
    '                    Dim param(1) As WriteParam

    '                    param(0).parameter = "status"
    '                    param(0).value = "Active"

    '                    param(1).parameter = "updated_at"
    '                    param(1).value = Now

    '                    dbThread.UpdateRow("pc_network_ports", param, filters_b, New List(Of FilterCast)(New FilterCast() {New FilterCast With {.columnname = "port_no", .caster = New CDatabaseCastEncryption}}))
    '                End If
    '            End If
    '        Else
    '            If search_port IsNot Nothing AndAlso search_port.Count > 0 Then
    '                If search_port(0)("status") <> "Inactive" Then
    '                    Dim param(1) As WriteParam

    '                    param(0).parameter = "status"
    '                    param(0).value = "Inactive"

    '                    param(1).parameter = "updated_at"
    '                    param(1).value = Now

    '                    dbThread.UpdateRow("pc_network_ports", param, filters_b, New List(Of FilterCast)(New FilterCast() {New FilterCast With {.columnname = "port_no", .caster = New CDatabaseCastEncryption}}))
    '                End If
    '            End If
    '        End If
    '    Next
    'End Sub
    'Private Function AddMainPC(ByVal pc As Dictionary(Of String, String)) As Integer
    '    Dim filters(0) As Filters

    '    filters(0).filtername = "name"
    '    filters(0).filtervalue = pc("Computer Name")
    '    filters(0).casted = True

    '    Dim search_pc = db.GetData("pc_main", filters, New List(Of FilterCast)(New FilterCast() {New FilterCast With {.columnname = "name", .caster = New CDatabaseCastEncryption}}))
    '    If search_pc IsNot Nothing AndAlso search_pc.Count = 0 Then
    '        Dim param(2) As WriteParam

    '        param(0).parameter = "name"
    '        param(0).value = pc("Computer Name")
    '        param(0).caster = New CDatabaseCastEncryption

    '        param(1).parameter = "opened_at"
    '        param(1).value = Now

    '        param(2).parameter = "agent_version"
    '        param(2).value = My.Application.Info.Version.ToString

    '        Return db.InsertRow("pc_main", param)
    '    Else
    '        Dim param(1) As WriteParam

    '        param(0).parameter = "opened_at"
    '        param(0).value = Now

    '        param(1).parameter = "agent_version"
    '        param(1).value = My.Application.Info.Version.ToString

    '        db.UpdateRow("pc_main", param, filters, New List(Of FilterCast)(New FilterCast() {New FilterCast With {.columnname = "name", .caster = New CDatabaseCastEncryption}}))
    '    End If

    '    Return search_pc(0)("id")
    'End Function

    'Private Sub SupplyPCInfo(ByVal pc As Dictionary(Of String, String), ByVal pc_id As Integer, ByVal pc_group As Integer)
    '    For Each pc_info In pc
    '        If pc_info.Value IsNot Nothing AndAlso pc_info.Value <> "" Then
    '            Dim filters(2) As Filters

    '            filters(0).filtername = "pc_main_id"
    '            filters(0).filtervalue = pc_id

    '            filters(1).filtername = "properties"
    '            filters(1).filtervalue = pc_info.Key
    '            filters(1).casted = True

    '            filters(2).filtername = "pc_info_group"
    '            filters(2).filtervalue = pc_group

    '            Dim search_pc_info = db.GetData("pc_info", filters, New List(Of FilterCast)(New FilterCast() {New FilterCast With {.columnname = "properties", .caster = New CDatabaseCastEncryption}}))
    '            If search_pc_info IsNot Nothing AndAlso search_pc_info.Count = 0 Then
    '                Dim param(4) As WriteParam

    '                param(0).parameter = "properties"
    '                param(0).value = pc_info.Key
    '                param(0).caster = New CDatabaseCastEncryption

    '                param(1).parameter = "value"
    '                param(1).value = pc_info.Value
    '                param(1).caster = New CDatabaseCastEncryption

    '                param(2).parameter = "pc_main_id"
    '                param(2).value = pc_id

    '                param(3).parameter = "updated_at"
    '                param(3).value = Now

    '                param(4).parameter = "pc_info_group"
    '                param(4).value = pc_group

    '                db.InsertRow("pc_info", param)

    '            ElseIf search_pc_info IsNot Nothing AndAlso search_pc_info.Count > 0 Then
    '                'If search_pc_info.Rows(0)("value") <> pc_info.Value Then
    '                Dim param(1) As WriteParam

    '                param(0).parameter = "value"
    '                param(0).value = pc_info.Value
    '                param(0).caster = New CDatabaseCastEncryption

    '                param(1).parameter = "updated_at"
    '                param(1).value = Now

    '                db.UpdateRow("pc_info", param, filters, New List(Of FilterCast)(New FilterCast() {New FilterCast With {.columnname = "properties", .caster = New CDatabaseCastEncryption}}))
    '                'End If
    '            End If
    '        End If
    '    Next

    'End Sub



    Private Function GetListSysInfo(ByVal stringIn As String) As Dictionary(Of String, String)
        Dim item As New Dictionary(Of String, String)

        Try
            Dim mcInfo As New ManagementClass(stringIn)
            Dim mcInfoCol As ManagementObjectCollection = mcInfo.GetInstances()
            Dim pdInfo As PropertyDataCollection = mcInfo.Properties

            For Each objMng As ManagementObject In mcInfoCol
                For Each prop As PropertyData In pdInfo
                    Try
                        If objMng.Properties(prop.Name).Value <> "" Then

                            Dim encypted_val As String = objMng.Properties(prop.Name).Value
                            Dim encypted_prop As String = prop.Name

                            item.Add(encypted_prop, encypted_val)
                        End If

                    Catch ex As Exception

                    End Try
                Next
            Next
        Catch ex As Exception

        End Try

        Return item
    End Function
    Public Function GetListSysInfo() As Dictionary(Of String, String)
        Dim item As New Dictionary(Of String, String)

        item.Add("name", My.Computer.Name.ToString())
        item.Add("Total Physical Memory", System.Math.Round(My.Computer.Info.TotalPhysicalMemory / (1024 * 1024), 2).ToString)
        item.Add("Available Physical Memory", System.Math.Round(My.Computer.Info.AvailablePhysicalMemory / (1024 * 1024), 2).ToString)
        item.Add("Operating System", My.Computer.Info.OSFullName)
        item.Add("opened_at", Now.ToString("yyyy-MM-dd HH:mm::ss"))
        item.Add("agent_version", My.Application.Info.Version.ToString)
        Return item
    End Function
    Private Function GetOpenPorts(ByVal portNum As Object) As Boolean
        Dim tcp As New TcpClient

        Try
            If tcp.ConnectAsync("127.0.0.1", portNum).Wait(1) Then
                tcp.Close()
                Return True
            End If

        Catch ex As Exception

        End Try

        tcp.Close()
        Return False
    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Public Sub Closing()
        activeThread = False
    End Sub
End Class
