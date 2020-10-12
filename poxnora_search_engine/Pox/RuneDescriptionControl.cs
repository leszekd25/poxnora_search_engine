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
        struct TracerViewData
        {
            public Pox.DataElement.ElementType Type;
            public int ID;
        }

        static Font RegularFont = new Font("Arial", 10, FontStyle.Regular);
        static Font BoldFont = new Font("Arial", 10, FontStyle.Bold);
        static Font ItalicFont = new Font("Arial", 10, FontStyle.Italic);

        List<PictureBox> Links = new List<PictureBox>();
        int CursorY = 0;

        List<TracerViewData> Tracer = new List<TracerViewData>();
        int TracerPosition = -1;
        TracerViewData CurrentTracer;

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

            CursorY += 32;

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

                    /*TextBoxDescription.SelectionColor = Color.LightGray;
                    TextBoxDescription.SelectionFont = RegularFont;
                    TextBoxDescription.AppendText("  (" + ab.NoraCost.ToString() + ")\r\n");*/

                    CursorY += 16;
                }
                else
                {
                    AddLine("", "(" + ab.NoraCost.ToString() + ")  " + ab.ToString());
                }

                AddLink(new Point() { X = 220, Y = CursorY }, new TracerViewData() { ID = ab.ID, Type = Pox.DataElement.ElementType.ABILITY });
            }
            foreach (var ab in c.UpgradeAbilities1_refs)
            {
                TextBoxDescription.SelectionColor = Color.PaleTurquoise;
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
                AddLink(new Point() { X = 220, Y = CursorY }, new TracerViewData() { ID = ab.ID, Type = Pox.DataElement.ElementType.ABILITY });
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
                AddLink(new Point() { X = 220, Y = CursorY }, new TracerViewData() { ID = ab.ID, Type = Pox.DataElement.ElementType.ABILITY });
            }
            TextBoxDescription.SelectionColor = Color.LightGray;
            TextBoxDescription.AppendText("\r\n");

            TextBoxDescription.SelectionFont = ItalicFont;
            TextBoxDescription.AppendText(c.Description + "\r\n");
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
        }

        private void AddLine(string bold_text, string regular_text)
        {
            CursorY += 16;
            TextBoxDescription.SelectionFont = BoldFont;
            TextBoxDescription.AppendText(bold_text);
            TextBoxDescription.SelectionFont = RegularFont;
            TextBoxDescription.AppendText(regular_text + "\r\n");
        }

        public void OnRuneImageLoad(Bitmap bmp)
        {
            RuneImage.Image = bmp;
        }

        public void SetHeight(int h)
        {
            this.Height = h;
            TextBoxDescription.Height = this.Height - RuneImage.Height - 3;
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

        private void AddLink(Point position, TracerViewData tag)
        {
            PictureBox pb = new PictureBox() { Location = position, Size = new Size(24, 14), Tag = tag };
            pb.Image = Properties.Resources.ImageGoTo;
            pb.Click += new EventHandler(LinkClicked);
            pb.Cursor = Cursors.Hand;
            Links.Add(pb);
            TextBoxDescription.Controls.Add(pb);
            pb.BringToFront();
        }

        private void LinkClicked(object sender, EventArgs e)
        {
            TracerGoTo((TracerViewData)((PictureBox)sender).Tag);
        }
    }
}
