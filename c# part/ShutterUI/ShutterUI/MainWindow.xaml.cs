using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ExperimentClass;
using Microsoft.Win32;

namespace ShutterUI
{
    public partial class MainWindow : Window
    {
        Cycle ExpProgram = new Cycle();
        CycleItem CurrentItem = new CycleItem();
        SolidColorBrush TxtStatusBrush = new SolidColorBrush();
        Boolean ExpStarted = false;

        public MainWindow()        {
            
            InitializeComponent();
            
            CycleInitialisation(ref ExpProgram);
            // SpItems.IsEnabled = false;
            TxtStatusBrush.Color = Colors.Red;
            TxtStatus.Foreground = TxtStatusBrush;

            BtnOpen.IsEnabled       =   false;            
            BtnStart.IsEnabled      =   false;
            BtnSendData.IsEnabled   =   false;
            

            BtnRemove.Click += BtnRemove_Click;
            CmbCycleItems.SelectionChanged += CmbCycleItems_SelectionChanged;
            CmbCycleItems.Loaded += CmbCycleItems_Loaded;
            BtnAdd.Click += BtnAdd_Click;           
            SpItems.PreviewTextInput += SpItems_PreviewTextInput;
            SpItems.PreviewKeyDown += SpItems_PreviewKeyDown;
            TxtCycleNumber.PreviewTextInput += TxtCycleNumber_PreviewTextInput;
            TxtCycleNumber.LostFocus += TxtCycleNumber_LostFocus;
            TxtDuration.PreviewTextInput += TxtDuration_PreviewTextInput;
            TxtDuration.LostFocus += TxtDuration_LostFocus;
            TxtOnTime.PreviewTextInput += TxtOnTime_PreviewTextInput;
            TxtOnTime.LostFocus += TxtOnTime_LostFocus;
            TxtOffTime.PreviewTextInput += TxtOffTime_PreviewTextInput;
            TxtOffTime.LostFocus += TxtOffTime_LostFocus;
            MenuNew.Click += MenuNew_Click;
            MenuClose.Click += MenuClose_Click;
            MenuOpen.Click += MenuOpen_Click;
            MenuSave.Click += MenuSave_Click;
            BtnConnect.Click += BtnConnect_Click;
           // BtnOpen.Click += BtnOpen_Click;
            BtnSendData.Click += BtnSendData_Click;
           // BtnStart.Click += BtnStart_Click;
        }
        public static readonly RoutedUICommand Start = new RoutedUICommand
            (
                "Start",
                "Start",
                typeof(ShutterCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F4)
                }
            );

        private async void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (ExpStarted == false)
            {
                if (ArduinoPort.IsOpen)
                {
                    ExpStarted = true;
                    ArduinoPort.Write("1234");
                    BtnStart.IsEnabled = false;
                    await Task.Delay(1500);
                    BtnStart.Content = "Cancel";
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
                            if (ArduinoPort.BytesToRead>3)
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
                    BtnStart.Content = "Start";
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
                BtnStart.Content = "Start";
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

        private async void BtnSendData_Click(object sender, RoutedEventArgs e)
        {
            TlbExperiment.IsEnabled = false;
            SpItems.IsEnabled = false;
            ArduinoPort.DiscardOutBuffer();
            ArduinoPort.DiscardInBuffer();
            TxtStatusBrush.Color = Colors.Black;
            ArduinoPort.Write("1001");
            TxtStatus.Text = "Sending data...";
            await Task.Delay(1500);
            byte[] IntToBytes = BitConverter.GetBytes(ExpProgram.CycleNumber); //sending number of cycles
            ArduinoPort.Write(IntToBytes, 0, 2);
            
            IntToBytes = BitConverter.GetBytes(Convert.ToInt16(ExpProgram.CycleItems.Count));  //sending number of cycleitems
            ArduinoPort.Write(IntToBytes, 0, 2);

            foreach (var item in ExpProgram.CycleItems) //sending all cycle parameters
            {
                IntToBytes = BitConverter.GetBytes(item.CycleDuration);  
                ArduinoPort.Write(IntToBytes, 0, 2);
                IntToBytes = BitConverter.GetBytes(item.OnTime);
                ArduinoPort.Write(IntToBytes, 0, 2);
                IntToBytes = BitConverter.GetBytes(item.OffTime);
                ArduinoPort.Write(IntToBytes, 0, 2);
               
            }
            Boolean DataWereSent = false;
            Int16 GetCode;
            while (!DataWereSent)
            {
                if (ArduinoPort.BytesToRead>0)
                {
                    Int16.TryParse(ArduinoPort.ReadLine(), out GetCode);
                    if (GetCode == 1604)
                    {
                       DataWereSent = true;
                   }
                   // DataWereSent = true;
                   // TxtStatus.Text = ArduinoPort.ReadLine();
                }
            }
            TlbExperiment.IsEnabled = true;
            SpItems.IsEnabled = true;
            BtnStart.IsEnabled = true;
            DataWereSentGlobal = true;
            TxtStatus.Text = "Data were succesfully sent, press start to continue...";
        }

        private async void BtnOpen_Click(object sender, RoutedEventArgs e)
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

                SwitchON = await OnOffButtonClick();

                if (SwitchON)
                {

                    BtnOpen.Content = "Close";
                    BtnStart.IsEnabled = false;
                    BtnSendData.IsEnabled = false;
                    TxtStatusBrush.Color = Colors.Black;
                    TxtStatus.Text = "Shutter was opened";
                }
                if (!SwitchON)
                {
                    BtnOpen.Content = "Open";
                    BtnStart.IsEnabled = true;
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

        private void Window_Closed(object sender, EventArgs e)
        {
            if (ArduinoPort.IsOpen)
            {
                ArduinoPort.Write("1000");
            }
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            SerialInitialization();
        }

        private void MenuSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog ExpSave = new SaveFileDialog();
            if (ExpSave.ShowDialog() == true)
            {
                if (ExpSave.FileName.Contains(".exprmnt"))
                {
                    ExpProgram.Save(ExpSave.FileName);                    
                }
                else
                {
                    ExpProgram.Save(ExpSave.FileName + ".exprmnt");                   
                }
            }
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ExpOpen = new OpenFileDialog();
            ExpOpen.Filter = "Experiment files(*exprmnt)|*.exprmnt";
            if (ExpOpen.ShowDialog() == true)
            {
                ExpProgram = Cycle.Load(ExpOpen.FileName);
                CurrentItem = ExpProgram.CycleItems[0];
                CmbCycleItems.Items.Clear();                
                for (int i = 0; i < ExpProgram.Count(); i++)
                {
                    CmbCycleItems.Items.Add(String.Format($"Step {i}"));
                }
                CmbCycleItems.SelectedIndex = 0;
                TxtCycleNumber.Text = ExpProgram.CycleNumber.ToString();
                TxtDuration.Text = CurrentItem.CycleDuration.ToString();
                TxtOnTime.Text = CurrentItem.OnTime.ToString();
                TxtOffTime.Text = CurrentItem.OffTime.ToString();
                ShowData();
            }
         }

        private void MenuClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuNew_Click(object sender, RoutedEventArgs e)
        {
            ExpProgram = new Cycle();
            // CycleInitialisation(ref ExpProgram);
            ExpProgram.CycleNumber = 1;

            ExpProgram.CycleItems.Add(new CycleItem(5, 5, 60));

            CurrentItem = ExpProgram.CycleItems[0];

            TxtCycleNumber.Text = ExpProgram.CycleNumber.ToString();
            CmbCycleItems.Items.Clear();
            for (int i = 0; i < ExpProgram.Count(); i++)
            {
                CmbCycleItems.Items.Add(String.Format($"Step {i}"));
            }
            CmbCycleItems.SelectedIndex = 0;
            ShowData();
        }

        private void TxtOffTime_LostFocus(object sender, RoutedEventArgs e)
        {
            short i;
            Int16.TryParse(TxtOffTime.Text, out i);
            try
            {
                CurrentItem.OffTime = i;
                ShowData();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void TxtOffTime_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            short i = 0;
            if (!Int16.TryParse(TxtOffTime.Text + e.Text, out i))
            {
                e.Handled = true;
            }

            if ((i < 1) || (i > 300))
            {
                e.Handled = true;
            }
        }

        private void TxtCycleNumber_LostFocus(object sender, RoutedEventArgs e)
        {
            short i;
            Int16.TryParse(TxtCycleNumber.Text, out i);
            try
            {
                ExpProgram.CycleNumber = i;
                ShowData();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void TxtDuration_LostFocus(object sender, RoutedEventArgs e)
        {
            short i;
            Int16.TryParse(TxtDuration.Text, out i);
            try
            {
                CurrentItem.CycleDuration = i;
                ShowData();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void TxtOnTime_LostFocus(object sender, RoutedEventArgs e)
        {
            short i;
            Int16.TryParse(TxtOnTime.Text, out i);
            try
            {
                CurrentItem.OnTime = i;
                ShowData();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void TxtOnTime_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            short i = 0;
            if (!Int16.TryParse(TxtOnTime.Text + e.Text, out i))
            {
                e.Handled = true;
            }

            if ((i < 1) || (i > 300))
            {
                e.Handled = true;
            }
            
        }

        

        private void TxtDuration_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            short i = 0;
            if (!Int16.TryParse(TxtDuration.Text + e.Text, out i))
            {
                e.Handled = true;
            }

            if ((i < 1) || (i > 600))
            {
                e.Handled = true;
            }
            
        }

        private void TxtCycleNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            short i = 0;
            if (!Int16.TryParse(TxtCycleNumber.Text+e.Text, out i))
            {
                e.Handled = true;
            }
            
            if ((i < 1) || (i > 100))
            {
                e.Handled = true;
            }
           
        }

        private void SpItems_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {                
                e.Handled = true;
            }
            
            
            if ((e.Key ==Key.Enter)&&(TxtCycleNumber.IsFocused)&&(TxtCycleNumber.Text.Length>0))
            {
                String s = TxtCycleNumber.Text.Substring(0, TxtCycleNumber.Text.Length);
                if (s!="")
                {
                    ExpProgram.CycleNumber = Int16.Parse(s);
                    ShowData();
                }
            }
            if ((e.Key == Key.Enter) && (TxtDuration.IsFocused) && (TxtDuration.Text.Length > 0))
            {
                String s = TxtDuration.Text.Substring(0, TxtDuration.Text.Length);
                if (s != "")
                {
                    short k;
                    k = Int16.Parse(s);
                    if ((k > 10) && (k < 601))
                    {
                        CurrentItem.CycleDuration = k;
                        ShowData();
                    }                    
                }
            }
            if ((e.Key == Key.Enter) && (TxtOnTime.IsFocused) && (TxtOnTime.Text.Length > 0))
            {
                String s = TxtOnTime.Text.Substring(0, TxtOnTime.Text.Length);
                if (s != "")
                {
                    short k;
                    k = Int16.Parse(s);
                    if ((k > 0) && (k <=300))
                    {
                        CurrentItem.OnTime = k;
                        ShowData();
                    }
                }
            }
            if ((e.Key == Key.Enter) && (TxtOffTime.IsFocused) && (TxtOffTime.Text.Length > 0))
            {
                String s = TxtOffTime.Text.Substring(0, TxtOffTime.Text.Length);
                if (s != "")
                {
                    short k;
                    k = Int16.Parse(s);
                    if ((k > 0) && (k <= 300))
                    {
                        CurrentItem.OffTime = k;
                        ShowData();
                    }
                }
            }
        }

        private void SpItems_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            short val;
            if (!Int16.TryParse(e.Text, out val))
            {
               e.Handled = true;
            }
        }       

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (ExpProgram.CycleItems.Count > 1)
            {
                ExpProgram.CycleItems.Remove(CurrentItem);
                CmbCycleItems.Items.Remove(CmbCycleItems.SelectedItem);
                for (int i = 0; i < ExpProgram.Count(); i++)
                {
                    CmbCycleItems.Items[i] = "Step " + i.ToString();
                }
                CurrentItem = ExpProgram.CycleItems[ExpProgram.Count() - 1];
                CmbCycleItems.SelectedItem = CmbCycleItems.Items[ExpProgram.Count() - 1];
                ShowData();
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (ExpProgram.CycleItems.Count < 10)
            {
                ExpProgram.CycleItems.Add(new CycleItem(5, 5, 60));
                CmbCycleItems.Items.Add(String.Format($"Step {(ExpProgram.Count() - 1).ToString()}"));
                CmbCycleItems.SelectedIndex = ExpProgram.Count() - 1;
                CurrentItem = ExpProgram.CycleItems[ExpProgram.Count() - 1];
                ShowData();
            }
        }

        private void CmbCycleItems_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentItem = ExpProgram.CycleItems[CmbCycleItems.SelectedIndex];
            TxtDuration.Text = CurrentItem.CycleDuration.ToString();
            TxtOnTime.Text = CurrentItem.OnTime.ToString();
            TxtOffTime.Text = CurrentItem.OffTime.ToString();
        }

        private void CmbCycleItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if ((CmbCycleItems.SelectedIndex >= 0) && (CmbCycleItems.SelectedIndex <= ExpProgram.Count()))
            {
                CurrentItem = ExpProgram.CycleItems[CmbCycleItems.SelectedIndex];
                TxtDuration.Text = CurrentItem.CycleDuration.ToString();
                TxtOnTime.Text = CurrentItem.OnTime.ToString();
                TxtOffTime.Text = CurrentItem.OffTime.ToString();
                
            }
        }

        private void CycleInitialisation(ref Cycle InitialProgram)
        {
            String[] args = App.Args;
            if (args == null)
            {
                InitialProgram.CycleNumber = 1;

                InitialProgram.CycleItems.Add(new CycleItem(5, 5, 60));

                TxtCycleNumber.Text = InitialProgram.CycleNumber.ToString();
                CmbCycleItems.Items.Clear();
                for (int i = 0; i < InitialProgram.Count(); i++)
                {
                    CmbCycleItems.Items.Add(String.Format($"Step {i}"));
                }
                CmbCycleItems.SelectedIndex = 0;
                ShowData();
            }
            else
            {
                try
                {
                                   
                    ExpProgram = Cycle.Load(args[0]);
                    CurrentItem = ExpProgram.CycleItems[0];
                    CmbCycleItems.Items.Clear();
                    for (int i = 0; i < ExpProgram.Count(); i++)
                    {
                        CmbCycleItems.Items.Add(String.Format($"Step {i}"));
                    }
                    CmbCycleItems.SelectedIndex = 0;
                    TxtCycleNumber.Text = ExpProgram.CycleNumber.ToString();
                    TxtDuration.Text = CurrentItem.CycleDuration.ToString();
                    TxtOnTime.Text = CurrentItem.OnTime.ToString();
                    TxtOffTime.Text = CurrentItem.OffTime.ToString();
                    ShowData();
                }
                catch
                {
                    MessageBox.Show("File load error");
                    App.Args = null;
                    CycleInitialisation(ref InitialProgram);
                }


            }

        }
        private void ShowData()
        {            
            StringBuilder sb = new StringBuilder();
            sb.Append("Number of cycles\t" + ExpProgram.CycleNumber.ToString() + "\n");
            int index = 0;
            foreach (var item in ExpProgram.CycleItems)
            {
                sb.Append("Step " + index.ToString() + "\tOn time\t" + item.OnTime.ToString() + "\tOff time\t" +
                    item.OffTime.ToString() + "\tDuration " + item.CycleDuration.ToString() + "\n");
                index++;
            }
            ExperimentViewer.Text = sb.ToString();
        }
    }
}
