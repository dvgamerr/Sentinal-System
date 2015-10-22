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
        public UInt16 SentinelPort;
        public IPAddress NetworkIP;
        public IPAddress InternetIP;
        public NodeJSArgs API;
        public DBArgs MSSQL;

        String KeyEncrypt = "TRAVOX!#7c7SD2";
        public Configuration()
        {
            API = new NodeJSArgs();
            MSSQL= new DBArgs();
            foreach (IPAddress address in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    NetworkIP = address;
                    break;
                }
            }

            if (File.Exists(Module.TravoxSentinel + Module.File_Config)) this.Load(); else this.Default();

        }

        public void Save()
        {
            TypeSFO FileConfig = new TypeSFO();
            FileConfig.Param("SentinelPort", SentinelPort);

            FileConfig.Param("NodeAPI.IPAddress", ECB.Encrypt(API.IPAddress, KeyEncrypt));
            FileConfig.Param("NodeAPI.Port", ECB.Encrypt(API.Port, KeyEncrypt));
            FileConfig.Param("MSSQL.Name", ECB.Encrypt(MSSQL.Name, KeyEncrypt));
            FileConfig.Param("MSSQL.ServerName", ECB.Encrypt(MSSQL.ServerName, KeyEncrypt));
            FileConfig.Param("MSSQL.Username", ECB.Encrypt(MSSQL.Username, KeyEncrypt));
            FileConfig.Param("MSSQL.Password", ECB.Encrypt(MSSQL.Password, KeyEncrypt));
            FileConfig.SaveAs(Module.TravoxSentinel + Module.File_Config);
        }

        private Boolean Load()
        {
            Boolean result = false;
            TypeSFO FileConfig = new TypeSFO();
            Byte[] bytes = Module.Read(Module.TravoxSentinel + Module.File_Config);

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
            API.IPAddress = "127.0.0.1";
            API.Port = "8223";
            MSSQL.ServerName = "db3.ns.co.th";
            MSSQL.Name = "travox_system";
            MSSQL.Username = "travoxmos";
            MSSQL.Password = "systrav";
        }

        private void Mapping(TableEntrie[] Data)
        {
            foreach (TableEntrie item in Data)
            {
                switch (item.Key)
                {
                    case "SentinelPort": SentinelPort = UInt16.Parse(item.Value); break;
                    case "NodeAPI.IPAddress": API.IPAddress = ECB.Decrypt(item.Value, KeyEncrypt); break;
                    case "NodeAPI.Port": API.Port = ECB.Decrypt(item.Value, KeyEncrypt); break;
                    case "MSSQL.Name": MSSQL.Name = ECB.Decrypt(item.Value, KeyEncrypt); break;
                    case "MSSQL.ServerName": MSSQL.ServerName = ECB.Decrypt(item.Value, KeyEncrypt); break;
                    case "MSSQL.Username": MSSQL.Username = ECB.Decrypt(item.Value, KeyEncrypt); break;
                    case "MSSQL.Password": MSSQL.Password = ECB.Decrypt(item.Value, KeyEncrypt); break;
                }
            }
        }
    }

    public class NodeJSArgs
    {
        public String IPAddress;
        public String Port;
    }

    public class DBArgs
    {
        public String ServerName;
        public String Username;
        public String Password;
        public String Name;
    }
}