using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace poxnora_search_engine.Pox
{
    public partial class RuneDescriptionControl : UserControl, IImageCacheSubscriber
    {
        public delegate void OnUpgradeClicked(int up_id, int up_index);

        struct TracerViewData
        {
            public Pox.DataElement.ElementType Type;
            public int ID;
        }

        public struct UpgradeAbilitySelection
        {
            public int AbilityIndex1;
            public int AbilityIndex2;
        }

        struct AbilityLinkData
        {
            public int UpgradeID;    // u1, u2
            public int UpgradeIndex; // which ability from the list was selected
        }

        static Font RegularFont = new Font("Arial", 10, FontStyle.Regular);
        static Font BoldFont = new Font("Arial", 10, FontStyle.Bold);
        static Font ItalicFont = new Font("Arial", 10, FontStyle.Italic);

        List<TracerViewData> Tracer = new List<TracerViewData>();
        int TracerPosition = -1;
        TracerViewData CurrentTracer;

        Champion SelectedChampion = null;
        public UpgradeAbilitySelection AbilitySelection = new UpgradeAbilitySelection() { AbilityIndex1 = 0, AbilityIndex2 = 0 };

        public Database database_ref = null;

        public OnUpgradeClicked UpgradeClicked = null;

        public RuneDescriptionControl()
        {
            InitializeComponent();
        }

        private Color GetColorByRarity(string rarity)
        {
            switch(rarity.ToLower())
            {
                case "common":
                    return Color.Gold;
                case "uncommon":
                    return Color.Red;
                case "rare":
                    return Color.DodgerBlue;
                case "exotic":
                    return Color.MediumOrchid;
                case "legendary":
                    return Color.Green;
                case "limited":
                    return Color.White;
                default:
                    return Color.LightGray;
            }
        }

        private Color GetColorByAttackType (string a_type)
        {
            switch(a_type.ToLower())
            {
                case "acid":
                    return Color.PaleGreen;
                case "disease":
                    return Color.Goldenrod;
                case "electricity":
                case "mouth lightning":
                case "red electricity":
                case "toe lightning":
                    return Color.DeepSkyBlue;
                case "fire":
                    return Color.DarkOrange;
                case "frost":
                    return Color.LightCyan;
                case "loss of life":
                    return Color.SlateBlue;
                case "magical":
                    return Color.DeepPink;
                case "poison":
                    return Color.Lime;
                case "psychic":
                    return Color.LightPink;
                case "sonic":
                    return Color.Yellow;
                default:
                    return Color.LightGray;
            }
        }

        public void ClearDescription()
        {
            RuneImage.Image = null;
            RuneImage.Hide();

            foreach (Control abc in PanelAbilities.Controls)
            {
                if (abc is AbilityControl)
                    Program.image_cache.RemoveAbilityImageSubscriber((AbilityControl)abc);
                if (abc is AbilitySelectionControl)
                    ((AbilitySelectionControl)abc).ClearAbilities();
            }
            PanelAbilities.Controls.Clear();
            PanelAbilities.Hide();
            PanelAbilities.Height = this.Height - PanelAbilities.Location.Y - 3;

            PanelChampionData.Hide();

            RTFLabelRuneInfo.Clear();
            RTFLabelRuneInfo.Location = RuneImage.Location;

            RTFLabelDescription.Clear();
            RTFLabelDescription.Hide();
            RTFLabelDescription.Location = new Point(PanelChampionData.Location.X, PanelChampionData.Location.Y + PanelChampionData.Height + 3);
            RTFLabelDescription.Height = PanelAbilities.Height;
        }

        private void SetRune(Rune r)
        {
            ClearDescription();

            RuneImage.Show();
            RTFLabelRuneInfo.Location = new Point(RuneImage.Location.X + RuneImage.Width + 3, RuneImage.Location.Y);

            Program.image_cache.LoadRuneImage(r.Hash);

            RTFLabelRuneInfo.SelectionColor = GetColorByRarity(r.Rarity);
            RTFLabelRuneInfo.SelectionFont = BoldFont;
            RTFLabelRuneInfo.AppendText(r.Name + "\r\n");

            RTFLabelRuneInfo.SelectionColor = Color.LightGray;
            AddLine(RTFLabelRuneInfo, "Expansion: ", r.Expansion);

            string faction = "";
            for (int i = 0; i < r.Faction.Count - 1; i++)
                faction += r.Faction[i] + ", ";
            if (r.Faction.Count != 0)
                faction += r.Faction[r.Faction.Count - 1];
            AddLine(RTFLabelRuneInfo, "Faction: ", faction);

            AddLine(RTFLabelRuneInfo, "Deck limit: ", r.DeckLimit.ToString());
            AddLine(RTFLabelRuneInfo, "", "");

            RTFLabelRuneInfo.SelectionColor = Color.LightGray;
            RTFLabelRuneInfo.SelectionFont = ItalicFont;
            RTFLabelRuneInfo.AppendText("Illustrated by  " + r.Artist);
        }

        public void SetChampionRune(Champion c, int u1_index, int u2_index)
        {
            SetRune(c);
            SelectedChampion = c;
            AbilitySelection = new UpgradeAbilitySelection() { AbilityIndex1 = u1_index, AbilityIndex2 = u2_index };

            PanelChampionData.Show();

            string cl = "";
            for (int i = 0; i < c.Class.Count - 1; i++)
                cl += c.Class[i] + ", ";
            if (c.Class.Count != 0)
                cl += c.Class[c.Class.Count - 1];
            LabelChampClass.Text = "Class: " + cl;

            string race = "";
            for (int i = 0; i < c.Race.Count - 1; i++)
                race += c.Race[i] + ", ";
            if (c.Race.Count != 0)
                race += c.Race[c.Race.Count - 1];
            LabelChampRace.Text = "Race: " + race;

            PanelAbilities.Show();

            AbilityControl abc;
            int ab_index = 0;
            foreach(var ab in c.BaseAbilities_refs)
            {
                abc = new AbilityControl();
                PanelAbilities.Controls.Add(abc);
                abc.Location = new Point(0, ab_index * abc.Height);
                abc.SetAbility(ab);
                abc.SetColor(Color.LightGray);
                abc.PictureBoxAbility.Tag = new TracerViewData() { Type = DataElement.ElementType.ABILITY, ID = ab.ID };
                abc.PictureBoxAbility.MouseDown += OnAbilityClick;
                Program.image_cache.AddAbilityImageSubscriber(ab.IconName, abc);
                ab_index += 1;
            }

            int NoraCost = c.NoraCost;
            // upgrade 1
            abc = new AbilityControl();
            foreach (var ab in c.UpgradeAbilities1_refs)
            {
                if (c.UpgradeAbilities1_refs.IndexOf(ab) == AbilitySelection.AbilityIndex1)
                {
                    PanelAbilities.Controls.Add(abc);
                    abc.Tag = 1;
                    abc.Location = new Point(0, ab_index * abc.Height);
                    abc.SetAbility(ab);
                    abc.SetColor(Color.DeepSkyBlue);
                    abc.PictureBoxAbility.Tag = new TracerViewData() { Type = DataElement.ElementType.ABILITY, ID = ab.ID };
                    abc.PictureBoxAbility.MouseDown += OnUpgradeAbilityClick;
                    NoraCost += ab.NoraCost;
                    Program.image_cache.AddAbilityImageSubscriber(ab.IconName, abc);
                    ab_index += 1;
                }
            }

            // upgrade 2
            abc = new AbilityControl();
            foreach (var ab in c.UpgradeAbilities2_refs)
            {
                if (c.UpgradeAbilities2_refs.IndexOf(ab) == AbilitySelection.AbilityIndex2)
                {
                    PanelAbilities.Controls.Add(abc);
                    abc.Tag = 2;
                    abc.Location = new Point(0, ab_index * abc.Height);
                    abc.SetAbility(ab);
                    abc.SetColor(Color.LimeGreen);
                    abc.PictureBoxAbility.Tag = new TracerViewData() { Type = DataElement.ElementType.ABILITY, ID = ab.ID };
                    abc.PictureBoxAbility.MouseDown += OnUpgradeAbilityClick;
                    NoraCost += ab.NoraCost;
                    Program.image_cache.AddAbilityImageSubscriber(ab.IconName, abc);
                    ab_index += 1;
                }
            }

            PanelAbilities.Height = 35 * ab_index;

            LabelData1.Text = string.Format("{0}\r\n{1}\r\n{2}\r\n\r\n{3}",
                c.Damage, c.MinRNG, c.MaxRNG, c.Size);
            LabelData2.Text = string.Format("{0}\r\n{1}\r\n{2}\r\n\r\n{3}",
                c.Speed, c.Defense, c.HitPoints, NoraCost);

            RTFLabelDescription.Show();
            RTFLabelDescription.Height = 150;
            RTFLabelDescription.Location = new Point(PanelAbilities.Location.X, PanelAbilities.Location.Y + PanelAbilities.Height + 3);

            RTFLabelDescription.SelectionColor = Color.LightGray;
            RTFLabelDescription.SelectionFont = ItalicFont;
            RTFLabelDescription.AppendText(c.Description);
        }

        public void OnAbilityClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
                TracerGoTo((TracerViewData)((PictureBox)sender).Tag);
        }

        public void OnUpgradeAbilityClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                AbilityControl abc = (AbilityControl)(((PictureBox)sender).Parent);

                AbilitySelectionControl asc = new AbilitySelectionControl();
                PanelAbilities.Controls.Add(asc);

                List<Ability> abis = new List<Ability>();
                int sel_ab = 0;
                if ((int)(abc.Tag) == 1)
                {
                    foreach (var ab in SelectedChampion.UpgradeAbilities1_refs)
                        abis.Add(ab);
                    asc.Tag = 1;
                    sel_ab = AbilitySelection.AbilityIndex1;
                }
                else if ((int)(abc.Tag) == 2)
                {
                    foreach (var ab in SelectedChampion.UpgradeAbilities2_refs)
                        abis.Add(ab);
                    asc.Tag = 2;
                    sel_ab = AbilitySelection.AbilityIndex2;
                }
                else
                {
                    PanelAbilities.Controls.Remove(asc);
                    return;
                }

                asc.SetAbilities(abis);
                asc.SetSelectedAbilitiy(sel_ab);
                asc.AbilitySelected = OnAbilitySelected;
                asc.BringToFront();
                asc.Location = new Point(abc.Location.X, Math.Max(0, Math.Min(PanelAbilities.Height - asc.Height, abc.Location.Y - asc.Height + abc.Height)));
            }
            else if(e.Button == MouseButtons.Right)
            {
                TracerGoTo((TracerViewData)((PictureBox)sender).Tag);
            }
        }

        public void OnAbilitySelected(int up_index, int abi_index)
        {
            bool changed = false;
            if (up_index == 1)
            {
                if (abi_index != AbilitySelection.AbilityIndex1)
                {
                    changed = true;
                    AbilitySelection.AbilityIndex1 = abi_index;
                }
            }
            else if (up_index == 2)
            {
                if (abi_index != AbilitySelection.AbilityIndex2)
                {
                    changed = true;
                    AbilitySelection.AbilityIndex2 = abi_index;
                }
            }

            if (!changed)
                return;

            foreach (Control abc2 in PanelAbilities.Controls)
            {
                if (abc2 is AbilityControl)
                    Program.image_cache.RemoveAbilityImageSubscriber((AbilityControl)abc2);
                if (abc2 is AbilitySelectionControl)
                    ((AbilitySelectionControl)abc2).ClearAbilities();
            }
            PanelAbilities.Controls.Clear();
            int NoraCost = SelectedChampion.NoraCost;

            AbilityControl abc;
            int ab_index = 0;
            foreach (var ab in SelectedChampion.BaseAbilities_refs)
            {
                abc = new AbilityControl();
                PanelAbilities.Controls.Add(abc);
                abc.Location = new Point(0, ab_index * abc.Height);
                abc.SetAbility(ab);
                abc.SetColor(Color.LightGray);
                abc.PictureBoxAbility.Tag = new TracerViewData() { Type = DataElement.ElementType.ABILITY, ID = ab.ID };
                abc.PictureBoxAbility.MouseDown += OnAbilityClick;
                Program.image_cache.AddAbilityImageSubscriber(ab.IconName, abc);
                ab_index += 1;
            }

            // upgrade 1
            abc = new AbilityControl();
            foreach (var ab in SelectedChampion.UpgradeAbilities1_refs)
            {
                if (SelectedChampion.UpgradeAbilities1_refs.IndexOf(ab) == AbilitySelection.AbilityIndex1)
                {
                    PanelAbilities.Controls.Add(abc);
                    abc.Tag = 1;
                    abc.Location = new Point(0, ab_index * abc.Height);
                    abc.SetAbility(ab);
                    abc.SetColor(Color.DeepSkyBlue);
                    abc.PictureBoxAbility.Tag = new TracerViewData() { Type = DataElement.ElementType.ABILITY, ID = ab.ID };
                    abc.PictureBoxAbility.MouseDown += OnUpgradeAbilityClick;
                    NoraCost += ab.NoraCost;
                    Program.image_cache.AddAbilityImageSubscriber(ab.IconName, abc);
                    ab_index += 1;
                }
            }

            // upgrade 2
            abc = new AbilityControl();
            foreach (var ab in SelectedChampion.UpgradeAbilities2_refs)
            {
                if (SelectedChampion.UpgradeAbilities2_refs.IndexOf(ab) == AbilitySelection.AbilityIndex2)
                {
                    PanelAbilities.Controls.Add(abc);
                    abc.Tag = 2;
                    abc.Location = new Point(0, ab_index * abc.Height);
                    abc.SetAbility(ab);
                    abc.SetColor(Color.LimeGreen);
                    abc.PictureBoxAbility.Tag = new TracerViewData() { Type = DataElement.ElementType.ABILITY, ID = ab.ID };
                    abc.PictureBoxAbility.MouseDown += OnUpgradeAbilityClick;
                    NoraCost += ab.NoraCost;
                    Program.image_cache.AddAbilityImageSubscriber(ab.IconName, abc);
                    ab_index += 1;
                }
            }

            LabelData1.Text = string.Format("{0}\r\n{1}\r\n{2}\r\n\r\n{3}",
                SelectedChampion.Damage, SelectedChampion.MinRNG, SelectedChampion.MaxRNG, SelectedChampion.Size);
            LabelData2.Text = string.Format("{0}\r\n{1}\r\n{2}\r\n\r\n{3}",
                SelectedChampion.Speed, SelectedChampion.Defense, SelectedChampion.HitPoints, NoraCost);

            UpgradeClicked?.Invoke(up_index, abi_index);
        }

        public void SetAbility(Ability a)
        {
            ClearDescription();

            RTFLabelRuneInfo.SelectionColor = Color.LightGray;
            RTFLabelRuneInfo.SelectionFont = BoldFont;
            RTFLabelRuneInfo.AppendText(a.ToString() + "\r\n");

            RTFLabelDescription.Show();

            //a.Description = database_ref.ExtractAbilitiesAndConditions(a.Description, ref a.DescriptionAbilities, ref a.DescriptionConditions, ref a.DescriptionMechanics);

            RTFLabelDescription.SelectionColor = Color.LightGray;

            AddLine(RTFLabelDescription, "Nora cost: ", a.NoraCost.ToString());
            AddLine(RTFLabelDescription, "AP cost: ", a.APCost.ToString());
            AddLine(RTFLabelDescription, "Cooldown: ", a.Cooldown.ToString());

            RTFLabelDescription.SelectionColor = Color.LightGray;
            RTFLabelDescription.AppendText("\r\n");

            RTFLabelDescription.SelectionFont = RegularFont;
            RTFLabelDescription.AppendText(a.Description + "\r\n");
        }

        public void SetSpellRune(Spell s)
        {
            SetRune(s);

            RTFLabelDescription.Show();

            //s.Description = database_ref.ExtractAbilitiesAndConditions(s.Description, ref s.DescriptionAbilities, ref s.DescriptionConditions, ref s.DescriptionMechanics);

            AddLine(RTFLabelDescription, "Nora cost: ", s.NoraCost.ToString());
            AddLine(RTFLabelDescription, "", "");
            AddLine(RTFLabelDescription, "", s.Description);
            AddLine(RTFLabelDescription, "", "");

            RTFLabelDescription.SelectionFont = ItalicFont;
            RTFLabelDescription.AppendText(s.Flavor + "\r\n");
        }

        public void SetRelicRune(Relic r)
        {
            SetRune(r);

            RTFLabelDescription.Show();

            //r.Description = database_ref.ExtractAbilitiesAndConditions(r.Description, ref r.DescriptionAbilities, ref r.DescriptionConditions, ref r.DescriptionMechanics);

            AddLine(RTFLabelDescription, "Nora cost: ", r.NoraCost.ToString());
            AddLine(RTFLabelDescription, "Defense: ", r.Defense.ToString());
            AddLine(RTFLabelDescription, "Hit points: ", r.HitPoints.ToString());
            AddLine(RTFLabelDescription, "Size: ", r.Size.ToString());
            AddLine(RTFLabelDescription, "", "");
            AddLine(RTFLabelDescription, "", r.Description);
            AddLine(RTFLabelDescription, "", "");

            RTFLabelDescription.SelectionFont = ItalicFont;
            RTFLabelDescription.AppendText(r.Flavor + "\r\n");
        }

        public void SetEquipmentRune(Equipment e)
        {
            SetRune(e);

            RTFLabelDescription.Show();

            //e.Description = database_ref.ExtractAbilitiesAndConditions(e.Description, ref e.DescriptionAbilities, ref e.DescriptionConditions, ref e.DescriptionMechanics);

            AddLine(RTFLabelDescription, "Nora cost: ", e.NoraCost.ToString());
            AddLine(RTFLabelDescription, "", "");
            AddLine(RTFLabelDescription, "", e.Description);
            AddLine(RTFLabelDescription, "", "");

            RTFLabelDescription.SelectionFont = ItalicFont;
            RTFLabelDescription.AppendText(e.Flavor + "\r\n");
        }

        public void SetCondition(FlavorElement c)
        {
            ClearDescription();

            RTFLabelDescription.Show();

            //c.Description = database_ref.ExtractAbilitiesAndConditions(c.Description, ref c.DescriptionAbilities, ref c.DescriptionConditions, ref c.DescriptionMechanics);

            RTFLabelDescription.SelectionColor = Color.White;
            AddLine(RTFLabelDescription, c.ToString(), "");

            RTFLabelDescription.SelectionFont = RegularFont;
            RTFLabelDescription.AppendText(c.Description + "\r\n");
        }

        public void SetMechanic(FlavorElement m)
        {
            ClearDescription();

            RTFLabelDescription.Show();

            //m.Description = database_ref.ExtractAbilitiesAndConditions(m.Description, ref m.DescriptionAbilities, ref m.DescriptionConditions, ref m.DescriptionMechanics);

            RTFLabelDescription.SelectionColor = Color.White;
            AddLine(RTFLabelDescription, m.ToString(), "");

            RTFLabelDescription.SelectionFont = RegularFont;
            RTFLabelDescription.AppendText(m.Description + "\r\n");
        }

        private void AddLine(RichTextBox rtb, string bold_text, string regular_text)
        {
            rtb.SelectionFont = BoldFont;
            rtb.AppendText(bold_text);
            rtb.SelectionFont = RegularFont;
            rtb.AppendText(regular_text + "\r\n");
        }

        public void OnImageLoad(Bitmap bmp)
        {
            RuneImage.Image = bmp;
        }

        public void SetHeight(int h)
        {
            this.Height = h;
            PanelAbilities.Height = this.Height - PanelAbilities.Location.Y - 3;
            RTFLabelDescription.Height = PanelAbilities.Height;
        }

        // tracer stuff
        void TracerGoTo(TracerViewData tvd)
        {
            while (Tracer.Count > TracerPosition + 1)
                Tracer.RemoveAt(TracerPosition + 1);

            Tracer.Add(CurrentTracer);
            TracerPosition += 1;

            TracerSetDescription(tvd);
        }

        private void TracerSetDescription(TracerViewData tvd)
        {
            switch (tvd.Type)
            {
                case Pox.DataElement.ElementType.CHAMPION:
                    if (database_ref.Champions.ContainsKey(tvd.ID))
                        SetChampionRune(database_ref.Champions[tvd.ID], database_ref.Champions[tvd.ID].DefaultUpgrade1Index, database_ref.Champions[tvd.ID].DefaultUpgrade2Index);
                    break;
                case Pox.DataElement.ElementType.ABILITY:
                    if (database_ref.Abilities.ContainsKey(tvd.ID))
                        SetAbility(database_ref.Abilities[tvd.ID]);
                    break;
                case Pox.DataElement.ElementType.SPELL:
                    if (database_ref.Spells.ContainsKey(tvd.ID))
                        SetSpellRune(database_ref.Spells[tvd.ID]);
                    break;
                case Pox.DataElement.ElementType.RELIC:
                    if (database_ref.Relics.ContainsKey(tvd.ID))
                        SetRelicRune(database_ref.Relics[tvd.ID]);
                    break;
                case Pox.DataElement.ElementType.EQUIPMENT:
                    if (database_ref.Equipments.ContainsKey(tvd.ID))
                        SetEquipmentRune(database_ref.Equipments[tvd.ID]);
                    break;
                default:
                    break;
            }

            CurrentTracer = tvd;
        }

        public void TracerGoBack()
        {
            if (TracerPosition == -1)
                return;

            TracerViewData tvd = Tracer[TracerPosition];
            TracerPosition -= 1;
        }

        public void TracerClear()
        {
            Tracer.Clear();
            TracerPosition = -1;
        }

        private void AbilityLinkClicked(object sender, EventArgs e)
        {
            TracerGoTo((TracerViewData)((PictureBox)sender).Tag);
        }
    }
}
