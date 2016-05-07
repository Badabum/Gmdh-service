using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
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

            using (IExcelDataReader excelReader = GetExcelReader(filePath))
            {
                var dataSet = excelReader.AsDataSet();
                var worksheet = dataSet.Tables[0];
                var rows = GetRows(worksheet);
                resultList.AddRange(
                    rows.Select(
                        dataRow => dataRow.ItemArray.Cast<double>().ToList()
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
        private static string GetExtesion(string filePath)
        {
            return Path.GetExtension(filePath);
        } 
        
    }
}
