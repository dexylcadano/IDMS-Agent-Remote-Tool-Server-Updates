Imports System.ComponentModel
Imports System.DirectoryServices
Imports System.Threading
Public Class frmMain
    Private database As CDatabase
    Private updatePass As New CDirectory
    Private sysInfo As New CSystemInformation
    Public pcID As Integer = 0

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        database = CDatabase.getInstance()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Hide()
        Me.Text = My.Application.Info.AssemblyName & " Beta v" & My.Application.Info.Version.ToString

        MainFunction()
    End Sub

    Public Property PC_ID() As Integer
        Get
            Return pcID
        End Get
        Set(ByVal value As Integer)
            pcID = value
        End Set
    End Property

    Public Sub MainFunction()
        Dim status As Boolean = database.CheckConnection()
        UpdateStatus(status)

        If status Then
            LoadFunction()
            'Dim PortsThread As Thread = New Thread(AddressOf LoadFunction)
            'PortsThread.Start()
        Else
            tmrRecon.Enabled = True
        End If
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

    Private Sub LoadFunction()
        pcID = sysInfo.GetInfo()
        'updatePass.UpdatePassword(pc)
        sysInfo.GetPorts(pcID)
        ' frmDeviceInformation.PC = pc
    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        If Me.WindowState = FormWindowState.Minimized Then
            'NotifyIcon1.Visible = True
            'NotifyIcon1.Icon = SystemIcons.Application
            'NotifyIcon1.BalloonTipIcon = ToolTipIcon.Info
            'NotifyIcon1.BalloonTipTitle = "Testing"
            'NotifyIcon1.BalloonTipText = "Testing"
            'NotifyIcon1.ShowBalloonTip(50000)
            Me.Hide()
            'ShowInTaskbar = True
        End If
    End Sub

    'Public Function getSoundDeviceStructure() As DataTable
    '    Dim dt As New DataTable
    '    dt.Columns.Add(New DataColumn("Manufacturer"))
    '    dt.Columns.Add(New DataColumn("Name"))
    '    dt.Columns.Add(New DataColumn("PNPDeviceID"))
    '    dt.Columns.Add(New DataColumn("ProductName"))
    '    Return dt
    'End Function

    Private Sub NotifyIcon1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles NotifyIcon1.MouseDoubleClick
        ' ShowInTaskbar = True
        Me.Show()
        Me.WindowState = FormWindowState.Normal

        ' NotifyIcon1.Visible = False
    End Sub

    Private Sub frmMain_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        sysInfo.Closing()

        NotifyIcon1.Dispose()
    End Sub

    Private Sub tmrRecon_Tick(sender As Object, e As EventArgs) Handles tmrRecon.Tick
        tmrRecon.Enabled = False

        MainFunction()
    End Sub

    Private Sub frmMain_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.Alt AndAlso Not e.Control AndAlso Not e.Shift AndAlso e.KeyCode = Keys.X Then
            AdminAuth.Show()
        End If
    End Sub
End Class
