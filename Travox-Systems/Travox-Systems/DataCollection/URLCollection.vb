Imports System.Collections.Specialized
Imports System.Web

Namespace DataCollection
    Public Class URLCollection
        Inherits NameValueCollection

        Public Function GETEncode(ByVal url As String) As String
            Dim result As String = "'"

            For Each item As String In Me.AllKeys
                If (Not String.IsNullOrEmpty(item)) Then result &= item & "=" & HttpContext.Current.Server.HtmlEncode(Me(item).ToString()) & "&"
            Next
            Return url & "?" & result.Remove(result.Length - 1, 1) & "'"
        End Function

        Public Function POSTEncode(ByVal url As String) As String
            Dim result As String = "{"
            For Each item As String In Me.AllKeys
                If (Not String.IsNullOrEmpty(item)) Then result &= """" & item & """:""" & HttpContext.Current.Server.HtmlEncode(Me(item)) & ""","
            Next
            result &= """url"":" & """" & url & """}"
            Return result
        End Function

    End Class
End Namespace
