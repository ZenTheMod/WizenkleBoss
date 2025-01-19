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

namespace WizenkleBoss.Content.Items.Notes
{
    public class AstronomyNotes : ModItem, IDontGetRightClicked
    {
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
        public override bool CanRightClick() => true;
        public override void RightClick(Player player) => OpenUI(player);
        public override bool? UseItem(Player player)
        {
            OpenUI(player);
            return true;
        }
        private static void OpenUI(Player player)
        {
            if (player.whoAmI != Main.myPlayer)
                return;

            if (player.mount.Active)
            {
                player.mount.Dismount(player);
            }

            IngameFancyUI.CoverNextFrame();
            Main.ClosePlayerChat();

            Main.mouseRightRelease = false;

                // I HATE FANCY UI
            Main.ingameOptionsWindow = false;
            Main.playerInventory = false;
            Main.editChest = false;
            Main.npcChatText = string.Empty;
            Main.chatText = string.Empty;
            Main.inFancyUI = true;

            NoteUISystem.CurrentNote = new(0, Language.GetTextValue("Mods.WizenkleBoss.Lore.Log0"));
            Main.InGameUI.SetState(NoteUISystem.noteUI);
        }
    }
}
