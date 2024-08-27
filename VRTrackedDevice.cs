using System;
using System.Text;
using System.Numerics;

using OVRSharp;
using OVRSharp.Math;
using Valve.VR;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace OVR_App_Template
{
    public class VRTrackedDevice
    {
        public VRTrackedDeviceClass Type { get; private set; }
        public uint DeviceID { get; private set; }

        private Matrix4x4 matrix;

        public Vector3 Position { get { return PositionFromMatrix(matrix); } }
        public Vector3 Rotation { get { return RotationFromMatrix(matrix); } }

        public VRTrackedDevice(uint DeviceID, VRTrackedDeviceClass Type)
        {
            this.DeviceID = DeviceID;
            this.Type = Type;
        }

        public void Update()
        {
            if(this.Type == VRTrackedDeviceClass.HMD)
            {
                TrackedDevicePose_t[] trackedDevicePoses = new TrackedDevicePose_t[1];

                OpenVR.System.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseStanding, 0, trackedDevicePoses);

                TrackedDevicePose_t pose = trackedDevicePoses[0];

                matrix = pose.mDeviceToAbsoluteTracking.ToMatrix4x4();
            }
            else
            {
                VRControllerState_t state = new VRControllerState_t();
                TrackedDevicePose_t pose = new TrackedDevicePose_t();

                OpenVR.System.GetControllerStateWithPose(ETrackingUniverseOrigin.TrackingUniverseStanding, this.DeviceID, ref state, 1, ref pose);

                matrix = pose.mDeviceToAbsoluteTracking.ToMatrix4x4();
            }

            PrintPosition(matrix);
            Console.WriteLine(this.Rotation.ToString());
        }

        private void PrintMatrix4x4(Matrix4x4 matrix)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("===========================================");
            Console.WriteLine($"{Pad(matrix.M11)} {Pad(matrix.M12)} {Pad(matrix.M13)}");
            Console.WriteLine($"{Pad(matrix.M21)} {Pad(matrix.M22)} {Pad(matrix.M23)}");
            Console.WriteLine($"{Pad(matrix.M31)} {Pad(matrix.M32)} {Pad(matrix.M33)}");
            Console.WriteLine($"{Pad(matrix.M41)} {Pad(matrix.M42)} {Pad(matrix.M43)}"); //X Y Z
        }
        private void PrintPosition(Matrix4x4 matrix)
        {
            Console.WriteLine($"{Enum.GetName(this.Type)}({this.DeviceID}): || {Pad(matrix.M41)} {Pad(matrix.M42)} {Pad(matrix.M43)}");
        }

        private Vector3 PositionFromMatrix(Matrix4x4 m)
        {
            return new Vector3(m.M41, m.M42, m.M43);
        }
        private Vector3 RotationFromMatrix(Matrix4x4 m)
        {
            float pitch, yaw, roll;
            float[,] matrix =
            {
                { m.M11, m.M12, m.M13},
                { m.M21, m.M22, m.M23},
                { m.M31, m.M32, m.M33}
            };

            // Check for Gimbal lock, when matrix[2, 0] is ±1
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

        private float RadToDeg(float rad)
        {
            return rad * (180f / (float)Math.PI);
        }

        private string Pad(object inp)
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

    }
}
