using OVRSharp;
using OVRSharp.Math;
using System.Runtime.InteropServices;
using Valve.VR;
using System.Linq;

namespace OVR_App_Template.Devices
{
    public class VRController : VRTrackedDevice
    {
        public Control ButtonA;
        public Control ButtonB;
        public Control Trigger;
        public Control Joystick;
        public Control Trackpad;
        public Control Grip;

        public VRController(uint DeviceID, VRTrackedDeviceClass Type) : base(DeviceID, Type)
        {
            ButtonA = new();
            ButtonB = new();
            Trigger = new();
            Joystick = new();
            Trackpad = new();
            Grip = new();
        }

        public override void Update()
        {
            VRControllerState_t state = new VRControllerState_t();
            TrackedDevicePose_t pose = new TrackedDevicePose_t();

            OpenVR.System.GetControllerStateWithPose(ETrackingUniverseOrigin.TrackingUniverseStanding, this.DeviceID, ref state, (uint)Marshal.SizeOf(typeof(VRControllerState_t)), ref pose);

            matrix = pose.mDeviceToAbsoluteTracking.ToMatrix4x4();

            bool[] buttonPressed = unToBinary(state.ulButtonPressed).Split("").Select((str) => { if (str == "1") return true; return false; }).ToArray();

            Console.WriteLine(this.unToBinary(state.ulButtonPressed));
            Console.WriteLine(this.unToBinary(state.ulButtonTouched));

            PrintDataToConsole();
            Console.WriteLine($"{AxisToString(state.rAxis0)}");
            Console.WriteLine($"{AxisToString(state.rAxis1)}");
        }

        protected override void PrintDataToConsole()
        {
            base.PrintDataToConsole();
        }

        private string AxisToString(VRControllerAxis_t axis)
        {
            return $"{{ X:{axis.x} Y:{axis.y} }}";
        }
    }

    public class Control
    {
        public bool IsDown { get; private set; } = false;
        public bool IsUp { get { return !IsDown; } }
        public bool IsTouched { get; private set; } = false;
        public float AxisX = 0f, AxisY = 0f;
        public float Axis { get { return AxisX; } set { AxisX = value; AxisY = value; } }

        public void Update(bool isDown, bool isTouched, float axis)
        {
            IsDown = isDown;
            IsTouched = IsTouched;
            Axis = axis;
        }
        public void Update(bool isDown, bool isTouched, float axisX, float axisY)
        {
            IsDown = isDown;
            IsTouched = IsTouched;
            AxisX = axisX;
            AxisY = axisY;
        }
    }
}
