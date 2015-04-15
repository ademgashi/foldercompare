using System;
using System.IO;
using System.Security.Cryptography;

namespace FolderCompare.Helpers
{
    public class FileSystemItem
    {
        public FileSystemItem(FileInfo file)
        {
            Name = file.Name;
            FullName = file.FullName;
            Size = file.Length;
            CreationTime = file.CreationTime;
            LastAccessTime = file.LastAccessTime;
            LastWriteTime = file.LastWriteTime;
            IsFolder = false;

            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(file.FullName))
                {
                    Md5Hash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty);
                    
                }
            }
            
        }


        public FileSystemItem(DirectoryInfo folder)
        {
            Name = folder.Name;
            FullName = folder.FullName;
            Size = null;
            CreationTime = folder.CreationTime;
            LastAccessTime = folder.LastAccessTime;
            LastWriteTime = folder.LastWriteTime;
            IsFolder = true;
        }

        public string Name { get; set; }
        public string FullName { get; set; }
        public long? Size { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastAccessTime { get; set; }
        public DateTime LastWriteTime { get; set; }

        public string Md5Hash { get; set; }
        public bool IsFolder { get; set; }

        
    }
}