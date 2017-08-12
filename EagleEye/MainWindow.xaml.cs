using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using AForge.Video.DirectShow;
using AForge.Video;
using System.Drawing;
using System.IO;
using System.Collections.Concurrent;
using System.Threading;
using EagleEye.Entities.Extensions;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using EagleEye.Entities.Controllers;
using EagleEye.Entities.Details;
using EagleEye.Entities;
using EagleEye.Startup;
using EagleEye.Entities.Wrappers;
using EagleEye.Entities.Events;
using EagleEye.Entities.DataContext;

namespace EagleEye
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private AppDataContext AppDataContext;
        private ControllerInitialiser Controllers;
        private VideoWrapper _videoHandler;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Startup();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            switch (this.WindowState)
            {
                case WindowState.Maximized:
                    break;
                case WindowState.Minimized:
                    AppDataContext.NotifyIconHandler.ShowAlert("EagleEye is minimized...", TimeSpan.FromSeconds(30));
                    break;
                case WindowState.Normal:

                    break;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
            AppDataContext.NotifyIconHandler.ShowAlert("EagleEye is minimized...", TimeSpan.FromSeconds(5));
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _videoHandler.Stop();
        }


        private void Startup()
        {
            using (new Splash(this))
            {
                AppDataContext = new AppDataContext()
                {
                    Devices = new ObservableCollection<string>(),
                    StrokeCollection = new System.Windows.Ink.StrokeCollection(),
                    VideoImage = null,
                    VideoSelectedImage = null,
                    ImageDifference = null,
                    FrameRate = 0,
                    ImageChangePercentage = 0.0,
                    Status = string.Empty
                };

                AppDataContext.NotifyIconHandler = new NotifyIconHandler(AppDataContext, Properties.Resources.Icon,
                    Properties.Resources.Icon_Working);
                AppDataContext.NotifyIconHandler.OnClickEvent += () =>
                {
                    WindowState = System.Windows.WindowState.Maximized;
                    Topmost = true;
                    Show();
                    Topmost = false;
                };

                (AppDataContext.Controllers = (Controllers = new ControllerInitialiser(AppDataContext))).InitialiseControllers();

                _videoHandler = new VideoWrapper(AppDataContext);
                _videoHandler.TriggerEvent += AppDataContext_TriggerEvent;
                _videoHandler.GetDevices();
                _videoHandler.Start();

                this.DataContext = AppDataContext;
            }
        }


        bool deviceDropDownOpened = false;
        private void cbDevices_DropDownOpened(object sender, EventArgs e)
        {
            deviceDropDownOpened = true;
        }

        private void cbDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (deviceDropDownOpened)
            {
                _videoHandler.Stop();
                _videoHandler.Start();
                deviceDropDownOpened = false;
            }
        }

        private void AppDataContext_TriggerEvent(object sender, EventArgs e)
        {
            var snapshot = (SnapShotEventArgs)e as SnapShotEventArgs;

            new TaskFactory().StartNew(() =>
            {
                AppDataContext.NotifyIconHandler.SetWorkingIcon();
                Thread.Sleep(AppDataContext.Configuration.WaitTimeSeconds * 1000);
                AppDataContext.NotifyIconHandler.SetStandardIcon();
            });


            App.Current.Dispatcher.Invoke((Action)delegate
            {
                AppDataContext.Controllers.SnapShotController.AddSnapShot(snapshot.SnapShot);
            });

            foreach (var action in AppDataContext.Actions)
            {
                switch (action.ActionType)
                {
                    case Entities.Enums.ActionTypes.MinimizeProcess:
                        foreach (var process in Process.GetProcessesByName(action.ActionValue))
                        {
                            if (!string.IsNullOrEmpty(process.MainWindowTitle))
                            {
                                User32Wrapper.MinimizeWindow(process.MainWindowHandle);
                            }
                        }
                        break;
                    case Entities.Enums.ActionTypes.MaximizeProcess:
                        foreach (var process in Process.GetProcessesByName(action.ActionValue))
                        {
                            if (!string.IsNullOrEmpty(process.MainWindowTitle))
                            {
                                User32Wrapper.MaximizeWindow(process.MainWindowHandle);
                            }
                        }
                        break;
                    case Entities.Enums.ActionTypes.ExecuteFile:
                        User32Wrapper.ExecuteFile(action.ActionValue);
                        break;
                    case Entities.Enums.ActionTypes.ExecuteCommand:
                        User32Wrapper.ExecuteCommand(action.ActionValue, false);
                        break;
                    case Entities.Enums.ActionTypes.ExecuteHiddenCommand:
                        User32Wrapper. ExecuteCommand(action.ActionValue, true);
                        break;
                }
            }
        }


        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            AppDataContext.StrokeCollection.Clear();
            AppDataContext.VideoSelectedImage = null;
            AppDataContext.ImageDifference = null;
            AppDataContext.ImageChangePercentage = 0.0;
            AppDataContext.Status = string.Empty;
        }



        private void btnClearSnapShots_Click(object sender, RoutedEventArgs e)
        {
            AppDataContext.Controllers.SnapShotController.DeleteAllSnapShots();
        }

        private void btnDeleteSnapShot_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var dataContext = (SnapShotDetails)button.DataContext;

            AppDataContext.Controllers.SnapShotController.DeleteSnapShot(dataContext);
        }



        private void btnAddAction_Click(object sender, RoutedEventArgs e)
        {
            AppDataContext.Actions.Add(AppDataContext.Action);
            AppDataContext.Action = new ActionDetails();
            Controllers.ActionController.SaveConfiguration();
        }

        private void btnDeleteAction_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var dataContext = (ActionDetails)button.DataContext;

            AppDataContext.Actions.Remove(dataContext);
            Controllers.ActionController.SaveConfiguration();
        }

    }
}
