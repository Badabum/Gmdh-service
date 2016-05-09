using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Gmdh.Core
{
    public class DataOperations
    {
        private readonly List<List<double>> _data;

        private DenseMatrix _mainDataMatrix;
        private DenseMatrix _trainingMatrix;
        private DenseMatrix _checkingMatrix;
        private DenseVector _testY;

        public void InitializeDataStructures(List<List<double>> data)
        {
            var lastColumn = data.Select(row => row.Last());
            _testY = DenseVector.OfEnumerable(lastColumn);
            var rows = data.Select(row => row.GetRange(0, row.Count - 1).ToArray()).ToList();
            _mainDataMatrix = DenseMatrix.OfRowArrays(rows);
            
        }

        public void SplitData(double trainingPercentage)
        {
            SplitDataIntoTrainingAndChecking(trainingPercentage);
        }
        
        private void SplitDataIntoTrainingAndChecking(double percentage)
        {
            var dataRowsCount = _mainDataMatrix.RowCount;
            var trainingRowsNumber = (int)Math.Ceiling(dataRowsCount*percentage);
            var checkingRowsNumber = _mainDataMatrix.RowCount - trainingRowsNumber;
            _trainingMatrix = DenseMatrix.OfMatrix(_mainDataMatrix.SubMatrix(0,trainingRowsNumber,0,_mainDataMatrix.ColumnCount));
            _checkingMatrix = DenseMatrix.OfMatrix(_mainDataMatrix.SubMatrix(trainingRowsNumber-1,checkingRowsNumber,0,_mainDataMatrix.ColumnCount));
        }

        public double CalculateRegularityCriteria()
        {
            throw new NotImplementedException();
        }

        public double CalculateUnbiasCriteria()
        {
            throw new NotImplementedException();
        }

        public void CombiAlgorithm()
        {
            
        }

        public void MiaAlgorithm()
        {
            
        }
        

        public double[] CurrentTestY => _testY.Values;
        public double[][] CurrentTestDataMatrix => _mainDataMatrix.ToRowArrays();
        public double[][] TrainingData => _trainingMatrix.ToRowArrays();
        public double[][] CheckingData =>  _checkingMatrix.ToRowArrays();
    }
}
