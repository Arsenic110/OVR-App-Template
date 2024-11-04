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

            bool[] buttonPressed = unToBitmask(state.ulButtonPressed);
            bool[] buttonTouched = unToBitmask(state.ulButtonTouched);

            if (buttonPressed.Length == 64 && buttonTouched.Length == 64)
            {
                ButtonA.Update(buttonPressed[2], buttonTouched[2], 0f);
                ButtonB.Update(buttonPressed[1], buttonTouched[1], 0f);
                Trigger.Update(buttonPressed[33], buttonTouched[33], state.rAxis1.x, state.rAxis1.y);
                Joystick.Update(buttonPressed[35], buttonTouched[35], state.rAxis0.x, state.rAxis0.y);
                Trackpad.Update(buttonPressed[32], buttonTouched[32], state.rAxis0.x, state.rAxis0.y);
                Grip.Update(buttonPressed[2], buttonTouched[2], 0f);
            }

            PrintDataToConsole();
        }

        protected override void PrintDataToConsole()
        {
            base.PrintDataToConsole();
            //Console.Write($"Trigger: {Trigger.IsDown} {Trigger.IsTouched} {Trigger.Axis}");
            //ConsolePad();
        }

        private string AxisToString(VRControllerAxis_t axis)
        {
            return $"{{ X:{axis.x} Y:{axis.y} }}";
        }

        private bool[] unToBitmask(ulong un)
        {
            //N.B.: The cast from ulong -> long can cause issue with bitmasks
            string atr = Convert.ToString((long)un, 2).PadLeft(64, '0');
            bool[] rtr = new bool[atr.Length];

            for(int i = 0; i < atr.Length; i++)
            {
                if (atr[i] == '1')
                    rtr[i] = true;
                else
                    rtr[i] = false;
            }

            rtr = rtr.Reverse().ToArray();

            return rtr;
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
            IsTouched = isTouched;
            Axis = axis;
        }
        public void Update(bool isDown, bool isTouched, float axisX, float axisY)
        {
            IsDown = isDown;
            IsTouched = isTouched;
            AxisX = axisX;
            AxisY = axisY;
        }
    }
}
