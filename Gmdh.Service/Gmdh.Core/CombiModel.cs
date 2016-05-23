using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gmdh.Core
{

    public class CombiModel
    {
        public List<CombiRowModel> AlgorithmRows { get; set; }
        public EquationModel BestModel { get; set; } 
    }
   
}
