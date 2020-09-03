using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Struct
{
    public class SignEffect_Damage : SignEffect
    {
        public override void DoEffect(Combat combat)
        {
            if(isAtker)
            {
                combat.beDamaged_dfd += combat.actor_atk.atk;
            }
            else
            {
                combat.beDamaged_atk += combat.actor_dfd.atk;
            }
        }

        public SignEffect_Damage()
        {
            name = "伤害";
        }
    }
}
