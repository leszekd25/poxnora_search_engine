using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poxnora_search_engine.Pox.Diff
{
    public enum DifferenceElementType { NEW, CHANGED, REMOVED }
    public struct DifferenceElement
    {
        public DifferenceElementType type;
        public int id;
    }
}
