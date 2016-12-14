using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace MySoft.Tools.EntityDesign
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            bool slient = false;

            //args = new string[] { @"E:\QingdaoProject\Shumi.Qingdao.Design" };
            if (args != null && args.Length > 0)
            {
                string designRootPath = args[0];
                string configFile = Path.Combine(designRootPath, "EntityDesignConfig.xml");

                try
                {
                    XmlTextReader reader = new XmlTextReader(configFile);
                    XmlSerializer serializer = new XmlSerializer(typeof(EntityDesignConfiguration));
                    EntityDesignConfiguration config = (EntityDesignConfiguration)serializer.Deserialize(reader);

                    if (config != null)
                    {
                        Assembly ass = Assembly.LoadFrom(designRootPath + "\\bin\\" + config.CompileMode + "\\" + config.InputDllName);

                        System.Text.Encoding encoding = System.Text.Encoding.Default;
                        if (!string.IsNullOrEmpty(config.OutputCodeFileEncoding))
                        {
                            encoding = System.Text.Encoding.GetEncoding(config.OutputCodeFileEncoding);
                        }

                        if (!string.IsNullOrEmpty(config.EntityCodePath))
                        {
                            AdvOptForm adv = new AdvOptForm();

                            bool isDirectory;
                            string entityCodePath = config.EntityCodePath;
                            string filePath = ParseRelativePath(designRootPath, entityCodePath, out isDirectory);
                            string fileBody = new CodeGenHelper(config.OutputNamespace, adv).GenEntitiesEx(ass, config.OutputLanguage.ToLower() == "c#" ? 0 : 1);

                            if (Path.GetFileName(filePath) != string.Empty)
                            {
                                WriteFile(filePath, fileBody, encoding);
                            }
                            else
                            {
                                DirectoryInfo info = new DirectoryInfo(filePath);
                                if (!info.Exists) info.Create(); //不存在，则创建目录

                                FileAttributes attribute = info.Attributes;
                                if ((attribute & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                                {
                                    info.Attributes = attribute ^ FileAttributes.ReadOnly;
                                }
                                Type[] types = ass.GetTypes();
                                if (types != null)
                                {
                                    List<Type> typelist = new List<Type>(types);
                                    typelist.RemoveAll(type =>
                                    {
                                        return !type.IsInterface;
                                    });

                                    string[] files = fileBody.Split(new string[] { "namespace", "Namespace" }, StringSplitOptions.RemoveEmptyEntries);
                                    IList<string> filelist = new List<string>(files);
                                    filelist.RemoveAt(0);

                                    Dictionary<string, string> dictTypeFiles = new Dictionary<string, string>();
                                    for (int index = 0; index < typelist.Count; index++)
                                    {
                                        if (config.OutputLanguage.ToLower() == "c#")
                                            dictTypeFiles[typelist[index].Name] = "namespace" + filelist[index];
                                        else
                                            dictTypeFiles[typelist[index].Name] = "Namespace" + filelist[index];
                                    }

                                    if (dictTypeFiles.Count > 0)
                                    {
                                        foreach (string txtKey in dictTypeFiles.Keys)
                                        {
                                            string path = string.Format("{0}\\{1}.cs", filePath.TrimEnd('\\'), txtKey);
                                            WriteFile(path, dictTypeFiles[txtKey], encoding);
                                        }
                                    }
                                }
                            }

                        }
                    }

                    slient = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }

            if (!slient)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new EntityDesign());
            }
        }

        /// <summary>
        /// 写入文件信息
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileBody"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        private static void WriteFile(string filePath, string fileBody, Encoding encoding)
        {
            FileInfo file = new FileInfo(filePath);
            if (!file.Exists)
            {
                FileStream fs = file.Create();
                file.Attributes = FileAttributes.Normal;
                fs.Close();
            }

            File.WriteAllText(file.FullName, fileBody, encoding);
        }

        /// <summary>
        /// Parses the relative path to absolute path.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        /// <param name="relativePath">The relative path.</param>
        /// <returns></returns>
        public static string ParseRelativePath(string basePath, string relativePath, out bool isDirectory)
        {
            isDirectory = false;
            if (relativePath.StartsWith("\\") || relativePath.StartsWith(".\\") || relativePath.Contains(":"))
            {
                if (relativePath.EndsWith("\\"))
                {
                    isDirectory = true;
                }
                return System.IO.Path.GetFullPath(relativePath);
            }

            basePath = basePath.Trim().Replace("/", "\\");
            relativePath = relativePath.Trim().Replace("/", "\\");

            string[] splittedBasePath = basePath.Split('\\');
            string[] splittedRelativePath = relativePath.Split('\\');

            StringBuilder sb = new StringBuilder();
            int parentTokenCount = 0;
            for (int i = 0; i < splittedRelativePath.Length; i++)
            {
                if (splittedRelativePath[i] == "..")
                {
                    parentTokenCount++;
                }
                else
                {
                    break;
                }
            }

            for (int i = 0; i < splittedBasePath.Length - parentTokenCount; i++)
            {
                if (!string.IsNullOrEmpty(splittedBasePath[i]))
                {
                    sb.Append(splittedBasePath[i]);
                    sb.Append("\\");
                }
            }

            for (int i = parentTokenCount; i < splittedRelativePath.Length; i++)
            {
                if (!string.IsNullOrEmpty(splittedRelativePath[i]))
                {
                    sb.Append(splittedRelativePath[i]);
                    sb.Append("\\");
                }
            }

            if (splittedRelativePath[splittedRelativePath.Length - 1].Trim() == string.Empty)
            {
                isDirectory = true;
                return sb.ToString();
            }
            return sb.ToString().TrimEnd('\\');
        }
    }
}