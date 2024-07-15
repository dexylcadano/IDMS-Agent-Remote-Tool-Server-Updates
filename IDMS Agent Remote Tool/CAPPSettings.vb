Public Class CAPPSettings
    Private path As String = ".\settings.txt"
    Private settings As Dictionary(Of String, String)

    Public Sub New()
        settings = GetSettings()
    End Sub

    Private Function GetSettings() As Dictionary(Of String, String)
        Dim settings As New Dictionary(Of String, String)

        Try
            Using reader As IO.StreamReader = New IO.StreamReader(path)
                Dim line As String = reader.ReadLine
                If (line.Trim() <> "") Then
                    settings.Add(line.Split("=")(0).Trim(), line.Split("=")(1).Trim())
                End If
            End Using
        Catch ex As Exception

        End Try

        Return settings
    End Function

    Public Sub UpdateAPPSettings(ByVal keyword As String, ByVal value As String)
        Dim file As System.IO.StreamWriter

        IO.File.WriteAllText(path, "")

        settings(keyword) = value

        file = My.Computer.FileSystem.OpenTextFileWriter(path, True)

        For Each setting_iter As KeyValuePair(Of String, String) In settings
            file.WriteLine(String.Format("{0} = {1}", setting_iter.Key.ToString, setting_iter.Value))
        Next

        file.Close()
    End Sub

    Public Function GetSetting(ByVal key As String) As Object
        Try
            Return settings(key)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

End Class
