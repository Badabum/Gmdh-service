using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Gmdh.Core.Tests
{
    [TestClass]
    public class FileOperationTests
    {
        [TestMethod]
        public void ReadExcelFile_XlsxExtension()
        {
            var currentDirectory = GetProjectFSPath();
            var filePath = $@"{currentDirectory}/test.xlsx";
            var data = FilesOperations.ReadExcelFile(filePath);
            Assert.IsNotNull(data);
        }

        [TestMethod]
        public void ReadValidExcelFile_XlsExtension_ShouldReturnNotEmptyData()
        {
            var currentDirectory = GetProjectFSPath();
            var filePath = $@"{currentDirectory}/testOldFormat.xls";
            var data = FilesOperations.ReadExcelFile(filePath);
            Assert.IsNotNull(data);
        }

        [TestMethod]
        public void ReadValidCsvFile_ShoulReturnNotEmptyData()
        {
            var currentDirectory = GetProjectFSPath();
            var filePath = $@"{currentDirectory}/testCSV.csv";
            var data = FilesOperations.ReadTextFile(filePath,';');
            Assert.IsNotNull(data);
        }


       

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException),"Smth wrong")]
        public void ReadExcelFileWithInvalidFormatOfCells_ShouldThrowException()
        {
            var currentDirectory = GetProjectFSPath();
            var filePath = $@"{currentDirectory}/invalid.xlsx";
            FilesOperations.ReadExcelFile(filePath);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void CreateFile_ThrowException_WhenPathIsEmpty()
        {
            var path = string.Empty;
            FilesOperations.SaveDataIntoFile(path,new double[1][]);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateFile_ThrowException_WhenPathDataIsNull()
        {
            var path = string.Empty;
            FilesOperations.SaveDataIntoFile(path, null);
        }

        private static string GetSolutionFSPath()
        {
            return System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName;
        }

        private static string GetProjectFSPath()
        {
            return $"{GetSolutionFSPath()}\\{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}";
        }
    }
}
