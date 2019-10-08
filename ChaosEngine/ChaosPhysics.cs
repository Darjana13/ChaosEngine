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
            CountMatrices();
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
        private static void CountMatrices()
        {
            foreach (ChaosObject obj in world)
                if (obj.parent == null)
                    obj.countMatrices();
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
            foreach (ChaosObject obj in world)
                foreach (Collider col in obj.getComponents<Collider>())
                    col.applyChanges();
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
    public sealed class ChaosObject
    {
        public bool enabled { get { if (parent != null && !parent.enabled) return false; return _enabled; } set { _enabled = value; } }
        public string name { get; private set; } = "object";
        public ChaosObject parent { get; private set; }
        public List<string> tags { get; private set; } = new List<string>();
        public Transform transform { get { return components[0] as Transform; } }
        internal bool shouldBeDestroyed { get; private set; } = false;
        private List<ChaosObject> childs = new List<ChaosObject>();
        private List<Component> components = new List<Component>() { new Transform() };
        private bool destroyed = false;
        private bool _enabled = true;

        public ChaosObject()
        {
            addComponent<Transform>();
        }
        public void setName(string name)
        {
            if (destroyed)
                throw new Exception("You're trying to access destroyed object.");

            this.name = name;
        }
        public Component addComponent(Type type)
        {
            if (destroyed)
                throw new Exception("You're trying to access destroyed object.");
            if (!type.IsSubclassOf(typeof(Component)))
                throw new Exception("Type must be inherited from component!");
            if (type.IsDefined(typeof(SinglePerObject), false) && getComponent(type) != null)
                throw new Exception("Only one component of this type per object.");

            Component comp = (Component)FormatterServices.GetUninitializedObject(type);
            comp.parent = this;
            comp.awake();
            return comp;
        }
        public T addComponent<T>() where T : Component
        {
            if (destroyed)
                throw new Exception("You're trying to access destroyed object.");
            if (typeof(T).IsDefined(typeof(SinglePerObject), false) && getComponent<T>() != null)
                throw new Exception("Only one component of this type per object.");

            T comp = (T)FormatterServices.GetUninitializedObject(typeof(T));
            comp.parent = this;
            comp.awake();
            return comp;
        }
        public Component getComponent(Type type)
        {
            if (!type.IsSubclassOf(typeof(Component)))
                throw new Exception("Type must be inherited from component!");
            foreach (Component comp in components)
                if (type.IsInstanceOfType(comp))
                    return comp;
            return null;
        }
        public T getComponent<T>() where T : Component
        {
            foreach (Component comp in components)
                if (comp is T)
                    return (T)comp;
            return null;
        }
        public Component[] getComponents(Type type)
        {
            if (!type.IsSubclassOf(typeof(Component)))
                throw new Exception("Type must be inherited from component!");
            List<Component> comps = new List<Component>();
            foreach (Component comp in components)
                if (type.IsInstanceOfType(comp))
                    comps.Add(comp);
            return comps.ToArray();
        }
        public T[] getComponents<T>() where T : Component
        {
            List<T> comps = new List<T>();
            foreach (Component comp in components)
                if (comp is T)
                    comps.Add((T)comp);
            return comps.ToArray();
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
            {
                if (this.parent != null)
                    this.parent.removeChild(this);
                this.parent = parent;
                this.parent.addChild(this);
            }
        }
        public void addChild(ChaosObject child)
        {
            childs.Add(child);
        }
        public void removeChild(ChaosObject child)
        {
            childs.Remove(child);
        }
        internal bool isDestroyed()
        {
            return destroyed;
        }
        internal void setDestroyedToTrue()
        {
            destroyed = true;
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
        internal void countMatrices()
        {
            transform.countMatrices();
            foreach (ChaosObject obj in childs)
                obj.countMatrices();
        }
    }
}
