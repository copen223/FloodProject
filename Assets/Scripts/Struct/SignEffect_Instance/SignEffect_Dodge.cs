using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Struct
{
    public class SignEffect_Dodge : SignEffect
    {
        public override void DoEffect(Combat combat)
        {
            if(isAtker)
            {
                // 攻击方，删去防御方带有接触标记的效果
                for(int i=0;i < combat.effects_list.Count;i++)
                {
                    var effect = combat.effects_list[i];
                    if(effect.effectTags_list.Contains(EffectTag.接触) && !effect.isAtker)
                    {
                        combat.effects_list.Remove(effect);
                        i--;
                    }
                }
            }
            else
            {
                // 防御方，删去攻击方带有接触标记的效果
                for (int i = 0; i < combat.effects_list.Count; i++)
                {
                    var effect = combat.effects_list[i];
                    if (effect.effectTags_list.Contains(EffectTag.接触) && effect.isAtker)
                    {
                        combat.effects_list.Remove(effect);
                        i--;
                    }
                }
            }
        }

        public SignEffect_Dodge()
        {
            name = "闪避";
            effectTags_list = new List<EffectTag> { };
            priority = 6;
        }
    }
}
