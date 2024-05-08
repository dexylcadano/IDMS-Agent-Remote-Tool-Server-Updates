Public Class AdminAuth
    Private database As CDatabase

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        database = CDatabase.getInstance()
    End Sub

    Private Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        Dim txtPass As String = editPassword.Text

        If (database.AuthAdmin(txtPass)) Then
            Me.Close()

            frmDeviceInformation.Show()
        Else
            MsgBox("Wrong Password!", MsgBoxStyle.OkOnly, "Error")
            editPassword.Text = ""
        End If
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub
End Class