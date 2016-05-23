using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Gmdh.Core
{
    public class DataProcessor
    {
        private DenseMatrix _mainDataMatrix;
        private DenseMatrix _trainingMatrix;
        private DenseMatrix _checkingMatrix;
        private DenseVector _testY;
        private DenseVector _trainingY;
        private DenseVector _checkingY;
        public DataProcessor(List<List<double>> data,double trainingCheckingSplitRate = 0.5)
        {
            InitializeDataStructures(data,trainingCheckingSplitRate);
        }

        private void InitializeDataStructures(List<List<double>> data,double splitDataRate=0.5)
        {
            var lastColumn = data.Select(row => row.Last());
            _testY = DenseVector.OfEnumerable(lastColumn);
            var dataRows = data.Select(row => row.GetRange(0, row.Count - 1).ToArray()).ToList();
            _mainDataMatrix = DenseMatrix.OfRowArrays(dataRows);
            SplitData(splitDataRate);
        }

        private void SplitData(double splitDataRate)
        {
            var dataRowsCount = _mainDataMatrix.RowCount;
            var trainingRowsNumber = (int)Math.Ceiling(dataRowsCount * splitDataRate);
            var checkingRowsNumber = _mainDataMatrix.RowCount - trainingRowsNumber;
            _trainingMatrix = DenseMatrix.OfMatrix(_mainDataMatrix.SubMatrix(0, trainingRowsNumber, 0, _mainDataMatrix.ColumnCount));
            _checkingMatrix = DenseMatrix.OfMatrix(_mainDataMatrix.SubMatrix(trainingRowsNumber - 1, checkingRowsNumber, 0, _mainDataMatrix.ColumnCount));
            _trainingY = DenseVector.OfVector(_testY.SubVector(0, trainingRowsNumber));
            _checkingY = DenseVector.OfVector(_testY.SubVector(trainingRowsNumber, checkingRowsNumber));
        }

        public double[] ExperimentValues => _testY.ToArray();
        public double[][] ArgumentsData => _mainDataMatrix.ToRowArrays();
        public double[][] TrainingData => _trainingMatrix.ToRowArrays();
        public double[][] CheckingData => _checkingMatrix.ToRowArrays();

        public DenseMatrix ArgumentsDataMatrix => _mainDataMatrix;
        public DenseVector ExperimentValuesVector => _testY;
        public DenseMatrix TrainingMatrix => _trainingMatrix;
        public DenseMatrix CheckingMatrix => _checkingMatrix;
        public DenseVector TrainingValues => _trainingY;
        public DenseVector CheckingValues => _checkingY;
    }
}
