using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace poxnora_search_engine
{
    public partial class ChampionBuilder : Form
    {
        Pox.Champion champion = new Pox.Champion();
        Pox.Champion loaded_champion_ref = null;

        public ChampionBuilder()
        {
            InitializeComponent();
        }

        private void LoadChampionNames()
        {
            HashSet<string> ChampionNameList = new HashSet<string>();
            foreach (var c in Program.database.Champions)
                ChampionNameList.Add(c.Value.Name);

            AutoCompleteStringCollection acsc = new AutoCompleteStringCollection();
            acsc.AddRange(ChampionNameList.ToArray());
            ChampionTemplate.AutoCompleteCustomSource = acsc;
            ChampionTemplate.AutoCompleteMode = AutoCompleteMode.Suggest;
            ChampionTemplate.AutoCompleteSource = AutoCompleteSource.CustomSource;
        }

        private void LoadAbilityNames()
        {
            AutoCompleteStringCollection acsc = new AutoCompleteStringCollection();
            acsc.AddRange(Program.database.AbilityNames.AllowedStrings.ToArray());
            TextAbility.AutoCompleteCustomSource = acsc;
            TextAbility.AutoCompleteMode = AutoCompleteMode.Suggest;
            TextAbility.AutoCompleteSource = AutoCompleteSource.CustomSource;
        }

        private void CalculateChampCost()
        {
            champion.CalculatePrognosedBaseNoraCostNew();

            int ab_cost = 0;
            foreach(var ab in champion.AllAbilities_refs)
                ab_cost += ab.NoraCost;

            TextStatsNoraCost.Text = "Nora cost: " + champion.PrognosedBaseNoraCost.ToString();
            TextAbilityNoraCost.Text = "Nora cost: " + ab_cost.ToString();
            TextEstimatedNoraCost.Text = "Total estimated nora cost: " + (champion.PrognosedBaseNoraCost + ab_cost).ToString();

            if (loaded_champion_ref != null)
                TextLoadedChampionNoraCost.Text = "Loaded champion nora cost: " + loaded_champion_ref.DefaultNoraCost.ToString();
            else
                TextLoadedChampionNoraCost.Text = "";
        }

        private void UpdateControls()
        {
            TextDamage.Text = champion.Damage.ToString();
            TextSpeed.Text = champion.Speed.ToString();
            TextMinRange.Text = champion.MinRNG.ToString();
            TextMaxRange.Text = champion.MaxRNG.ToString();
            TextDefense.Text = champion.Defense.ToString();
            TextHitPoints.Text = champion.HitPoints.ToString();
            TextSize.Text = champion.Size.ToString();

            ListAbilities.Items.Clear();
            foreach (var ab in champion.AllAbilities_refs)
                ListAbilities.Items.Add(ab.ToString() + " (" + ab.NoraCost + " nora)");



            CalculateChampCost();
        }

        private void Clear()
        {
            champion.Damage = 0;
            champion.Speed = 0;
            champion.MinRNG = 1;
            champion.MaxRNG = 1;
            champion.Defense = 0;
            champion.HitPoints = 0;
            champion.Size = 1;

            champion.AllAbilities_refs.Clear();

            loaded_champion_ref = null;

            UpdateControls();
        }

        private void ChampionBuilder_Load(object sender, EventArgs e)
        {
            TextAbilityNoraCost.Text = "";
            TextEstimatedNoraCost.Text = "";
            TextStatsNoraCost.Text = "";

            LoadChampionNames();
            LoadAbilityNames();

            Clear();
        }


        private void LoadFromChampions_Click(object sender, EventArgs e)
        {
            int champ_id = -1;
            foreach(var ch in Program.database.Champions)
            {
                if(ch.Value.Name == ChampionTemplate.Text)
                {
                    champ_id = ch.Key;
                    break;
                }
            }

            if (champ_id == -1)
                return;

            Pox.Champion base_champ = Program.database.Champions[champ_id];

            champion.Damage = base_champ.Damage;
            champion.Speed = base_champ.Speed;
            champion.MinRNG = base_champ.MinRNG;
            champion.MaxRNG = base_champ.MaxRNG;
            champion.Defense = base_champ.Defense;
            champion.HitPoints = base_champ.HitPoints;
            champion.Size = base_champ.Size;

            champion.AllAbilities_refs.Clear();

            foreach (var ab in base_champ.BaseAbilities_refs)
                champion.AllAbilities_refs.Add(ab);

            int up1 = base_champ.Upgrade1[base_champ.DefaultUpgrade1Index];
            champion.AllAbilities_refs.Add(Program.database.Abilities[up1]);

            int up2 = base_champ.Upgrade2[base_champ.DefaultUpgrade2Index];
            champion.AllAbilities_refs.Add(Program.database.Abilities[up2]);

            loaded_champion_ref = base_champ;

            UpdateControls();
        }

        private void ClearControls_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void TextDamage_Validated(object sender, EventArgs e)
        {
            int current = champion.Damage;
            if  (( !int.TryParse(TextDamage.Text, out champion.Damage))
                || (champion.Damage < 0) 
                || (champion.Damage > Pox.Champion.DamageLimit))
            {
                champion.Damage = current;
                TextDamage.Text = current.ToString();
                return;
            }

            UpdateControls();
        }

        private void TextSpeed_Validated(object sender, EventArgs e)
        {
            int current = champion.Speed;
            if ((!int.TryParse(TextSpeed.Text, out champion.Speed))
                || (champion.Speed < 0)
                || (champion.Speed > Pox.Champion.SpeedLimit))
            {
                champion.Speed = current;
                TextSpeed.Text = current.ToString();
                return;
            }

            UpdateControls();
        }

        private void TextMinRange_Validated(object sender, EventArgs e)
        {
            int current = champion.MinRNG;
            if ((!int.TryParse(TextMinRange.Text, out champion.MinRNG))
                ||(champion.MinRNG < 1)
                ||(champion.MinRNG > champion.MaxRNG)
                ||((champion.MaxRNG - champion.MinRNG) > Pox.Champion.RangeDifferenceLimit))
            {
                champion.MinRNG = current;
                TextMinRange.Text = current.ToString();
                return;
            }

            UpdateControls();
        }

        private void TextMaxRange_Validated(object sender, EventArgs e)
        {
            int current = champion.MaxRNG;
            if ((!int.TryParse(TextMaxRange.Text, out champion.MaxRNG))
                ||(champion.MaxRNG < champion.MinRNG)
                ||(champion.MaxRNG > Pox.Champion.MaxRangeLimit)
                ||((champion.MaxRNG - champion.MinRNG) > Pox.Champion.RangeDifferenceLimit))
            {
                champion.MaxRNG = current;
                TextMaxRange.Text = current.ToString();
                return;
            }

            UpdateControls();
        }

        private void TextDefense_Validated(object sender, EventArgs e)
        {
            int current = champion.Defense;
            if ((!int.TryParse(TextDefense.Text, out champion.Defense))
                ||(champion.Defense > Pox.Champion.DefenseLimit))
            {
                champion.Defense = current;
                TextDefense.Text = current.ToString();
                return;
            }

            UpdateControls();
        }

        private void TextHitPoints_Validated(object sender, EventArgs e)
        {
            int current = champion.HitPoints;
            if (!int.TryParse(TextHitPoints.Text, out champion.HitPoints))
            {
                TextHitPoints.Text = current.ToString();
                return;
            }

            UpdateControls();
        }

        private void TextSize_Validated(object sender, EventArgs e)
        {
            int current = champion.Size;
            if ((!int.TryParse(TextSize.Text, out champion.Size))
                ||(champion.Size < 1)
                ||(champion.Size > Pox.Champion.SizeLimit))
            {
                champion.Size = current;
                TextSize.Text = current.ToString();
                return;
            }

            UpdateControls();
        }

        private void AddAbility_Click(object sender, EventArgs e)
        {
            int ab_id = Program.database.GetAbilityIDByName(TextAbility.Text);
            if (ab_id == 0)
                return;

            champion.AllAbilities_refs.Add(Program.database.Abilities[ab_id]);

            UpdateControls();
        }

        private void RemoveAbility_Click(object sender, EventArgs e)
        {
            if (ListAbilities.SelectedIndex == -1)
                return;

            champion.AllAbilities_refs.RemoveAt(ListAbilities.SelectedIndex);

            UpdateControls();
        }

        private void ListAbilities_SelectedIndexChanged(object sender, EventArgs e)
        {
            RemoveAbility.Enabled = (ListAbilities.SelectedIndex != -1);

            if (ListAbilities.SelectedIndex == -1)
                return;

            Program.main_form.external_SetRuneDescriptionAbility(champion.AllAbilities_refs[ListAbilities.SelectedIndex]);
        }

        public void external_SetChampionTemplate(string c)
        {
            ChampionTemplate.Text = c;
        }
    }
}
