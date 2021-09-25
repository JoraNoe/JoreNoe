using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JoreNoe.JoreFile
{
    public class JoreFile
    {
        /// <summary>
        /// 解压功能(解压压缩文件到指定目录)
        /// </summary>
        /// <param name="ZipStream">待解压的文件</param>
        /// <param name="zipedFolder">指定解压目标目录</param>
        /// <param name="password">密码</param>
        /// <returns>解压结果</returns>
        public static bool UnZip(string ZipPath, string zipedFolder, string password)
        {
            bool result = true;
            FileStream fs = null;
            ZipInputStream zipStream = null;
            ZipEntry ent = null;
            string fileName;

            if (!Directory.Exists(zipedFolder))
                Directory.CreateDirectory(zipedFolder);

            try
            {
                zipStream = new ZipInputStream(File.OpenRead(ZipPath));
                if (!string.IsNullOrEmpty(password)) zipStream.Password = password;
                while ((ent = zipStream.GetNextEntry()) != null)
                {
                    if (!string.IsNullOrEmpty(ent.Name))
                    {

                        string EncodingName = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(ent.Name));



                        fileName = Encoding.Unicode.GetString(Encoding.Unicode.GetBytes(Path.Combine(zipedFolder, EncodingName)));

                        fileName = fileName.Replace('/', '\\');//change by Mr.HopeGi

                        int index = ent.Name.LastIndexOf('/');
                        if (index != -1 || fileName.EndsWith("\\"))
                        {
                            string tmpDir = (index != -1 ? fileName.Substring(0, fileName.LastIndexOf('\\')) : fileName) + "\\";
                            if (!Directory.Exists(tmpDir))
                            {
                                Directory.CreateDirectory(tmpDir);
                            }
                            if (tmpDir == fileName)
                            {
                                continue;
                            }
                        }

                        fs = File.Create(fileName);
                        int size = 2048;
                        byte[] data = new byte[size];
                        while (true)
                        {
                            size = zipStream.Read(data, 0, data.Length);
                            if (size > 0)
                                fs.Write(data, 0, data.Length);
                            else
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                //直接抛出异常 
                throw new Exception(e.Message);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
                if (zipStream != null)
                {
                    zipStream.Close();
                    zipStream.Dispose();
                }
                if (ent != null)
                {
                    ent = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
            return result;
        }

        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="CompressFolderPath">要压缩的文件夹路径</param>
        /// <param name="CompressPath">压缩输出路径</param>
        /// <param name="CompressName"></param>
        /// <param name="CompressCategory"></param>
        /// <returns></returns>
        public static bool Compress(string CompressFolderPath, string CompressPath, string CompressName, string CompressCategory = "zip")
        {
            //创建压缩包
            FileStream FileWrite = new FileStream(Path.Combine(CompressPath, @$"\{CompressName}.{CompressCategory}"), FileMode.OpenOrCreate, FileAccess.ReadWrite);
            FileWrite.Close();

            var Files = new DirectoryInfo(CompressFolderPath);
            List<string> FilesPath = new List<string>();
            foreach (FileInfo FileItem in Files.GetFiles())//遍历文件夹下所有文件
            {
                FilesPath.Add(FileItem.FullName);
            }

            CreateZipFile(FilesPath.ToArray(), Path.Combine(CompressPath, @$"\{CompressName}.{CompressCategory}"));
            return true;
        }

        /// <summary>
        /// 创建压缩包
        /// </summary>
        /// <param name="Files"></param>
        /// <param name="zipFilePath"></param>
        public static void CreateZipFile(string[] Files, string zipFilePath)
        {
            try
            {
                string[] filenames = Files;//Directory.GetFiles(filesPath);
                using (ZipOutputStream s = new ZipOutputStream(File.Create(zipFilePath)))
                {

                    s.SetLevel(9); // 压缩级别 0-9
                                   //s.Password = "123"; //Zip压缩文件密码
                    byte[] buffer = new byte[4096]; //缓冲区大小
                    foreach (string file in filenames)
                    {
                        ZipEntry entry = new ZipEntry(Path.GetFileName(file));
                        entry.DateTime = DateTime.Now;
                        s.PutNextEntry(entry);
                        using (FileStream fs = File.OpenRead(file))
                        {
                            int sourceBytes;
                            do
                            {
                                sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                s.Write(buffer, 0, sourceBytes);
                            } while (sourceBytes > 0);
                        }
                    }
                    s.Finish();
                    s.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
