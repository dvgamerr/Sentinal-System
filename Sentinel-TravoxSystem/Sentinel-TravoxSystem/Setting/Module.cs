using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travox.Systems;

public class Module
{

    public static String TravoxSentinel = Path.GetTempPath() + @"..\Travox_Sentinel\";
    public static String TravoxTemp = Path.GetTempPath() + @"Travox\";
    public static String Config = "tx.configure";
    public static String BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

    public static void WriteException(Exception e, String folder = "")
    {
        StringBuilder excep = new StringBuilder();
        String FolderException = @"Exception\";

        if (!MBOS.Null(folder)) FolderException += folder;
        excep.AppendLine(e.Message.ToString());
        if (e.InnerException != null) excep.AppendLine("------------------------------------------------").AppendLine(e.InnerException.Message);
        excep.AppendLine("------------------------------------------------");
        excep.AppendLine(e.StackTrace.ToString());

        if (!Directory.Exists(TravoxSentinel + FolderException)) Directory.CreateDirectory(TravoxSentinel + FolderException);
        Module.Write(String.Format(@"{1}\Excep{0}.txt", MBOS.Timestamp().ToString(), TravoxSentinel + FolderException), excep.ToString());
        e = null;
        excep = null;
        FolderException = null;
    }

    public static String ReadText(String filename)
    {
        if (File.Exists(filename))
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                filename = "";
                Byte[] Buffer = new Byte[fs.Length];
                Int32 BytesTransferred = (Int32)fs.Length;
                Int32 BytesIndex = 0;
                while (BytesTransferred > 0)
                {
                    Int32 n = fs.Read(Buffer, BytesIndex, BytesTransferred);
                    filename += Encoding.UTF8.GetString(Buffer, BytesIndex, BytesTransferred);

                    if (n == 0) break;
                    BytesIndex += n;
                    BytesTransferred -= n;
                }
                BytesTransferred = Buffer.Length;
            }
        }
        else
        {
            filename = "";
        }
        return filename;
    }
    public static Byte[] Read(String filename)
    {
        Byte[] Buffer = new Byte[0];
        if (File.Exists(filename))
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                Buffer = new Byte[fs.Length];
                Int32 BytesTransferred = (Int32)fs.Length;
                Int32 BytesIndex = 0;
                while (BytesTransferred > 0)
                {
                    Int32 n = fs.Read(Buffer, BytesIndex, BytesTransferred);
                    if (n == 0) break;
                    BytesIndex += n;
                    BytesTransferred -= n;
                }
                BytesTransferred = Buffer.Length;
            }
        }
        return Buffer;
    }
    public static Boolean Write(String filename, String Text)
    {
        Boolean success = false;
        if (!Directory.Exists(Path.GetDirectoryName(filename))) Directory.CreateDirectory(Path.GetDirectoryName(filename));
        if (File.Exists(filename))
        {
            File.Delete(filename);
            using (FileStream fs = new FileStream(filename, FileMode.CreateNew, FileAccess.Write))
            {
                if (fs.CanWrite)
                {
                    fs.Write(Encoding.UTF8.GetBytes(Text), 0, Encoding.UTF8.GetByteCount(Text));
                    success = true;
                }
            }
        }
        return success;
    }
}