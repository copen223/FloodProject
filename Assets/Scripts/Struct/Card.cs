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
    }
}
