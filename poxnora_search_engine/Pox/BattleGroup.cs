using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace poxnora_search_engine.Pox
{
    public struct ChampionBG
    {
        public int ChampionID;
        public int Ability1;
        public int Ability2;
    }

    public class BattleGroup
    {
        const int CurrentVersion = 1;

        public List<ChampionBG> Champions = new List<ChampionBG>();
        public List<int> Spells = new List<int>();
        public List<int> Relics = new List<int>();
        public List<int> Equipments = new List<int>();
        public int SelectedAvatar;
        public int SelectedFactions;

        public int GetRuneCount()
        {
            return Champions.Count + Spells.Count + Relics.Count + Equipments.Count;
        }

        // used for iteration over runes
        public Rune GetRune(Database db, int rune_index)
        {
            if (rune_index < 0)
            {
                return null;
            }

            // get champion
            if (rune_index < Champions.Count)
            {
                if (!db.Champions.ContainsKey(Champions[rune_index].ChampionID))
                    return null;

                return db.Champions[Champions[rune_index].ChampionID];
            }
            rune_index -= Champions.Count;

            // get spell
            if (rune_index < Spells.Count)
            {
                if (!db.Spells.ContainsKey(Spells[rune_index]))
                    return null;

                return db.Spells[Spells[rune_index]];
            }
            rune_index -= Spells.Count;

            // get relic
            if (rune_index < Relics.Count)
            {
                if (!db.Relics.ContainsKey(Relics[rune_index]))
                    return null;

                return db.Relics[Relics[rune_index]];
            }
            rune_index -= Relics.Count;

            // get equipment
            if (rune_index < Equipments.Count)
            {
                if (!db.Equipments.ContainsKey(Equipments[rune_index]))
                    return null;

                return db.Equipments[Equipments[rune_index]];
            }
            rune_index -= Equipments.Count;

            return null;
        }

        public byte[] ToRawMemory()
        {
            byte[] data = null;
            byte[] data2 = null;

            using (MemoryStream ms2 = new MemoryStream())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (BinaryWriter bw = new BinaryWriter(ms))
                    {
                        // version
                        bw.Write((byte)CurrentVersion);

                        // rune counts
                        bw.Write((byte)GetRuneCount());
                        bw.Write((byte)Champions.Count);
                        bw.Write((byte)Spells.Count);
                        bw.Write((byte)Relics.Count);
                        bw.Write((byte)Equipments.Count);

                        // champions
                        foreach (var cbg in Champions)
                        {
                            bw.Write((ushort)cbg.ChampionID);
                            bw.Write((ushort)cbg.Ability1);
                            bw.Write((ushort)cbg.Ability2);
                        }

                        // spells
                        foreach (var s in Spells)
                            bw.Write((ushort)s);

                        // relics
                        foreach (var r in Relics)
                            bw.Write((ushort)r);

                        // equipments
                        foreach (var e in Equipments)
                            bw.Write((ushort)e);

                        // faction data
                        bw.Write((sbyte)SelectedAvatar);
                        bw.Write((sbyte)SelectedFactions);

                        data = ms.ToArray();

                    }
                }

                // compress
                try
                {
                    using (MemoryStream ms = new MemoryStream(data))
                    {
                        using (DeflateStream ds = new DeflateStream(ms2, CompressionMode.Compress))
                        {
                            ms.CopyTo(ds);
                        }
                    }

                    data2 = ms2.ToArray();
                }
                catch (Exception e)
                {
                    return null;
                }
            }

            return data2;
        }

        public bool FromRawMemory(byte[] array)
        {
            Champions.Clear();
            Spells.Clear();
            Relics.Clear();
            Equipments.Clear();
            SelectedAvatar = Utility.NO_INDEX;
            SelectedFactions = Utility.NO_INDEX;

            byte[] data = null;

            // decompress
            using (MemoryStream ms = new MemoryStream())
            {
                try
                {
                    using (MemoryStream ms2 = new MemoryStream(array))
                    {
                        using (DeflateStream ds = new DeflateStream(ms2, CompressionMode.Decompress))
                        {
                            ds.CopyTo(ms);
                        }
                    }
                    data = ms.ToArray();
                }
                catch (Exception e)
                {
                    return false;
                }
            }

            // read
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    // validation
                    if (ms.Length - ms.Position < sizeof(byte))
                    {
                        return false;
                    }

                    byte Version = br.ReadByte();       // useful later on
                    if (Version > CurrentVersion)
                    {
                        return false;
                    }

                    // validation
                    if (ms.Length - ms.Position < 5 * sizeof(byte))
                    {
                        return false;
                    }

                    byte RuneCount, ChampCount, SpellCount, RelicCount, EquipCount = 0;
                    RuneCount = br.ReadByte();
                    ChampCount = br.ReadByte();
                    SpellCount = br.ReadByte();
                    RelicCount = br.ReadByte();
                    EquipCount = br.ReadByte();

                    // validation
                    if ((int)RuneCount != (ChampCount + SpellCount + RelicCount + EquipCount))
                    {
                        return false;
                    }

                    // validation
                    if (ms.Length - ms.Position < ChampCount * sizeof(ushort) * 3)
                    {
                        return false;
                    }

                    for (int i = 0; i < ChampCount; i++)
                    {
                        ChampionBG cua = new ChampionBG();
                        cua.ChampionID = br.ReadUInt16();
                        cua.Ability1 = br.ReadUInt16();
                        cua.Ability2 = br.ReadUInt16();
                        Champions.Add(cua);
                    }

                    // validation
                    if (ms.Length - ms.Position < SpellCount * sizeof(ushort))
                    {
                        return false;
                    }

                    for (int i = 0; i < SpellCount; i++)
                    {
                        Spells.Add(br.ReadUInt16());
                    }

                    // validation
                    if (ms.Length - ms.Position < RelicCount * sizeof(ushort))
                    {
                        return false;
                    }

                    for (int i = 0; i < RelicCount; i++)
                    {
                        Relics.Add(br.ReadUInt16());
                    }

                    // validation
                    if (ms.Length - ms.Position < EquipCount * sizeof(ushort))
                    {
                        return false;
                    }

                    for (int i = 0; i < EquipCount; i++)
                    {
                        Equipments.Add(br.ReadUInt16());
                    }

                    // validation
                    if (ms.Length - ms.Position < 2 * sizeof(sbyte))
                    {
                        return false;
                    }

                    SelectedAvatar = br.ReadSByte();
                    SelectedFactions = br.ReadSByte();
                }
            }

            return true;
        }
    }
}
