using UnityEngine;
using Verse;

namespace Craftsmanship
{
    public class CraftsmanshipMod : Mod
    {
        public static CraftsmanshipSettings Settings;

        public CraftsmanshipMod(ModContentPack content)
            : base(content)
        {
            Settings = GetSettings<CraftsmanshipSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.DoWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Craftsmanship".Translate();
        }
    }
}
