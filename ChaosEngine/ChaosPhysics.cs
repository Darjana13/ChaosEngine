using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace ChaosEngine
{
    internal static class ChaosPhysics
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
        public static bool HasObject(ChaosObject obj)
        {
            return world.IndexOf(obj) != -1;
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
            DestroyObjects();
        }
        private static void DestroyObjects()
        {
            for (int i = 0; i < world.Count; i++)
                if (world[i].shouldBeDestroyed)
                {
                    world[i].setDestroyedToTrue();
                    world.RemoveAt(i);
                    i--;
                }
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
        public ChaosObject parent { get; private set; }
        public List<ChaosObject> childs = new List<ChaosObject>();
        public List<string> tags = new List<string>();
        internal bool shouldBeDestroyed { get; private set; } = false;
        private List<Component> components = new List<Component>() { new Transform() };
        private bool destroyed = false;
        public Component addComponent(Type type)
        {
            if (destroyed)
                throw new Exception("You're trying to access destroyed object.");

            Component comp = (Component)FormatterServices.GetUninitializedObject(type);
            type.GetProperty("parent", System.Reflection.BindingFlags.Instance).SetValue(comp, this);
            return comp;
        }
        public T addComponent<T>()
        {
            if (destroyed)
                throw new Exception("You're trying to access destroyed object.");

            Type type = typeof(T);
            if (!type.IsSubclassOf(typeof(Component)))
                throw new Exception(type.ToString() + " not a component!");
            T comp = (T)FormatterServices.GetUninitializedObject(typeof(T));
            (comp as Component).parent = this;
            return comp;
        }
        internal bool isDestroyed()
        {
            return destroyed;
        }
        internal void setDestroyedToTrue()
        {
            destroyed = true;
        }
        public void destroy()
        {
            if (destroyed)
                throw new Exception("You're trying to access destroyed object.");

            shouldBeDestroyed = true;
        }
        public void setParent(ChaosObject parent)
        {
            if (destroyed)
                throw new Exception("You're trying to access destroyed object.");

            if (ChaosPhysics.HasObject(parent))
                this.parent = parent;
        }
        internal void fixedUpdate()
        {
            foreach (Component comp in components)
                comp.fixedUpdate();
        }
        internal void update()
        {
            foreach (Component comp in components)
                comp.update();
        }
        internal void lateUpdate()
        {
            foreach (Component comp in components)
                comp.lateUpdate();
        }
    }
}
