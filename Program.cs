using System;

using System.Net.Http;
using System.Net.Http.Json;

namespace OVR_App_Template
{
    public class Program
    {
        static void Main(string[] args)
        {
            App app = new();
            Console.ReadKey();
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

                //vrDevices.RightController.ButtonA.IsDown

                System.Threading.Thread.Sleep(50);
            }
        }
    }

    class TestApp
    {
        public TestApp()
        {
            int i = 0;

            while(true)
            {

                System.Threading.Thread.Sleep(50);
            }
        }
    }
}
