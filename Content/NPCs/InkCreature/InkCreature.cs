using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizenkleBoss.Common.Helper;
using WizenkleBoss.Common.Ink;
using Microsoft.Xna.Framework.Graphics;

namespace WizenkleBoss.Content.NPCs.InkCreature
{
    public class InkCreature : ModNPC, IDrawWiggly
    {
        public override string Texture => "WizenkleBoss/Assets/Textures/MagicPixel";

        public override void SetDefaults()
        {
            NPC.width = 140;
            NPC.height = 140;
            NPC.aiStyle = -1;
            NPC.damage = 0;
            NPC.defense = 20; 
            NPC.lifeMax = 20000; 

            NPC.noGravity = true;
            NPC.noTileCollide = true;

            NPC.knockBackResist = 0f;
            NPC.hide = true;
        }

            // i hate contact damage
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public void Shape()
        {
            Main.spriteBatch.Draw(TextureRegistry.Bloom.Value, NPC.Center - Main.screenLastPosition, null, Color.White with { A = 0 }, 0f, TextureRegistry.Bloom.Value.Size() / 2f, 3.6f, SpriteEffects.None, 0f);
        }
    }
}
