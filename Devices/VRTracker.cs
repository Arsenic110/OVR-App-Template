using OVRSharp;
using OVRSharp.Math;
using Valve.VR;

namespace OVR_App_Template.Devices
{
    public class VRTracker : VRTrackedDevice
    {
        public VRTracker(uint DeviceID, VRTrackedDeviceClass Type) : base(DeviceID, Type)
        {

        }

        public override void Update()
        {
            VRControllerState_t state = new VRControllerState_t();
            TrackedDevicePose_t pose = new TrackedDevicePose_t();

            OpenVR.System.GetControllerStateWithPose(ETrackingUniverseOrigin.TrackingUniverseStanding, this.DeviceID, ref state, 1, ref pose);

            matrix = pose.mDeviceToAbsoluteTracking.ToMatrix4x4();

            PrintDataToConsole();
        }
    }
}
