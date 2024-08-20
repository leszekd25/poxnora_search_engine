using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace poxnora_search_engine.Pox
{
    public struct SynergyContextKey: IEquatable<SynergyContextKey>
    {
        public string Faction1;
        public string Faction2;

        public SynergyContextKey(string f1, string f2)
        {
            if(f1 == "")
            {
                Faction1 = f2;
                Faction2 = f1;
                return;
            }
            else if(f2 == "")
            {
                Faction1 = f1;
                Faction2 = f2;
                return;
            }

            if(f1.CompareTo(f2) > 0)
            {
                Faction1 = f2;
                Faction2 = f1;
            }
            else
            {
                Faction1 = f1;
                Faction2 = f2;
            }
        }

        public bool Equals(SynergyContextKey other)
        {
            return (Faction1 == other.Faction1) && (Faction2 == other.Faction2);
        }

        public bool ContainsChampion(Champion c)
        {
            if ((Faction1 == "")&&(Faction2 == ""))
            {
                return true;
            }

            if (!c.Faction.Contains(Faction1))
            {
                if ((Faction2 == "") || (!c.Faction.Contains(Faction2)))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return Faction1.GetHashCode() ^ Faction2.GetHashCode();
        }
    }

    public struct ChampionPower: IComparable<ChampionPower>, IEquatable<ChampionPower>
    {
        public int ID;
        public float NoraMod;

        public ChampionPower(int id, float noramod)
        {
            ID = id;
            NoraMod = noramod;
        }

        public bool Equals(ChampionPower other)
        {
            return (ID == other.ID) && (NoraMod == other.NoraMod);
        }

        public int CompareTo(ChampionPower other)
        {
            return NoraMod.CompareTo(other.NoraMod);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode() ^ NoraMod.GetHashCode();
        }
    }
    public struct PowerData
    {
        public int ChampCount;
        public float MaxPower;
        public float AveragePower;

        public PowerData(int cc, float mp, float ap)
        {
            ChampCount = cc;
            MaxPower = mp;
            AveragePower = ap;
        }
    }


    public class SynergyContext
    {
        public int ID;
        public List<ChampionPower> ChampionsAvailable = new List<ChampionPower>();
        public PowerData GlobalPower = new PowerData();
        public Dictionary<string, PowerData> RacePower = new Dictionary<string, PowerData>();
        public Dictionary<string, PowerData> ClassPower = new Dictionary<string, PowerData>();
    }


    public class DBPlugin_SynergyComparer
    {
        Database db;
        public Dictionary<SynergyContextKey, SynergyContext> Synergies = new Dictionary<SynergyContextKey, SynergyContext>();
        public bool Ready { get; private set; } = false;

        public DBPlugin_SynergyComparer()
        {

        }

        public void Load(Database _db)
        {
            if(Ready)
            {
                return;
            }

            db = _db;

            List<string> factions = db.Factions.GetStringsSortedDefault();
            List<string> races = db.Races.GetStringsSortedDefault();
            List<string> classes = db.Classes.GetStringsSortedDefault();

            // this will crudely calculate relative power of a class/race globally and in each faction
            // it does so by averaging the nora modifier of 10 champs with lowest nora modifier in each context
            // context is full faction or split faction

            // calculate possible contexts
            List<SynergyContextKey> contexts = new List<SynergyContextKey>();
            for(int i = 0; i < factions.Count; i++)
            {
                contexts.Add(new SynergyContextKey(factions[i], ""));
                for(int j = 0; j < i; j++)
                {
                    contexts.Add(new SynergyContextKey(factions[i], factions[j]));
                }
            }

            // create list of champion runes for each context
            List<SynergyContext> synergies = new List<SynergyContext>();
            List<ChampionPower> tmp_champs = new List<ChampionPower>();
            for(int i = 0; i < contexts.Count; i++)
            {
                SynergyContext sc = new SynergyContext();
                sc.ID = i;
                SynergyContextKey sck = contexts[i];
                foreach(var c in db.Champions)
                {
                    if(sck.ContainsChampion(c.Value))
                    {
                        if (c.Value.Rarity != "LIMITED")
                        {
                            sc.ChampionsAvailable.Add(new ChampionPower(c.Key, c.Value.PrognosedBaseNoraCostDifference));
                        }
                    }    
                }
                sc.ChampionsAvailable.Sort();
                synergies.Add(sc);

                // calculate max power
                float maxnoramodsum = 0;
                for (int k = 0; k < 10; k++)
                {
                    maxnoramodsum += sc.ChampionsAvailable[k].NoraMod;
                }
                maxnoramodsum /= 10;
                float avgnoramodsum = 0;
                for (int k = 0; k < sc.ChampionsAvailable.Count; k++)
                {
                    avgnoramodsum += sc.ChampionsAvailable[k].NoraMod;
                }
                avgnoramodsum /= sc.ChampionsAvailable.Count;
                sc.GlobalPower = new PowerData(sc.ChampionsAvailable.Count, maxnoramodsum, avgnoramodsum);

                // calculate power per race
                for (int j = 0; j < races.Count; j++)
                {
                    tmp_champs.Clear();
                    string race = races[j];

                    for (int k = 0; k < sc.ChampionsAvailable.Count; k++)
                    {
                        if (db.Champions[sc.ChampionsAvailable[k].ID].Race.Contains(race))
                        {
                            tmp_champs.Add(sc.ChampionsAvailable[k]);
                        }
                    }

                    maxnoramodsum = 0;
                    int kmax = Math.Min(10, tmp_champs.Count);
                    for (int k = 0; k < kmax; k++)
                    {
                        maxnoramodsum += tmp_champs[k].NoraMod;
                    }
                    if (kmax != 0)
                    {
                        maxnoramodsum /= kmax;
                    }
                    avgnoramodsum = 0;
                    for (int k = 0; k < tmp_champs.Count; k++)
                    {
                        avgnoramodsum += tmp_champs[k].NoraMod;
                    }
                    if (tmp_champs.Count != 0)
                    {
                        avgnoramodsum /= tmp_champs.Count;
                    }
                    sc.RacePower[race] = new PowerData(tmp_champs.Count, maxnoramodsum, avgnoramodsum);
                }

                // calculate power per class
                for (int j = 0; j < classes.Count; j++)
                {
                    tmp_champs.Clear();
                    string klass = classes[j];

                    for (int k = 0; k < sc.ChampionsAvailable.Count; k++)
                    {
                        if (db.Champions[sc.ChampionsAvailable[k].ID].Class.Contains(klass))
                        {
                            tmp_champs.Add(sc.ChampionsAvailable[k]);
                        }
                    }

                    maxnoramodsum = 0;
                    int kmax = Math.Min(10, tmp_champs.Count);
                    for (int k = 0; k < kmax; k++)
                    {
                        maxnoramodsum += tmp_champs[k].NoraMod;
                    }
                    if (kmax != 0)
                    {
                        maxnoramodsum /= kmax;
                    }
                    avgnoramodsum = 0;
                    for (int k = 0; k < tmp_champs.Count; k++)
                    {
                        avgnoramodsum += tmp_champs[k].NoraMod;
                    }
                    if (tmp_champs.Count != 0)
                    {
                        avgnoramodsum /= tmp_champs.Count;
                    }
                    sc.ClassPower[klass] = new PowerData(tmp_champs.Count, maxnoramodsum, avgnoramodsum);
                }

                Synergies[sck] = sc;
            }

            Ready = true;
        }
    }
}
