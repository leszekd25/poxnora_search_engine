using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poxnora_search_engine
{

    public class StringLibrary
    {
        public HashSet<string> AllowedStrings { get; } = new HashSet<string>();

        private static int[,] levenshtein_matrix = new int[256,256];

        class StringLevenshteinComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return GetLevenshteinDistance(x, y);
            }
        }

        public void GetStringsSorted(string input, ref List<string> output, int max_results)
        {
            List<string> sorted_strings = AllowedStrings.ToList();

            sorted_strings.Sort(new StringLevenshteinComparer());

            output.Clear();
            for (int i = 0; i < Math.Min(max_results, sorted_strings.Count); i++)
                output.Add(sorted_strings[i]);
        }

        private static int GetLevenshteinDistance(string s1, string s2)
        {
            if (s1.Length == 0)
                return s2.Length;
            if (s2.Length == 0)
                return s1.Length;

            for (int i = 0; i <= s1.Length; i++)
                levenshtein_matrix[i, 0] = i;

            for (int i = 0; i <= s2.Length; i++)
                levenshtein_matrix[0, i] = i;

            for(int i = 1; i <= s1.Length; i++)
                for(int j = 1; j <= s2.Length; j++)
                {
                    int cost = (s2[j - 1] == s1[i - 1]) ? 0 : 1;
                    levenshtein_matrix[i, j] = Math.Min(Math.Min(levenshtein_matrix[i - 1, j] + 1, levenshtein_matrix[i, j - 1]), levenshtein_matrix[i - 1, j - 1] + cost);
                }

            return levenshtein_matrix[s1.Length, s2.Length];
        }
    }
}
