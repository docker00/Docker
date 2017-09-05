using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient.Framework.NetCore10;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Host.Extensions
{
    public class FileReader
    {
        public string FilePath { get; set; }

        public FileReader(string filePath)
        {
            FilePath = filePath;
        }

        public async Task<List<string>> LogView()
        {
            /*var result = new List<string>();
            const Int32 BufferSize = 128;
            var filestream = new FileStream(this.FilePath,
                                          FileMode.Open,
                                          FileAccess.Read,
                                          FileShare.ReadWrite);
            using (var streamReader = new StreamReader(filestream, System.Text.Encoding.UTF8, true, BufferSize))
            {
                String line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (result.Count >= 20)
                    {
                        return result;
                    }
                    result.Add(await streamReader.ReadLineAsync());
                }
            }
            result.Reverse();
            return result;*/

            var result = new List<string>();

            var filestream = new FileStream(this.FilePath,
                                          FileMode.Open,
                                          FileAccess.Read,
                                          FileShare.ReadWrite);
            using (var reader = new StreamReader(filestream))
            {
                //To read the last 8024 bytes
                int sizeFile = 8024;
                if (reader.BaseStream.Length > sizeFile)
                {
                    reader.BaseStream.Seek(-sizeFile, SeekOrigin.End);
                }
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    result.Add(line.Replace("[Information]", "<span style=\"color:blue\">[Information]</span>")
                                   .Replace("[Debug]", "<span style=\"color:green\">[Debug]</span>")
                                   .Replace("[Error]", "<span style=\"color:red\">[Error]</span>"));
                }
            }
            result.Reverse();
            return result;
        }

    }
}
