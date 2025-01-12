using Terraria;
using Terraria.ModLoader;

namespace WizenkleBoss.Common.Ink
{
    [Autoload(Side = ModSide.Client)]
    public class InkEffect : ModSceneEffect
    {
        public override int Music => -1;
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals($"WizenkleBoss:{InkShaderData.ShaderKey}", isActive);
        }
        public override bool IsSceneEffectActive(Player player) => InkSystem.AnyActiveInk;
    }
}
