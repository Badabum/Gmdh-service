using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
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
            _trainingY = DenseVector.OfVector(_testY.SubVector(0, trainingRowsNumber));
            _checkingY = DenseVector.OfVector(_testY.SubVector(trainingRowsNumber,checkingRowsNumber));
        }

        public double CalculateRegularityCriteria(bool[]model) 
        {
            throw new NotImplementedException();
        }

        public double CalculateUnbiasCriteria()
        {
            throw new NotImplementedException();
            
        }

        /// <summary>
        /// Inner criteria realisation - just calculate coeficient using least squares method
        /// </summary>
        /// <param name="experimentY"></param>
        /// <param name="experimentArgs"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public Tuple<double[],double>CalculateModelParamsForModel(DenseVector experimentY, DenseMatrix experimentArgs, bool[] model)
        {
            var currentModelActiveCoeficients =
                model.Select((elem, index) => new {elem, index})
                    .Where(tuple => tuple.elem == true)
                    .Select(tuple => tuple.index)
                    .OrderBy(index=>index);
            var columns = currentModelActiveCoeficients.Select(currentModelActiveCoeficient => experimentArgs.Column(currentModelActiveCoeficient)).ToList();
            var matrix = DenseMatrix.OfColumnVectors(columns);
            var coeficients = MultipleRegression.DirectMethod(matrix, experimentY);
            var sumOfLeastSquares = CalculateLeastSumSquaresForModel(matrix, experimentY, coeficients.ToArray());
            return new Tuple<double[], double>(coeficients.ToArray(),sumOfLeastSquares);
        }

        public double CalculateLeastSumSquaresForModel(DenseMatrix experimentData, DenseVector experimentY,
            double[] coeficients)
        {
            var sum = 0.0;
            //model function is like that = a0+a1x1+a2x2+...
            for (int i = 0; i < experimentData.RowCount; i++)
            {
                var row = experimentData.Row(i);
                var functionValue = coeficients.Select((coef, index) => coef*row[index]).Sum();
                var experimentValue = experimentY[i];
                sum += Math.Pow(experimentValue - functionValue, 2);
            }
            return sum;
        }

        public HashSet<bool[]> GetAllCombinations(int columns,  int itemsToBeSelected)
        {
            var combinationDictionary = new HashSet<bool[]>(new BooleanArrayEqualityComparer());
            var numberOfCombinations = Combinatorics.Combinations(columns, itemsToBeSelected);
            while (numberOfCombinations > 0)
            {
                var currentCombination = Combinatorics.GenerateCombination(columns, itemsToBeSelected);
                var added = combinationDictionary.Add(currentCombination);
                if (added)
                {
                    numberOfCombinations--;
                }
            }
            return combinationDictionary;

        }

        public List<HashSet<bool[]>> GetAllRowsOfModelByComplexity(int maxComplexity)
        {
            var rows = new List<HashSet<bool[]>>();
            for (var i = 1; i <= maxComplexity; i++)//current complexity of model, e.g. number of arguments
            {
                var row = GetAllCombinations(maxComplexity, i);
                rows.Add(row);
            }
            return rows;
        } 

        public void CombiAlgorithm(int maxComplexity=20)
        {
            var setOfModelsByComplexity = GetAllRowsOfModelByComplexity(maxComplexity);
            var bestModelsForEachLayer = new List<Tuple<double[],double>>();
            foreach (var setOfModels in setOfModelsByComplexity)
            {
                var currentComplexityModelsParams = setOfModels.Select(model => CalculateModelParamsForModel(_trainingY, _trainingMatrix, model)).ToList();
                var bestModel =
                    currentComplexityModelsParams.Aggregate((elem1, elem2) => elem1.Item2 < elem2.Item2 ? elem1 : elem2);
                bestModelsForEachLayer.Add(bestModel);
                //var exteralCriteriaValueForModel = CalculateRegularityCriteria()
            }
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
