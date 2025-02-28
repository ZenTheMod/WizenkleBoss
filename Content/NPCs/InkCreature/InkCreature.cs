using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizenkleBoss.Common.Helpers;
using WizenkleBoss.Common.Ink;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Security.Policy;
using rail;
using System.Collections.Generic;
using WizenkleBoss.Common.Registries;

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

        public override void AI()
        {

        }

            // i hate contact damage
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public void Shape()
        {
            Vector2 position = NPC.Center - Main.screenPosition;


            Main.spriteBatch.Draw(Textures.Bloom.Value, position, null, (Color.White * 0.22f) with { A = 0 }, 0f, Textures.Bloom.Value.Size() / 2f, 3.6f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(InkCreatureAssetRegistry.Eye.Value, position, null, Color.White with { A = 0 }, 0f, InkCreatureAssetRegistry.Eye.Value.Size() / 2f, 0.8f, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Textures.Bloom.Value, position, null, (Color.White * 0.08f) with { A = 0 }, 0f, Textures.Bloom.Value.Size() / 2f, 5.6f, SpriteEffects.None, 0f);
        }
    }
}
