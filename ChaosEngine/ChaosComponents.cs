using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChaosEngine
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    sealed class SinglePerObject : Attribute { }
    [System.AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    sealed class AlwaysEnabled : Attribute { }
    public abstract class Component
    {
        public bool enabled { get; set; }
        public ChaosObject parent { get; internal set; }
        /// <summary>
        /// Called when component is added to an object
        /// </summary>
        public virtual void awake() { }
        /// <summary>
        /// Called every physical frame
        /// </summary>
        public virtual void fixedUpdate() { }
        /// <summary>
        /// Called every frame
        /// </summary>
        public virtual void update() { }
        /// <summary>
        /// Called every frame after all update()
        /// </summary>
        public virtual void lateUpdate() { }
    }

    [SinglePerObject]
    [AlwaysEnabled]
    public sealed class Transform : Component
    {
        public Vector3 position { get; set; }
        public Quaternion rotation { get; set; }
        public Vector3 scale { get; set; }
        public Vector3 forward { get { return localToWorldMatrix.multByDirection(Vector3.forward); } }
        public Vector3 right { get { return localToWorldMatrix.multByDirection(Vector3.right); } }
        public Vector3 up { get { return localToWorldMatrix.multByDirection(Vector3.up); } }
        public Vector3 back { get { return localToWorldMatrix.multByDirection(Vector3.back); } }
        public Vector3 left { get { return localToWorldMatrix.multByDirection(Vector3.left); } }
        public Vector3 down { get { return localToWorldMatrix.multByDirection(Vector3.down); } }
        public Matrix4 localToWorldMatrix { get; private set; }
        public Matrix4 worldToLocalMatrix { get; private set; }
        internal void countMatrices()
        {
            Matrix4 mat = rotation.toRotationMatrix();
            mat.v03 += position.x;
            mat.v13 += position.y;
            mat.v23 += position.z;
            mat.v00 *= scale.x;
            mat.v10 *= scale.x;
            mat.v20 *= scale.x;
            mat.v01 *= scale.y;
            mat.v11 *= scale.y;
            mat.v21 *= scale.y;
            mat.v02 *= scale.z;
            mat.v12 *= scale.z;
            mat.v22 *= scale.z;

            if (parent.parent != null)
                mat *= parent.parent.transform.worldToLocalMatrix;
            worldToLocalMatrix = mat;
            localToWorldMatrix = mat.inversed();
        }

        public override void awake()
        {
            position = Vector3.zero;
            rotation = Quaternion.Identity();
            scale = new Vector3(1, 1, 1);
        }
    }

    public sealed class Mesh : Component
    {
        private Model model;
    }

    public enum RigidbodyContraints
    {
        None = 0,
        positionX = 1,
        positionY = 2,
        positionZ = 4,
        position = 7,
        rotationX = 8,
        rotationY = 16,
        rotationZ = 32,
        rotation = 56,
        all = 64
    }
    public static class RigidbodyConstraintsExtend
    {
        public static bool isFreezed(this RigidbodyContraints con, RigidbodyContraints constraint)
        {
            return (con & constraint) == constraint;
        }                                           
    }

    [SinglePerObject]
    public sealed class Rigidbody : Component
    {
        public Vector3 velocity { get; set; }
        public Vector3 angularVelocty { get; set; }
        public double mass { get { return _mass; } set { if (value <= 0) throw new Exception("Mass can't be zero or negative."); _mass = value; } }
        public double drag { get { return _drag; } set { if (value < 0 || value > 1) throw new Exception("Drag must be in range [0, 1]."); _drag = value; } }
        public double angularDrag { get { return _angularDrag; } set { if (value < 0 || value > 1) throw new Exception("AngularDrag must be in range [0, 1]."); _angularDrag = value; } }
        public bool useGravity { get; set; }
        public RigidbodyContraints constraints { get; set; }
        public bool isStatic { get; set; }
        private double _mass;
        private double _drag;
        private double _angularDrag;
        public override void awake()
        {
            velocity = Vector3.zero;
            angularVelocty = Vector3.zero;
            mass = 1;
            drag = 0;
            angularDrag = 0;
            constraints = RigidbodyContraints.None;
            isStatic = false;
            useGravity = true;
        }
        public Vector3 inertiaTensor()
        {
            return new Vector3(1, 1, 1);
            // calculate inertia tensor
            // i don't give a fuck what is this (i guess it's like combination of inertia moments for base vectors) so good luck :DD
        }
        public void applyForce(Vector3 force)
        {
            applyImpulse(force / ChaosTime.fixedDeltaTime);
        }
        public void applyForce(Vector3 force, Vector3 point)
        {
            applyImpulse(force / ChaosTime.fixedDeltaTime, point);
        }
        public void applyImpulse(Vector3 impulse)
        {
            // applying impulse to object center
        }
        public void applyImpulse(Vector3 impulse, Vector3 point)
        {
            // applying impulse to point (in global space)
        }
        public override void fixedUpdate()
        {
            // moving and rotating object
        }
    }

    public enum PhysicMaterialCombine
    {
        Minimum,
        Maximum,
        Multiply,
        Average,
        DiminishingUtility
    }

    public sealed class PhysicMaterial
    {
        public double bounciness { get { return _bounciness; } set { if (value > 1 || value < 0) throw new ArgumentOutOfRangeException(); _bounciness = value; } }
        public double dynamicFriction { get { return _dynamicFriction; } set { if (value > 1 || value < 0) throw new ArgumentOutOfRangeException(); _dynamicFriction = value; } }
        public double staticFriction { get { return _staticFriction; } set { if (value > 1 || value < 0) throw new ArgumentOutOfRangeException(); _staticFriction = value; } }
        public PhysicMaterialCombine bouncinessCombine { get; set; }
        public PhysicMaterialCombine frictionCombine { get; set; }
        private double _bounciness;
        private double _dynamicFriction;
        private double _staticFriction;

        public PhysicMaterial(double bounciness = 0.25, double dynamicFriction = 0.4, double staticFriction = 0.75)
        {
            this.bounciness = bounciness;
            this.dynamicFriction = dynamicFriction;
            this.staticFriction = staticFriction;
        }
        public double combineBouncinessWith(PhysicMaterial other)
        {
            switch (bouncinessCombine)
            {
                case PhysicMaterialCombine.Minimum:
                    return Math.Min(bounciness, other.bounciness);
                case PhysicMaterialCombine.Maximum:
                    return Math.Max(bounciness, other.bounciness);
                case PhysicMaterialCombine.Average:
                    return (bounciness + other.bounciness) / 2.0;
                case PhysicMaterialCombine.Multiply:
                    return bounciness * other.bounciness;
                case PhysicMaterialCombine.DiminishingUtility:
                    return bounciness + (1 - bounciness) * other.bounciness;
            }
            throw new NotImplementedException();
        }
        public double combineDynamicFrictionWith(PhysicMaterial other)
        {
            switch (frictionCombine)
            {
                case PhysicMaterialCombine.Minimum:
                    return Math.Min(dynamicFriction, other.dynamicFriction);
                case PhysicMaterialCombine.Maximum:
                    return Math.Max(dynamicFriction, other.dynamicFriction);
                case PhysicMaterialCombine.Average:
                    return (dynamicFriction + other.dynamicFriction) / 2.0;
                case PhysicMaterialCombine.Multiply:
                    return dynamicFriction * other.dynamicFriction;
                case PhysicMaterialCombine.DiminishingUtility:
                    return dynamicFriction + (1 - dynamicFriction) * other.dynamicFriction;
            }
            throw new NotImplementedException();
        }
        public double combineStaticFrictionWith(PhysicMaterial other)
        {
            switch (frictionCombine)
            {
                case PhysicMaterialCombine.Minimum:
                    return Math.Min(staticFriction, other.staticFriction);
                case PhysicMaterialCombine.Maximum:
                    return Math.Max(staticFriction, other.staticFriction);
                case PhysicMaterialCombine.Average:
                    return (staticFriction + other.staticFriction) / 2.0;
                case PhysicMaterialCombine.Multiply:
                    return staticFriction * other.staticFriction;
                case PhysicMaterialCombine.DiminishingUtility:
                    return staticFriction + (1 - staticFriction) * other.staticFriction;
            }
            throw new NotImplementedException();
        }
    }

    public struct CollisionInfo
    {

    }

    public abstract class Collider : Component
    {
        public bool isTrigger { get; set; }
        public PhysicMaterial physicMaterial { get; set; }
        public Vector3 offset { get; set; }
        protected List<Vector3> vertices { get { return _vertices; } set { _vertices = value; recountRadius(); } }
        protected List<Vector3> _vertices;
        protected List<int[]> polygons { get; set; }
        private double outerSphereRadius { get; set; } // squared
        private double staticFrictionEpsilon;
        public override void awake()
        {
            physicMaterial = new PhysicMaterial();
            offset = Vector3.zero;
            isTrigger = false;
            polygons = new List<int[]>();
            _vertices = new List<Vector3>();
            outerSphereRadius = 0;
            staticFrictionEpsilon = 1e-9;
        }
        private void recountRadius()
        {
            foreach (Vector3 vertex in vertices)
            {
                double curMag = vertex.squaredMagnitude();
                if (curMag > outerSphereRadius)
                    outerSphereRadius = curMag;
            }
        }
        internal void applyChanges()
        {
            // after-collision applying changes
        }
        public static CollisionInfo CheckCollisionFor(Collider col1, Collider col2)
        {
            // checking all collision here
            return new CollisionInfo();
        }
        public static void HandleCollisionFor(Collider col1, Collider col2)
        {
            // resolving all collisions here
        }
    }
}
