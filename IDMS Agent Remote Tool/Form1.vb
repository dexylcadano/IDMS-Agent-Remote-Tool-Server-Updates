Imports System.ComponentModel
Imports System.DirectoryServices
Imports System.IO
Imports System.IO.Compression
Imports System.Net.NetworkInformation
Imports System.Threading
Public Class frmMain
    Private Const VersionUrl As String = "https://git.dswd.gov.ph/dpcadano/idms-agent-remote-tool-server-updates/-/raw/main/Version.txt"
    Private Const DownloadUrl As String = "https://git.dswd.gov.ph/dpcadano/idms-agent-remote-tool-server-updates/-/raw/main/IDMSAgentRemoteToolSetup.zip"
    Private Const CurrentVersion As String = "0.0.0.8"

    Private sysInfo As New CSystemInformation
    Public Property pcID As Integer = 0
    Private agent As CAgentAPI

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            Dim client1 As New HttpClient()

            Dim latestVersion As String = client1.GetStringAsync(VersionUrl).Result.Trim()

            If latestVersion <> CurrentVersion Then
                Dim result As DialogResult = MessageBox.Show($"A new version ({latestVersion}) is available. Would you like to update?", "Update Available", MessageBoxButtons.YesNo)

                If result = DialogResult.Yes Then
                    DownloadUpdate()
                End If
            Else
                Me.Hide()
                Me.Text = My.Application.Info.AssemblyName & " Beta v" & My.Application.Info.Version.ToString
                Dim scanThread As Thread = New Thread(AddressOf Scan)
                scanThread.Start()
            End If
        Catch ex As Exception
            MessageBox.Show("Error checking for updates: " & ex.Message)
        End Try
    End Sub

    Private Sub Scan()
        Try
            Me.Invoke(Sub() UpdateStatus(True, "Scanning..."))

            agent = CAgentAPI.GetInstance()

            pcID = agent.UpdatePCMainInfo(sysInfo.GetListSysInfo())
            agent.UpdatePCInfo(sysInfo.GetPCInfo(), pcID)
            agent.UpdatePCPorts(sysInfo.CheckPorts(), pcID)
            agent.UpdatePCInstalledSoftwares(sysInfo.checkInstalledSoftwares(), pcID)

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

    Private Async Sub DownloadUpdate()
        Try
            If File.Exists(".\IDMSAgentRemoteToolSetup.msi") Then
                File.Delete(".\IDMSAgentRemoteToolSetup.msi")
            End If

            Dim downloadPath As String = Path.Combine(Application.StartupPath, "IDMSAgentRemoteToolSetup.zip")

            Using client As New HttpClient()
                Using response As HttpResponseMessage = Await client.GetAsync(DownloadUrl)
                    response.EnsureSuccessStatusCode()
                    Dim fileBytes As Byte() = Await response.Content.ReadAsByteArrayAsync()
                    File.WriteAllBytes(downloadPath, fileBytes)
                End Using
            End Using

            Dim zipPath As String = ".\IDMSAgentRemoteToolSetup.zip"
            Dim extractPath As String = ".\"
            ZipFile.ExtractToDirectory(zipPath, extractPath)

            Dim process As New Process()
            process.StartInfo.FileName = "msiexec.exe"
            process.StartInfo.Arguments = "/i IDMSAgentRemoteToolSetup.msi"

            MessageBox.Show("Update downloaded. The application will now close for the update to be applied.", "Update Downloaded")
            Application.Exit()
            process.Start()

        Catch ex As HttpRequestException
            MessageBox.Show("Error downloading the update: " & ex.Message)
        Catch ex As UnauthorizedAccessException
            MessageBox.Show("Error: No permission to write to the download path. " & ex.Message)
        Catch ex As Exception
            MessageBox.Show("An unexpected error occurred: " & ex.Message)
        End Try
    End Sub

End Class
