using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChaosEngine
{
    abstract class Component
    {
        public ChaosObject parent { get; internal set; }
        public virtual void fixedUpdate() { }
        public virtual void update() { }
        public virtual void lateUpdate() { }
    }

    class Transform : Component
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        //public Matrix4 localToWorldMatrix()
        //{
        //
        //}
        //public Matrix4 worldToLocalMatrix()
        //{
        //
        //}
        //public Vector3 forward()
        //{
        //
        //}
        //public Vector3 up()
        //{
        //
        //}
    }

    class Mesh : Component
    {
        private Model model;
    }
}
