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

        // !!!RIGHT-HANDED COORDINATE SYSTEM!!! x - right, y - up, z - forward
        public static Vector3 right { get { return new Vector3(1, 0, 0); } }
        public static Vector3 left { get { return new Vector3(-1, 0, 0); } }
        public static Vector3 forward { get { return new Vector3(0, 0, 1); } }
        public static Vector3 back { get { return new Vector3(0, 0, -1); } }
        public static Vector3 up { get { return new Vector3(0, 1, 0); } }
        public static Vector3 down { get { return new Vector3(0, -1, 0); } }

        private static double epsilon = 1e-6;
        public double x { get; private set; }
        public double y { get; private set; }
        public double z { get; private set; }
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
        public static Vector3 operator*(double value, Vector3 vec)
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
        public Vector3 compMul(Vector3 vec)
        {
            return new Vector3(x * vec.x, y * vec.y, z * vec.z);
        }
        /// <summary>
        /// Returns normalized copy of this vector
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
    class Matrix4
    {
        private double[,] values = new double[4,4]; // [rowIndex, columnIndex]
        public Matrix4()
        {

        }
        public Matrix4(double[] values)
        {
            for (int i = 0; i < values.Length && i < 16; i++)
                this.values[i / 4, i % 4] = values[i];
        }
        /// <summary>
        /// Transposes this matrix
        /// </summary>
        public void transpose()
        {
            double temp;
            for (int i = 0; i < 3; i++)
                for (int j = i + 1; j < 4; j++)
                {
                    temp = values[i, j];
                    values[i, j] = values[j, i];
                    values[j, i] = temp;
                }
        }
        /// <summary>
        /// Returns transposed copy of this matrix
        /// </summary>
        public Matrix4 transposed()
        {
            Matrix4 newMatrix = new Matrix4();
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    newMatrix.values[i, j] = values[j, i];
            return newMatrix;
        }
        /// <summary>
        /// Returns identity matrix
        /// </summary>
        public static Matrix4 Identity()
        {
            return new Matrix4(new double[] { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 });
        }
        public static Matrix4 operator*(Matrix4 mat1, Matrix4 mat2)
        {
            Matrix4 newMatrix = new Matrix4();
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    for (int c = 0; c < 4; c++)
                        newMatrix.values[i, j] = mat1.values[i, c] * mat2.values[c, j];
            return newMatrix;
        }
        /// <summary>
        /// Multiplies matrix by vector, where vector represents point in space (vector = (x, y, z, 1))
        /// </summary>
        public Vector3 multByPoint(Vector3 vec)
        {
            return new Vector3(values[0, 0], values[1, 0], values[2, 0]) * vec.x +
                   new Vector3(values[0, 1], values[1, 1], values[2, 1]) * vec.y +
                   new Vector3(values[0, 2], values[1, 2], values[2, 2]) * vec.z +
                   new Vector3(values[0, 3], values[1, 3], values[2, 3]);
        }
        /// <summary>
        /// Multiplies matrix by vector, where vector represents direction (vector = (x, y, z, 0))
        /// </summary>
        public Vector3 multByDirection(Vector3 vec)
        {
            return new Vector3(values[0, 0], values[1, 0], values[2, 0]) * vec.x +
                   new Vector3(values[0, 1], values[1, 1], values[2, 1]) * vec.y +
                   new Vector3(values[0, 2], values[1, 2], values[2, 2]) * vec.z;
        }
    }
    // do NOT ask me about this, i don't give a fuck how this works.
    class Quaternion
    {
        private double w, x, y, z;
        public Quaternion(double w = 0, double x = 0, double y = 0, double z = 0)
        {
            this.w = w;
            this.x = x;
            this.y = y;
            this.z = z;
        }
        /// <summary>
        /// Normalizes this quaternion
        /// </summary>
        public void normalize()
        {
            double length = magnitude();
            w /= length;
            x /= length;
            y /= length;
            z /= length;
        }
        /// <summary>
        /// Returns normalized copy of this quaternion
        /// </summary>
        public Quaternion normalized()
        {
            double length = magnitude();
            return new Quaternion(w / length, x / length, y / length, z / length);
        }
        /// <summary>
        /// Returns magnitude of this quaternion, equal to length()
        /// </summary>
        public double magnitude()
        {
            return Math.Sqrt(w * w + x * x + y * y + z * z);
        }
        /// <summary>
        /// Returns length of this quaternion, equal to magnitude()
        /// </summary>
        public double length()
        {
            return magnitude();
        }
        /// <summary>
        /// returns squared magnitude of this quaternion, equal to squaredLength()
        /// </summary>
        public double squaredMagnitude()
        {
            return w * w + x * x + y * y + z * z;
        }
        /// <summary>
        /// returns squared length of this quaternion, equal to squaredMagnitude()
        /// </summary>
        public double squaredLength()
        {
            return squaredMagnitude();
        }
        /// <summary>
        /// Returns inversed copy of this quaternion (conjugated/squaredMagnitude)
        /// </summary>
        public Quaternion inversed()
        {
            Quaternion q = conjugated();
            double magn = squaredMagnitude();
            q.w /= magn;
            q.x /= magn;
            q.y /= magn;
            q.z /= magn;
            return q;
        }
        /// <summary>
        /// Inverses this quaternion (conjugated/squaredMagnitude)
        /// </summary>
        public void inverse()
        {
            double magn = squaredMagnitude();
            conjugate();
            w /= magn;
            x /= magn;
            y /= magn;
            z /= magn;
        }
        /// <summary>
        /// Conjugates this vector (w, -x, -y, -z)
        /// </summary>
        public void conjugate()
        {
            x = -x;
            y = -y;
            z = -z;
        }
        /// <summary>
        /// Returns conjugated copy of this quaternion (w, -x, -y, -z)
        /// </summary>
        public Quaternion conjugated()
        {
            return new Quaternion(w, -x, -y, -z);
        }
        /// <summary>
        /// Returns quaternion (1, 0, 0, 0)
        /// </summary>
        public static Quaternion Identity()
        {
            return new Quaternion(1);
        }
        /// <summary>
        /// Combines 2 rotations. Important: Second rotation comes first.
        /// </summary>
        public static Quaternion operator*(Quaternion q1, Quaternion q2)
        {
            return new Quaternion(q1.w * q2.w - q1.x * q2.x - q1.y - q2.y - q1.z * q2.z,
                                  q1.w * q2.x + q1.x * q2.w + q1.y * q2.z - q1.z * q2.y,
                                  q1.w * q2.y + q1.y * q2.w + q1.x * q2.z - q1.z * q2.x,
                                  q1.w * q2.z + q1.z * q2.w + q1.x * q2.y - q1.y * q2.x);
        }
        public static Quaternion operator*(Quaternion q, Vector3 v)
        {
            return new Quaternion(- q.x * v.x - q.y - v.y - q.z * v.z,
                                  q.w * v.x + q.y * v.z - q.z * v.y,
                                  q.w * v.y + q.x * v.z - q.z * v.x,
                                  q.w * v.z + q.x * v.y - q.y * v.x);
        }
        /// <summary>
        /// Rotates vector v by quaternion q
        /// </summary>
        public Vector3 rotateVector(Vector3 vec)
        {
            Quaternion result = this * vec * inversed();
            return new Vector3(result.x, result.y, result.z);
        }
        /// <summary>
        /// Returns rotation matrix equal to this quaternion
        /// </summary>
        public Matrix4 toRotationMatrix()
        {
            return new Matrix4(new double[] { 1-2*(y*y+z*z), 2*(x*y-w*z), 2*(x*z+w*y), 0,
                                              2*(x*y+w*z), 1-2*(x*x+z*z), 2*(y*z-w*x), 0,
                                              2*(x*z-w*y), 2*(y*z+w*x), 1-2*(x*x+y*y), 0,
                                              0, 0, 0, 1 });
        }
        /// <summary>
        /// Returns quaternion represented by rotation around axis
        /// </summary>
        public static Quaternion FromAxisAngle(Vector3 axis, double angle)
        {
            double cos = Math.Cos(angle / 2.0);
            double sin = Math.Sin(angle / 2.0);
            return new Quaternion(cos, sin * axis.x, sin * axis.y, sin * axis.z);
        }
        /// <summary>
        /// Returns quaternion representation of rotation in eulers. Order: YXZ
        /// </summary>
        public static Quaternion fromEuler(Vector3 eulers)
        {
            return FromAxisAngle(Vector3.forward, eulers.z) *
                   FromAxisAngle(Vector3.right, eulers.x) *
                   FromAxisAngle(Vector3.up, eulers.y);
        }
        /// <summary>
        /// Returns eulers representation of rotation in this quaternion
        /// </summary>
        public Vector3 toEuler()
        {
            if (x * y + z * w == 0.5)
                return new Vector3(Math.Asin(2 * x * y + 2 * z * w), 2 * Math.Atan2(x, w), 0);
            else
            if (x * y + z * w == -0.5)
                return new Vector3(Math.Asin(2 * x * y + 2 * z * w), -2 * Math.Atan2(x, w), 0);
            else
                return new Vector3(Math.Asin(2 * x * y + 2 * z * w), Math.Atan2(2 * y * w - 2 * x * z, 1 - 2 * y * y - 2 * z * z), Math.Atan2(2 * x * w - 2 * y * z, 1 - 2 * x * x - 2 * z * z));
        }
    }
}