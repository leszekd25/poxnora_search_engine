using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poxnora_search_engine.Pox.Diff
{
    public class DatabaseDifferenceCalculator
    {
        public Database CurrentDatabase_ref { get; private set; }
        public Database PreviousDatabase { get; private set; }

        public bool ready { get; private set; } = false;

        public List<DifferenceElement> DifferingChampions { get; } = new List<DifferenceElement>();
        public List<DifferenceElement> DifferingAbilities { get; } = new List<DifferenceElement>();
        public List<DifferenceElement> DifferingSpells { get; } = new List<DifferenceElement>();
        public List<DifferenceElement> DifferingRelics { get; } = new List<DifferenceElement>();
        public List<DifferenceElement> DifferingEquipments { get; } = new List<DifferenceElement>();

        public void LoadDatabases(string previous_database)
        {
            CurrentDatabase_ref = Program.database;

            PreviousDatabase = new Database();
            PreviousDatabase.LoadJSON(previous_database, "");

            ready = (CurrentDatabase_ref.ready && PreviousDatabase.ready);
            if (!ready)
                PreviousDatabase = null;
        }

        public void Calculate()
        {
            if (!ready)
                return;

            // load champions
            foreach (Champion c in PreviousDatabase.Champions.Values)
            {
                if (CurrentDatabase_ref.Champions.ContainsKey(c.ID))
                {
                    if (!c.Equals(CurrentDatabase_ref.Champions[c.ID]))
                        DifferingChampions.Add(new DifferenceElement() { type = DifferenceElementType.CHANGED, id = c.ID });
                }
                else
                    DifferingChampions.Add(new DifferenceElement() { type = DifferenceElementType.REMOVED, id = c.ID });
            }
            foreach (Champion c in CurrentDatabase_ref.Champions.Values)
                if (!PreviousDatabase.Champions.ContainsKey(c.ID))
                    DifferingChampions.Add(new DifferenceElement() { type = DifferenceElementType.NEW, id = c.ID });

            // load abilities
            foreach (Ability a in PreviousDatabase.Abilities.Values)
            {
                if (CurrentDatabase_ref.Abilities.ContainsKey(a.ID))
                {
                    if (!a.Equals(CurrentDatabase_ref.Abilities[a.ID]))
                        DifferingAbilities.Add(new DifferenceElement() { type = DifferenceElementType.CHANGED, id = a.ID });
                }
                else
                    DifferingAbilities.Add(new DifferenceElement() { type = DifferenceElementType.REMOVED, id = a.ID });
            }
            foreach (Ability a in CurrentDatabase_ref.Abilities.Values)
                if (!PreviousDatabase.Abilities.ContainsKey(a.ID))
                    DifferingAbilities.Add(new DifferenceElement() { type = DifferenceElementType.NEW, id = a.ID });

            // load spells
            foreach (Spell s in PreviousDatabase.Spells.Values)
            {
                if (CurrentDatabase_ref.Spells.ContainsKey(s.ID))
                {
                    if (!s.Equals(CurrentDatabase_ref.Spells[s.ID]))
                        DifferingSpells.Add(new DifferenceElement() { type = DifferenceElementType.CHANGED, id = s.ID });
                }
                else
                    DifferingSpells.Add(new DifferenceElement() { type = DifferenceElementType.REMOVED, id = s.ID });
            }
            foreach (Spell s in CurrentDatabase_ref.Spells.Values)
                if (!PreviousDatabase.Spells.ContainsKey(s.ID))
                    DifferingSpells.Add(new DifferenceElement() { type = DifferenceElementType.NEW, id = s.ID });

            // load relics
            foreach (Relic r in PreviousDatabase.Relics.Values)
            {
                if (CurrentDatabase_ref.Relics.ContainsKey(r.ID))
                {
                    if (!r.Equals(CurrentDatabase_ref.Relics[r.ID]))
                        DifferingRelics.Add(new DifferenceElement() { type = DifferenceElementType.CHANGED, id = r.ID });
                }
                else
                    DifferingRelics.Add(new DifferenceElement() { type = DifferenceElementType.REMOVED, id = r.ID });
            }
            foreach (Relic r in CurrentDatabase_ref.Relics.Values)
                if (!PreviousDatabase.Relics.ContainsKey(r.ID))
                    DifferingRelics.Add(new DifferenceElement() { type = DifferenceElementType.NEW, id = r.ID });

            // load equipments
            foreach (Equipment e in PreviousDatabase.Equipments.Values)
            {
                if (CurrentDatabase_ref.Equipments.ContainsKey(e.ID))
                {
                    if (!e.Equals(CurrentDatabase_ref.Equipments[e.ID]))
                        DifferingEquipments.Add(new DifferenceElement() { type = DifferenceElementType.CHANGED, id = e.ID });
                }
                else
                    DifferingEquipments.Add(new DifferenceElement() { type = DifferenceElementType.REMOVED, id = e.ID });
            }
            foreach (Equipment e in CurrentDatabase_ref.Equipments.Values)
                if (!PreviousDatabase.Equipments.ContainsKey(e.ID))
                    DifferingEquipments.Add(new DifferenceElement() { type = DifferenceElementType.NEW, id = e.ID });
        }
    }
}
