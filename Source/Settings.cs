using UnityEngine;
using Verse;

namespace Craftsmanship
{
    public class CraftsmanshipSettings : ModSettings
    {
        public float chancePercent = 50f; // User-facing percentage (0–100) Default is 50%
        public float xpMultiplier = 1f;
        public bool requirePassion = true; // Default to only shared passions
        public bool socialBuff = true; // Default to true, allowing social buffs

        public override void ExposeData()
        {
            Scribe_Values.Look(ref chancePercent, "chancePercent", 50f);
            Scribe_Values.Look(ref xpMultiplier, "xpMultiplier", 1f); // Default 1x
            Scribe_Values.Look(ref requirePassion, "requirePassion", true); // Allow for any skill or passions only.
            Scribe_Values.Look(ref socialBuff, "socialBuff", true); // Default to true, allowing social buffs
        }

        public void DoWindowContents(Rect rect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(rect);

            listing.Gap(12f);
            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            listing.Label("Craftsmanship_SettingsHeader".Translate());
            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;
            listing.Label("Craftsmanship_SettingsDescription".Translate());
            listing.Gap(6f);

            // 🎛 Interaction chance setting
            listing.Label(
                "Craftsmanship_ChanceSettingLabel".Translate(chancePercent.ToString("F0") + "%"),
                tooltip: "Craftsmanship_ChanceSettingTooltip".Translate()
            );
            chancePercent = listing.Slider(chancePercent, 0f, 100f);

            listing.Gap();

            // XP multiplier setting
            listing.Label(
                "Craftsmanship_XPMultiplierLabel".Translate(xpMultiplier.ToString("F2") + "x"),
                tooltip: "Craftsmanship_XPMultiplierTooltip".Translate()
            );
            xpMultiplier = listing.Slider(xpMultiplier, 0.1f, 5f);
            listing.Gap();
            // Checkbox for passion requirement
            listing.CheckboxLabeled(
                "Craftsmanship_RequirePassionLabel".Translate(),
                ref requirePassion,
                "Craftsmanship_RequirePassionTooltip".Translate()
            );
            listing.Gap();
            // Checkbox for social buff
            listing.CheckboxLabeled(
                "Craftsmanship_SocialBuffLabel".Translate(),
                ref CraftsmanshipMod.Settings.socialBuff,
                "Craftsmanship_SocialBuffTooltip".Translate()
            );

            listing.End();
        }
    }
}
