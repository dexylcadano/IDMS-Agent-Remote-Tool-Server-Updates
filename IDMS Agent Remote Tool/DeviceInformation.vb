Public Class frmDeviceInformation
    Dim dbUser As CDatabaseEHealth
    Dim db As CDatabase


    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.dbUser = CDatabaseEHealth.getInstanceEHealth()
        Me.db = CDatabase.getInstance()
    End Sub

    Private Sub frmDeviceInformation_Load(sender As Object, e As EventArgs) Handles Me.Load
        txtPCName.Text = My.Computer.Name.ToString()

        Dim users As DataTable = dbUser.GetUsers()
        users.Columns.Add("fullName", GetType(String), "first_name + ' ' + middle_name + ' ' + last_name")
        listUsers.DataSource = users
        cmbUsers.DataSource = listUsers
        cmbUsers.DisplayMember = "fullName"
        cmbUsers.ValueMember = "user_id"

        loadInformation()
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim userID As Integer = cmbUsers.SelectedValue
        Dim mainForm As frmMain = Application.OpenForms("frmMain")

        If mainForm.PC_ID < 1 Then
            MsgBox("Unable to fetch PC information!", MsgBoxStyle.OkOnly, "Error")

            Exit Sub
        End If

        If userID < 1 Or cmbUsers.SelectedItem Is Nothing Then
            MsgBox("Please check Selected User!", MsgBoxStyle.OkOnly, "Error")

            Exit Sub
        End If

        db.setUser(mainForm.PC_ID, userID, txtLocation.Text)
        Me.Close()
        MsgBox("Information successfully updated!", MsgBoxStyle.OkOnly, "Done")
    End Sub

    Private Sub loadInformation()
        Dim mainForm As frmMain = Application.OpenForms("frmMain")

        Dim userID = db.getUser(mainForm.PC_ID)

        If userID Then
            cmbUsers.SelectedValue = userID
        Else
            cmbUsers.SelectedIndex = -1
        End If

        txtLocation.Text = db.getLocation(mainForm.PC_ID)
    End Sub
End Class