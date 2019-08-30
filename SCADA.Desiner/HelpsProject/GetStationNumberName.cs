using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO;

namespace SCADA.Desiner.HelpsProject
{
    class GetStationNumberName
    {
        /// <summary>
        /// возвращаем коллекцию вида название станции -- номер станции
        /// </summary>
        /// <returns></returns>
        public static List<Station> GetCollectionStation()
        {
            int buffer = 0;
            List<Station> answer = new List<Station>();
            try
            {
            if (ConfigurationManager.AppSettings["folder_neman"] != null && !string.IsNullOrEmpty(ConfigurationManager.AppSettings["folder_neman"]))
                {
                    DirectoryInfo info = new DirectoryInfo(ConfigurationManager.AppSettings["folder_neman"]);
                    if (info.Exists)
                    {
                        foreach (DirectoryInfo dic in info.GetDirectories())
                        {
                            foreach (var file in dic.GetFiles(string.Format("TI{0}.ASM", dic.Name), SearchOption.TopDirectoryOnly))
                            {
                                foreach (string str in File.ReadAllLines(file.FullName, Encoding.GetEncoding(866)))
                                {
                                    string[] massiv = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    if (massiv.Length > 1 && massiv[0].ToUpper() == "@BEGIN" && int.TryParse(dic.Name, out buffer))
                                    {
                                        answer.Add(new Station() { NameStation =  str.Substring(str.IndexOf(massiv[0]) + massiv[0].Length).Trim(new char[] { '\'', ' ' }), NumberStation = dic.Name });
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else new FileNotFoundException("Папки {0} не существует !!!", ConfigurationManager.AppSettings["folder_neman"]);
                }
                return answer;
            }
            catch
            {
                return answer;
            }
        }

      
    }
}
