using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poxnora_search_engine.Pox
{
    public class RuneGroup
    {
        public int ID = Utility.NO_INDEX;
        public string Name = "";
        public HashSet<int> RuneID = new HashSet<int>();
        public int RuneLimit = 0;

        public bool Load(string rg_string)
        {
            int cur = 0;
            while(true)
            {
                char cur_char = rg_string[cur];
                while (cur_char == ' ') { cur++; cur_char = rg_string[cur]; }
                int runeid = 0;
                while ((cur_char >= '0') && (cur_char <= '9')) { runeid *= 10; runeid += cur_char - '0'; cur++; cur_char = rg_string[cur]; }
                RuneID.Add(runeid);
                while (cur_char == ' ') { cur ++; cur_char = rg_string[cur]; }
                if (cur_char == ',') { cur++; continue; }
                else if(cur_char == '-') { cur++; break; }
                else { return false; }
            }

            int runelimit_start_pos = rg_string.IndexOf('(');
            if(runelimit_start_pos == Utility.NO_INDEX)
            {
                return false;
            }
            Name = rg_string.Substring(cur, runelimit_start_pos - cur).Trim();
            cur = runelimit_start_pos + 1;

            char cur_char2;
            while (cur < rg_string.Length) 
            {
                cur_char2 = rg_string[cur];
                if ((cur_char2 >= '0') && (cur_char2 <= '9'))
                { 
                    RuneLimit = cur_char2 - '0';
                    break;
                }
                cur++;
            }
            if(RuneLimit == 0)
            {
                return false;
            }

            return true;
        }
    }

    public class RuneGroupLibrary
    {
        public List<RuneGroup> RuneGroups = new List<RuneGroup>();
        public Dictionary<int, int> RuneToGroup = new Dictionary<int, int>();

        public RuneGroup GetGroup(int rune_id)
        {
            if (RuneToGroup.ContainsKey(rune_id))
                return RuneGroups[RuneToGroup[rune_id]];
            else
                return null;
        }

        public void Clear()
        {
            RuneGroups.Clear();
            RuneToGroup.Clear();
        }
    }

    public class DBPlugin_RuneGroups
    {
        public RuneGroupLibrary Champions = new RuneGroupLibrary();
        public RuneGroupLibrary Spells = new RuneGroupLibrary();
        public RuneGroupLibrary Relics = new RuneGroupLibrary();
        public RuneGroupLibrary Equipments = new RuneGroupLibrary();
        public bool Ready = false;

        public bool Load()
        {
            if(Ready)
            {
                return true;
            }

            string[] Lines = Properties.Resources.RuneGroups.Split('\n');     // fast enough
            RuneGroupLibrary rgl = null;
            foreach(var s in Lines)
            {
                if (s[0] == '#')
                {
                    if (rgl == null)
                        rgl = Champions;
                    else if (rgl == Champions)
                        rgl = Spells;
                    else if (rgl == Spells)
                        rgl = Relics;
                    else if (rgl == Relics)
                        rgl = Equipments;
                    else
                    {
                        Unload();
                        return false;
                    }

                    continue;
                }

                RuneGroup rg = new RuneGroup();
                if(!rg.Load(s))
                {
                    Unload();
                    return false;
                }

                rgl.RuneGroups.Add(rg);
                rg.ID = rgl.RuneGroups.Count - 1;
                foreach (var i in rg.RuneID)
                    rgl.RuneToGroup[i] = rgl.RuneGroups.Count - 1;
            }

            Ready = true;
            return true;
        }

        public void Unload()
        {
            Champions.Clear();
            Spells.Clear();
            Relics.Clear();
            Equipments.Clear();

            Ready = false;
        }
    }
}
