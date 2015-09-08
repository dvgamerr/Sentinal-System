using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Travox.Systems;
using Travox.Systems.DataCollection;
using Travox.Systems.FileManager;
using Travox.Systems.Security;

namespace Travox.Sentinel.Setting
{
    public class Configuration
    {
        public String CrawlerScript;
        public String CrawlerConfig;
        public UInt16 SentinelPort;
        public IPAddress NetworkIP;
        public IPAddress InternetIP;
        public NodeJSArgs NodeJS;

        String KeyEncrypt = "TRAVOX!#7c7SD2";
        String ConfigDirectory = "config.travox";
        public Configuration()
        {
            NodeJS = new NodeJSArgs();
            NodeJS.Database = new DBArgs();
            ConfigDirectory = Module.TravoxSentinel + ConfigDirectory;
            foreach (IPAddress address in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    NetworkIP = address;
                    break;
                }
            }

            if (File.Exists(ConfigDirectory)) this.Load(); else this.Default();

        }

        public void Save()
        {
            TypeSFO FileConfig = new TypeSFO();
            FileConfig.Param("SentinelPort", SentinelPort);

            FileConfig.Param("CrawlerScript", CrawlerScript);
            FileConfig.Param("CrawlerConfig", CrawlerConfig);

            FileConfig.Param("NodeJS.IPAddress", ECB.Encrypt(NodeJS.IPAddress, KeyEncrypt));
            FileConfig.Param("NodeJS.Port", ECB.Encrypt(NodeJS.Port, KeyEncrypt));
            FileConfig.Param("MySQL.Name", ECB.Encrypt(NodeJS.Database.Name, KeyEncrypt));
            FileConfig.Param("MySQL.ServerName", ECB.Encrypt(NodeJS.Database.ServerName, KeyEncrypt));
            FileConfig.Param("MySQL.Username", ECB.Encrypt(NodeJS.Database.Username, KeyEncrypt));
            FileConfig.Param("MySQL.Password", ECB.Encrypt(NodeJS.Database.Password, KeyEncrypt));
            FileConfig.SaveAs(ConfigDirectory);
        }

        private Boolean Load()
        {
            Boolean result = false;
            TypeSFO FileConfig = new TypeSFO();
            Byte[] bytes = Module.Read(ConfigDirectory);

            if (bytes.Length > 0)
            {
                result = true;
                FileConfig.Load(bytes);
                this.Mapping(FileConfig.ToArray);
            }


            return result;
        }

        public void Reset()
        {

        }

        public void Default()
        {
            SentinelPort = 8220;
            NodeJS.IPAddress = "127.0.0.1";
            NodeJS.Port = "8223";
            NodeJS.Database.ServerName = "db3.ns.co.th";
            NodeJS.Database.Name = "travox_system";
            NodeJS.Database.Username = "travoxmos";
            NodeJS.Database.Password = "systrav";
        }

        private void Mapping(TableEntrie[] Data)
        {
            foreach (TableEntrie item in Data)
            {
                switch (item.Key)
                {
                    case "SentinelPort": SentinelPort = UInt16.Parse(item.Value); break;
                    case "CrawlerScript": CrawlerScript = item.Value; break;
                    case "CrawlerConfig": CrawlerConfig = item.Value; break;
                    case "NodeJS.IPAddress": NodeJS.IPAddress = ECB.Decrypt(item.Value, KeyEncrypt); break;
                    case "NodeJS.Port": NodeJS.Port = ECB.Decrypt(item.Value, KeyEncrypt); break;
                    case "MySQL.Name": NodeJS.Database.Name = ECB.Decrypt(item.Value, KeyEncrypt); break;
                    case "MySQL.ServerName": NodeJS.Database.ServerName = ECB.Decrypt(item.Value, KeyEncrypt); break;
                    case "MySQL.Username": NodeJS.Database.Username = ECB.Decrypt(item.Value, KeyEncrypt); break;
                    case "MySQL.Password": NodeJS.Database.Password = ECB.Decrypt(item.Value, KeyEncrypt); break;
                }
            }
        }
    }

    public class NodeJSArgs
    {
        public String IPAddress;
        public String Port;
        public DBArgs Database;
    }

    public class DBArgs
    {
        public String ServerName;
        public String Username;
        public String Password;
        public String Name;
    }
}