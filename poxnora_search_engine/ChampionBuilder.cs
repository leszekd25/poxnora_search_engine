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
            {
                TextLoadedChampionNoraCost.Text = "Loaded champion nora cost: " + loaded_champion_ref.DefaultNoraCost.ToString() + ", current nora modifier: " + loaded_champion_ref.PrognosedBaseNoraCostDifference.ToString();

                // calculation of minimum durability index
                float hp_min = champion.HitPoints;
                float def_min = champion.Defense;
                // find abilities that affect these values
                foreach(Pox.Ability ability in champion.AllAbilities_refs)
                {
                    if(ability.ClassName == "Tough")
                    {
                        def_min += ability.Level;
                    }
                    else if(ability.ClassName == "StatBonusDefense")
                    {
                        def_min += ability.Level;
                    }
                    else if(ability.ClassName == "StatBonusHealth")
                    {
                        hp_min += 2 + 2 * ability.Level;
                    }
                    else if(ability.ClassName == "Regeneration")
                    {
                        def_min += 1 * ability.Level;   // 2 heal points = 1 def, assuming hit twice
                    }
                    else if(ability.ClassName == "Blessed")
                    {
                        def_min += 2.5f;                
                    }
                    else if(ability.ClassName == "Loyalty")
                    {
                        hp_min += 5;
                    }
                    else if(ability.ClassName == "FragileDefense")
                    {
                        hp_min += 9;
                    }
                    else if(ability.ClassName == "LifeSiphon")
                    {
                        def_min += (champion.Damage - 2) / 3.2f;
                    }
                }
                
                LabelMinimumDurIndex.Text = $"Minimum durability index: {champion.CalculateBaseDurabilityIndex(hp_min, def_min, champion.PrognosedBaseNoraCost + ab_cost + loaded_champion_ref.PrognosedBaseNoraCostDifference)}";

                // calculation of maximum durability index
                // per damage range type: evasive and reflexes is counted exclusively (max of those)
                float range_dmg_reduction = 0; // 0 - 0%, 100 - 100%
                int hidden_instances = 0;
                float blocks_per_turn = 0;
                bool has_hunter = false;
                float ignores_per_120turns = 0;
                bool has_scale_armor = false;
                float incorporeal_amount = 0;
                float type_dmg_resistance = 0f; // 0 - 0%, 100 - 100%
                bool has_majestic = false;
                bool has_mirrored = false;
                bool has_absorb = false;


                foreach (Pox.Ability ability in champion.AllAbilities_refs)
                {
                    if ((ability.ClassName == "Evasive") || (ability.ClassName == "Reflexes"))
                    {
                        range_dmg_reduction = Math.Max(range_dmg_reduction, 5 + ability.Level * 15);
                    }
                    else if (ability.ClassName == "TreeForm")
                    {
                        def_min += 2;
                        hp_min += 10;
                    }
                    else if ((ability.ClassName.StartsWith("Domain"))&&(ability.UseCount != 0))
                    {
                        def_min += 1;
                    }
                    else if (ability.ClassName.Contains("Camouflage"))
                    {
                        hidden_instances += 1;
                    }
                    else if (ability.ClassName.Contains("Acolyte"))
                    {
                        def_min += 6;       // max 12 hp per round
                    }
                    else if ((ability.ClassName == "Dodge") || (ability.ClassName == "Block"))
                    {
                        blocks_per_turn += 0.5f * ability.Level;
                    }
                    else if(ability.ClassName.Contains("Deflect"))
                    {
                        blocks_per_turn += 0.25f;
                    }
                    else if(ability.ClassName == "OblivionShield")
                    {
                        blocks_per_turn += 1f;
                    }
                    else if(ability.ClassName == "NohkanDo")
                    {
                        blocks_per_turn += 2f;
                    }
                    else if(ability.ClassName == "ScaleArmor")
                    {
                        has_scale_armor = true;
                    }
                    else if((ability.ClassName.StartsWith("Hunter"))&&(ability.ClassName[6] != 's'))
                    {
                        has_hunter = true;
                    }
                    else if((ability.ClassName == "Dragonflight"))
                    {
                        has_hunter = true;
                    }
                    else if((ability.ClassName == "ProvisionDefense"))
                    {
                        def_min += 1;
                    }
                    else if(ability.ClassName == "Bold")
                    {
                        def_min += Math.Min(5, champion.Speed - 1);
                    }
                    else if((ability.ClassName == "Turtle")||(ability.ClassName == "TortunShell"))
                    {
                        ignores_per_120turns += 40;  // once per 3 turns
                    }
                    else if(ability.ClassName == "Charm")
                    {
                        ignores_per_120turns += 60;  // once per 2 turns
                    }
                    else if(ability.ClassName == "Fear")
                    {
                        ignores_per_120turns += 40;
                    }
                    else if(ability.ClassName == "HorrificAura")
                    {
                        ignores_per_120turns += ability.Level == 1 ? 24 : (ability.Level == 2 ? 30 : 40);
                    }
                    else if(ability.ClassName == "Abash")
                    {
                        ignores_per_120turns += 80;
                    }
                    else if(ability.ClassName == "Blinding")
                    {
                        ignores_per_120turns += 60;
                    }
                    else if(ability.ClassName == "RighteousShield")
                    {
                        ignores_per_120turns += 24;
                    }
                    else if(ability.ClassName == "PlantRegeneration")
                    {
                        def_min += 1 * ability.Level;   // 2 heal points = 1 def, assuming hit twice
                    }
                    else if((ability.ClassName == "CorporealForm")||(ability.ClassName == "Ghost"))
                    {
                        incorporeal_amount = 1;
                    }
                    else if(ability.ClassName == "Vaporize")
                    {
                        incorporeal_amount = Math.Max(incorporeal_amount, 0.75f);
                    }
                    else if(ability.ClassName == "Ethereal")
                    {
                        incorporeal_amount = Math.Max(incorporeal_amount, 0.5f);
                    }
                    else if(ability.ClassName == "PhaseShift")
                    {
                        incorporeal_amount = Math.Max(incorporeal_amount, 0.3333f);
                    }
                    else if(ability.ClassName == "Intensify")
                    {
                        hp_min += 20;
                    }
                    else if (ability.ClassName.StartsWith("Resistance"))
                    {
                        type_dmg_resistance = Math.Max(type_dmg_resistance, 5 + ability.Level * 15);
                    }
                    else if(ability.ClassName == "ChaosShield")
                    {
                        type_dmg_resistance = 50;
                    }
                    else if (ability.ClassName == "TwistedEssence")
                    {
                        def_min += 2.5f;
                    }
                    else if(ability.ClassName == "TrailHallowedGround")
                    {
                        def_min += 2.5f;
                    }
                    else if(ability.ClassName == "MeleeSpecialist")
                    {
                        def_min += 1 * ability.Level;
                    }
                    else if(ability.ClassName == "Dread")
                    {
                        def_min += 2;
                    }
                    else if(ability.ClassName == "RelicGuardian")
                    {
                        def_min += 2;
                    }
                    else if(ability.ClassName == "Calcify")
                    {
                        def_min += 3;
                    }
                    else if(ability.ClassName == "Majestic")
                    {
                        has_majestic = true;
                    }
                    else if (ability.ClassName == "Ascendance")
                    {
                        hp_min += 30;
                        def_min += 2;
                    }
                    else if(ability.ClassName == "Grow")
                    {
                        hp_min += 60;
                    }
                    else if(ability.ClassName == "Mirrored")
                    {
                        has_mirrored = true;
                    }
                    else if(ability.ClassName.StartsWith("Quest"))
                    {
                        if (ability.ClassName == "QuestCompleteTraining")
                        {
                            hp_min += 10;
                        }
                        else if ((ability.ClassName == "QuestVisions") || (ability.ClassName == "QuestKillEnemy"))
                        {
                            hp_min += 25;
                            def_min += 1;
                        }
                        else if(ability.ClassName == "QuestRunner")
                        {
                            hp_min += 16;
                        }
                        else if(ability.ClassName == "QuestSupportAlly")
                        {
                            def_min += 4.5f;
                        }
                    }
                    else if(ability.ClassName == "HealingMass")
                    {
                        def_min += 1.5f;    // 3 per turn
                    }
                    else if((ability.ClassName == "HealSelf")||(ability.ClassName == "HealingSingle"))
                    {
                        def_min += 2;       // 4 per turn
                    }
                    else if(ability.ClassName == "Absorb")
                    {
                        has_absorb = true;
                    }
                    else if(ability.ClassName == "Masochist")
                    {
                        def_min += 2;
                    }
                    else if(ability.ClassName.StartsWith("Ally"))
                    {
                        hp_min += 7;
                    }
                }
                ignores_per_120turns = Math.Min(119, ignores_per_120turns);
                blocks_per_turn = Utility.Sigmoid(2, blocks_per_turn);

                hp_min /= 1 - (range_dmg_reduction / 100);
                hp_min /= 1 - (type_dmg_resistance / 100);
                if(has_majestic)
                {
                    hp_min *= 2;
                }
                if(has_mirrored)
                {
                    hp_min *= 2;
                }
                if(has_absorb)
                {
                    hp_min *= 2;
                }
                def_min += hidden_instances * 2;
                if(has_hunter)
                {
                    def_min += 2;
                }
                if(has_scale_armor)
                {
                    hp_min /= 0.75f;
                }
                hp_min /= (1 - (ignores_per_120turns / 120.0f));
                hp_min *= (2 / (2 - blocks_per_turn));
                hp_min *= (1 + 3 * incorporeal_amount);

                LabelMaximumDurIndex.Text = $"Maximum durability index: {champion.CalculateBaseDurabilityIndex(hp_min, def_min, champion.PrognosedBaseNoraCost + ab_cost + loaded_champion_ref.PrognosedBaseNoraCostDifference)}";
            }
            else
            {
                TextLoadedChampionNoraCost.Text = "";
                LabelMinimumDurIndex.Text = "";
                LabelMaximumDurIndex.Text = "";
            }

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
            int champ_id = Utility.NO_INDEX;
            foreach (var ch in Program.database.Champions)
            {
                if(ch.Value.Name == ChampionTemplate.Text)
                {
                    champ_id = ch.Key;
                    break;
                }
            }

            if (champ_id == Utility.NO_INDEX)
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
