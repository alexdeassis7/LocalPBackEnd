using FluentFTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public static class FtpHelper
    {
        public static void UploadFile(string host, string username, string password, string workingDirectory, string fileToUpload, string filenameInServer)
        {
            FtpClient client = new FtpClient(host);

            client.Credentials = new NetworkCredential(username, password);

            client.Connect();

            if (!client.DirectoryExists(workingDirectory))
            {
                client.CreateDirectory(workingDirectory);
            }

            if (client.DirectoryExists(workingDirectory))
            {
                client.UploadFile(fileToUpload, workingDirectory + '/' + filenameInServer);
            }

            client.Disconnect();
        }
    }
}
