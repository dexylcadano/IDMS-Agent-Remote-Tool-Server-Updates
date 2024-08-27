Imports System.ComponentModel
Imports System.DirectoryServices
Imports System.IO
Imports System.IO.Compression
Imports System.Net.NetworkInformation
Imports System.Security
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
                Else
                    loadStartup()
                End If
            Else
                loadStartup()
            End If
        Catch ex As Exception
            MessageBox.Show("Error checking for updates: " & ex.Message)
            loadStartup()
        End Try
    End Sub

    Private Sub loadStartup()
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
        Dim LDAPAuthentication = ActiveDirectoryAuthentication()
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

            If LDAPAuthentication Then
                Application.Exit()
            End If
        Catch ex As HttpRequestException
            MessageBox.Show("Error downloading the update: " & ex.Message)
        Catch ex As UnauthorizedAccessException
            MessageBox.Show("Error: No permission to write to the download path. " & ex.Message)
        Catch ex As Exception
            MessageBox.Show("An unexpected error occurred: " & ex.Message)
        End Try
    End Sub

    Private Function ActiveDirectoryAuthentication() As Boolean
        Dim path = "LDAP://entdswd.local"
        Dim user = "IDMS_Admin"
        Dim pass = "1dm$@4dm1Nb$$muD"
        'Dim user = "icts"
        'Dim pass = "optimasDrOwp455W0rd!"
        Dim domain As String = "entdswd.local"
        Dim de As New DirectoryEntry(path, user, pass, AuthenticationTypes.Secure)
        Try
            Dim ds As DirectorySearcher = New DirectorySearcher(de)
            ds.Filter = $"(sAMAccountName={user})"
            Dim result As SearchResult = ds.FindOne()

            Dim isAdmin = IsUserAdminInLDAP(de, result)

            If isAdmin Then
                InstallApplication(user, pass, domain)
                Return True
            Else
                MessageBox.Show("User is authenticated but does not have administrative privileges.")
            End If
        Catch ex As Exception
            MessageBox.Show("Authentication failed: " & ex.Message)
        End Try
        Return False
    End Function
    Private Function IsUserAdminInLDAP(de As DirectoryEntry, result As SearchResult) As Boolean
        Dim adminGroupDn As String = "CN=08_OU_Administrators,OU=Administrator Identities,OU=Restricted,OU=FO8,DC=ENTDSWD,DC=LOCAL"
        Try
            For Each group As String In result.Properties("memberOf")
                If group.Contains(adminGroupDn) Then
                    Return True
                End If
            Next
        Catch ex As Exception
            MessageBox.Show("Error checking admin privileges: " & ex.Message)
        End Try

        Return False
    End Function
    Private Sub InstallApplication(userName As String, password As String, domain As String)
        Try
            Dim securePassword As New SecureString()
            For Each c As Char In password
                securePassword.AppendChar(c)
            Next

            Dim setupFilePath As String = System.IO.Path.Combine(Application.StartupPath, "setup.exe")

            Dim startInfo As New ProcessStartInfo()
            startInfo.FileName = "msiexec.exe"
            startInfo.Arguments = $"/i ""{setupFilePath}"""
            startInfo.UseShellExecute = False
            startInfo.UserName = userName
            startInfo.Password = securePassword
            startInfo.Domain = domain

            startInfo.WorkingDirectory = Application.StartupPath

            Dim process As Process = Process.Start(startInfo)

            process.WaitForExit()

            Dim errors As String = process.StandardError.ReadToEnd()

            If process.ExitCode = 0 Then
                MessageBox.Show("Installation completed successfully.")
            Else
                MessageBox.Show($"Installation failed. Errors: {errors}")
            End If

        Catch ex As Exception
            MessageBox.Show("Failed to install: " & ex.Message)
        End Try
    End Sub
End Class
