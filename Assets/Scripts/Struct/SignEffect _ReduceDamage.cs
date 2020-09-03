using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Struct
{
    public class SignEffect_ReduceDamage : SignEffect
    {
        public override void DoEffect(Combat combat)
        {
            if (isAtker)
            {
                combat.beDamaged_atk -= combat.actor_atk.dfd;
            }
            else
            {
                combat.beDamaged_dfd -= combat.actor_atk.dfd;
            }
        }

        public SignEffect_ReduceDamage()
        {
            name = "减伤";
        }
    }
}
