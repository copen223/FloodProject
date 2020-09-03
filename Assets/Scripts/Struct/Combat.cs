using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Struct
{

    public class Combat
    {
        public ActorMono actor_atk;
        public ActorMono actor_dfd;
        public Card card_atk;
        public Card card_dfd;

        public List<SignEffect> effects_list = new List<SignEffect>();

        // 结果缓存
        public float beDamaged_atk = 0;
        public float beDamaged_dfd = 0;
        public List<string> animationInfo_list = new List<string>();
        public bool isEnd = false;

        public Combat(ActorMono user,Card atkCard,ActorMono target)
        {
            card_atk = atkCard;
            actor_dfd = target;
            card_dfd = target.FocusedCard;
            actor_atk = user;
        }

        public void StartThisCombat()
        {
            Combat2Card();
            DoEffects();
            DoResult();
        }

        private void Combat2Card()
        {
            Combat2Sign(card_atk.sign_up, card_dfd.sign_up);
            Combat2Sign(card_atk.sign_down, card_dfd.sign_down);
        }

        private void Combat2Sign(CardSign atk,CardSign dfd)
        {
            if (atk.type != CardSign.Type.atk)
                return;
            atk.effect.isAtker = true;
            effects_list.Add(atk.effect);
           

            if(atk.intensity<=dfd.intensity)
            {
                dfd.effect.isAtker = false;
                effects_list.Add(dfd.effect);
                isEnd = true;
            }
        }

        private void DoEffects()
        {
            EffectSortByPriority sort = new EffectSortByPriority();
            effects_list.Sort(sort);

            for(int i=0;i<effects_list.Count;i++)
            {
                effects_list[i].DoEffect(this);
            }
        }

        private void DoResult()
        {
            actor_atk.Behit(beDamaged_atk);
            actor_dfd.Behit(beDamaged_dfd);
        }

        private class EffectSortByPriority : IComparer<SignEffect>
        {
            public int Compare(SignEffect x, SignEffect y)
            {
                if (x.priority > y.priority)
                    return 1;
                else if (x.priority == y.priority)
                    return 0;
                else
                    return -1;
            }
        }
    }
}
