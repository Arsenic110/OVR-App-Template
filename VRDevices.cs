using System;
using System.Text;
using System.Numerics;

using OVRSharp;
using OVRSharp.Math;
using Valve.VR;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;

namespace OVR_App_Template
{
    public class VRDevices
    {
        private List<VRTrackedDevice> _trackedDevices = new List<VRTrackedDevice>();
        public VRTrackedDevice[] trackedDevices
        {
            get
            {
                return _trackedDevices.ToArray();
            }
        }

        public VRTrackedDevice HMD
        {
            get
            {
                return trackedDevices[OpenVR.k_unTrackedDeviceIndex_Hmd]; //HMD is always zero, and must be connected
            }
            private set
            {
                trackedDevices[OpenVR.k_unTrackedDeviceIndex_Hmd] = value;
            }
        }
        public VRTrackedDevice RightController
        {
            get
            {
                return GetController(VRTrackedDeviceClass.RightController);
            }
        }
        public VRTrackedDevice LeftController
        {
            get
            {
                return GetController(VRTrackedDeviceClass.LeftController);
            }
        }

        private string[] OldETrackedDeviceClasses;
        private string[] CurrentETrackedDeviceClasses;

        public VRDevices()
        {
            InitializeDevices();
        }

        private void InitializeDevices()
        {
            Debug.WriteLine("Initializing Devices");

            //clear list
            _trackedDevices = new List<VRTrackedDevice>();

            //looping through all possible devices
            for (uint i = 0; i < OpenVR.k_unMaxTrackedDeviceCount; i++)
            {

                ETrackedDeviceClass tClass = OpenVR.System.GetTrackedDeviceClass(i);

                //HMD
                if (tClass == ETrackedDeviceClass.HMD)
                {
                    _trackedDevices.Add(new VRTrackedDevice(i, VRTrackedDeviceClass.HMD));
                    continue;
                }

                if (tClass == ETrackedDeviceClass.Controller && OpenVR.System.GetControllerRoleForTrackedDeviceIndex(i) == ETrackedControllerRole.LeftHand)
                {
                    _trackedDevices.Add(new VRTrackedDevice(i, VRTrackedDeviceClass.LeftController));
                    continue;
                }

                if (tClass == ETrackedDeviceClass.Controller && OpenVR.System.GetControllerRoleForTrackedDeviceIndex(i) == ETrackedControllerRole.RightHand)
                {
                    _trackedDevices.Add(new VRTrackedDevice(i, VRTrackedDeviceClass.RightController));
                    continue;
                }

                if (tClass == ETrackedDeviceClass.GenericTracker)
                {
                    _trackedDevices.Add(new VRTrackedDevice(i, VRTrackedDeviceClass.HTCTracker));
                    continue;
                }
            }


        }

        public void Update()
        {
            InitializeDevices();

            //query OpenVR for all device poses
            TrackedDevicePose_t[] trackedDevicePoses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];

            OpenVR.System.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseStanding, 0, trackedDevicePoses);

            for(int i = 0; i < trackedDevices.Length; i++)
            {
                Console.WriteLine(trackedDevices[i].DeviceID + " || " + OpenVR.System.GetTrackedDeviceClass(trackedDevices[i].DeviceID) + " "+ Enum.GetName(trackedDevicePoses[trackedDevices[i].DeviceID].eTrackingResult));

                trackedDevices[i].Update();
            }

            Console.SetCursorPosition(0, 0);

            //assign old values
            OldETrackedDeviceClasses = CurrentETrackedDeviceClasses;
        }

        private VRTrackedDevice GetController(VRTrackedDeviceClass controllerClass)
        {
            VRTrackedDevice searchResult = null;

            if (controllerClass != VRTrackedDeviceClass.LeftController || controllerClass != VRTrackedDeviceClass.RightController)
                return searchResult;

            _trackedDevices.ForEach((device) =>
            {
                if (device.Type == controllerClass)
                {
                    searchResult = device;
                    return;
                }
            });

            return searchResult;
        }
    }
}
