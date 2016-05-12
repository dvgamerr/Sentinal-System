Imports System.Collections.Specialized
Imports Travox.Systems

Namespace UI
    Public Class Page
        Inherits Global.System.Web.UI.Page

        Private Const SessionTimeExpire As Integer = 3600 ' Time Second 
        Private RequestValues As New NameValueCollection()

        Public Property _REQUEST() As NameValueCollection
            Get
                Return RequestValues
            End Get
            Set(ByVal value As NameValueCollection)
                RequestValues = value
            End Set
        End Property

        Protected Overrides Sub InitializeCulture()
            If (MBOS.Debug) Then Response.AddHeader("Access-Control-Allow-Origin", "*")
            Me.UICulture = "en-US"
            Me.Culture = "en-US"
        End Sub

        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Try
                For Each key As String In Request.Form.Keys
                    RequestValues.Add(key, Request.Form(key))
                Next
                For Each key As String In Request.QueryString.Keys
                    RequestValues.Add(key, Request.QueryString(key))
                Next
            Catch
                Throw New Exception("'Form' and 'QueryString' is key Contains")
            End Try
        End Sub

        Private Sub Page_Error(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Error
            Response.Write(JSON.Serialize(Of CallbackException)(New CallbackException(Server.GetLastError)))
            Response.End()
        End Sub

    End Class
End Namespace