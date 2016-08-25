﻿Imports System.Collections.Specialized
Imports System.Text
Imports System.Text.RegularExpressions

Namespace DataCollection
    Public Class RequestBuilder
        Const UserAgent As [String] = "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/44.0.2403.125 Safari/537.36"

        Private ContentLength As Int32 = 0
        Private ParamVariable As NameValueCollection
        Private GetVariable As NameValueCollection
        Private PostVariable As NameValueCollection
        Public Cookie As CookieBuilder

        Public By As Method = Method.[GET]
        Public Referer As [String]
        ' http:'forum.tirkx.com/main/index.php
        Public Origin As [String]
        ' http:'forum.tirkx.com
        Public Connection As [String] = "keep-alive"
        Public CacheControl As [String] = "max-age=0"
        Public ContentType As [String]
        Public Accept As [String]

        Public CurrentURL As Uri
        Public Enum Method
            [POST]
            [GET]
        End Enum

        Public Sub New(URL As String, Optional HTTPS As Boolean = False)
            Cookie = New CookieBuilder()
            ParamVariable = New NameValueCollection()
            GetVariable = New NameValueCollection()
            PostVariable = New NameValueCollection()
            If Not Regex.Match(URL, "(http|https):'").Success Then URL = IIf(HTTPS, "https:\\", "http:\\") & URL
            CurrentURL = New Uri(URL)
        End Sub

        Public Sub [GET](name As String, value As String)
            GetVariable.Add(name, value)
        End Sub

        Public Sub AddBody(name As String, value As String)
            PostVariable.Add(name, value)
        End Sub

        Public Sub AddHeader(name As String, value As String)
            ParamVariable.Add(name, value)
        End Sub

        Public Sub Clear()
            PostVariable.Clear()
            GetVariable.Clear()
        End Sub

        Public Sub SetCookie(name As String, value As String)
            If String.Join("=", Cookie.AllKeys).Contains(name) Then Cookie(name) = value Else Cookie.Add(name, value)
        End Sub

        Public Overrides Function ToString() As String
            Dim _post As [String] = POSTString()
            ContentLength = _post.Length
            Dim _sb As New StringBuilder()
            _sb.AppendLine(By.ToString() + " " + CurrentURL.PathAndQuery + " HTTP/1.1")
            _sb.AppendLine("Host: " & CurrentURL.Host & IIf(CurrentURL.Port = 80, "", ":" & CurrentURL.Port))
            _sb.AppendLine("Cache-Control: " + CacheControl)

            If Not [String].IsNullOrEmpty(ContentType) Then
                _sb.AppendLine("Content-Type: " + ContentType)
            ElseIf PostVariable.Count > 0 Then
                _sb.AppendLine("Content-Type: application/x-www-form-urlencoded; charset=UTF-8")
                _sb.AppendLine("X-Requested-With: XMLHttpRequest")
            End If
            If Not [String].IsNullOrEmpty(Accept) Then
                _sb.AppendLine("Accept: " + Accept)
            ElseIf PostVariable.Count > 0 Then
                _sb.AppendLine("Accept: application/json, text/javascript, */*; q=0.01")
                _sb.AppendLine("Accept-Language: th-TH,th;q=0.8,en-US;q=0.6,en;q=0.4,ja;q=0.2")
            Else
                _sb.AppendLine("Accept: text/html")
            End If

            For Each key As String In ParamVariable.AllKeys
                _sb.AppendFormat("{0}: {1}", key, PostVariable(key))
            Next

            If Not [String].IsNullOrEmpty(UserAgent) Then _sb.AppendLine("User-Agent: " + UserAgent)
            If Not [String].IsNullOrEmpty(Referer) Then _sb.AppendLine("Referer: " + Referer)
            If Not [String].IsNullOrEmpty(Origin) Then _sb.AppendLine("Origin: " + Origin)
            If ContentLength > 0 Then _sb.AppendLine("Content-Length: " + ContentLength.ToString())
            If Cookie.Count > 0 Then _sb.AppendLine("Cookie: " + CookieString())



            _sb.AppendLine("Connection: " + Connection)
            _sb.AppendLine("")
            If PostVariable.Count > 0 Then
                _sb.Append(_post)
                _sb.AppendLine("")
            End If
            Return _sb.ToString()
        End Function

        Private Function POSTString() As [String]
            Dim _s As [String] = ""
            For Each key As String In PostVariable.AllKeys
                _s += key & "=" & Uri.EscapeUriString(PostVariable(key)) & "&"
            Next
            If _s.Length > 0 Then _s = _s.Substring(0, _s.Length - 1)

            Return _s
        End Function

        Private Function CookieString() As [String]
            Dim _s As [String] = ""
            For Each key As String In Cookie.AllKeys
                _s += key & "=" & Cookie(key) & "; "
            Next
            If _s.Length > 0 Then _s = _s.Substring(0, _s.Length - 2)
            Return _s
        End Function

    End Class
End Namespace
