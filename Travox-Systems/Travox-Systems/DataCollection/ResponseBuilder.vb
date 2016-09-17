Imports System.Text.RegularExpressions
Imports System.Text
Imports System.IO

Namespace DataCollection
    Public Class ResponseBuilder
        Private ResponseSuccess As [Boolean] = False
        Public Index As Int64 = 0
        Public Raw As StringBuilder

        Public Sub New()
            Raw = New StringBuilder()
        End Sub

        Public ReadOnly Property Headers As String
            Get
                Return Raw.ToString().Substring(0, Index)
            End Get
        End Property

        Public ReadOnly Property Body As String
            Get
                Dim result As String = ""
                If Raw.Length > Index Then result = Raw.ToString().Substring(Index + 4, Raw.Length - Index - 4)
                Return result
            End Get
        End Property


        Public Sub Append(text As [String])
            Raw.Append(text)
            Me.ExecuteHeaders()
        End Sub

        Public Sub Remove(startIndex As Int32, length As Int32)
            Raw.Remove(startIndex, length)
        End Sub

        Private Sub ExecuteHeaders()
            If Index = 0 Then
                Dim ExecuteRaw As Match = Regex.Match(Raw.ToString(), "\r\n\r\n")
                If (ExecuteRaw.Success) Then Index = ExecuteRaw.Index
            End If
        End Sub

        Public Overrides Function ToString() As String
            Return Me.Body
        End Function
    End Class
End Namespace
