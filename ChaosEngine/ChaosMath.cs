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
    // TODO: write this shit
    class Quaternion
    {

    }
}
