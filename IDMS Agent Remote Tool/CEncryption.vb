Imports System.Security.Cryptography
Imports System.Text
Imports System.Web.Script.Serialization

'https//gist.github.com/doncadavona/fd493b6ced456371da8879c22bb1c263
Public Class CEncryption
    Private Shared ReadOnly encoding As Encoding = Encoding.UTF8
    Public Function Encrypt(ByVal input As String) As String
        '   Return AES_Encrypt(AES_Encrypt(input, My.Resources.EPVal1), My.Resources.EPVal2)
        'Return AES_Encrypt_v2(input, "OlCwFuRf0qsgLHG7CPf4s09pQhIT5tdfF8tQvxONugg=")
        Return Encrypt_AES_256_CBC(Encrypt_AES_256_CBC(input, My.Resources.EPVal1), My.Resources.EPVal2)
        'Return Encrypt_AES_256_CBC(input, My.Resources.EPVal1)
    End Function

    Public Function Decrypt(ByVal input As String) As String
        'Return AES_Decrypt(AES_Decrypt(input, My.Resources.EPVal2), My.Resources.EPVal1)
        Return Decrypt_AES_256_CBC(Decrypt_AES_256_CBC(input, My.Resources.EPVal2), My.Resources.EPVal1)
        'Return Decrypt_AES_256_CBC(input, My.Resources.EPVal1)
    End Function


    Private Function Encrypt_AES_256_CBC(ByVal input As String, ByVal key As String) As String
        Try
            Dim aes As RijndaelManaged = GetEncryptor(key)

            Dim AESEncrypt As ICryptoTransform = aes.CreateEncryptor(aes.Key, aes.IV)
            Dim buffer() As Byte = encoding.GetBytes(input)

            Dim encryptedText As String = Convert.ToBase64String(AESEncrypt.TransformFinalBlock(buffer, 0, buffer.Length))

            Dim mac As String = ""

            mac = BitConverter.ToString(HmacSHA256_Function(Convert.ToBase64String(aes.IV) + encryptedText, key)).Replace("-", "").ToLower()

            Dim keyValues As New Dictionary(Of String, Object)
            keyValues.Add("iv", Convert.ToBase64String(aes.IV))
            keyValues.Add("value", encryptedText)
            keyValues.Add("mac", mac)

            Dim serializer As New JavaScriptSerializer

            Return Convert.ToBase64String(encoding.GetBytes(serializer.Serialize(keyValues)))
        Catch ex As Exception
            Return ""
        End Try
    End Function


    Private Function Decrypt_AES_256_CBC(ByVal input As String, ByVal key As String) As String
        If input.Trim = "" Then
            Return ""
        End If

        Try
            Dim aes As RijndaelManaged = GetEncryptor(key)

            Dim base64Decoded() As Byte = Convert.FromBase64String(input)
            Dim base64DecodedStr As String = encoding.GetString(base64Decoded)

            Dim serializer As New JavaScriptSerializer
            Dim keyValues As New Dictionary(Of String, Object)

            keyValues = serializer.Deserialize(Of Dictionary(Of String, Object))(base64DecodedStr)

            aes.IV = Convert.FromBase64String(keyValues("iv"))
            Dim AESDecrypt As ICryptoTransform = aes.CreateDecryptor(aes.Key, aes.IV)
            Dim buffer() As Byte = Convert.FromBase64String(keyValues("value"))

            Return encoding.GetString(AESDecrypt.TransformFinalBlock(buffer, 0, buffer.Length))
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Private Function HmacSHA256_Function(ByVal data As String, ByVal key As String) As Byte()
        Using hmac As HMACSHA256 = New HMACSHA256(encoding.GetBytes(key))
            Return hmac.ComputeHash(encoding.GetBytes(data))
        End Using
    End Function

    Private Function GetEncryptor(ByVal key As String) As RijndaelManaged
        Dim aes As New RijndaelManaged
        aes.KeySize = 256
        aes.BlockSize = 128
        aes.Padding = PaddingMode.PKCS7
        aes.Mode = CipherMode.CBC

        aes.Key = encoding.GetBytes(key)
        aes.GenerateIV()

        Return aes
    End Function

    Shared Function GetHash(theInput As String) As String

        Using hasher As SHA256 = SHA256.Create()

            Dim hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(theInput & My.Resources.HashSalt))

            Return Convert.ToBase64String(hash)
        End Using

    End Function

    Shared Function CheckHash(hashedStr As String, newInput As String) As Boolean
        Dim newHash As String = GetHash(newInput)

        Return String.Compare(hashedStr, newHash, False) = 0
    End Function
End Class
