using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Struct
{
    public class SignEffect_Damage : SignEffect
    {
        public float Damage_multiply
        {
            get
            {
                return card.damage_multiply;
            }
        }
        public override void DoEffect(Combat combat)
        {
            if(isAtker)
            {
                combat.beDamaged_dfd += combat.actor_atk.atk * Damage_multiply;
            }
            else
            {
                combat.beDamaged_atk += combat.actor_dfd.atk * Damage_multiply;
            }
        }

        public SignEffect_Damage()
        {
            name = "攻击";
            effectTags_list = new List<EffectTag> { EffectTag.接触};
            priority = 2;
        }
    }
}
