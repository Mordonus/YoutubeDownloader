﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace YoutubeDownloader
{
    public sealed class CustomMediaTransportControls : MediaTransportControls
    {
        public CustomMediaTransportControls()
        {
            DefaultStyleKey = typeof(CustomMediaTransportControls);
        }

        protected override void OnApplyTemplate()
        {
            //This is where you would get your custom button and create an event handler for its click method.
            Button RateButton = GetTemplateChild("RateButton") as Button;
            RateButton.Click += RateButton_Click;
            base.OnApplyTemplate();
        }

        private void RateButton_Click(object sender, RoutedEventArgs e)
        {
            //When creating a button, you would add your custom code here

        }
    }

}
