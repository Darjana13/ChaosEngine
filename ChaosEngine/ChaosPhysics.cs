using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ChaosEngine
{
    static class ChaosPhysics
    {
        private static List<ChaosObject> world = new List<ChaosObject>();
        private static double timeSinceLastFixedUpdate = 0;
        /// <summary>
        /// Returns copy of world list
        /// </summary>
        public static List<ChaosObject> GetWorld()
        {
            return new List<ChaosObject>(world);
        }
        public static void Frame()
        {
            world.RemoveAll((obj) => { return obj.shouldBeDestroyed; });
            timeSinceLastFixedUpdate += ChaosTime.deltaTime;
            if (timeSinceLastFixedUpdate > ChaosTime.fixedDeltaTime)
            {
                timeSinceLastFixedUpdate = 0;
                FixedUpdate();
            }
            Update();
            LateUpdate();
        }
        private static void FixedUpdate()
        {
            foreach (ChaosObject obj in world)
                obj.fixedUpdate();
        }
        private static void Update()
        {
            foreach (ChaosObject obj in world)
                obj.update();
        }
        private static void LateUpdate()
        {
            foreach (ChaosObject obj in world)
                obj.lateUpdate();
        }
        // ????????????????????????????????????????
        //public static ChaosObject Instantiate(ChaosObject obj)
        //{
        //
        //}
    }
    class ChaosObject
    {
        public string name = "object";
        public ChaosObject parent = null;
        public List<ChaosObject> childs = new List<ChaosObject>();
        public List<string> tags = new List<string>();
        private List<Component> components = new List<Component>();
        public bool shouldBeDestroyed { get; private set; } = false;
        public void destroy()
        {
            shouldBeDestroyed = true;
        }
        public void fixedUpdate()
        {
            foreach (Component comp in components)
                comp.fixedUpdate();
        }
        public void update()
        {
            foreach (Component comp in components)
                comp.update();
        }
        public void lateUpdate()
        {
            foreach (Component comp in components)
                comp.lateUpdate();
        }
    }
}
