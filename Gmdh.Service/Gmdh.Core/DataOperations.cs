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
        private DenseVector _testY;

        public void InitializeDataStructures(List<List<double>> data)
        {
            var lastColumn = data.Select(row => row.Last());
            _testY = DenseVector.OfEnumerable(lastColumn);
            var rows = data.Select(row => row.GetRange(0, row.Count - 1).ToArray()).ToList();
            _mainDataMatrix = DenseMatrix.OfRowArrays(rows);
        }

        public double[] CurrentTestY => _testY.Values;
        public double[][] CurrentTestDataMatrix => _mainDataMatrix.ToRowArrays();
    }
}
