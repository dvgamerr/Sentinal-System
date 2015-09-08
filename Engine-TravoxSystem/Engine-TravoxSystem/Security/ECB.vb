Imports System.Configuration
Imports System.Security.Cryptography
Imports System.Text

Namespace Security
    Public Class ECB
        Public Shared Function Encrypt(plainString As String, keyString As String) As String
            Dim tdes As New TripleDESCryptoServiceProvider()
            Dim hashmd5 As New MD5CryptoServiceProvider()
            Dim toEncryptArray As [Byte]() = UTF8Encoding.UTF8.GetBytes(plainString)
            Dim keyArray As [Byte]() = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(keyString))
            tdes.Key = keyArray
            tdes.Mode = CipherMode.ECB
            tdes.Padding = PaddingMode.PKCS7

            Dim cTransform As ICryptoTransform = tdes.CreateEncryptor()
            Dim resultArray As [Byte]() = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length)

            tdes.Clear()
            hashmd5.Clear()
            Return Convert.ToBase64String(resultArray, 0, resultArray.Length)
        End Function

        Public Shared Function Decrypt(cipherString As String, keyString As String) As String
            Dim tdes As New TripleDESCryptoServiceProvider()
            Dim hashmd5 As New MD5CryptoServiceProvider()
            Dim toEncryptArray As [Byte]() = Convert.FromBase64String(cipherString)
            Dim keyArray As [Byte]() = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(keyString))
            tdes.Key = keyArray
            tdes.Mode = CipherMode.ECB
            tdes.Padding = PaddingMode.PKCS7

            Dim cTransform As ICryptoTransform = tdes.CreateDecryptor()
            Dim resultArray As [Byte]() = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length)

            tdes.Clear()
            hashmd5.Clear()
            Return UTF8Encoding.UTF8.GetString(resultArray)
        End Function
    End Class
End Namespace