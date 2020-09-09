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
                // 攻击方发动减伤效果 受到的伤害减少自己的防御力的数值
                combat.beDamaged_atk -= combat.actor_atk.dfd;
                if (combat.beDamaged_atk <= 0)
                    combat.beDamaged_atk = 0;
            }
            else
            {
                // 防御方发动减伤效果
                combat.beDamaged_dfd -= combat.actor_dfd.dfd;
                if (combat.beDamaged_dfd <= 0)
                    combat.beDamaged_dfd = 0;

            }
        }

        public SignEffect_ReduceDamage()
        {
            name = "减伤";
            priority = 6;
        }
    }
}
