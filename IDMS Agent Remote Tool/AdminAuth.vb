Public Class AdminAuth
    Private agent As CAgentAPI

    Private token As String

    Private Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        Dim txtPass As String = editPassword.Text

        agent = CAgentAPI.GetInstance()

        Try
            Dim myData As New Dictionary(Of String, String) From {{"password", txtPass}}

            token = agent.GetToken(myData)

            frmDeviceInformation.token = Me.token
            frmDeviceInformation.Show()

            Me.Close()
        Catch ex As Exception
            MsgBox("Wrong Password!", MsgBoxStyle.OkOnly, "Error")
            editPassword.Text = ""
        End Try
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    Private Sub AdminAuth_Load(sender As Object, e As EventArgs) Handles Me.Load
        editPassword.Select()
    End Sub
End Class