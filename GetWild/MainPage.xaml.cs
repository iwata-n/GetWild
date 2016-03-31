using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 を参照してください

namespace GetWild
{

    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private LightSensor sensor;
        private uint thresholdValue;
        private MediaElement media = new MediaElement();

        public MainPage()
        {
            this.InitializeComponent();

            this.sensor = LightSensor.GetDefault();
            if (sensor == null)
            {
            }
        }

        async private void getWildAndTouch()
        {
            if (media.CurrentState == MediaElementState.Playing)
            {
                return;
            }
            var uri = new System.Uri("ms-appx:///Assets/getwild.mp3");
            var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);
            var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);

            media.SetSource(stream, file.ContentType);
            media.Play();
        }

        /// <summary>
        /// ページが表示された時に実行
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // センサのコールバックの登録
            this.sensor.ReadingChanged += new TypedEventHandler<LightSensor, LightSensorReadingChangedEventArgs>(ReadingChanged);

            this.threshold_slider.ValueChanged += new RangeBaseValueChangedEventHandler(ValueChanged);
        }

        async private void ReadingChanged(object sender, LightSensorReadingChangedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                LightSensorReading reading = e.Reading;
                this.nowValue.Text = String.Format("{0,5:0.00}", reading.IlluminanceInLux);

                if ((uint)reading.IlluminanceInLux < this.thresholdValue)
                {
                    getWildAndTouch();
                }
            });
        }

        async private void ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                this.thresholdValue = (uint)(e.NewValue) * 10;
                this.threshold.Text = String.Format("{0,5:0.00}", this.thresholdValue);
            });
        }
    }
}
