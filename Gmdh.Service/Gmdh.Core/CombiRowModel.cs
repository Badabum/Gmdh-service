using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gmdh.Core
{
    public class CombiRowModel
    {
        public List<EquationModel> Models{ get; set; }
        public EquationModel BestModel { get; set; }
        public double OuterCriteriaValueOfBestModel { get; set; }
    }
}
