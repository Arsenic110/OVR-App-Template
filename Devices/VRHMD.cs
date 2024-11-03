using OVRSharp;
using OVRSharp.Math;
using Valve.VR;

namespace OVR_App_Template.Devices
{
    public class VRHMD : VRTrackedDevice
    {
        public VRHMD(uint DeviceID, VRTrackedDeviceClass Type) : base (DeviceID, Type)
        {

        }

        public override void Update()
        {
            TrackedDevicePose_t[] trackedDevicePoses = new TrackedDevicePose_t[1];

            OpenVR.System.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseStanding, 0, trackedDevicePoses);

            TrackedDevicePose_t pose = trackedDevicePoses[0];

            matrix = pose.mDeviceToAbsoluteTracking.ToMatrix4x4();

            PrintDataToConsole();
        }
    }
}
