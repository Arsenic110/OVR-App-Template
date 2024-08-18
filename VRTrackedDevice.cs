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
        public Vector3 Position { get; private set; }
        public Vector3 Rotation { get; private set; }

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

                Matrix4x4 matrix = pose.mDeviceToAbsoluteTracking.ToMatrix4x4();

                PrintPosition(matrix);
            }
            else
            {
                PrintPosition(ControllerStateToMatrix());
            }
        }

        private Matrix4x4 ControllerStateToMatrix()
        {
            VRControllerState_t state = new VRControllerState_t();
            TrackedDevicePose_t pose = new TrackedDevicePose_t();

            OpenVR.System.GetControllerStateWithPose(ETrackingUniverseOrigin.TrackingUniverseStanding, this.DeviceID, ref state, 1, ref pose);

            Matrix4x4 m = pose.mDeviceToAbsoluteTracking.ToMatrix4x4();

            return m;
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
            Console.WriteLine($"{this.DeviceID}: || {Pad(matrix.M41)} {Pad(matrix.M42)} {Pad(matrix.M43)}");
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
