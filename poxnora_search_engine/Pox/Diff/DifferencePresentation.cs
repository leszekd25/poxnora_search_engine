using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace poxnora_search_engine.Pox.Diff
{
    public class DifferencePresentation
    {
        public Panel PanelRef = null;
        public Button ButtonPreviousRef = null;
        public Button ButtonCurrentRef = null;
        public DatabaseDifferenceCalculator DiffCalculatorRef = null;
        Font ChangeInfoFont = new Font("Arial", 10);

        public void ClearChangeInfo()
        {
            PanelRef.Controls.Clear();
        }

        private void PushChangeInfo(string info)
        {
            Point previous_location = new Point(3, 3);
            int previous_height = 0;
            if (PanelRef.Controls.Count != 0)
            {
                previous_location = PanelRef.Controls[PanelRef.Controls.Count - 1].Location;
                previous_height = PanelRef.Controls[PanelRef.Controls.Count - 1].Size.Height;
            }

            PanelRef.Controls.Add(new Label()
            {
                Location = new Point(previous_location.X, previous_location.Y + previous_height + 5),
                Text = info,
                MaximumSize = new Size(PanelRef.Width - 32, 0),
                AutoSize = true,
                Font = ChangeInfoFont
            });
        }

        private void LoadDataElementChangeInfo(Pox.DataElement prev_elem, Pox.DataElement curr_elem)
        {
            int prev_changes = PanelRef.Controls.Count;

            if (prev_elem.Name != curr_elem.Name)
                PushChangeInfo(string.Format("Name changed from {0} to {1}", prev_elem.Name, curr_elem.Name));
            if (prev_elem.Description != curr_elem.Description)
                PushChangeInfo(string.Format("Description changed\r\nOld:\r\n    {0}\r\nNew:\r\n    {1}", prev_elem.Description, curr_elem.Description));
            if (!(prev_elem is Pox.Champion))
            {
                if (prev_elem.NoraCost < curr_elem.NoraCost)
                    PushChangeInfo(string.Format("Nora cost increased by {0}", curr_elem.NoraCost - prev_elem.NoraCost));
                if (prev_elem.NoraCost > curr_elem.NoraCost)
                    PushChangeInfo(string.Format("Nora cost decreased by {0}", prev_elem.NoraCost - curr_elem.NoraCost));
            }
            else
            {
                if (((Pox.Champion)prev_elem).BaseNoraCost < ((Pox.Champion)curr_elem).BaseNoraCost)
                    PushChangeInfo(string.Format("Nora cost increased by {0}", ((Pox.Champion)curr_elem).BaseNoraCost - ((Pox.Champion)prev_elem).BaseNoraCost));
                if (((Pox.Champion)prev_elem).BaseNoraCost > ((Pox.Champion)curr_elem).BaseNoraCost)
                    PushChangeInfo(string.Format("Nora cost decreased by {0}", ((Pox.Champion)prev_elem).BaseNoraCost - ((Pox.Champion)curr_elem).BaseNoraCost));
            }

            if (prev_changes != PanelRef.Controls.Count)
                PushChangeInfo("");
        }

        private void LoadRuneChangeInfo(Pox.Rune prev_rune, Pox.Rune curr_rune)
        {
            LoadDataElementChangeInfo(prev_rune, curr_rune);

            int prev_changes = PanelRef.Controls.Count;

            if (prev_rune.Rarity != curr_rune.Rarity)
                PushChangeInfo(string.Format("Rarity changed from {0} to {1}", prev_rune.Rarity, curr_rune.Rarity));
            foreach (string f in prev_rune.Faction)
                if (!(curr_rune.Faction.Contains(f)))
                    PushChangeInfo(string.Format("Faction removed: {0}", f));
            foreach (string f in curr_rune.Faction)
                if (!(prev_rune.Faction.Contains(f)))
                    PushChangeInfo(string.Format("Faction added: {0}", f));
            if (prev_rune.ForSale != curr_rune.ForSale)
                PushChangeInfo(prev_rune.ForSale ? "No longer for sale" : "Now for sale");
            if (prev_rune.Tradeable != curr_rune.Tradeable)
                PushChangeInfo(prev_rune.Tradeable ? "No longer tradeable" : "Now tradeable");
            if (prev_rune.AllowRanked != curr_rune.AllowRanked)
                PushChangeInfo(prev_rune.AllowRanked ? "Now banned from ranked play" : "No longer banned from ranked play");
            if (prev_rune.DeckLimit != curr_rune.DeckLimit)
                PushChangeInfo(string.Format("Deck limit changed from {0} to {1}", prev_rune.DeckLimit, curr_rune.DeckLimit));
            if (prev_rune.Cooldown != curr_rune.Cooldown)
                PushChangeInfo(string.Format("Cooldown changed from {0} to {1}", prev_rune.Cooldown, curr_rune.Cooldown));

            if (prev_changes != PanelRef.Controls.Count)
                PushChangeInfo("");
        }

        private void LoadChampionChangeInfo(Pox.Champion prev_champion, Pox.Champion curr_champion)
        {
            LoadRuneChangeInfo(prev_champion, curr_champion);

            if ((prev_champion.MinRNG != curr_champion.MinRNG) || (prev_champion.MaxRNG != curr_champion.MaxRNG))
                PushChangeInfo(string.Format("Range changed from {0}-{1} to {2}-{3}", prev_champion.MinRNG, prev_champion.MaxRNG, curr_champion.MinRNG, curr_champion.MaxRNG));
            if (prev_champion.Defense != curr_champion.Defense)
                PushChangeInfo(string.Format("Defense changed from {0} to {1}", prev_champion.Defense, curr_champion.Defense));
            if (prev_champion.HitPoints != curr_champion.HitPoints)
                PushChangeInfo(string.Format("Hit points changed from {0} to {1}", prev_champion.HitPoints, curr_champion.HitPoints));
            if (prev_champion.Damage != curr_champion.Damage)
                PushChangeInfo(string.Format("Damage changed from {0} to {1}", prev_champion.Damage, curr_champion.Damage));
            if (prev_champion.Speed != curr_champion.Speed)
                PushChangeInfo(string.Format("Speed changed from {0} to {1}", prev_champion.Speed, curr_champion.Speed));
            if (prev_champion.Size != curr_champion.Size)
                PushChangeInfo(string.Format("Size changed from {0}x{1} to {2}x{3}", prev_champion.Size, prev_champion.Size, curr_champion.Size, curr_champion.Size));
            foreach (string c in prev_champion.Class)
                if (!(curr_champion.Class.Contains(c)))
                    PushChangeInfo(string.Format("Class removed: {0}", c));
            foreach (string c in curr_champion.Class)
                if (!(prev_champion.Class.Contains(c)))
                    PushChangeInfo(string.Format("Class added: {0}", c));
            foreach (string r in prev_champion.Race)
                if (!(curr_champion.Race.Contains(r)))
                    PushChangeInfo(string.Format("Race removed: {0}", r));
            foreach (string r in curr_champion.Race)
                if (!(prev_champion.Race.Contains(r)))
                    PushChangeInfo(string.Format("Race added: {0}", r));
            foreach (int a in prev_champion.Abilities)
                if (!(curr_champion.Abilities.Contains(a)))
                    PushChangeInfo(string.Format("Ability removed: {0}", DiffCalculatorRef.PreviousDatabase.Abilities[a].ToString()));
            foreach (int a in curr_champion.Abilities)
                if (!(prev_champion.Abilities.Contains(a)))
                    PushChangeInfo(string.Format("Ability added: {0}", DiffCalculatorRef.CurrentDatabase_ref.Abilities[a].ToString()));

            bool upgrade1_changed = false;
            foreach (int a in prev_champion.Upgrade1)
                if (!(curr_champion.Upgrade1.Contains(a)))
                {
                    upgrade1_changed = true;
                    break;
                }
            foreach (int a in curr_champion.Upgrade1)
                if (!(prev_champion.Upgrade1.Contains(a)))
                {
                    upgrade1_changed = true;
                    break;
                }
            if (upgrade1_changed)
            {
                StringBuilder sb = new StringBuilder("Upgrade 1 changed\r\nOld:\r\n");
                List<int> upg1_sorted = new List<int>(prev_champion.Upgrade1); upg1_sorted.Sort();
                foreach (int a in upg1_sorted)
                    sb.Append(string.Format("{0}{1}\r\n", DiffCalculatorRef.PreviousDatabase.Abilities[a].ToString(), prev_champion.IsUpgrade1Default(a) ? " (default)" : ""));
                sb.Append("New:\r\n");
                upg1_sorted = new List<int>(curr_champion.Upgrade1); upg1_sorted.Sort();
                foreach (int a in upg1_sorted)
                    sb.Append(string.Format("{0}{1}\r\n", DiffCalculatorRef.CurrentDatabase_ref.Abilities[a].ToString(), curr_champion.IsUpgrade1Default(a) ? " (default)" : ""));

                PushChangeInfo(sb.ToString());
            }
            else
            {
                if (prev_champion.Upgrade1[prev_champion.DefaultUpgrade1Index] != curr_champion.Upgrade1[curr_champion.DefaultUpgrade1Index])
                    PushChangeInfo(string.Format("Default upgrade 1 changed from {0} to {1}",
                        DiffCalculatorRef.PreviousDatabase.Abilities[prev_champion.Upgrade1[prev_champion.DefaultUpgrade1Index]].ToString(),
                        DiffCalculatorRef.CurrentDatabase_ref.Abilities[curr_champion.Upgrade1[curr_champion.DefaultUpgrade1Index]].ToString()));
            }

            bool upgrade2_changed = false;
            foreach (int a in prev_champion.Upgrade2)
                if (!(curr_champion.Upgrade2.Contains(a)))
                {
                    upgrade2_changed = true;
                    break;
                }
            foreach (int a in curr_champion.Upgrade2)
                if (!(prev_champion.Upgrade2.Contains(a)))
                {
                    upgrade2_changed = true;
                    break;
                }
            if (upgrade2_changed)
            {
                StringBuilder sb = new StringBuilder("Upgrade 2 changed\r\nOld:\r\n");
                List<int> upg2_sorted = new List<int>(prev_champion.Upgrade2); upg2_sorted.Sort();
                foreach (int a in upg2_sorted)
                    sb.Append(string.Format("{0}{1}\r\n", DiffCalculatorRef.PreviousDatabase.Abilities[a].ToString(), prev_champion.IsUpgrade2Default(a) ? " (default)" : ""));
                sb.Append("New:\r\n");
                upg2_sorted = new List<int>(curr_champion.Upgrade2); upg2_sorted.Sort();
                foreach (int a in upg2_sorted)
                    sb.Append(string.Format("{0}{1}\r\n", DiffCalculatorRef.CurrentDatabase_ref.Abilities[a].ToString(), curr_champion.IsUpgrade2Default(a) ? " (default)" : ""));

                PushChangeInfo(sb.ToString());
            }
            else
            {
                if (prev_champion.Upgrade2[prev_champion.DefaultUpgrade2Index] != curr_champion.Upgrade2[curr_champion.DefaultUpgrade2Index])
                    PushChangeInfo(string.Format("Default upgrade 2 changed from {0} to {1}",
                        DiffCalculatorRef.PreviousDatabase.Abilities[prev_champion.Upgrade2[prev_champion.DefaultUpgrade2Index]].ToString(),
                        DiffCalculatorRef.CurrentDatabase_ref.Abilities[curr_champion.Upgrade2[curr_champion.DefaultUpgrade2Index]].ToString()));
            }

            // special: calculate new prognosed nora cost difference
            if (prev_champion.PrognosedBaseNoraCostDifference != curr_champion.PrognosedBaseNoraCostDifference)
                PushChangeInfo(string.Format("Prognosed nora cost difference changed from {0} to {1}",
                    prev_champion.PrognosedBaseNoraCostDifference, curr_champion.PrognosedBaseNoraCostDifference));
        }

        private void LoadAbilityChangeInfo(Pox.Ability prev_ability, Pox.Ability curr_ability)
        {
            LoadDataElementChangeInfo(prev_ability, curr_ability);

            if (prev_ability.APCost != curr_ability.APCost)
                PushChangeInfo(string.Format("AP cost changed from {0} to {1}", prev_ability.APCost, curr_ability.APCost));
            if (prev_ability.Cooldown != curr_ability.Cooldown)
                PushChangeInfo(string.Format("Cooldown changed from {0} to {1}", prev_ability.Cooldown, curr_ability.Cooldown));
        }

        private void LoadSpellChangeInfo(Pox.Spell prev_spell, Pox.Spell curr_spell)
        {
            LoadRuneChangeInfo(prev_spell, curr_spell);

            if (prev_spell.Flavor != curr_spell.Flavor)
                PushChangeInfo(string.Format("Flavor text changed\r\nOld:\r\n    {0}\r\nNew:\r\n    {1}", prev_spell.Flavor, curr_spell.Flavor));
        }

        private void LoadEquipmentChangeInfo(Pox.Equipment prev_equipment, Pox.Equipment curr_equipment)
        {
            LoadRuneChangeInfo(prev_equipment, curr_equipment);

            if (prev_equipment.Flavor != curr_equipment.Flavor)
                PushChangeInfo(string.Format("Flavor text changed\r\nOld:\r\n    {0}\r\nNew:\r\n    {1}", prev_equipment.Flavor, curr_equipment.Flavor));
        }

        private void LoadRelicChangeInfo(Pox.Relic prev_relic, Pox.Relic curr_relic)
        {
            LoadRuneChangeInfo(prev_relic, curr_relic);

            if (prev_relic.Defense != curr_relic.Defense)
                PushChangeInfo(string.Format("Defense changed from {0} to {1}", prev_relic.Defense, curr_relic.Defense));
            if (prev_relic.HitPoints != curr_relic.HitPoints)
                PushChangeInfo(string.Format("Hit points changed from {0} to {1}", prev_relic.HitPoints, curr_relic.HitPoints));
            if (prev_relic.Size != curr_relic.Size)
                PushChangeInfo(string.Format("Size changed from {0}x{1} to {2}x{2}", prev_relic.Size, prev_relic.Size, curr_relic.Size, curr_relic.Size));
            if (prev_relic.Flavor != curr_relic.Flavor)
                PushChangeInfo(string.Format("Flavor text changed\r\nOld:\r\n    {0}\r\nNew:\r\n    {1}", prev_relic.Flavor, curr_relic.Flavor));
        }


        public void LoadChangeInfo(Pox.Diff.DifferenceLink diff_link)
        {
            ButtonPreviousRef.Show();
            ButtonCurrentRef.Show();

            Pox.DataElement prev = diff_link.PreviousElement;
            Pox.DataElement curr = diff_link.CurrentElement;

            if (prev == null)
            {
                if (curr == null)
                {
                    PanelRef.Controls.Add(new Label() { Text = "<!!!INVALID!!!>" });
                    ButtonCurrentRef.Hide();
                }
                else
                {
                    PanelRef.Controls.Add(new Label() { Text = "Added" });
                }

                ButtonPreviousRef.Hide();
                return;
            }
            else if (curr == null)
            {
                PanelRef.Controls.Add(new Label() { Text = "Removed" });
                ButtonCurrentRef.Hide();
                return;
            }

            // load data element change info
            switch (diff_link.ElemType)
            {
                case Pox.DataElement.ElementType.CHAMPION:
                    LoadChampionChangeInfo((Pox.Champion)prev, (Pox.Champion)curr);
                    break;
                case Pox.DataElement.ElementType.ABILITY:
                    LoadAbilityChangeInfo((Pox.Ability)prev, (Pox.Ability)curr);
                    break;
                case Pox.DataElement.ElementType.SPELL:
                    LoadSpellChangeInfo((Pox.Spell)prev, (Pox.Spell)curr);
                    break;
                case Pox.DataElement.ElementType.EQUIPMENT:
                    LoadEquipmentChangeInfo((Pox.Equipment)prev, (Pox.Equipment)curr);
                    break;
                case Pox.DataElement.ElementType.RELIC:
                    LoadRelicChangeInfo((Pox.Relic)prev, (Pox.Relic)curr);
                    break;
                default:
                    break;
            }
        }
    }
}
