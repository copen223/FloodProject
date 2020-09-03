using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Struct
{
    public class CardSign
    {
        public enum Type
        {
            none,
            atk,
            dfd
        }

        public enum SubType
        {
            none,
            atk_chop,
            atk_stab,
            atk_blunt,
            dfd_dodge,
            dfd_block,
            dfd_parry
        }

        public enum Pos
        {
            up,
            down
        }


        public int intensity;
        public Type type;
        public SubType subType;
        public SignEffect effect;
        public Pos pos;

    }
}
