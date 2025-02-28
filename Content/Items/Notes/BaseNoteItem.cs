using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using WizenkleBoss.Common.ILDetourSystems;
using WizenkleBoss.Content.Rarities;
using WizenkleBoss.Content.UI.Notes;
using WizenkleBoss.Common.Ink;
using WizenkleBoss.Content.UI;
using WizenkleBoss.Common.Helpers;

namespace WizenkleBoss.Content.Items.Notes
{
    public abstract class BaseNoteItem : ModItem, IDontGetRightClicked
    {
        public override string LocalizationCategory => "Lore.Notes";
        public override string Texture => "WizenkleBoss/Content/Items/Notes/NoteStarTexture";
        public virtual Note Note => new();
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;

            Item.useTime = 5;
            Item.useAnimation = 10;
            Item.autoReuse = false;

            Item.useStyle = ItemUseStyleID.HoldUp;

            Item.rare = ModContent.RarityType<StarboundRarity>();
            Item.noUseGraphic = true;
        }
            // This is so the player can open it via a right click similar to a bossbag but NOT decrement the stack.
        public override bool CanRightClick() => true;
        public override void RightClick(Player player) => OpenUI(player);
        public override bool? UseItem(Player player)
        {
            OpenUI(player);
            return true;
        }
        private void OpenUI(Player player)
        {
            if (player.whoAmI != Main.myPlayer)
                return;

            NoteUISystem.CurrentNote = Note;

            Helper.GenericOpenFancyUI(NoteUISystem.noteUI, player);
        }
        public override LocalizedText Tooltip => Language.GetText("Mods.WizenkleBoss.Lore.Read");
    }
}
