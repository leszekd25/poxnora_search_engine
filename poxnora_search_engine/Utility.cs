using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace poxnora_search_engine
{
    static public class Utility
    {
        public const int NO_INDEX = -1;

        public const string APP_VERSION = "31.7.2022.1";

        public static Random RNG = new Random();

        public static float Sigmoid(float lim, float x)
        {
            float tmp1 = (float)(1 + Math.Exp(-x / (lim / 2)));
            float sgm = ((lim * 2) / tmp1) - lim;
            return sgm;
        }

    }
}
