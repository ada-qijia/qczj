﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AutoWP7.UcControl
{
    public partial class MediaElementUc : UserControl
    {
        #region Property

        DispatcherTimer timer = null;
        DispatcherTimer menutimer = null;
        bool isMenuShown = false;
        double videoDuration;
        bool manipulating = false;
        public bool IsPlaying = false;
        bool isFullScreen = false;

        #endregion

        #region DependencyProperty : CoverImage

        public ImageSource CoverImageSource
        {
            get { return (ImageSource)GetValue(CoverImageSourceProperty); }
            set
            {
                SetValue(CoverImageSourceProperty, value);
            }
        }

        public static readonly DependencyProperty CoverImageSourceProperty =
            DependencyProperty.Register("CoverImageSource", typeof(ImageSource), typeof(MediaElementUc), new PropertyMetadata(null, CoverImageSourceChanged));

        private static void CoverImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MediaElementUc control = d as MediaElementUc;
            control.coverImage.Source = e.NewValue as ImageSource;
        }

        #endregion

        #region DependencyProperty : VideoSource

        public Uri VideoSource
        {
            get { return (Uri)GetValue(VideoSourceProperty); }
            set
            {
                SetValue(VideoSourceProperty, value);
            }
        }

        public static readonly DependencyProperty VideoSourceProperty =
            DependencyProperty.Register("VideoSource", typeof(Uri), typeof(MediaElementUc), new PropertyMetadata(null));

        #endregion

        public MediaElementUc()
        {
            InitializeComponent();
            this.mediaElement.MediaOpened += mediaElement_MediaOpened;
            this.mediaElement.CurrentStateChanged += mediaElement_CurrentStateChanged;
        }

        #region Public Method

        public void SetSource(string url)
        {
            Uri uri = new Uri(url, UriKind.Absolute);
            this.VideoSource = uri;
        }

        public void SetCover(string url)
        {
            Uri uri = new Uri(url, UriKind.Absolute);
            this.CoverImageSource = new BitmapImage(uri);
        }

        #endregion

        #region MediaElement Event

        void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            videoDuration = this.mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
            this.totalTimeTextSmall.Text = this.totalTimeTextBig.Text = TimeSpanToString(this.mediaElement.NaturalDuration.TimeSpan);
            this.volumeBar.Value = this.mediaElement.Volume;
            StartTimer();
            ShowMenu(true);
        }

        void mediaElement_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            if (this.mediaElement.CurrentState == MediaElementState.Playing)
            {
                playButtonSmall.Opacity = playButtonBig.Opacity = 0;
                playButtonSmall.IsHitTestVisible = playButtonBig.IsHitTestVisible = false;
                pauseButtonSmall.Opacity = pauseButtonBig.Opacity = 1;
                pauseButtonSmall.IsHitTestVisible = pauseButtonBig.IsHitTestVisible = true;
            }
            else
            {
                playButtonSmall.Opacity = playButtonBig.Opacity = 1;
                playButtonSmall.IsHitTestVisible = playButtonBig.IsHitTestVisible = true;
                pauseButtonSmall.Opacity = pauseButtonBig.Opacity = 0;
                pauseButtonSmall.IsHitTestVisible = pauseButtonBig.IsHitTestVisible = false;
            }
        }

        #endregion

        #region Control Menu

        private void mediaElement_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (isMenuShown)
            {
                if (menutimer.IsEnabled)
                {
                    menutimer.Stop();
                    return;
                }
                HideMenu();
            }
            else
            {
                ShowMenu(false);
            }
        }

        private void StartTimer()
        {
            if (timer == null)
            {
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(200);
                timer.Tick += timer_Tick;
            }
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (!manipulating)
                    {
                        this.timeBarSmall.Value = this.timeBarBig.Value = this.timeBarBig.Value = this.mediaElement.Position.TotalSeconds * 100 / videoDuration;
                    }
                    this.currentTimeTextSmall.Text = this.currentTimeTextBig.Text = TimeSpanToString(this.mediaElement.Position);
                }));
        }

        private void ShowMenu(bool autoHide)
        {
            VisualStateManager.GoToState(this, "TimePanelShown", true);
            VisualStateManager.GoToState(this, "VolumePanelShown", true);
            isMenuShown = true;

            if (autoHide)
            {
                if (menutimer == null)
                {
                    menutimer = new DispatcherTimer();
                    menutimer.Interval = TimeSpan.FromMilliseconds(3000);
                    menutimer.Tick += menuTimer_Tick;
                }
                menutimer.Start();
            }
        }

        private void HideMenu()
        {
            VisualStateManager.GoToState(this, "TimePanelHidden", true);
            VisualStateManager.GoToState(this, "VolumePanelHidden", true);
            isMenuShown = false;
        }

        private void menuTimer_Tick(object sender, EventArgs e)
        {
            menutimer.Stop();
            HideMenu();
        }

        public string TimeSpanToString(TimeSpan duration)
        {
            return string.Format("{0:D2}:{1:D2}:{2:D2}", duration.Hours, duration.Minutes, duration.Seconds);
        }

        private void timeBar_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            Slider slider = sender as Slider;
            this.mediaElement.Position = TimeSpan.FromSeconds(slider.Value * videoDuration / 100);
            manipulating = false;
        }

        private void timeBar_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            manipulating = true;
        }

        const double VOLUME_C = 9.21045;
        private void volumeBar_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            UpdateVolume();
        }

        private void UpdateVolume()
        {
            //http://blog.poxiao.me/p/wp8-mediaelement-volume-control/
            this.mediaElement.Volume = Math.Log(this.volumeBar.Value * (Math.Pow(Math.E, VOLUME_C) - 1) + 1) / VOLUME_C;
            //this.mediaElement.Volume = this.volumeBar.Value;
        }

        private void playPauseButtons_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PlayPause();
        }

        private void PlayPause()
        {
            IsPlaying = !IsPlaying;
            if (IsPlaying)
            {
                mediaElement.Play();
            }
            else
            {
                mediaElement.Pause();
            }
            //playButtonSmall.Opacity = playing ? 0 : 1;
            //playButtonSmall.IsHitTestVisible = playing ? false : true;
            //pauseButtonSmall.Opacity = playing ? 1 : 0;
            //pauseButtonSmall.IsHitTestVisible = playing ? true : false;
        }

        #endregion

        #region Cover

        private void cover_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.coverImage.IsHitTestVisible = false;
            this.coverImage.Visibility = Visibility.Collapsed;
            this.mediaElement.Source = VideoSource;
        }

        #endregion

        #region Orientation

        public event EventHandler<bool> FullScreen;

        private void screenSizeButtons_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SetFullScreen(!isFullScreen);
        }

        private void SetFullScreen(bool toFullScreen)
        {
            isFullScreen = toFullScreen;

            screenExpandButtonSmall.Opacity = screenExpandButtonBig.Opacity = isFullScreen ? 0 : 1;
            screenExpandButtonSmall.IsHitTestVisible = screenExpandButtonBig.IsHitTestVisible = isFullScreen ? false : true;
            screenShrinkButtonSmall.Opacity = screenShrinkButtonBig.Opacity = isFullScreen ? 1 : 0;
            screenShrinkButtonSmall.IsHitTestVisible = screenShrinkButtonBig.IsHitTestVisible = isFullScreen ? true : false;

            timePanelSmall.Opacity = isFullScreen ? 0 : 1;
            timePanelSmall.IsHitTestVisible = isFullScreen ? false : true;
            timePanelBig.Opacity = isFullScreen ? 1 : 0;
            timePanelBig.IsHitTestVisible = isFullScreen ? true : false;

            if (FullScreen != null)
            {
                FullScreen(this, isFullScreen);
            }
        }

        #endregion

        #region Forward / Backward

        const double FAST_MOVE_MAX = 3d;
        const double FAST_MOVE_MIN = -3d;
        const double VOLUME_INCREMENT = 0.05d;

        private void Forward()
        {
            double targetTime = this.mediaElement.Position.TotalSeconds + 30;
            targetTime = targetTime > videoDuration ? videoDuration : targetTime;
            this.mediaElement.Position = TimeSpan.FromSeconds(targetTime);
        }

        private void Backward()
        {
            double targetTime = this.mediaElement.Position.TotalSeconds - 30;
            targetTime = targetTime < 0 ? 0 : targetTime;
            this.mediaElement.Position = TimeSpan.FromSeconds(targetTime);
        }

        private void forwardButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Forward();
        }

        private void backwardButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Backward();
        }

        private void mediaElement_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            if (e.DeltaManipulation.Translation.X > FAST_MOVE_MAX)
            {
                Forward();
            }
            else if (e.DeltaManipulation.Translation.X < FAST_MOVE_MIN)
            {
                Backward();
            }
            else if (e.DeltaManipulation.Translation.Y > FAST_MOVE_MAX)
            {
                double targetVolume = this.volumeBar.Value - VOLUME_INCREMENT;
                this.volumeBar.Value = targetVolume < 0 ? 0 : targetVolume;
                UpdateVolume();
            }
            else if (e.DeltaManipulation.Translation.Y < FAST_MOVE_MIN)
            {
                double targetVolume = this.volumeBar.Value + VOLUME_INCREMENT;
                this.volumeBar.Value = targetVolume > 1 ? 1 : targetVolume;
                UpdateVolume();
            }
        }

        #endregion

    }
}
