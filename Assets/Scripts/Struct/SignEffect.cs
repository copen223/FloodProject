using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Struct
{
    public abstract class SignEffect
    {
        public int priority;

        public string name;

        public abstract void DoEffect(Combat combat);

        public bool isAtker;
    }
}
