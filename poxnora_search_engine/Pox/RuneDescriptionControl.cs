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

        List<PictureBox> Links = new List<PictureBox>();
        int CursorY = 0;

        List<TracerViewData> Tracer = new List<TracerViewData>();
        int TracerPosition = -1;
        TracerViewData CurrentTracer;

        UpgradeAbilitySelection AbilitySelection = new UpgradeAbilitySelection() { AbilityIndex1 = 0, AbilityIndex2 = 0 };
        bool ShowUpgradeAbilitySelection = false;

        public OnUpgradeClicked UpgradeClicked = null;

        public Database database_ref = null;
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
            CursorY = 0;

            TextBoxDescription.Clear();
            foreach (var pb in Links)
                TextBoxDescription.Controls.Remove(pb);
            Links.Clear();
        }

        private void SetRune(Rune r)
        {
            ClearDescription();

            Program.image_cache.LoadRuneImage(r.Hash);

            TextBoxDescription.Clear();

            TextBoxDescription.SelectionColor = Color.LightGray;
            TextBoxDescription.SelectionFont = ItalicFont;
            TextBoxDescription.AppendText("Illustrated by  "+r.Artist + "\r\n\r\n");

            TextBoxDescription.SelectionColor = GetColorByRarity(r.Rarity);
            TextBoxDescription.SelectionFont = BoldFont;
            TextBoxDescription.AppendText(r.Name + "\r\n");

            TextBoxDescription.SelectionColor = Color.LightGray;
            AddLine("Expansion: ", r.Expansion);

            string faction = "";
            for (int i = 0; i < r.Faction.Count - 1; i++)
                faction += r.Faction[i] + ", ";
            if (r.Faction.Count != 0)
                faction += r.Faction[r.Faction.Count - 1];
            AddLine("Faction: ", faction);

            AddLine("Deck limit: ", r.DeckLimit.ToString());
            AddLine("", "");
        }

        public void SetChampionRune(Champion c)
        {
            SetRune(c);

            AddLine("Default nora cost: ", c.DefaultNoraCost.ToString());

            string cl = "";
            for (int i = 0; i < c.Class.Count - 1; i++)
                cl += c.Class[i] + ", ";
            if (c.Class.Count != 0)
                cl += c.Class[c.Class.Count - 1];
            AddLine("Class: ", cl);

            string race = "";
            for (int i = 0; i < c.Race.Count - 1; i++)
                race += c.Race[i] + ", ";
            if (c.Race.Count != 0)
                race += c.Race[c.Race.Count - 1];
            AddLine("Race: ", race);

            AddLine("Damage: ", c.Damage.ToString());
            AddLine("Speed: ", c.Speed.ToString());
            AddLine("Range: ", c.MinRNG.ToString() + " - " + c.MaxRNG.ToString());
            AddLine("Defense: ", c.Defense.ToString());
            AddLine("Hit points: ", c.HitPoints.ToString());
            AddLine("Size: ", c.Size.ToString());
            AddLine("", "");

            AddLine("Abilities:", "");

            int ablink_pos_y = TextBoxDescription.GetPositionFromCharIndex(TextBoxDescription.Text.Length - 1).Y + 18;
            foreach (var ab in c.BaseAbilities_refs)
            {
                if (ab.Name.Contains("Attack: "))
                {
                    TextBoxDescription.SelectionColor = Color.LightGray;
                    TextBoxDescription.SelectionFont = RegularFont;
                    TextBoxDescription.AppendText("(" + ab.NoraCost.ToString() + ")  Attack: ");

                    TextBoxDescription.SelectionColor = GetColorByAttackType(ab.Name.Substring(8));
                    TextBoxDescription.SelectionFont = BoldFont;
                    TextBoxDescription.AppendText(ab.Name.Substring(8) + "\r\n");
                }
                else
                {
                    AddLine("", "(" + ab.NoraCost.ToString() + ")  " + ab.ToString());
                }
            }
            foreach (var ab in c.UpgradeAbilities1_refs)
            {
                if (c.UpgradeAbilities1_refs.IndexOf(ab) == c.DefaultUpgrade1Index)
                {
                    TextBoxDescription.SelectionColor = Color.Cyan;
                    AddLine("(" + ab.NoraCost.ToString() + ")  " + ab.ToString(), "");
                }
                else
                {
                    TextBoxDescription.SelectionColor = Color.DeepSkyBlue;
                    AddLine("", "(" + ab.NoraCost.ToString() + ")  " + ab.ToString());
                }
            }
            foreach (var ab in c.UpgradeAbilities2_refs)
            {
                if (c.UpgradeAbilities2_refs.IndexOf(ab) == c.DefaultUpgrade2Index)
                {
                    TextBoxDescription.SelectionColor = Color.Lime;
                    AddLine("(" + ab.NoraCost.ToString() + ")  " + ab.ToString(), "");
                }
                else
                {
                    TextBoxDescription.SelectionColor = Color.LimeGreen;
                    AddLine("", "(" + ab.NoraCost.ToString() + ")  " + ab.ToString());
                }
            }
            TextBoxDescription.SelectionColor = Color.LightGray;
            TextBoxDescription.AppendText("\r\n");

            TextBoxDescription.SelectionFont = ItalicFont;
            TextBoxDescription.AppendText(c.Description + "\r\n");

            // add abilities

            TextBoxDescription.SelectionLength = 0;
            TextBoxDescription.SelectionStart = 0;
            TextBoxDescription.ScrollToCaret();

            for(int i = 0; i < c.BaseAbilities_refs.Count; i++)
            {
                Ability ab = c.BaseAbilities_refs[i];
                AddAbilityLink(new Point() { X = 238, Y = ablink_pos_y }, new TracerViewData() { ID = ab.ID, Type = Pox.DataElement.ElementType.ABILITY });
                ablink_pos_y += 16;
            }
            for (int i = 0; i < c.UpgradeAbilities1_refs.Count; i++)
            {
                Ability ab = c.UpgradeAbilities1_refs[i];
                AddAbilityLink(new Point() { X = 238, Y = ablink_pos_y }, new TracerViewData() { ID = ab.ID, Type = Pox.DataElement.ElementType.ABILITY });

                if(ShowUpgradeAbilitySelection)
                    AddUpgradeLink(new Point() { X = 220, Y = ablink_pos_y }, (AbilitySelection.AbilityIndex1 == i), new AbilityLinkData() { UpgradeID = 1, UpgradeIndex = i });

                ablink_pos_y += 16;
            }
            for (int i = 0; i < c.UpgradeAbilities2_refs.Count; i++)
            {
                Ability ab = c.UpgradeAbilities2_refs[i];
                AddAbilityLink(new Point() { X = 238, Y = ablink_pos_y }, new TracerViewData() { ID = ab.ID, Type = Pox.DataElement.ElementType.ABILITY }); 

                if (ShowUpgradeAbilitySelection)
                    AddUpgradeLink(new Point() { X = 220, Y = ablink_pos_y }, (AbilitySelection.AbilityIndex2 == i), new AbilityLinkData() { UpgradeID = 2, UpgradeIndex = i });

                ablink_pos_y += 16;
            }

            ShowUpgradeAbilitySelection = false;
        }

        public void SetAbility(Ability a)
        {
            ClearDescription();

            a.Description = database_ref.ExtractAbilitiesAndConditions(a.Description, ref a.DescriptionAbilities, ref a.DescriptionConditions);

            TextBoxDescription.Clear();

            TextBoxDescription.SelectionColor = Color.White;
            AddLine(a.ToString(), "");


            TextBoxDescription.SelectionColor = Color.LightGray;

            AddLine("Nora cost: ", a.NoraCost.ToString());
            AddLine("AP cost: ", a.APCost.ToString());
            AddLine("Cooldown: ", a.Cooldown.ToString());
            
            TextBoxDescription.SelectionColor = Color.LightGray;
            TextBoxDescription.AppendText("\r\n");

            TextBoxDescription.SelectionFont = RegularFont;
            TextBoxDescription.AppendText(a.Description + "\r\n");

            ShowUpgradeAbilitySelection = false;
        }

        public void SetSpellRune(Spell s)
        {
            SetRune(s);

            s.Description = database_ref.ExtractAbilitiesAndConditions(s.Description, ref s.DescriptionAbilities, ref s.DescriptionConditions);

            AddLine("Nora cost: ", s.NoraCost.ToString());
            AddLine("", "");
            AddLine("", s.Description);
            AddLine("", "");

            TextBoxDescription.SelectionFont = ItalicFont;
            TextBoxDescription.AppendText(s.Flavor + "\r\n");

            ShowUpgradeAbilitySelection = false;
        }

        public void SetRelicRune(Relic r)
        {
            SetRune(r);

            r.Description = database_ref.ExtractAbilitiesAndConditions(r.Description, ref r.DescriptionAbilities, ref r.DescriptionConditions);

            AddLine("Nora cost: ", r.NoraCost.ToString());
            AddLine("Defense: ", r.Defense.ToString());
            AddLine("Hit points: ", r.HitPoints.ToString());
            AddLine("Size: ", r.Size.ToString());
            AddLine("", "");
            AddLine("", r.Description);
            AddLine("", "");

            TextBoxDescription.SelectionFont = ItalicFont;
            TextBoxDescription.AppendText(r.Flavor + "\r\n");

            ShowUpgradeAbilitySelection = false;
        }

        public void SetEquipmentRune(Equipment e)
        {
            SetRune(e);

            e.Description = database_ref.ExtractAbilitiesAndConditions(e.Description, ref e.DescriptionAbilities, ref e.DescriptionConditions);

            AddLine("Nora cost: ", e.NoraCost.ToString());
            AddLine("", "");
            AddLine("", e.Description);
            AddLine("", "");

            TextBoxDescription.SelectionFont = ItalicFont;
            TextBoxDescription.AppendText(e.Flavor + "\r\n");

            ShowUpgradeAbilitySelection = false;
        }

        private void AddLine(string bold_text, string regular_text)
        {
            TextBoxDescription.SelectionFont = BoldFont;
            TextBoxDescription.AppendText(bold_text);
            TextBoxDescription.SelectionFont = RegularFont;
            TextBoxDescription.AppendText(regular_text + "\r\n");
        }

        public void OnImageLoad(Bitmap bmp)
        {
            RuneImage.Image = bmp;
        }

        public void SetHeight(int h)
        {
            this.Height = h;
            TextBoxDescription.Height = this.Height - RuneImage.Height - 3;
        }

        // to be able to select champion abilities in bg builder
        public void ShowAbilitySelection(int ab1_index, int ab2_index)
        {
            ShowUpgradeAbilitySelection = true;
            AbilitySelection = new UpgradeAbilitySelection() { AbilityIndex1 = ab1_index, AbilityIndex2 = ab2_index };
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
                        SetChampionRune(database_ref.Champions[tvd.ID]);
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

        private void AddAbilityLink(Point position, TracerViewData tag)
        {
            PictureBox pb = new PictureBox() { Size = new Size(16, 16), Tag = tag };
            pb.Image = Properties.Resources.ab_icon_qmark;
            pb.Click += new EventHandler(AbilityLinkClicked);
            pb.Cursor = Cursors.Hand;
            Links.Add(pb);
            TextBoxDescription.Controls.Add(pb);
            pb.Location = new Point(position.X, position.Y);
            pb.BringToFront();
        }

        private void AddUpgradeLink(Point position, bool selected, AbilityLinkData ald)
        {
            PictureBox pb = new PictureBox() { Size = new Size(16, 16), Tag = ald };
            pb.Image = (selected ? Properties.Resources.ab_icon_checked : Properties.Resources.ab_icon_unchecked);
            pb.Tag = ald;
            pb.Click += new EventHandler(UpgradeLinkClicked);
            pb.Cursor = Cursors.Hand;
            Links.Add(pb);
            TextBoxDescription.Controls.Add(pb);
            pb.Location = new Point(position.X, position.Y);
            pb.BringToFront();
        }

        private void AbilityLinkClicked(object sender, EventArgs e)
        {
            TracerGoTo((TracerViewData)((PictureBox)sender).Tag);
        }

        private void UpgradeLinkClicked(object sender, EventArgs e)
        {
            AbilityLinkData ald = (AbilityLinkData)(((PictureBox)sender).Tag);
            bool new_upg = false;

            if(ald.UpgradeID == 1)
                new_upg = (ald.UpgradeIndex != AbilitySelection.AbilityIndex1);
            else if(ald.UpgradeID == 2)
                new_upg = (ald.UpgradeIndex != AbilitySelection.AbilityIndex2);

            if(new_upg)
            {
                // shoddy
                foreach (PictureBox pb in Links)
                {
                    if (pb.Tag == null)
                        continue;

                    if (pb.Tag is AbilityLinkData)
                    {
                        AbilityLinkData ald2 = (AbilityLinkData)(pb.Tag);
                        if (ald2.UpgradeID == ald.UpgradeID)
                        {
                            pb.Image = Properties.Resources.ab_icon_unchecked;
                        }
                    }
                }

                if(ald.UpgradeID == 1)
                    AbilitySelection.AbilityIndex1 = ald.UpgradeIndex;
                else if(ald.UpgradeID == 2)
                    AbilitySelection.AbilityIndex2 = ald.UpgradeIndex;

                ((PictureBox)sender).Image = Properties.Resources.ab_icon_checked;
                UpgradeClicked?.Invoke(ald.UpgradeID, ald.UpgradeIndex);
            }
        }



        private void TextBoxDescription_ContentsResized(object sender, ContentsResizedEventArgs e)
        {
            RichTextBox rtf = (RichTextBox)sender;

            CursorY = TextBoxDescription.GetPositionFromCharIndex(TextBoxDescription.Text.Length - 1).Y;//e.NewRectangle.Height;
        }
    }
}
