using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Struct
{
    public class CardEffect_Resume : CardEffect
    {
        private int resume_num;
        public enum ResumeType
        {
            HP,
            AP
        }

        private string resume_what;

        public CardEffect_Resume(string _trigger,int _resume_num,string what)
        {
            trigger = _trigger;
            name = "恢复";
            functionTarget = FunctionTarget.self;
            effectType = EffectType.afterCombat;
            resume_num = _resume_num;
            resume_what = what;
            priority = 6;
        }

        public override void DoEffect(Combat combat)
        {
            ActorMono target;
            if(isAtker)
            {
                target = combat.actor_atk;
            }
            else
            {
                // 防御方使用这张卡
                target = combat.actor_dfd;
            }

            switch (resume_what)
            {
                case "生命":target.ResumeHitPoint(resume_num);break;
                case "行动":target.ResumeActionPoint(resume_num);break;
                default:break;
                    
            }
        }
        public override string GetDescription()
        {
            return "恢复" + resume_what + "" + resume_num + "点; ";
        }


    }
}
