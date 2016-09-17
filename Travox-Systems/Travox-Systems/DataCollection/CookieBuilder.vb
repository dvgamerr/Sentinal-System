Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.Text

Namespace DataCollection
    Public Class CookieBuilder
        Inherits NameValueCollection

        Public Sub New()

        End Sub

        Public Overrides Function ToString() As String
            Dim data As String = ""
            For Each key As String In MyBase.AllKeys
                Data += key + "=" + MyBase.Get(key) + "; "
            Next
            Return data.ToString()
        End Function
    End Class
End Namespace