Imports System.ComponentModel
Imports System.DirectoryServices
Imports System.Threading
Public Class frmMain

    Private sysInfo As New CSystemInformation
    Public Property pcID As Integer = 0
    Private agent As CAgentAPI

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Hide()
        Me.Text = My.Application.Info.AssemblyName & " Beta v" & My.Application.Info.Version.ToString

        Dim scanThread As Thread = New Thread(AddressOf Scan)
        scanThread.Start()
    End Sub

    Private Sub Scan()
        Try
            Me.Invoke(Sub() UpdateStatus(True, "Scanning..."))

            agent = CAgentAPI.GetInstance()

            pcID = agent.UpdatePCMainInfo(sysInfo.GetListSysInfo())
            agent.UpdatePCInfo(sysInfo.GetPCInfo(), pcID)
            agent.UpdatePCPorts(sysInfo.CheckPorts(), pcID)

            Me.Invoke(Sub() UpdateStatus(True))
        Catch ex As Exception
            Me.Invoke(Sub() UpdateStatus(False, ex.Message.ToString))
        End Try
    End Sub

    Public Sub UpdateStatus(ByVal status As Boolean)
        If status Then
            lblStatus.Text = "Online"
            imgStatusIcon.Image = My.Resources.Led_Green
        Else
            lblStatus.Text = "Offline"
            imgStatusIcon.Image = My.Resources.Led_Gray
        End If
    End Sub
    Public Sub UpdateStatus(ByVal status As Boolean, ByVal msg As String)
        lblStatus.Text = msg

        If status Then
            imgStatusIcon.Image = My.Resources.Led_Green
        Else
            imgStatusIcon.Image = My.Resources.Led_Gray
        End If
    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        If Me.WindowState = FormWindowState.Minimized Then
            Me.Hide()
        End If
    End Sub

    Private Sub NotifyIcon1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles NotifyIcon1.MouseDoubleClick
        Me.Show()
        Me.WindowState = FormWindowState.Normal
    End Sub

    Private Sub frmMain_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        NotifyIcon1.Dispose()
    End Sub

    Private Sub frmMain_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.Alt AndAlso Not e.Control AndAlso Not e.Shift AndAlso e.KeyCode = Keys.X Then
            AdminAuth.Show()
        End If
    End Sub
End Class
