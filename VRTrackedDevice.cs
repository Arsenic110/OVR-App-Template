using System;
using System.Text;
using System.Numerics;
using System.Runtime.InteropServices;

using OVRSharp;
using OVRSharp.Math;
using Valve.VR;

namespace OVR_App_Template
{
    public abstract class VRTrackedDevice
    {
        public VRTrackedDeviceClass Type { get; protected set; }
        public uint DeviceID { get; protected set; }

        protected Matrix4x4 matrix;

        public Vector3 Position { get { return PositionFromMatrix(matrix); } }
        public Vector3 Rotation { get { return RotationFromMatrix(matrix); } }

        public VRTrackedDevice(uint DeviceID, VRTrackedDeviceClass Type)
        {
            this.DeviceID = DeviceID;
            this.Type = Type;
        }

        public abstract void Update();

        protected void PrintMatrix4x4(Matrix4x4 matrix)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("===========================================");
            Console.WriteLine($"{Pad(matrix.M11)} {Pad(matrix.M12)} {Pad(matrix.M13)}");
            Console.WriteLine($"{Pad(matrix.M21)} {Pad(matrix.M22)} {Pad(matrix.M23)}");
            Console.WriteLine($"{Pad(matrix.M31)} {Pad(matrix.M32)} {Pad(matrix.M33)}");
            Console.WriteLine($"{Pad(matrix.M41)} {Pad(matrix.M42)} {Pad(matrix.M43)}"); //X Y Z
        }

        protected void PrintPosition(Matrix4x4 matrix)
        {
            Console.WriteLine($"{Enum.GetName(this.Type)}({this.DeviceID}): || {Pad(matrix.M41)} {Pad(matrix.M42)} {Pad(matrix.M43)}");
        }

        protected void PrintRotation(Matrix4x4 matrix)
        {
            Vector3 rot = RotationFromMatrix(matrix);

            Console.WriteLine($"{Enum.GetName(this.Type)}({this.DeviceID}): || {Pad(rot.X)} {Pad(rot.Y)} {Pad(rot.Z)}");
        }

        protected Vector3 PositionFromMatrix(Matrix4x4 m)
        {
            return new Vector3(m.M41, m.M42, m.M43);
        }

        protected Vector3 RotationFromMatrix(Matrix4x4 m)
        {
            float pitch, yaw, roll;
            float[,] matrix =
            {
                { m.M11, m.M12, m.M13},
                { m.M21, m.M22, m.M23},
                { m.M31, m.M32, m.M33}
            };

            // Check for Gimbal lock, when matrix[2, 0] is +-1
            if (matrix[2, 0] > 0.998f) // Close to +1
            {
                pitch = (float)Math.Atan2(matrix[0, 1], matrix[0, 2]);
                yaw = (float)Math.PI / 2;
                roll = 0;
            }
            else if (matrix[2, 0] < -0.998f) // Close to -1
            {
                pitch = (float)Math.Atan2(matrix[0, 1], matrix[0, 2]);
                yaw = -(float)Math.PI / 2;
                roll = 0;
            }
            else
            {
                pitch = (float)Math.Atan2(-matrix[2, 1], matrix[2, 2]);
                yaw = (float)Math.Asin(matrix[2, 0]);
                roll = (float)Math.Atan2(-matrix[1, 0], matrix[0, 0]);
            }


            return new Vector3(RadToDeg(pitch), RadToDeg(yaw), RadToDeg(roll));
        }

        protected virtual void PrintDataToConsole()
        {
            Vector3 rot = RotationFromMatrix(matrix);
            Console.WriteLine($"{Enum.GetName(this.Type)}({this.DeviceID}): || X{Pad(matrix.M41)} Y{Pad(matrix.M42)} Z{Pad(matrix.M43)} || X{Pad(rot.X)} Y{Pad(rot.Y)} Z{Pad(rot.Z)}");
        }

        protected float RadToDeg(float rad)
        {
            return rad * (180f / (float)Math.PI);
        }

        protected string Pad(object inp)
        {
            const int length = 5;

            bool negative = false;

            string rawInput = inp.ToString() ?? "";
            string input = "";

            if (rawInput[0] == '-')
            {
                negative = true;
                rawInput = rawInput.Substring(1);
            }

            for(int i = 0; i < length; i++)
            {
                if (i >= rawInput.Length)
                    input += '0';
                else
                    input += rawInput[i];
            }

            return negative ? "-" + input.PadRight(length, '0') : "+" + input.PadRight(length, '0');
        }

        protected string unToBinary(ulong inp)
        {
            return Convert.ToString((long)inp, 2).PadLeft(64, '0');
        }
    }
}
