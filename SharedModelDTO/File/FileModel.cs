using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModelDTO.File
{
    public class FileModel
    {
        private bool hasFiles = true;
        public bool HasFile { get { return this.hasFiles; } set { this.hasFiles = value; } }
        public byte[] file_bytes { get; set; }
        public byte[] file_bytes_compressed { get; set; }
        public string transaction_type { get; set; }
        public string datetime_process { get; set; }
        public string file_name { get; set; }
        public string file_name_zip { get; set; }
        private string extension = "txt";
        public string file_extension { get { return extension; } set { this.extension = value; } }
    }
}
