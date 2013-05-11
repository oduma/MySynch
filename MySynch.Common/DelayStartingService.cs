using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace MySynch.Common
{
    public class DelayStartingService:WcfHostServiceBase
    {
        protected Timer Timer;

        protected void StartTimer(double timerInterval, ElapsedEventHandler timerElapsedMethod)
        {
            Timer = new Timer();
            Timer.Interval = 60000;
            Timer.Elapsed += timerElapsedMethod;
            Timer.Enabled = true;
            Timer.Start();

        }
    }
}
