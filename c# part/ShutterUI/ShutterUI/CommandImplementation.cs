using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ShutterUI
{
    public partial class MainWindow : Window
    {
        private void StartCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private async void StartCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (ExpStarted == false)
            {
                if (ArduinoPort.IsOpen)
                {
                    ExpStarted = true;
                    ArduinoPort.Write("1234");
                    BtnStart.IsEnabled = false;
                    await Task.Delay(1500);
                    BtnStart.Content = "Cancel (F4)";
                    BtnStart.IsEnabled = true;
                    TxtStatusBrush.Color = Colors.Black;
                    int Sum = 0;
                    foreach (var item in ExpProgram.CycleItems)
                    {
                        Sum += item.CycleDuration;
                    }
                    TxtStatus.Text = $"Program has started... Total program time is {Sum} s";
                    BtnConnect.IsEnabled = false;
                    BtnOpen.IsEnabled = false;
                    BtnSendData.IsEnabled = false;
                    MainMenu.IsEnabled = false;
                    SpItems.IsEnabled = false;
                    Boolean AnswerRecieved = false;
                    Int16 GetCode = 0;
                    await Task.Run(() =>
                    {
                        while (!AnswerRecieved)
                        {
                            Task.Delay(100);
                            if (ArduinoPort.BytesToRead > 3)
                            {
                                Int16.TryParse(ArduinoPort.ReadLine(), out GetCode);
                                if (GetCode == 1604)
                                {
                                    AnswerRecieved = true;
                                }
                            }
                        }
                    });
                    TxtStatus.Text = $"Program has been finished";
                    BtnStart.Content = "Start (F4)";
                    ExpStarted = false;
                    TxtStatusBrush.Color = Colors.Black;
                    BtnConnect.IsEnabled = true;
                    BtnOpen.IsEnabled = true;
                    BtnSendData.IsEnabled = true;
                    MainMenu.IsEnabled = true;
                    SpItems.IsEnabled = true;
                }
            }
            else
            {
                ArduinoPort.Write("1000");
                BtnStart.IsEnabled = false;
                await Task.Delay(1500);
                BtnStart.IsEnabled = true;
                BtnStart.Content = "Start (F4)";
                ExpStarted = false;
                TxtStatusBrush.Color = Colors.Black;
                TxtStatus.Text = "Experiment was terminated...";
                BtnConnect.IsEnabled = true;
                BtnOpen.IsEnabled = true;
                BtnSendData.IsEnabled = true;
                MainMenu.IsEnabled = true;
                SpItems.IsEnabled = true;
            }
        }
        private async void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            BtnOpen.IsEnabled = false;
            BtnStart.IsEnabled = false;
            if (!ArduinoPort.IsOpen)
            {
                MessageBox.Show("Serial ports is closed");
                BtnConnect.IsEnabled = true;

            }
            else
            {
                BtnStart.IsEnabled = false;
                BtnSendData.IsEnabled = false;
                SwitchON = await OnOffButtonClick();

                if (SwitchON)
                {

                    BtnOpen.Content = "Close (F3)";
                    
                    TxtStatusBrush.Color = Colors.Black;
                    TxtStatus.Text = "Shutter was opened";
                }
                if (!SwitchON)
                {
                    BtnOpen.Content = "Open (F3)";
                    if (DataWereSentGlobal) BtnStart.IsEnabled = true;
                    BtnSendData.IsEnabled = true;
                    if (DataWereSentGlobal)
                    {
                        BtnSendData.IsEnabled = true;
                    }
                    TxtStatusBrush.Color = Colors.Black;
                    TxtStatus.Text = "Shutter was closed";
                }

                BtnOpen.IsEnabled = true;
            }
        }
    }
}
