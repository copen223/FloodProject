using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Struct
{
    public class Card
    {
        public string cardName;
        public ActorMono holder;
        public CardSign sign_up;
        public CardSign sign_down;
        public List<CardEffect> effects_list = new List<CardEffect>();
        public string cast_type;
        public int memory_cost;
        public int cast_extent_x;
        public int cast_extent_y;

        public bool isFocused;
        public int focusCount;


        public float damage_multiply;

        public bool IfCanCast(ActorMono actor)
        {
            float dis_x = UnityEngine.Mathf.Abs(actor.WorldPos.x - holder.WorldPos.x);
            float dis_y = UnityEngine.Mathf.Abs(actor.WorldPos.y - holder.WorldPos.y);

            if (dis_x > cast_extent_x + 0.5f || dis_y > cast_extent_y)
            {
                return false;
            }
            else
                return true;
        }
    }
}
