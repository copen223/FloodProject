﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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


        public static string SignTypeName(SubType _type)
        {
            switch(_type)
            {
                case SubType.atk_blunt:
                    return "钝击";
                case SubType.atk_chop:
                    return "劈砍";
                case SubType.atk_stab:
                    return "突刺";
                case SubType.dfd_block:
                    return "格挡";
                case SubType.dfd_dodge:
                    return "闪避";
                case SubType.dfd_parry:
                    return "招架";
                case SubType.none:
                    return "";
            }
            return "";
        }

        public int intensity;
        public Type type;
        public SubType subType;
        public SignEffect effect;
        public Pos pos;
    }
}
