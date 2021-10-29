using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Compression;
using SharedModelDTO.Models.Transaction;

namespace Tools.File.Zip
{
    public static class FileCompression
    {
        public static SharedModelDTO.File.FileModel ZipFileToBytes(SharedModelDTO.File.FileModel FileProcess)
        {
            try
            {
                //string fileName = type + "_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".txt";
                FileProcess.file_name = FileProcess.transaction_type + "_" + FileProcess.datetime_process + "." + FileProcess.file_extension;
                FileProcess.file_name_zip = "Zip_" + FileProcess.transaction_type + "_" + FileProcess.datetime_process + ".zip";

                using (var outStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                    {
                        var fileInArchive = archive.CreateEntry(FileProcess.file_name, CompressionLevel.Optimal);
                        using (var entryStream = fileInArchive.Open())
                        using (var fileToCompressStream = new MemoryStream(FileProcess.file_bytes))
                        {
                            fileToCompressStream.CopyTo(entryStream);
                        }
                    }
                    FileProcess.file_bytes_compressed = outStream.ToArray();
                }

                return FileProcess;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static SharedModelDTO.File.FileModel RetentionsToZIPFile(List<SharedModelDTO.File.FileModel> files, RetentionsByDateDownloadModel transactionsDownloadRetentions)
        {
            SharedModelDTO.File.FileModel FileDTO = new SharedModelDTO.File.FileModel();
            FileDTO.transaction_type = "RETENTION";
            FileDTO.datetime_process = DateTime.Now.ToString("yyyyMMddHHmmss");
            FileDTO.file_extension = "zip";
            FileDTO.file_name_zip = string.Format("Withholding_certificates_{0}_{1}_{2}_{3}.zip", transactionsDownloadRetentions.Merchant.Name, transactionsDownloadRetentions.date.ToString("yyyyMMdd"), transactionsDownloadRetentions.Organism.Name, transactionsDownloadRetentions.Certificates_Created);
            using (var outStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                {
                    foreach (var item in files)
                    {
                        item.file_name = item.transaction_type + "_" + item.file_name + "." + item.file_extension;
                        //System.IO.File.WriteAllBytes(string.Format(@"C:\Users\Erica-NT\Documents\LP\{0}", item.file_name), item.file_bytes);
                        var fileInArchive = archive.CreateEntry(item.file_name, CompressionLevel.Optimal);
                        using (var entryStream = fileInArchive.Open())
                        {
                            using (var fileToCompressStream = new MemoryStream(item.file_bytes))
                            {
                                fileToCompressStream.CopyTo(entryStream);
                            }
                        }
                    }
                }
                //FileDTO.Base64Data = Convert.ToBase64String(outStream.ToArray());
                FileDTO.file_bytes_compressed = outStream.ToArray();
            }
            // System.IO.File.WriteAllBytes(string.Format(@"C:\Users\Erica-NT\Documents\LP\{0}", FileDTO.file_name_zip), FileDTO.file_bytes_compressed);
            return FileDTO;
        }

        public static SharedModelDTO.File.FileModel FilesToZIPFile(List<SharedModelDTO.File.FileModel> files, string transaction_type)
        {
            SharedModelDTO.File.FileModel FileDTO = new SharedModelDTO.File.FileModel();
            FileDTO.transaction_type = transaction_type;
            FileDTO.datetime_process = DateTime.Now.ToString("yyyyMMddHHmmss");
            FileDTO.file_extension = "zip";
            FileDTO.file_name_zip =string.Format("{0}_{1}.zip", FileDTO.transaction_type, FileDTO.datetime_process);
            using (var outStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                {
                    foreach (var item in files)
                    {                        
                        item.file_name = item.transaction_type + "_" + item.file_name + "." + item.file_extension;
                        //System.IO.File.WriteAllBytes(string.Format(@"C:\Users\Erica-NT\Documents\LP\{0}", item.file_name), item.file_bytes);
                        var fileInArchive = archive.CreateEntry(item.file_name, CompressionLevel.Optimal);
                        using (var entryStream = fileInArchive.Open())
                        {
                            using (var fileToCompressStream = new MemoryStream(item.file_bytes))
                            {
                                fileToCompressStream.CopyTo(entryStream);
                            }
                        }
                    }
                }
                //FileDTO.Base64Data = Convert.ToBase64String(outStream.ToArray());
                FileDTO.file_bytes_compressed = outStream.ToArray();
            }
           // System.IO.File.WriteAllBytes(string.Format(@"C:\Users\Erica-NT\Documents\LP\{0}", FileDTO.file_name_zip), FileDTO.file_bytes_compressed);
            return FileDTO;
        }
    }
}
