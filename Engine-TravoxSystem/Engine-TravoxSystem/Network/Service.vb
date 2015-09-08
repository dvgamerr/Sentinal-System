Namespace Network
    Public Class Service

        Public Sub Service()

        End Sub

        Public Shared Sub Starting(serviceName As String)

        End Sub

        Public Shared Sub Stoping(serviceName As String)

        End Sub
        '    public static void StartWindowService(string serviceName) 
        '{ 
        'ProcessStartInfo processstartInfo = new ProcessStartInfo("sc.exe"); 
        'processstartInfo.Arguments = string.Format(CultureInfo.InvariantCulture, "start {0} ", serviceName); 
        'processstartInfo.RedirectStandardOutput = true; 
        'processstartInfo.UseShellExecute = false; 
        'using (System.Diagnostics.Process p = new System.Diagnostics.Process()) 
        '{ 
        'p.StartInfo = processstartInfo; 
        'if (p.Start()) 
        '{ 
        'p.WaitForExit(); 
        '} 
        '} 
        '} 
        'public static void StopWindowService(string serviceName) 
        '{ 
        'ProcessStartInfo processstartInfo = new ProcessStartInfo("sc.exe"); 
        'processstartInfo.Arguments = string.Format(CultureInfo.InvariantCulture, "stop {0} ", serviceName); 
        'processstartInfo.RedirectStandardOutput = true; 
        'processstartInfo.UseShellExecute = false; 
        'using (System.Diagnostics.Process p = new System.Diagnostics.Process()) 
        '{ 
        'p.StartInfo = processstartInfo; 
        'if (p.Start()) 
        '{ 
        'p.WaitForExit(); 
        '} 
        '} 
        '}
    End Class

End Namespace