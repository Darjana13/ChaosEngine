using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChaosEngine
{
    abstract class Component
    {
        public abstract void fixedUpdate();
        public abstract void update();
        public abstract void lateUpdate();
    }
}
