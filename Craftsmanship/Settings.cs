using SettingsHelper;
using UnityEngine;
using Verse;

namespace Craftsmanship
{
    public class CraftsmahshipSettings : ModSettings
    {

        private static float chance = 1f;
        
        public void DoWindowContents(Rect wrect)
        {
            Listing_Standard options = new Listing_Standard();
            Color defaultColor = GUI.color;
            options.Begin(wrect);

            GUI.color = defaultColor;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            options.Gap();
            // Same GUI colour as Merciless
            GUI.color = new Color(1f, 0.2f, 0.2f);
            options.AddLabeledSlider("Settings_chance".Translate(), ref chance, 1, 100,
                rightAlignedLabel: chance.ToStringByStyle(ToStringStyle.FloatTwo, ToStringNumberSense.Factor), roundTo: 0.01f);
            GUI.color = defaultColor;
            options.Gap();
            options.End();
            
            Mod.GetSettings<CraftsmahshipSettings>().Write();

        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref chance, "Settings_chance", 1f);
        }

    }

    public class SurvivalTools : Mod
    {
        public CraftsmahshipSettings settings;

        public SurvivalTools(ModContentPack content) : base(content)
        {
            GetSettings<CraftsmahshipSettings>();
        }

        public override string SettingsCategory() => "CraftsmanshipSettingsCategory".Translate();

        public override void DoSettingsWindowContents(Rect inRect)
        {
            GetSettings<CraftsmahshipSettings>().DoWindowContents(inRect);
        }

    }

}
