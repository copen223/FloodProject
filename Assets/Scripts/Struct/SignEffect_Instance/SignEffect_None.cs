using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Struct
{
    public class SignEffect_None : SignEffect
    {
        public override void DoEffect(Combat combat)
        {

        }

        public SignEffect_None()
        {
            name = "空白";
        }
    }
}
