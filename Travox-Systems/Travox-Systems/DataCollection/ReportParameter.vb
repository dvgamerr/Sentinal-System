Imports System.Collections.Generic
Namespace DataCollection
    Public Class ReportParameter
        Public Name As String
        Public Value As String
        Public SubReport As String
        Public Sub New(ByVal param_name As String, ByVal value_param As String)
            Name = param_name
            Value = value_param
        End Sub
        Public Sub New(ByVal param_name As String, ByVal value_param As String, ByVal subreport_name As String)
            Name = param_name
            Value = value_param
            SubReport = subreport_name
        End Sub
    End Class
End Namespace