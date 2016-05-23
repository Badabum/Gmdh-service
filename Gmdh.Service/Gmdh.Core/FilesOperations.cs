using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Xml;
using CsvHelper;
using Excel;


namespace Gmdh.Core
{
    public static class FilesOperations
    {
        /// <summary>
        /// Reads the excel file specified in fileName param. We assume that data in file is stored like matrix, and there is only one worksheet in the specified file
        /// </summary>
        /// <param name="filePath">Full path to excel file to read</param>
        /// <returns></returns>
        public static List<List<double>> ReadExcelFile(string filePath)
        {
            var resultList = new List<List<double>>();

            using (IExcelDataReader excelReader = GetExcelReader(filePath,true))
            {
                var dataSet = excelReader.AsDataSet();
                var worksheet = dataSet.Tables[0];
                var rows = GetRows(worksheet);
                resultList.AddRange(
                    rows.Select(
                        dataRow => dataRow.ItemArray.Skip(1).Cast<double>().ToList()
                        )
                    );

                return resultList;
            }
        }

        /// <summary>
        /// Reads the text files with sample data matrix in it.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="separator"></param>
        /// <returns>List of lists of int(two dimensional matrix)</returns>
        public static List<List<double>> ReadTextFile(string filePath,char separator=';')
        {

            using (var fileStream = new FileStream(filePath,FileMode.Open,FileAccess.Read))
            {
                using (var streamReader = new StreamReader(fileStream))
                {
                    var resultList = new List<List<double>>();
                    while (!streamReader.EndOfStream)
                    {
                        var row = streamReader.ReadLine().Split(separator);
                        var doubleList = row.Select(double.Parse);
                        resultList.Add(doubleList.ToList());
                    }
                    return resultList;
                }
            }
        }

        /// <summary>
        /// Saves data into plain text file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="array"></param>
        public static bool SaveDataIntoFile(string filePath, double[][] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }
            using (var fileStream = new FileStream(filePath,FileMode.Create,FileAccess.Write))
            {
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    foreach (var row in data)
                    {
                        streamWriter.WriteLine(string.Join(" ",row));
                    }
                }
                return true;
            }
        }

        public static List<List<double>> ReadFile(string path)
        {
            var fileExtension = Path.GetExtension(path);
            var data = new List<List<double>>();
            switch (fileExtension)
            {
                case ".xlsx":
                {
                    data =  ReadExcelFile(path);
                    break;
                }
                case ".csv":
                {
                    data = ReadTextFile(path);
                        break;
                }
                case ".txt":
                {
                    data = ReadTextFile(path, ' ');
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(fileExtension));
                }
            }
            return data;
        }

        public static IEnumerable<string> GetFiles(string directoryPath)
        {
            var files = Directory.GetFiles(directoryPath);
            return files.Select(Path.GetFileName);
        } 
        private static IExcelDataReader GetExcelReader(string filePath,bool isfirstRowsAsColumnNames=false)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var fileExtension = GetExtesion(filePath);
                IExcelDataReader reader;
                switch (fileExtension)
                {
                    case ".xls":
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                        break;
                    }
                    case ".xlsx":
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        break;
                    }
                    default:
                    {
                        throw new ArgumentOutOfRangeException(nameof(fileExtension));
                    }
                }
                reader.IsFirstRowAsColumnNames = isfirstRowsAsColumnNames;
                
                return reader;
            }
        }

        private static IEnumerable<DataRow> GetRows(DataTable table)
        {
            var resultSet = from DataRow row in table.Rows
                            select row;
            return resultSet;
        }

        public static string SaveIntoExcelFile(List<double> modelCoeficients,string fileExtension = ".xlsx")
        {
            throw new NotImplementedException();
        }

        public static string SaveExcelFile(List<List<double>> data, string fileExtension = ".xlsx")
        {
            throw new NotImplementedException();
        }

        public static string SaveIntoTextFile(List<List<double>> data,string path)
        {
            using (var streamWriter = new StreamWriter(path))
            {
                foreach (var dataAsString in data.Select(row => string.Join(" ", row)))
                {
                    streamWriter.WriteLine(dataAsString);
                }
            }
            return path;
        }

        public static string SaveToTextFile(CombiModel combiModel,string path)
        {
            using (var streamWriter = new StreamWriter(path))
            {
                var algRows = combiModel.AlgorithmRows;
                var i = 0;
                foreach (var combiRowModel in algRows)
                {
                    streamWriter.WriteLine($"Step #{++i}:");
                    foreach (var model in combiRowModel.Models)
                    {
                        streamWriter.WriteLine(model.ToString());
                    }
                    streamWriter.WriteLine($"{combiRowModel.BestModel.ToString()} Outer criteria: {combiRowModel.OuterCriteriaValueOfBestModel}");

                }
            }
            return path;
        }



        private static string GetExtesion(string filePath)
        {
            return Path.GetExtension(filePath);
        } 
        
    }
}
