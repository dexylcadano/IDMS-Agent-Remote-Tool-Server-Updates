Public Class CPCLogs
    Dim db As CDatabase

    Public Sub New(ByRef db As CDatabase)
        Me.db = db

    End Sub

    Public Sub AddLogs(ByVal PC_id As Integer, ByVal log As String)
        Dim param(2) As WriteParam

        param(0).parameter = "pc_main_id"
        param(0).value = PC_id

        param(1).parameter = "log"
        param(1).value = log
        param(1).caster = New CDatabaseCastEncryption

        param(2).parameter = "created_at"
        param(2).value = Now
        db.InsertRow("pc_logs", param)
    End Sub
End Class
