using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChaosEngine
{
    class Vector3
    {
        public static Vector3 zero { get { return new Vector3(); } }
        private static double epsilon = 1e-6;
        double x, y, z;
        public Vector3(double x = 0, double y = 0, double z = 0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        /// <summary>
        /// Magnitude of vector. Equals to length()
        /// </summary>
        public double magnitude()
        {
            return Math.Sqrt(squaredMagnitude());
        }
        /// <summary>
        /// Magnitude of vector without root. Equals to squaredLength()
        /// </summary>
        public double squaredMagnitude()
        {
            return scalMul(this);
        }
        /// <summary>
        /// Length of vector. Equals to magnitude()
        /// </summary>
        public double length()
        {
            return magnitude();
        }
        /// <summary>
        /// Length of vector without root. Equals to squaredMagnitude()
        /// </summary>
        public double squaredLength()
        {
            return squaredMagnitude();
        }
        /// <summary>
        /// Checks if vector small enough to be considered a zero vector
        /// </summary>
        public bool isZero()
        {
            return squaredMagnitude() < epsilon;
        }
        public static Vector3 operator+(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }
        public static Vector3 operator-(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }
        public static Vector3 operator*(Vector3 vec, double value)
        {
            return new Vector3(vec.x * value, vec.y * value, vec.z * value);
        }
        public static Vector3 operator/(Vector3 vec, double value)
        {
            return new Vector3(vec.x / value, vec.y / value, vec.z / value);
        }
        /// <summary>
        /// Scalar multiplication
        /// </summary>
        public double scalMul(Vector3 vec)
        {
            return x * vec.x + y * vec.y + z * vec.z;
        }
        /// <summary>
        /// Scalar multiplication
        /// </summary>
        public static double operator*(Vector3 v1, Vector3 v2)
        {
            return v1.scalMul(v2);
        }
        /// <summary>
        /// Vector multiplication
        /// </summary>
        public Vector3 vecMul(Vector3 vec)
        {
            return new Vector3(y * vec.z - z * vec.y, x * vec.z - z * vec.x, x * vec.y - y * vec.x);
        }
        /// <summary>
        /// Vector multiplication
        /// </summary>
        public static Vector3 operator%(Vector3 v1, Vector3 v2)
        {
            return v1.vecMul(v2);
        }
        /// <summary>
        /// Component multiplication
        /// </summary>
        /// <returns>New vector - (x1*x2, y1*y2, z1*z2)</returns>
        public static Vector3 compMul(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
        }
        /// <summary>
        /// Returns normalized new vector
        /// </summary>
        public Vector3 normalized()
        {
            return this / magnitude();
        }
        /// <summary>
        /// Normalizes this vector
        /// </summary>
        public void normalize()
        {
            double magn = magnitude();
            x /= magn;
            y /= magn;
            z /= magn;
        }
        /// <summary>
        /// Checks if vectors same enough to be considered equal
        /// </summary>
        public bool equals(Vector3 vec)
        {
            return (vec - this).isZero();
        }
        /// <summary>
        /// Projects vector on another vector
        /// </summary>
        public Vector3 projectOnVector(Vector3 vec)
        {
            if (vec.isZero())
                return Vector3.zero;
            return vec * (this * vec / vec.squaredMagnitude());
        }
        /// <summary>
        /// Projects vector on flat
        /// </summary>
        /// <param name="flatNorm">Normal vector to flat (not necessary normalized)</param>
        /// <returns></returns>
        public Vector3 projectOnFlat(Vector3 flatNorm)
        {
            return this - flatNorm * (this * flatNorm / flatNorm.squaredMagnitude());
        }
        /// <summary>
        /// Checks if vectors are located on parallel lines
        /// </summary>
        /// <returns>True if vectors are located on parallel lines, false otherwise</returns>
        public bool isCollinearTo(Vector3 vec)
        {
            return (this % vec).isZero();
        }
        public override string ToString()
        {
            return "(" + x.ToString() + ", " + y.ToString() + ", " + z.ToString() + ")";
        }
    }
}
