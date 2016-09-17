Imports System.Text.RegularExpressions

Public Class JSON
    'Public Shared Function Serialize(Of T)(ByVal v As T) As String
    '    Dim memStream As New IO.MemoryStream()
    '    Dim dataSerializer As New DataContractJsonSerializer(v.[GetType]())
    '    dataSerializer.WriteObject(memStream, v)
    '    Dim jsonString As String = Text.Encoding.UTF8.GetString(memStream.ToArray())
    '    memStream.Close()
    '    Return jsonString
    'End Function
    'Public Shared Function Deserialize(Of T)(ByVal v As String) As T
    '    Dim dataSerializer As New DataContractJsonSerializer(GetType(T))
    '    Dim ms As New IO.MemoryStream(Text.Encoding.UTF8.GetBytes(v))
    '    Return DirectCast(dataSerializer.ReadObject(ms), T)
    'End Function

    'Public Shared Function FixString(ByVal plaintext As String) As String
    '    plaintext = plaintext.Replace("\\r\\n", "<br>").Replace("\", "\\").Replace("""", "\""").Replace("'", "\""")
    '    Return Regex.Replace(plaintext, "\s+", " ")
    'End Function
End Class
