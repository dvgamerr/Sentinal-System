Imports System.Security.Cryptography
Imports System.IO

Namespace Security
    Public Class SHA512

        Private Const KeyEncryption As String = "Mbos3S@E1e!M38vY"
        Private Const PassPhrase As String = "MBOS!8502#FAR33"
        Private Const SaltCryptography As String = "MBOSEngineSecurity2013"

        Public Shared Function Encrypt(ByVal plainText As String) As String
            Dim password As Rfc2898DeriveBytes = New Rfc2898DeriveBytes(PassPhrase, Text.Encoding.ASCII.GetBytes(SaltCryptography), 2)
            Dim symmetric As New RijndaelManaged()
            symmetric.Mode = CipherMode.CBC

            Dim textBytes As Byte() = Text.Encoding.UTF8.GetBytes(plainText)
            Dim mem As New MemoryStream()
            Dim crypt As New CryptoStream(mem, symmetric.CreateEncryptor(password.GetBytes(32), Text.Encoding.ASCII.GetBytes(KeyEncryption)), CryptoStreamMode.Write)
            crypt.Write(textBytes, 0, textBytes.Length)
            crypt.FlushFinalBlock()
            plainText = System.Convert.ToBase64String(mem.ToArray())
            mem.Close()
            crypt.Close()
            Return plainText
        End Function

        Public Shared Function Decrypt(ByVal cipherText As String) As String
            Dim password As Rfc2898DeriveBytes = New Rfc2898DeriveBytes(PassPhrase, Text.Encoding.ASCII.GetBytes(SaltCryptography), 2)
            Dim symmetric As New RijndaelManaged()
            symmetric.Mode = CipherMode.CBC

            Dim mem As New MemoryStream(System.Convert.FromBase64String(cipherText))
            Dim crypt As New CryptoStream(mem, symmetric.CreateDecryptor(password.GetBytes(32), Text.Encoding.ASCII.GetBytes(KeyEncryption)), CryptoStreamMode.Read)
            Dim textBytes As Byte() = New Byte(System.Convert.FromBase64String(cipherText).Length - 1) {}
            cipherText = Text.Encoding.UTF8.GetString(textBytes, 0, crypt.Read(textBytes, 0, textBytes.Length))
            mem.Close()
            crypt.Close()
            Return cipherText
        End Function
    End Class
End Namespace
