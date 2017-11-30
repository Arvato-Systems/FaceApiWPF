using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Pen = System.Drawing.Pen;
using Brushes = System.Drawing.Brushes;
using SystemFonts = System.Drawing.SystemFonts;
using System.Drawing.Imaging;
using System.Configuration;

namespace FaceApiWFP
{
    /// <summary>
    /// Initial Idea: https://www.codeproject.com/Articles/462527/Camera-Face-Detection-in-Csharp-Using-Emgu-CV-Open
    /// Using: http://www.emgu.com/wiki/index.php/Main_Page
    /// </summary>
    public partial class MainWindow : Window
    {
        Capture capture;                        // EmguCV WebCam Capture Class
        DispatcherTimer timer;
        DateTime lastSecond;                    // time measurement for counting FPS
        DateTime lastAnalyse;                   // time measurment  for the next Azure request 
        int frames = 0;
        int last_framerate;                     // the framerate of the last second
        FaceModel[] faceResults;                // results of the Azure FaceApi
        string str_zoomValue = String.Empty;    // current zoom value
        Resolution resolution;                  // current camera resolution
        int sendAnalyseTimeframe;               // current time in ms for sendending the next Azure FaceApi analysis request
        string azureFaceApiUrl = String.Empty;  // URL to the Azure FaceApi (configure in app.config)
        string subscriptionKey = String.Empty;  // SubscriptionKey for the Azure FaceApi
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initiate all variables, setting the starting resolution and getting API URL and API Key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            capture = new Capture();
            resolution = Resolution.R640x480;
            str_zoomValue = capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Zoom).ToString();
            sendAnalyseTimeframe = 500;
            last_framerate = 0;

            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            lastSecond = DateTime.Now;
            lastAnalyse = DateTime.Now;
            timer.Stop();

            azureFaceApiUrl = ConfigurationManager.AppSettings["FaceApiUrl"];
            subscriptionKey = ConfigurationManager.AppSettings["FaceApiKey"];
        }

        void timer_Tick(object sender, EventArgs e)
        {
            frames++;
            DateTime currentTime = DateTime.Now;
            if ((currentTime - lastSecond).Seconds >= 1)
            {
                last_framerate = frames;
                frames = 0;
                lastSecond = currentTime;
            }
            if ((currentTime - lastAnalyse).TotalMilliseconds >= sendAnalyseTimeframe)
            {
                Bitmap currentFrame = TakePicture();
                using (MemoryStream pictureStream = new MemoryStream())
                {
                    currentFrame.Save(pictureStream, ImageFormat.Jpeg);
                    pictureStream.Seek(0, SeekOrigin.Begin);
                    SendPictureForAnalyseAsync(pictureStream);
                    pictureStream.Close();
                }
                lastAnalyse = currentTime;
            }

            info.Text = $"FPS: {last_framerate.ToString()}\n" +
                $"Z: x{str_zoomValue}\n" +
                $"{resolution.ToString()}\n" +
                $"TF: {sendAnalyseTimeframe.ToString()}";

            Bitmap pictureWithInfos = ProcessFaceModelOnBitmap(TakePicture(), faceResults);
            image1.Source = ToBitmapSource(pictureWithInfos);
        }

        private void TakeSnapshotAndAnalyseAndDrawResults()
        {
            using (MemoryStream pictureStream = new MemoryStream())
            {
                Bitmap picture = TakePicture();
                picture.Save(pictureStream, ImageFormat.Jpeg);
                pictureStream.Seek(0, SeekOrigin.Begin);
                FaceModel[] faceResults = SendPictureForAnalyseSync(pictureStream);
                pictureStream.Close();

                Bitmap pictureWithInfos = ProcessFaceModelOnBitmap(TakePicture(), faceResults);
                image1.Source = ToBitmapSource(pictureWithInfos);
            }
        }

        private Bitmap ProcessFaceModelOnBitmap(Bitmap bitmap, FaceModel[] faces)
        {
            if (faces == null || faces.Length < 0)
                return bitmap;

            var manipulatedBitmap = new Bitmap(bitmap);
            using (var g = Graphics.FromImage(manipulatedBitmap))
            {
                foreach (FaceModel fm in faces)
                {
                    FaceRectangle fr = fm.FaceRectangle;
                    g.DrawRectangle(new Pen(Brushes.Red, 2), fr.Left, fr.Top, fr.Width, fr.Height);
                    g.DrawString(ExtractFacemodelText(fm), SystemFonts.StatusFont, Brushes.Yellow, fr.Left, fr.Top + fr.Height);
                }
            }
            return manipulatedBitmap;
        }

        /// <summary>
        /// Extracts the output text from one given FaceModel and outputs the most 3 emotions and the age
        /// </summary>
        /// <param name="fm"></param>
        /// <returns></returns>
        private string ExtractFacemodelText(FaceModel fm) {
            Emotion emotion = fm.FaceAttributes.Emotion;

            List<EmotionElement> emotions = new List<EmotionElement>();
            emotions.Add(new EmotionElement("Anger", emotion.Anger));
            emotions.Add(new EmotionElement("Contemp", emotion.Contempt));
            emotions.Add(new EmotionElement("Disgust", emotion.Disgust));
            emotions.Add(new EmotionElement("Fear", emotion.Fear));
            emotions.Add(new EmotionElement("Happiness", emotion.Happiness));
            emotions.Add(new EmotionElement("Neutral", emotion.Neutral));
            emotions.Add(new EmotionElement("Sadness", emotion.Sadness));
            emotions.Add(new EmotionElement("Surprise", emotion.Surprise));
            emotions.Sort(delegate (EmotionElement a, EmotionElement b)
            {
                if (a.EmotionValue < b.EmotionValue) return 1;
                if (a.EmotionValue > b.EmotionValue) return -1;
                return 0;
            });

            string result = $"Age: {fm.FaceAttributes.Age}\n";
            for(int i=0; i<=2; i++)
            {
                if(emotions[i].EmotionValue >= 0.1)
                    result += $"{emotions[i].EmotionName}: {emotions[i].EmotionValue}\n";
            }

            return result;
        }    

        private Bitmap TakePicture()
        {
            return capture.QueryFrame().Bitmap;
            //return capture.QuerySmallFrame().Bitmap;
        }

        /// <summary>
        /// Building the URI for calling the Azure FaceAPI. Returns age, gender and emotions
        /// </summary>
        /// <returns></returns>
        string BuildUri()
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request parameters
            queryString["returnFaceId"] = "true";
            queryString["returnFaceLandmarks"] = "false";
            queryString["returnFaceAttributes"] = "age,gender,emotion";
            var uri = $"{azureFaceApiUrl}/detect?{queryString}";
            return uri;
        }

        HttpContent buildContent(MemoryStream imageStream)
        {
            if (imageStream == null)
                throw new ArgumentException("no input image stream found");
            byte[] byteData = null;
            using (BinaryReader reader = new BinaryReader(imageStream))
            {
                byteData = reader.ReadBytes((int)imageStream.Length);
            }
            var content = new ByteArrayContent(byteData);

            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            return content;
        }

        HttpClient BuildClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            return client;
        }

        /// <summary>
        /// Sending the picture synchrone to the Azure FaceAPI and parse the response into the FaceModel
        /// </summary>
        /// <param name="imageStream"></param>
        /// <returns>multiple FaceModels (one for each face)</returns>
        private FaceModel[] SendPictureForAnalyseSync(MemoryStream imageStream)
        {
            HttpResponseMessage response = BuildClient().PostAsync(BuildUri(), buildContent(imageStream)).GetAwaiter().GetResult();
            string responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            FaceModel[] result = null;
            result = FaceModel.FromJson(responseString);
            return result;
        }

        /// <summary>
        /// Sending the picture asynchrone to the Azure FaceAPI, response will be parsed by <see cref="PostAsyncCallback(HttpResponseMessage)"/>
        /// </summary>
        /// <param name="imageStream"></param>
        private void SendPictureForAnalyseAsync(MemoryStream imageStream)
        {
            BuildClient().PostAsync(
                BuildUri(),
                buildContent(imageStream))
                .ContinueWith(task => PostAsyncCallback(task.Result));
        }

        void PostAsyncCallback(HttpResponseMessage response)
        {
            string responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            faceResults = FaceModel.FromJson(responseString);
        }

        #region Helper function
        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        public static BitmapSource ToBitmapSource(Bitmap image)
        {
            using (System.Drawing.Bitmap source = image/*.Bitmap*/)
            {
                IntPtr ptr = source.GetHbitmap(); //obtain the Hbitmap

                BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr); //release the HBitmap
                return bs;
            }
        }
        #endregion

        #region buttons
        private void Btn_StartStop_Click(object sender, RoutedEventArgs e)
        {
            timer.IsEnabled = !timer.IsEnabled;
        }

        private void Btn_TakePicture_Click(object sender, RoutedEventArgs e)
        {
            TakeSnapshotAndAnalyseAndDrawResults();
        }

        private void ManipulateZoom(double zoomOffset)
        {
            double zoomValue = capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Zoom);
            zoomValue += zoomOffset;
            capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Zoom, zoomValue);
            str_zoomValue = zoomValue.ToString();
        }

        private void Btn_ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            ManipulateZoom(-1.0);
        }

        private void Btn_ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            ManipulateZoom(1.0);
        }

        private void Btn_SwitchResolution_Click(object sender, RoutedEventArgs e)
        {
            resolution = (Resolution)(((int)resolution + 1) % 4);

            switch (resolution)
            {
                case Resolution.R320x240:
                    capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameWidth, 320);
                    capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameHeight, 240);
                    break;
                case Resolution.R640x480:
                    capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameWidth, 640);
                    capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameHeight, 480);
                    break;
                case Resolution.R960x720:
                    capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameWidth, 960);
                    capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameHeight, 720);
                    break;
                case Resolution.R1280x960:
                    capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameWidth, 1280);
                    capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameHeight, 960);
                    break;
                default:
                    break;
            }
        }

        private void Btn_SwitchAnalyseTime_Click(object sender, RoutedEventArgs e)
        {
            switch (sendAnalyseTimeframe)
            {
                case 150:
                    sendAnalyseTimeframe = 300;
                    break;
                case 300:
                    sendAnalyseTimeframe = 500;
                    break;
                case 500:
                    sendAnalyseTimeframe = 1000;
                    break;
                case 1000:
                    sendAnalyseTimeframe = int.MaxValue;
                    faceResults = null;
                    break;
                case int.MaxValue:
                    sendAnalyseTimeframe = 150;
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}
