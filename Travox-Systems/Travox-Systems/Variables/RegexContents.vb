Imports System.Text
Imports System.Text.RegularExpressions

Public Class RegexContents
    Private data As New StringBuilder()

    Public Sub New(ByVal contents As String)
        Me.New(New StringBuilder(contents))
    End Sub
    Public Sub New(ByVal contents As StringBuilder)
        data = contents
    End Sub

    Public Function TirmLeft(ByVal TirmString As String) As Boolean
        Dim index As Integer = data.ToString().IndexOf(TirmString)
        If (index > -1) Then
            data.Remove(0, index - 1)
            Return True
        End If
        Return False
    End Function
    Public Function TirmRight(ByVal TirmString As String) As Boolean
        Dim index As Integer = data.ToString().IndexOf(TirmString)
        If (index > -1) Then
            data.Remove(index, data.Length - index)
            Return True
        End If
        Return False
    End Function
    Public Function Tirm(ByVal LeftString As String, ByVal RightString As String) As Boolean
        Dim index_l As Integer = data.ToString().IndexOf(LeftString), index_r As Integer = data.ToString().IndexOf(RightString)
        If (index_l > -1 And index_r > -1) Then
            data.Remove(index_l, (index_r - index_l) + RightString.Length)
            Return True
        End If
        Return False
    End Function

    Public Function Crop(ByVal LeftString As String, ByVal RightString As String) As String
        Dim getContent As String = data.ToString(), index_l As Integer = Regex.Match(getContent, LeftString).Index, index_r As Integer = Regex.Match(getContent, RightString).Index
        If (index_l > -1 And index_r > 0) Then
            getContent = getContent.Substring(index_l + Regex.Replace(LeftString, "\\", "").Length, index_r - index_l - Regex.Replace(LeftString, "\\", "").Length)
        ElseIf (index_r = -1 Or index_r = 0) Then
            getContent = getContent.Substring(index_l, data.ToString().Length - index_l)
        End If
        Return getContent
    End Function
    Public Shared Function RemoveEndOfLine(ByVal contents As String) As String
        contents = Regex.Replace(contents.Trim, vbCrLf, " ", RegexOptions.IgnoreCase)
        contents = Regex.Replace(contents.Trim, vbCr, " ", RegexOptions.IgnoreCase)
        contents = Regex.Replace(contents.Trim, vbLf, " ", RegexOptions.IgnoreCase)
        Return contents.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Trim()
    End Function
    Public Overrides Function ToString() As String
        Return data.ToString()
    End Function
End Class
