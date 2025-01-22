using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using WizenkleBoss.Common.Helpers;
using WizenkleBoss.Common.ILDetourSystems;
using WizenkleBoss.Content.Projectiles;
using WizenkleBoss.Content.Rarities;
using WizenkleBoss.Content.UI;
using WizenkleBoss.Content.UI.Notes;

namespace WizenkleBoss.Content.Items.Notes.PreInk
{
    public class One : BaseNoteItem
    {
        public override Note Note => new(0, "Mods.WizenkleBoss.Lore.Log0");
    }
}
