Imports Microsoft.VisualBasic.Logging

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
        Dim getUsers = agent.GetAllUsers(token)
        Dim getOffices = agent.GetAllOfficialOffices(token)

        Try
            txtPCName.Text = My.Computer.Name.ToString()

            If getUsers IsNot Nothing Then
                cmbUsers.DataSource = agent.GetAllUsers(token)
                cmbUsers.DisplayMember = "name"
                cmbUsers.ValueMember = "user_id"
            End If

            If getOffices IsNot Nothing Then
                cmbOffices.DataSource = agent.GetAllOfficialOffices(token)
                cmbOffices.DisplayMember = "office_name"
                cmbOffices.ValueMember = "office_id"
            End If

            loadInformation()
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("error")
            System.Diagnostics.Debug.WriteLine(ex)
        End Try
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim userID As Integer = cmbUsers.SelectedValue
        Dim locationID As Integer = cmbOffices.SelectedValue
        Dim mainForm As frmMain = Application.OpenForms("frmMain")

        If mainForm.pcID < 1 Then
            MsgBox("Unable to fetch PC information!", MsgBoxStyle.OkOnly, "Error")

            Exit Sub
        End If

        If userID < 1 Or cmbUsers.SelectedItem Is Nothing Then
            MsgBox("Please check Selected User!", MsgBoxStyle.OkOnly, "Error")

            Exit Sub
        End If

        If locationID < 1 Or cmbOffices.SelectedItem Is Nothing Then
            MsgBox("Please check Selected Location!", MsgBoxStyle.OkOnly, "Error")

            Exit Sub
        End If

        Try
            Dim newInfo As New Dictionary(Of String, String) From {{"user", userID.ToString}, {"location", locationID.ToString}}

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
            Dim userID = user("user").ToString()
            Dim locationID = user("location").ToString()
            cmbUsers.SelectedValue = userID
            cmbOffices.SelectedValue = locationID
        Catch ex As Exception
            cmbUsers.SelectedIndex = -1
            cmbOffices.SelectedIndex = -1
        End Try
    End Sub
End Class