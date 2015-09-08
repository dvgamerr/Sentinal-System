using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travox.Sentinel.Engine
{
    public class CrawlerService
    {
        Process node;
        ProcessStartInfo NodeInfo;

        public CrawlerService()
        {
            NodeInfo = new ProcessStartInfo("node");
            NodeInfo.CreateNoWindow = true;
            NodeInfo.RedirectStandardError = true;
            NodeInfo.RedirectStandardOutput = true;
            NodeInfo.UseShellExecute = false;
        }

        public void Start(String ScriptPath, DataReceivedEventHandler outoutData, DataReceivedEventHandler errorData)
        {
            if (!App.WebCrawlerConnected && !String.IsNullOrWhiteSpace(ScriptPath))
            {
                NodeInfo.Arguments = Path.GetFileName(ScriptPath);
                NodeInfo.WorkingDirectory = Path.GetDirectoryName(ScriptPath);

                node = Process.Start(NodeInfo);
                node.BeginOutputReadLine();
                node.BeginErrorReadLine();

                node.OutputDataReceived += outoutData;
                node.ErrorDataReceived += errorData;
            }
        }

        public void Stop()
        {
            foreach (Process list in Process.GetProcessesByName("node"))
            {
                if (String.IsNullOrEmpty(list.MainWindowTitle)) list.Kill();
                node = null;
                App.WebCrawlerConnected = false;
            }
        }

    }
}
