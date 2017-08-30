using AForge.Video;
using AForge.Video.DirectShow;
using EagleEye.Entities.DataContext;
using EagleEye.Entities.Details;
using EagleEye.Entities.Events;
using EagleEye.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace EagleEye.Entities.Wrappers
{
    public class VideoWrapper
    {
        private readonly AppDataContext _appDataContext;

        private VideoCaptureDevice videoSource = null;
        private Bitmap _previousImage = null;
        private Bitmap _currentImage = null;
        private BitmapImage _imageDifference = null;
        private Stopwatch _frameRateStopwatch = null;
        private int _currentFrameRate = 0;
        private int _previousSecondFrameRate = 0;

        public event EventHandler TriggerEvent;

        public VideoWrapper(AppDataContext AppDataContext)
        {
            _appDataContext = AppDataContext;

        }

        public void GetDevices()
        {
            // enumerate video devices
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in videoDevices)
            {
                _appDataContext.Devices.Add(device.Name);
            };


            //set device as first device
            if (_appDataContext.Devices.Count > 0)
                _appDataContext.Configuration.Device = _appDataContext.Devices.FirstOrDefault();
        }

        public FilterInfo GetDeviceByName(string Name)
        {
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in videoDevices)
            {
                if (device.Name == Name)
                    return device;
            };
            return null;
        }

        public void Start()
        {
            if (!string.IsNullOrEmpty(_appDataContext.Configuration.Device))
            {
                var device = GetDeviceByName(_appDataContext.Configuration.Device);
                if (device != null)
                {
                    // create video source
                    videoSource = new VideoCaptureDevice(device.MonikerString);
                    // set NewFrame event handler
                    videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);

                    // start the video source
                    videoSource.Start();

                    new TaskFactory().StartNew(() =>
                    {
                        PerformComparison();
                    });
                }
            }
        }

        public void Stop()
        {
            //signal to stop when you no longer need capturing
            if (videoSource != null)
                videoSource.SignalToStop();

            _appDataContext.FrameRate = 0;
            _appDataContext.VideoImage = null;
            _appDataContext.VideoSelectedImage = null;
            _appDataContext.ImageDifference = null;
            _appDataContext.StrokeCollection.Clear();
            _appDataContext.VideoSelectedImage = null;
            _appDataContext.ImageDifference = null;
            _appDataContext.ImageChangePercentage = 0.0;
            _appDataContext.Status = string.Empty;
        }




        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = eventArgs.Frame;

            if (_appDataContext.Configuration.MirrorImage)
                bitmap.RotateFlip(RotateFlipType.Rotate180FlipY);

            ProcessImage(bitmap);
            CalculateFrameRate();
        }
        
        private void ProcessImage(Bitmap originalBitmap)
        {
            BitmapImage originalImage = originalBitmap.BitmapToBitmapImage();
            Bitmap selectedBitmap = GetSelectedAreaBitmap(originalBitmap);
            BitmapImage selectedImage = selectedBitmap != null ? selectedBitmap.BitmapToBitmapImage() : null;

            PrepareComparison(selectedBitmap);

            //set the properties back on main thread.
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                _appDataContext.VideoImage = originalImage;

                _appDataContext.VideoSelectedImage = selectedImage;

                _appDataContext.ImageDifference = _imageDifference;

                _appDataContext.FrameRate = _previousSecondFrameRate;
            });
        }

        private Bitmap GetSelectedAreaBitmap(Bitmap image)
        {
            var stroke = _appDataContext.StrokeCollection.FirstOrDefault();
            if (stroke != null)
            {
                int smallestX = int.MaxValue;
                int smallestY = int.MaxValue;
                int largestX = int.MinValue;
                int largestY = int.MinValue;

                stroke.StylusPoints.ToList().ForEach(point =>
                {
                    if (point.X < smallestX)
                    {
                        smallestX = Convert.ToInt32(point.X);
                    }
                    if (point.Y < smallestY)
                    {
                        smallestY = Convert.ToInt32(point.Y);
                    }
                    if (point.X > largestX)
                    {
                        largestX = Convert.ToInt32(point.X);
                    }
                    if (point.Y > largestY)
                    {
                        largestY = Convert.ToInt32(point.Y);
                    }

                });

                int width = largestX - smallestX;
                int height = largestY - smallestY;

                var section = new Rectangle(new Point(smallestX > 0 ? smallestX : 1, smallestY > 0 ? smallestY : 1), new Size(width > 0 ? width : 1, height > 0 ? height : 1));
                var croppedImage = image.CropImage(section);
                return croppedImage;
            }
            return null;
        }

        private void PrepareComparison(Bitmap selectedBitmap)
        {
            if (_currentImage != null)
                _previousImage = _currentImage;

            if (selectedBitmap != null)
                _currentImage = selectedBitmap;
        }

        private void CalculateFrameRate()
        {
            try
            {
                if (_frameRateStopwatch == null)
                    _frameRateStopwatch = new Stopwatch();

                if (!_frameRateStopwatch.IsRunning)
                    _frameRateStopwatch.Start();

                if (_frameRateStopwatch.Elapsed.TotalSeconds >= 1)
                {
                    _frameRateStopwatch.Reset();
                    _previousSecondFrameRate = _currentFrameRate;
                    _currentFrameRate = 1;
                }
                else
                {
                    _currentFrameRate++;
                }
            }
            catch (Exception e)
            {
            }
        }




        private void PerformComparison()
        {
            var delayStopwatch = new Stopwatch();
            delayStopwatch.Start();

            while (videoSource != null && videoSource.IsRunning)
            {
                try
                {
                    if (_appDataContext.VideoSelectedImage != null && _appDataContext.Configuration.AutoCalculatePixelChangeTolerance && _imageDifference == null)
                    {
                        _appDataContext.Status = "Calculating Pixel Change Tolerance...";
                        AutoCalculatePixelChangeTolerance();
                    }



                    if (_appDataContext.VideoSelectedImage != null)
                    {
                        _appDataContext.Status = "Monitoring...";

                        var result = CalculateDifferentPercentageOnPixels();
                        if (result != null)
                        {
                            var percentage = result.Item1;
                            var imageDiff = result.Item2;

                            if (percentage > 0 && percentage > _appDataContext.Configuration.ImageChangePercentageTriggerValue)
                            {
                                if (delayStopwatch.Elapsed.Seconds > _appDataContext.Configuration.WaitTimeSeconds)
                                {
                                    delayStopwatch.Reset();
                                    delayStopwatch.Start();
                                    TriggerEvent?.Invoke(this, new SnapShotEventArgs() { SnapShot = new SnapShotDetails() { Image = imageDiff, Percentage = percentage, DateTime = DateTime.Now } });
                                }

                            }

                            _appDataContext.ImageChangePercentage = percentage;
                            _imageDifference = imageDiff;
                        }
                    }
                    else
                    {
                        _appDataContext.Status = string.Empty;
                        _imageDifference = null;
                    }
                }
                catch (Exception e)
                { }
            }
        }

        private void AutoCalculatePixelChangeTolerance()
        {
            _appDataContext.Configuration.PixelChangeTolerance = 0;

            var syncStopwatch = new Stopwatch();

            while (videoSource != null && videoSource.IsRunning && _appDataContext.VideoSelectedImage != null)
            {
                try
                {
                    var result = CalculateDifferentPercentageOnPixels();
                    if (result != null)
                    {
                        var percentage = result.Item1;
                        var imageDiff = result.Item2;

                        if (percentage > 0)
                        {
                            _appDataContext.Configuration.PixelChangeTolerance++;
                        }
                        else
                        {
                            if (!syncStopwatch.IsRunning)
                                syncStopwatch.Start();

                            _appDataContext.Status = string.Format("Calculating Pixel Change Tolerance... {0} seconds remaining...", _appDataContext.Configuration.WaitTimeSeconds - syncStopwatch.Elapsed.TotalSeconds);

                            if (syncStopwatch.Elapsed.TotalSeconds > _appDataContext.Configuration.WaitTimeSeconds)
                                break;
                        }

                        _appDataContext.ImageChangePercentage = percentage;
                        _imageDifference = imageDiff;
                    }
                }
                catch (Exception e)
                { }
            }
            _imageDifference = null;
        }

        private Tuple<double, BitmapImage> CalculateDifferentPercentageOnPixels()
        {
            if (_currentImage != null && _previousImage != null)
            {
                int DifferentPixels = 0;
                Bitmap first = _currentImage;
                Bitmap second = _previousImage;
                Bitmap container = new Bitmap(first.Width, first.Height);
                for (int i = 0; i < first.Width; ++i)
                {
                    for (int j = 0; j < first.Height; ++j)
                    {
                        Color secondColor = second.GetPixel(i, j);
                        Color firstColor = first.GetPixel(i, j);

                        var aDiff = Math.Abs(firstColor.A - secondColor.A);
                        var bDiff = Math.Abs(firstColor.B - secondColor.B);
                        var gDiff = Math.Abs(firstColor.G - secondColor.G);
                        var rDiff = Math.Abs(firstColor.R - secondColor.R);

                        if (aDiff >= _appDataContext.Configuration.PixelChangeTolerance || bDiff >= _appDataContext.Configuration.PixelChangeTolerance || gDiff >= _appDataContext.Configuration.PixelChangeTolerance || rDiff >= _appDataContext.Configuration.PixelChangeTolerance)
                        {
                            DifferentPixels++;
                            container.SetPixel(i, j, Color.Red);
                        }
                        else
                        {
                            container.SetPixel(i, j, firstColor);
                        }
                    }
                }
                int TotalPixels = first.Width * first.Height;
                float difference = (float)((float)DifferentPixels / (float)TotalPixels);
                return new Tuple<double, BitmapImage>((Math.Round(difference * 100, 2)), container.BitmapToBitmapImage());
            }
            return null;
        }


    }
}
