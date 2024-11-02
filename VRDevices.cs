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

        private readonly string[] OldETrackedDeviceClasses = new string[OpenVR.k_unMaxTrackedDeviceCount];
        private readonly string[] CurrentETrackedDeviceClasses = new string[OpenVR.k_unMaxTrackedDeviceCount];

        public VRDevices()
        {
            //update method handles device initialization
            Update();
        }

        private void InitializeDevices(TrackedDevicePose_t[] trackedDevicePoses)
        {
            Debug.WriteLine("==========Initializing Devices==========");

            //clear list
            _trackedDevices.Clear();

            //looping through all possible devices
            for (uint i = 0; i < OpenVR.k_unMaxTrackedDeviceCount; i++)
            {
                if (trackedDevicePoses[i].eTrackingResult == ETrackingResult.Uninitialized)
                    continue;

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

            for (uint i = 0; i < OpenVR.k_unMaxTrackedDeviceCount; i++)
            {
                OldETrackedDeviceClasses[i] = CurrentETrackedDeviceClasses[i];
            }
        }

        public void Update()
        {
            //query OpenVR for all device poses
            TrackedDevicePose_t[] trackedDevicePoses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];

            OpenVR.System.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseStanding, 0, trackedDevicePoses);

            for (uint i = 0; i < OpenVR.k_unMaxTrackedDeviceCount; i++)
            {
                //use this as a kind-of "UUID" for each device.
                CurrentETrackedDeviceClasses[i] = (Enum.GetName(OpenVR.System.GetTrackedDeviceClass(i)) + " " + trackedDevicePoses[i].eTrackingResult.ToString()).ToString();
            }

            for (uint i = 0; i < OpenVR.k_unMaxTrackedDeviceCount; i++)
            {
                //if the status or class changes for any given device ID, this will trigger an update.
                if (OldETrackedDeviceClasses[i] != CurrentETrackedDeviceClasses[i])
                {
                    InitializeDevices(trackedDevicePoses);
                    break;
                }
            }

            for (int i = 0; i < trackedDevices.Length; i++)
            {
                trackedDevices[i].Update();
            }

            Console.SetCursorPosition(0, 0);
        }


        private VRTrackedDevice GetController(VRTrackedDeviceClass controllerClass)
        {
            VRTrackedDevice searchResult = null!; //postfix ! is a null-forgiving operator

            if (controllerClass != VRTrackedDeviceClass.LeftController || controllerClass != VRTrackedDeviceClass.RightController)
            {
                return searchResult;
            }

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

        private void DebugWriteTheThing()
        {
            for(int i = 0; i < 10; i++)
            {
                Console.WriteLine(CurrentETrackedDeviceClasses[i] + " ||| " + OldETrackedDeviceClasses[i]);
            }
        }

    }
}
