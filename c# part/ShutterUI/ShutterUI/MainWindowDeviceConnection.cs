using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ShutterUI
{
    public partial class MainWindow : Window
    {
        Boolean SwitchON = false;
     
        Boolean DataWereSentGlobal = false;

        SerialPort ArduinoPort = new SerialPort();
       
        public void SerialSendInt16(Int16 IntToSend)
        {             
            byte[] IntToBytes = BitConverter.GetBytes(IntToSend);
            ArduinoPort.Write(IntToBytes, 0, 2);
        }
        public Task<Boolean> OnOffButtonClick()
        {
            return Task.Run(() =>
            {
                if (ArduinoPort.IsOpen)
                {
                    if (SwitchON == false)
                    {
                        ArduinoPort.Write("1111");

                        Thread.Sleep(1000);

                        SwitchON = !SwitchON;
                    }
                    else if (SwitchON == true)
                    {
                        ArduinoPort.Write("1000");

                        Thread.Sleep(1000);

                        SwitchON = !SwitchON;
                    }
                }
                return SwitchON;
            });

        }
        public async void SerialInitialization()
        {
            Boolean PortFound           =  false;         
            String ArduinoReturnCode    =  "";
            Int32 IntCode = 0;
            ArduinoPort.Close();
            TxtStatus.Text = "Initialization...";
            BtnOpen.IsEnabled = false;
            
            BtnStart.IsEnabled = false;
            BtnSendData.IsEnabled = false;

            if (SerialPort.GetPortNames() == null)
            {
                MessageBox.Show("Connect shutter control to computer");
            }

            if (SerialPort.GetPortNames() != null)
            {
                while (PortFound != true)
                {
                    foreach (string s in SerialPort.GetPortNames())
                    {

                        if (!PortFound)
                        {
                            ArduinoPort.PortName = s;
                            ArduinoPort.BaudRate = 115200;
                            ArduinoPort.ReadTimeout = 1000;
                            ArduinoPort.WriteTimeout =1000;
                            TxtStatusBrush.Color = Colors.Black;
                            TxtStatus.Text = "Searching for device...";
                            try
                            {
                               // TxtStatus.Text = $"Trying port {s}...";
                                ArduinoPort.Open();
                            }
                            catch
                            {
                                TxtStatus.Text = $"Port {s} connection error";
                            }

                            if (ArduinoPort.IsOpen)
                            {
                                ArduinoPort.DiscardOutBuffer();

                                ArduinoPort.DiscardInBuffer();

                                ArduinoPort.Write("1989");

                                await Task.Delay(1000);
                                ArduinoReturnCode = null;
                                try
                                {
                                    ArduinoReturnCode = ArduinoPort.ReadLine();
                                }
                                catch
                                {

                                }
                                if (ArduinoReturnCode == null)
                                {
                                    ArduinoReturnCode = "1234";
                                }
                                Int32.TryParse(ArduinoReturnCode, out IntCode);
                                if (IntCode == 1603)
                                {
                                    TxtStatus.Text = $"Connected to port {s}";                                    

                                    PortFound = true;
                                    TxtStatusBrush.Color = Colors.Green;

                                    BtnOpen.IsEnabled = true;                                    
                                    BtnStart.IsEnabled = false;
                                    BtnSendData.IsEnabled = true;
                                }
                                else
                                {
                                    ArduinoPort.Close();
                                }
                            }
                        }
                    }
                }
            }
           
        }
    }
  }

