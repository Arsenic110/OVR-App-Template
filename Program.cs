using System;

namespace OVR_App_Template
{
    public class Program
    {
        static void Main(string[] args)
        {
            App app = new();
        }
    }

    class App : OVRSharp.Application
    {
        public App() : base(ApplicationType.Background)
        {
            VRDevices vrDevices = new VRDevices();
            while(true)
            {
                vrDevices.Update();
                System.Threading.Thread.Sleep(50);
            }
        }
    }
}
