Imports System.IO
Imports System.IO.Compression
Imports System.Management
Imports System.Net.Sockets
Imports System.Threading.Tasks
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports Microsoft.Win32

'https://www.codeguru.com/visual-basic/obtaining-computer-information-with-visual-basic-net/

Public Class CSystemInformation
    Public Function GetPCInfo() As Dictionary(Of String, Dictionary(Of String, String))
        Dim allInfo As New Dictionary(Of String, Dictionary(Of String, String))

        allInfo.Add("1", GetListSysInfo("Win32_Processor"))
        allInfo.Add("2", GetListSysInfo("Win32_NetworkAdapter"))
        allInfo.Add("3", GetListSysInfo("Win32_PhysicalMemory"))
        allInfo.Add("4", GetListSysInfo("Win32_BaseBoard"))
        allInfo.Add("5", GetListSysInfo("Win32_VideoController"))
        Return allInfo
    End Function

    Public Function CheckPorts() As List(Of Dictionary(Of String, String))
        Dim portsInfoList As New List(Of Dictionary(Of String, String))

        For i As Integer = 1 To 65535
            Dim portStatus = GetOpenPorts(i)

            If portStatus Then
                Dim portsInfo As New Dictionary(Of String, String)

                portsInfo.Add("port_no", i)
                portsInfo.Add("status", "Active")
                portsInfoList.Add(portsInfo)
            End If
        Next

        Return portsInfoList
    End Function

    Private Function GetListSysInfo(ByVal stringIn As String) As Dictionary(Of String, String)
        Dim item As New Dictionary(Of String, String)

        Try
            Dim mcInfo As New ManagementClass(stringIn)
            Dim mcInfoCol As ManagementObjectCollection = mcInfo.GetInstances()
            Dim pdInfo As PropertyDataCollection = mcInfo.Properties

            For Each objMng As ManagementObject In mcInfoCol
                For Each prop As PropertyData In pdInfo
                    Try
                        If objMng.Properties(prop.Name).Value.ToString.Trim <> "" Then

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
    Public Function checkInstalledSoftwares() As List(Of Dictionary(Of String, String))
        Dim itemList As New List(Of Dictionary(Of String, String))

        Dim installedSoftware = GetInstalledSoftware()
        For Each software In installedSoftware
            Dim items As New Dictionary(Of String, String)
            items.Add("software_name", software.Value)
            itemList.Add(items)
        Next
        Return itemList
    End Function

    Private Function GetInstalledSoftware() As Dictionary(Of String, String)
        Dim softwareList As New Dictionary(Of String, String)()
        Dim registryPaths As String() = {
            "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
            "SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
        }
        Try
            For Each registryPath In registryPaths
                Using key As RegistryKey = Registry.LocalMachine.OpenSubKey(registryPath)
                    If key IsNot Nothing Then
                        For Each subKeyName In key.GetSubKeyNames()
                            Using subKey As RegistryKey = key.OpenSubKey(subKeyName)
                                If subKey IsNot Nothing Then
                                    Dim displayName As String = CStr(subKey.GetValue("DisplayName", ""))
                                    If Not String.IsNullOrEmpty(displayName) Then
                                        softwareList(displayName) = displayName
                                    End If
                                End If
                            End Using
                        Next
                    End If
                End Using
            Next
        Catch ex As Exception
        End Try
        Return softwareList
    End Function
End Class
