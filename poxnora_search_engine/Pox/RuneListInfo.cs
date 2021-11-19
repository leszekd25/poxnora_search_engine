using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poxnora_search_engine.Pox
{
    public enum ViewModeEnum { NONE = -1, GRID, IMAGES }

    public class RuneListInfo
    {
        public ViewModeEnum ViewMode;
        public Pox.DataElement.ElementType ViewType;
        public Pox.Filters.BaseFilter Filter;
        public bool ApplyFilter;
    }
}
