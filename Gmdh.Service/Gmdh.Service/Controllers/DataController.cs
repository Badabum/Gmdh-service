using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Gmdh.Core;

namespace Gmdh.Service.Controllers
{
    public class DataController : ApiController
    {
        private DataOperations dataProcessing;
        public DataController()
        {
            dataProcessing = new DataOperations();
        }
        [HttpPost]
        [Route("api/data/splited")]
        public DataModel SplitedData(List<List<double>> data)
        {
            dataProcessing.InitializeDataStructures(data);
            var model = new DataModel()
            {
                TestY = dataProcessing.CurrentTestY,
                CheckingData = dataProcessing.CheckingData,
                TrainingData = dataProcessing.TrainingData
            };
            return model;
        }
      
        [HttpPost]
        [Route("api/training/combi")]
        public CombiModel TrainModel(AlgModel parameters)
        {
            dataProcessing.InitializeDataStructures(parameters);
            var gmdhAlgo = new GmdhAlgorithmBase(dataProcessing.TrainingData,dataProcessing.CheckingData,dataProcessing.TrainingValues,dataProcessing.CheckingValues);
            var trainingResult = gmdhAlgo.RunAlgorithm(parameters.Complexity);
            return trainingResult;
        }

        public class DataModel
        {
            public double[] TestY { get; set; }
            public double[][] TrainingData { get; set; }
            public double[][] CheckingData { get; set; }
        }

    }
}
