using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Storage;


namespace Gmdh.Core
{
    public class GmdhAlgorithmBase
    {
        private readonly DenseMatrix _trainingData;
        private readonly DenseMatrix _checkingData;
        private readonly DenseVector _trainingValues;
        private readonly DenseVector _checkingValues;
        private readonly int numberOfArguments;
        private Func<EquationModel, DenseVector, DenseMatrix, double> externalCriteriaFunc;
        private Func<DenseMatrix, DenseVector, bool[],Tuple<double[],double>> internalCriteria;

        //public GmdhAlgorithmBase(DenseMatrix trainingData, DenseMatrix checkingData, DenseVector trainingValues, DenseVector checkingValues)
        //{
        //    _trainingData = trainingData;
        //    _checkingData = checkingData;
        //    _trainingValues = trainingValues;
        //    _checkingValues = checkingValues;
        //    numberOfArguments = _trainingData.ColumnCount;
        //}

        public GmdhAlgorithmBase(double[][] trainingX, double[][] checkingX, double[] trainingY,double[] checkingY)
        {
            _trainingData = DenseMatrix.OfRowArrays(trainingX);
            _checkingData = DenseMatrix.OfRowArrays(checkingX);
            _trainingValues = DenseVector.OfArray(trainingY);
            _checkingValues = DenseVector.OfArray(checkingY);
            numberOfArguments = _trainingData.ColumnCount;
        }

        public CombiModel RunAlgorithm(int maxComplexity = 20)
        {
            var models = new List<CombiRowModel>();
            
            
            var setsOfModelsByComplexity = GetAllModelsOfIncreasingComplexity(maxComplexity);
            foreach (var modelsSet in setsOfModelsByComplexity)
            {
                var setDataModel = new CombiRowModel();
                var currentComplexityModelsParams = modelsSet.Select(model =>
                {
                    var modelObj = new EquationModel()
                    {
                        Coeficients = model
                    };
                    var paramsForModel = internalCriteria(_trainingData, _trainingValues, model);
                    var mappedCoefsToModelParams = MapCoeficientsToModel(model, paramsForModel.Item1);
                    modelObj.CoeficientValues = mappedCoefsToModelParams;
                    modelObj.InnerCriteriaValue = paramsForModel.Item2;
                    return modelObj;
                });
                setDataModel.Models = currentComplexityModelsParams.ToList();
                var bestModelForSet =
                    currentComplexityModelsParams.Aggregate(
                        (m1, m2) => m1.InnerCriteriaValue < m2.InnerCriteriaValue ? m1 : m2);
                setDataModel.BestModel = bestModelForSet;
                var externalCriteriaValue = externalCriteriaFunc(bestModelForSet, _checkingValues, _checkingData);
                setDataModel.OuterCriteriaValueOfBestModel = externalCriteriaValue;
                models.Add(setDataModel);
            }
            var bestModel = GetBestModel(models);
            var combiModel = new CombiModel()
            {
                BestModel = bestModel,
                AlgorithmRows = models
            };
            return combiModel;
        }

        

        protected virtual List<HashSet<bool[]>> GetAllModelsOfIncreasingComplexity(int maxComplexity)
        {
            var rows = new List<HashSet<bool[]>>();
            for (var i = 1; i <= maxComplexity; i++)//current complexity of model, e.g. number of arguments
            {
                var row = GetAllCombinations(numberOfArguments, i);
                rows.Add(row);
            }
            return rows;
        }

        protected virtual HashSet<bool[]> GetAllCombinations(int arguments, int itemsToBeSelected)
        {
            var combinationsSet = new HashSet<bool[]>(new BooleanArrayEqualityComparer());
            var numberOfCombinations = Combinatorics.Combinations(arguments, itemsToBeSelected);
            while (numberOfCombinations > 0)
            {
                var currentCombination = Combinatorics.GenerateCombination(arguments, itemsToBeSelected);
                var added = combinationsSet.Add(currentCombination);
                if (added)
                {
                    numberOfCombinations--;
                }
            }
            return combinationsSet;
        } 

        protected virtual EquationModel GetBestModel(List<CombiRowModel> models)
        {
            var bestRow =
                models.Aggregate(
                    (el1, el2) => el1.OuterCriteriaValueOfBestModel < el2.OuterCriteriaValueOfBestModel ? el1 : el2);
            return bestRow.BestModel;
        }

        protected virtual double[] MapCoeficientsToModel(bool[] model, double[] coeficients)
        {
            var newCoeficientsArray = new double[model.Length];
            var currentCoeficientIndex = 0;
            for (int i = 0; i < model.Length; i++)
            {
                if (model[i])
                {
                    newCoeficientsArray[i] = coeficients[currentCoeficientIndex];
                    currentCoeficientIndex++;
                }
            }
            return newCoeficientsArray;
        }
        
  
    }
}
