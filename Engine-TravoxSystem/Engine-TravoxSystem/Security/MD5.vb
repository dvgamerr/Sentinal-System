Imports System.Security.Cryptography
Imports System.IO

Namespace Security
    Public Class MD5

        Public Shared Function Encrypt(ByVal plainText As String) As String
            Return BitConverter.ToString(MD5CryptoServiceProvider.Create().ComputeHash(Text.Encoding.UTF8.GetBytes(plainText))).Replace("-", "").ToLower()
        End Function

        Public Shared Function Compare(ByVal plainText As String, ByVal cipherText As String) As Boolean
            Dim checkSum As Boolean = False
            If (MD5.Encrypt(plainText) = cipherText.Trim()) Then checkSum = True
            Return checkSum
        End Function
    End Class

End Namespace
