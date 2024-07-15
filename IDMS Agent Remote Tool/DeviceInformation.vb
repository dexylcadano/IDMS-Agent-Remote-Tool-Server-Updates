Public Class frmDeviceInformation
    Public Property token As String
    Private agent As CAgentAPI

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        agent = CAgentAPI.GetInstance()
    End Sub

    Private Sub frmDeviceInformation_Load(sender As Object, e As EventArgs) Handles Me.Load

        Try
            txtPCName.Text = My.Computer.Name.ToString()

            cmbUsers.DataSource = agent.GetAllUsers(token)
            cmbUsers.DisplayMember = "name"
            cmbUsers.ValueMember = "user_id"

            loadInformation()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim userID As Integer = cmbUsers.SelectedValue
        Dim mainForm As frmMain = Application.OpenForms("frmMain")

        If mainForm.pcID < 1 Then
            MsgBox("Unable to fetch PC information!", MsgBoxStyle.OkOnly, "Error")

            Exit Sub
        End If

        If userID < 1 Or cmbUsers.SelectedItem Is Nothing Then
            MsgBox("Please check Selected User!", MsgBoxStyle.OkOnly, "Error")

            Exit Sub
        End If

        Try
            Dim newInfo As New Dictionary(Of String, String) From {{"user", userID.ToString}, {"location", txtLocation.Text}}

            agent.SetUser(token, mainForm.pcID, newInfo)

            Me.Close()
            MsgBox("Information successfully updated!", MsgBoxStyle.OkOnly, "Done")
        Catch ex As Exception
            MsgBox("Unable to update PC information!", MsgBoxStyle.OkOnly, "Error")
            Me.Close()
        End Try

    End Sub

    Private Sub loadInformation()
        Dim mainForm As frmMain = Application.OpenForms("frmMain")

        Dim user = agent.GetUser(mainForm.pcID, token)

        Try
            Dim userID = CInt(user("user").ToString())

            cmbUsers.SelectedValue = userID
        Catch ex As Exception
            cmbUsers.SelectedIndex = -1
        End Try

        txtLocation.Text = user("location").ToString()
    End Sub
End Class