using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.Differentiation;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearRegression;

namespace Gmdh.Core
{
    public class DataOperations
    {
        private readonly List<List<double>> _data;

        private DenseMatrix _mainDataMatrix;
        private DenseMatrix _trainingMatrix;
        private DenseMatrix _checkingMatrix;
        private DenseVector _testY;
        private DenseVector _trainingY;
        private DenseVector _checkingY;
        private List<List<Tuple<double[], double>>> _modelsCoeficiensAndCriteriaValues;
        private List<CombiRowModel> _models;
        public EquationModel _bestModel;
        private int _maxComplexity;



        public double[] CurrentTestY => _testY.Values;
        public double[][] CurrentTestDataMatrix => _mainDataMatrix.ToRowArrays();
        public double[][] TrainingData => _trainingMatrix.ToRowArrays();
        public double[][] CheckingData => _checkingMatrix.ToRowArrays();
        public double[] CheckingValues => _checkingY.ToArray();
        public double[] TrainingValues => _trainingY.ToArray();

        public DenseMatrix TrainingX => _trainingMatrix;
        public DenseMatrix CheckingX => _checkingMatrix;
        public DenseVector TrainingY => _trainingY;
        public DenseVector CheckingY => _checkingY;

        public void InitializeDataStructures(List<List<double>> data,int maxComplexity=20)
        {
            _maxComplexity = maxComplexity;
            var lastColumn = data.Select(row => row.Last());
            _testY = DenseVector.OfEnumerable(lastColumn);
            var rows = data.Select(row => row.GetRange(0, row.Count - 1).ToArray()).ToList();
            _mainDataMatrix = DenseMatrix.OfRowArrays(rows);
            _modelsCoeficiensAndCriteriaValues = new List<List<Tuple<double[], double>>>();
            _models = new List<CombiRowModel>();
            //SplitData(0.5);
        }

        public void InitializeDataStructures(AlgModel paramseters)
        {
            _maxComplexity = paramseters.Complexity;
            _trainingMatrix = DenseMatrix.OfRowArrays(paramseters.TrainingData);
           
            var lastColumn = DenseVector.OfVector(_trainingMatrix.Column(_trainingMatrix.ColumnCount - 1));
            _trainingMatrix = DenseMatrix.OfMatrix(_trainingMatrix.SubMatrix(0,_trainingMatrix.RowCount-1,0,_trainingMatrix.ColumnCount-2));
            _trainingY = lastColumn;
            if (!paramseters.UseTrainingAsChecking)
            {
                _checkingMatrix = DenseMatrix.OfRowArrays(paramseters.CheckingModel);
                var yVector = DenseVector.OfVector(_checkingMatrix.Column(_checkingMatrix.ColumnCount - 1));
                _checkingMatrix =
                    DenseMatrix.OfMatrix(_checkingMatrix.SubMatrix(0, _checkingMatrix.RowCount - 1, 0,
                        _checkingMatrix.ColumnCount - 2));
                _checkingY = yVector;
            }
            if (paramseters.UseTrainingAsChecking)
            {
                SplitData(0.5,_trainingMatrix,_trainingY);
            }
        }

        public void SplitData(double trainingPercentage,DenseMatrix matrixToSplit,DenseVector yToSplit)
        {
            SplitDataIntoTrainingAndChecking(trainingPercentage,matrixToSplit,yToSplit);
        }
        
        private void SplitDataIntoTrainingAndChecking(double percentage,DenseMatrix matrixToSplit,DenseVector yToSplit)
        {
            var dataRowsCount = matrixToSplit.RowCount;
            var trainingRowsNumber = (int)Math.Ceiling(dataRowsCount*percentage);
            var checkingRowsNumber = matrixToSplit.RowCount - trainingRowsNumber;

            var trainingMatrix = DenseMatrix.OfMatrix(matrixToSplit.SubMatrix(0,trainingRowsNumber,0,matrixToSplit.ColumnCount));
            var checkingMatrix = DenseMatrix.OfMatrix(matrixToSplit.SubMatrix(trainingRowsNumber-1,checkingRowsNumber,0,matrixToSplit.ColumnCount));
            var trainingY = DenseVector.OfVector(yToSplit.SubVector(0, trainingRowsNumber));
            var checkingY = DenseVector.OfVector(yToSplit.SubVector(trainingRowsNumber,checkingRowsNumber));

            _trainingMatrix = trainingMatrix;
            _checkingMatrix = checkingMatrix;
            _trainingY = trainingY;
            _checkingY = checkingY;

        } 
    }
}
