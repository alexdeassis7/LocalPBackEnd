using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public class SftpHelper
    {
        public static void UploadFile(string host, string username, string password, string workingDirectory, string fileToUpload, string filenameInServer, int port = 22)
        {
            var connectionInfo = new ConnectionInfo(
                                                    host,
                                                    port,
                                                    username,
                                                    new PasswordAuthenticationMethod(username, password));

            using (var client = new SftpClient(connectionInfo))
            {
                client.Connect();

                client.ChangeDirectory(workingDirectory);

                using (var fileStream = new FileStream(fileToUpload, FileMode.Open))
                {
                    client.BufferSize = 4 * 1024; // bypass Payload error large files
                    client.UploadFile(fileStream, filenameInServer);
                }

                client.Disconnect();
            }
        }

        public static string SearchAndDownloadFile(string host, string username, string password, string workingDirectory, string localDownloadPath, int port = 22)
        {
            var connectionInfo = new ConnectionInfo(
                                                    host,
                                                    port,
                                                    username,
                                                    new PasswordAuthenticationMethod(username, password));
            using (var client = new SftpClient(connectionInfo))
            {
                client.Connect();

                var files = client.ListDirectory(workingDirectory).OrderByDescending(x=>x.LastWriteTime).Take(10).ToList();

                var fileNamesExistInServer = files.Select(x => x.Name);

                var todayDownloadPath = Path.Combine(localDownloadPath, DateTime.Now.ToString("yyyyMMdd"));

                if (!Directory.Exists(todayDownloadPath)) Directory.CreateDirectory(todayDownloadPath);

                var fileNamesInLocalPath = Directory.GetFiles(todayDownloadPath).Select(x=>Path.GetFileName(x));

                var fileNamesToImport = fileNamesExistInServer.Except(fileNamesInLocalPath);

                var remoteFilename = string.Empty;

                foreach (var file in fileNamesToImport)
                {
                     remoteFilename = file;

                    using (Stream str = System.IO.File.OpenWrite(Path.Combine(todayDownloadPath, remoteFilename)))
                        client.DownloadFile(Path.Combine(workingDirectory, remoteFilename), str);

                    //client.DeleteFile("/" + workingDirectory + "/" + remoteFilename);

                    remoteFilename = Path.Combine(todayDownloadPath, remoteFilename);

                    break;
                }

                return remoteFilename;
            }
        }

    }
}
