using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gmdh.Core
{
    public class EquationModel
    {
        public bool[] Coeficients { get; set; }
        public double[] CoeficientValues { get; set; }
        public double InnerCriteriaValue { get; set; }
        public string Equation => ToString();
        public int Complexity { get; set; }
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            var equation = "";
            for (int i = 0; i < Coeficients.Length; i++)
            {
                if (Coeficients[i])
                {
                    var sign = "";
                    if (CoeficientValues[i] >= 0)
                    {
                        sign = "+";
                    }
                    stringBuilder.Append($"{sign} {CoeficientValues[i].ToString("F2")} x{i + 1} ");
                }

            }
            if (stringBuilder[0] == '+')
            {
                stringBuilder.Remove(0, 1);
            }
            stringBuilder.Insert(0, "y = ");
            return stringBuilder.ToString();
        }
    }
}
