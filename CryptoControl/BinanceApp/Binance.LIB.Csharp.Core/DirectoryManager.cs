using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binance.LIB.Csharp.Core
{
    public static class DirectoryManager
    {
        public static object FileImage { get; private set; }

        public static List<String> ListDirectories(String path)
        {
            List<String> directories = new List<String>();
            var dirs = Directory.GetDirectories(path).ToList<String>();
            foreach (String s in dirs)
                directories.Add(s.Split('\\').Last());
            return directories;
        }
        public static String ReadFile(String path)
        {
            if (File.Exists(path))
                return File.ReadAllText(path);
            else return "";
        }


        public static void ExcuteBtachFile(String file)
        {
            System.Diagnostics.Process.Start(file);
        }
        public static void ExecuteBatchCommand(string file)
        {
            string command = string.Format(file);
            ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            procStartInfo.WorkingDirectory = "C:\\GpSeismicDatabases\\";
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = false;
            // Now we create a process, assign its ProcessStartInfo and start it
            Process proc = new Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            proc.WaitForExit();
        }

        public static void WriteFile(String vars, String path)
        {
            if (File.Exists(path))
                DeleteFile(path);
            File.WriteAllText(path, vars);
        }

        public static void AppendToFile(String vars, String path)
        {
            if (!File.Exists(path))
                File.WriteAllText(path, vars);
            else
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(vars);
                }
            }
        }

        public static List<String> ListFiles(String path)
        {
            List<String> files = new List<String>();
            var fs = Directory.GetFiles(path).ToList<String>();
            foreach (String f in fs)
                files.Add(f.Split('\\').Last());
            return files;
        }

        public static List<String> ListFilesRecursive(String path, List<String> files)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);


            foreach (FileInfo file in dirInfo.GetFiles())
                files.Add(file.FullName);

            foreach (DirectoryInfo subDir in dirInfo.GetDirectories())
                ListFilesRecursive(path + @"\" + subDir.Name, files);


            return files;
        }
   
        public static void CreateDirectory(String path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
        public static void CopyFile(String path1, String path2)
        {

            File.Copy(path1, path2);
        }
        public static void DeleteFile(String path)
        {

            File.Delete(path);
        }

        public static void SaveFileFromStream(String fileFullPath, byte[] buffer)
        {
            Stream stream = new MemoryStream(buffer);
            FileStream fileStream = File.Create(fileFullPath, (int)stream.Length);
            // Initialize the bytes array with the stream length and then fill it with data
            byte[] bytesInStream = new byte[stream.Length];
            stream.Read(bytesInStream, 0, bytesInStream.Length);
            // Use write method to write to the file specified above
            fileStream.Write(bytesInStream, 0, bytesInStream.Length);
            fileStream.Close();
            stream.Flush();
        }
    }
   
}
