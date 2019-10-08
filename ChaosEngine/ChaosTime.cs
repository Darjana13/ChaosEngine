using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ChaosEngine
{
    public static class ChaosTime
    {
        public static double deltaTime { get; private set; }
        public static double fixedDeltaTime { get; private set; } = 0.04; // ~25 physical frames per second
        private static Stopwatch time;

        public static void Initialize()
        {
            time = new Stopwatch();
            time.Start();
        }
        public static void UpdateTime()
        {
            time.Stop();
            deltaTime = time.ElapsedMilliseconds / 1000.0;
            time.Restart();
        }
    }
}
