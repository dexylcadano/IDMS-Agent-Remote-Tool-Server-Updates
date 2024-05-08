Public Class CDatabaseCastEncryption
    Inherits CDatabaseCast

    Dim encryptor As New CEncryption

    Public Overrides Function Cast(ByVal val As Object) As Object
        Return encryptor.Encrypt(val)
    End Function

    Public Overrides Function DeCast(ByVal val As Object) As Object
        Return encryptor.Decrypt(val)
    End Function
End Class
