using System;
using System.Threading;
using Wooting;

namespace Wooting_RGB_Fricker
{
    internal class Program
    {
        static Random RndRGB { get; set; }
        static Boolean ShouldLoopRun { get; set; }
        static Int32 FPS { get; set; }
        static Int32 mode { get; set; }

        static void Main(string[] args)
        {
            RndRGB = new Random();

            Console.Write("Select mode 0 for SetFull or mode 1 for SetSingle: ");
            String Input = Console.ReadLine();

            mode = Convert.ToInt32(Input);

            Console.Write("Enter the desired FPS: ");
            String Input2 = Console.ReadLine();

            FPS = Convert.ToInt32(Input2);

            if (FPS < 0)
            {
                Console.WriteLine("FPS have to be positive");
                Console.ReadKey();
                return;
            } else if (FPS > 1000) {
                Console.WriteLine("FPS cant be higher than 1000");
                Console.ReadKey();
                return;
            }

            if (FPS == 0) FPS = 1000;


            ShouldLoopRun = true;

            Console.CancelKeyPress += new ConsoleCancelEventHandler(StopTheFun);

            BeginTheFricking();
        }
        static void StopTheFun(object Sender, ConsoleCancelEventArgs args)
        {
            args.Cancel = true;
            StopTheFun();
        }

        static void StopTheFun()
        {
            Console.WriteLine("Stopping the party...");
            ShouldLoopRun = false;
            RGBControl.ResetRGB();
            RGBControl.Close();
        }

        static void BeginTheFricking()
        {
            if (!RGBControl.IsConnected())
            {
                Console.WriteLine("You dont even have a Wooting connected...");
                return;
            }

            RGBDeviceInfo device = RGBControl.GetDeviceInfo();

            RGBControl.SetDisconnectedCallback((DisconnectedCallback)StopTheFun);

            if (mode == 0)
            {
                Console.WriteLine("Full Fricking in progress...");
                while (ShouldLoopRun)
                {
                    KeyColour[,] keys = new KeyColour[RGBControl.MaxRGBRows, RGBControl.MaxRGBCols];
                    for (byte i = 0; i < device.MaxColumns; i++)
                    {
                        for (byte j = 0; j < device.MaxRows; j++)
                        {
                            keys[j, i] = new KeyColour((byte)RndRGB.Next(0, 255), (byte)RndRGB.Next(0, 255), (byte)RndRGB.Next(0, 255));
                        }
                    }
                    RGBControl.SetFull(keys);

                    RGBControl.UpdateKeyboard();

                    if (FPS > 0) Thread.Sleep(1000 / FPS);
                }
            }
            else
            {
                Console.WriteLine("Single Key Fricking in progress...");
                while (ShouldLoopRun)
                {
                    for (byte i = 0; i < device.MaxColumns; i++)
                    {
                        for (byte j = 0; j < device.MaxRows; j++)
                        {
                            RGBControl._SetKey(j, i, (byte)RndRGB.Next(0, 255), (byte)RndRGB.Next(0, 255), (byte)RndRGB.Next(0, 255));
                        }
                    }
                    RGBControl.UpdateKeyboard();

                    if (FPS > 0) Thread.Sleep(1000 / FPS);
                }
            }
        }
    }
}
