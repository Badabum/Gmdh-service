using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gmdh.Core.Tests
{
    [TestClass]
    public class DataOperationsTest
    {
        private List<List<double>> sampleData;
        [TestInitialize]
        public void InitializeTest()
        {
            sampleData = new List<List<double>>()
            {
                new List<double>()
                {
                    1,2.23,123.123, 123.32,323,4
                },
                new List<double>()
                {
                    1,2.23,123.123, 123.32,323,4
                },
                new List<double>()
                {
                    1,2.23,123.123, 123.32,323,4
                }
            };
        }
        [TestCleanup]
        public void TestCleanup()
        {
            sampleData = null;
        }
        [TestMethod]
        public void DataInitialization_ShouldCreateYVectorFromLastColumn()
        {
            var dataProcessor = new DataOperations();
            dataProcessor.InitializeDataStructures(sampleData);

            CollectionAssert.AreEqual(dataProcessor.CurrentTestY,new double[] {4,4,4});
        }

        [TestMethod]
        public void DataInitialization_ShouldCreateMainMatrixFromInputWithoutLastColumn()
        {
            var sampleData = new List<List<double>>()
            {
                new List<double>()
                {
                    1,
                    2,
                    3
                },
                new List<double>()
                {
                    1,
                    2,
                    3
                },
                new List<double>()
                {
                    1,
                    2,
                    3
                }
            };
            var expectedValue = new[] {new double[] {1, 2}, new double[] {1, 2}, new double[] {1, 2}};
            var dataProcessor = new DataOperations();
            dataProcessor.InitializeDataStructures(sampleData);
            Assert.AreEqual(dataProcessor.CurrentTestDataMatrix.Length,expectedValue.Length);
            Assert.AreEqual(dataProcessor.CurrentTestDataMatrix[0].Length,expectedValue[0].Length);
        }
        [TestMethod]
        public void SplitData_ShouldSplitDataInTwoArrays()
        {
            var dataProcessor = new DataOperations();
            dataProcessor.InitializeDataStructures(sampleData);
            dataProcessor.SplitData(0.6);

            CollectionAssert.AreEqual(dataProcessor.CheckingData[0],new[] { 1, 2.23, 123.123, 123.32, 323 });   
        }
    }
}
