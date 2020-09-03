using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Struct
{
    public class Timer
    {
        public float timer;

        public void ReStart()
        {
            timer = 0;
        }

        public void TimerTick(float timeChange)
        {
            timer += timeChange;
        }

        public bool IsOver(float maxTime)
        {
            return maxTime < timer;
        }
    }
}
