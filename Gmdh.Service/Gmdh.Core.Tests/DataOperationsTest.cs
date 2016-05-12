using System;
using System.Collections.Generic;
using System.Linq;
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


        [TestMethod]
        public void GetCOmbinationsOf2ElemsFrom4_ShouldReturn6UniqueCombinations()
        {
            var dataProcessor = new DataOperations();
            var combinations = dataProcessor.GetAllCombinations(4, 2);
            Assert.AreEqual(6,combinations.Count);

        }

        [TestMethod]
        public void GetCombinationOf20ElementsFrom20Elements_ShouldReturn1000Elements()
        {
            var dataProcessor = new DataOperations();
            var combinations = dataProcessor.GetAllCombinations(20, 20);
            Assert.AreEqual(1,combinations.Count);
        }

        [TestMethod]
        public void GetAllRowsOfModel_ShouldReturn10RowsForMaxComplexity10()
        {
            var dataOperations = new DataOperations();
            var modelRows = dataOperations.GetAllRowsOfModelByComplexity(10);
            Assert.AreEqual(10,modelRows.Count);
        }
        [TestMethod]
        public void GetAllRowsOfModel_ShouldReturnRowsInGrouwingComplexityOrder()
        {
            var dataOperations = new DataOperations();
            var modelRows = dataOperations.GetAllRowsOfModelByComplexity(10);
            var firsModel = modelRows[0].First();
            var secondRow = modelRows[1].First();
            var lastModel = modelRows.Last().First();
            var countOfArgsFirst = firsModel.Count(el => el == true);
            var countOfArgsSecond = secondRow.Count(el => el == true);
            var countOFArgsLast = lastModel.Count(el => el == true);
            Assert.AreEqual(1,countOfArgsFirst);
            Assert.AreEqual(2,countOfArgsSecond);
            Assert.AreEqual(10,countOFArgsLast);
        }
        
    }
}
