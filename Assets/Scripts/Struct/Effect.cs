using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Struct
{
    public abstract class Effect
    {
        public Card card;

        public string name;

        public List<EffectTag> effectTags_list = new List<EffectTag>();
        public abstract void DoEffect(Combat combat);

        public int priority;

        public bool isAtker;
    }

    public enum EffectTag
    {
        接触
    }
}
