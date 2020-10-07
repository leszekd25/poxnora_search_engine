using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poxnora_search_engine.Pox.Diff
{
    public class DifferenceLink
    {
        public DataElement PreviousElement = null;
        public DataElement CurrentElement = null;

        public override string ToString()
        {
            if (PreviousElement == null)
            {
                if (CurrentElement == null)
                    return "<!!!INVALID!!!>";
                else
                    return "ADDED: " + CurrentElement.ToString();
            }
            else
            {
                if (CurrentElement == null)
                    return "REMOVED: " + PreviousElement.ToString();
                else
                    return "CHANGED: " + PreviousElement.ToString();
            }

            return "<???CODE FLOW ERROR???>";
        }
    }
}
