using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gmdh.Core
{
    public class AlgModel
    {
        public int Complexity { get; set; }
        public string Criteria { get; set; }
        public int Models { get; set; }
        public double Precision { get; set; }
        public bool UseTrainingAsChecking { get; set; }
        public double[][] TrainingData { get; set; }
        public double[][] CheckingModel { get; set; }
    }
}
