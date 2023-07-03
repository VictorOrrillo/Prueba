using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Prueba
{
    public partial class MainPage : ContentPage
    {
        private CascadeClassifier faceCascade;
        private Mat registeredFace;

        public MainPage()
        {
            InitializeComponent();
            LoadCascadeClassifier();
        }

        private void LoadCascadeClassifier()
        {
            var cascadeFilePath = "haarcascade_frontalface_default.xml";

            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "haarcascade_frontalface_default.xml");
        }

        private async void RegisterFaceButton_Clicked(object sender, EventArgs e)
        {
            var photo = await TakePhoto();

            if (photo != null)
            {
                using (var image = new Mat(photo))
                using (var grayMat = new Mat())
                {
                    Cv2.CvtColor(image, grayMat, ColorConversionCodes.BGR2GRAY);

                    var faces = faceCascade.DetectMultiScale(grayMat, 1.1, 10, 0, new OpenCvSharp.Size(30, 30));

                    if (faces.Length == 1)
                    {
                        registeredFace = grayMat[faces[0]];

                        await DisplayAlert("Success", "Face registered successfully.", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Error", "No valid face detected.", "OK");
                    }
                }
            }
        }

        private async void LoginButton_Clicked(object sender, EventArgs e)
        {
            var photo = await TakePhoto();

            if (photo != null)
            {
                using (var image = new Mat(photo))
                using (var grayMat = new Mat())
                {
                    Cv2.CvtColor(image, grayMat, ColorConversionCodes.BGR2GRAY);

                    var faces = faceCascade.DetectMultiScale(grayMat, 1.1, 10, 0, new OpenCvSharp.Size(30, 30));

                    if (faces.Length == 1)
                    {
                        var currentFace = grayMat[faces[0]];

                        var result = Cv2.CompareHist(registeredFace, currentFace, HistCompMethods.Correl);

                        if (result > 0.9)
                        {
                            await DisplayAlert("Success", "Face recognized. Login successful.", "OK");
                        }
                        else
                        {
                            await DisplayAlert("Error", "Face not recognized. Login failed.", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Error", "No valid face detected.", "OK");
                    }
                }
            }
        }

        private async Task<string> TakePhoto()
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();

                return photo?.FullPath;
            }
            catch (FeatureNotSupportedException)
            {
                await DisplayAlert("Error", "Capturing photos is not supported on this device.", "OK");
            }
            catch (PermissionException)
            {
                await DisplayAlert("Error", "The necessary permissions for accessing the camera have not been granted.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }

            return null;
        }
    }
}
