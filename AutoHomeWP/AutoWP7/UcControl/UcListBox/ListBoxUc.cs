using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace AutoWP7.UcControl.UcListBox
{
    public class ListBoxUc : ListBox
    {
        private bool isPulling = false;
        private ScrollViewer ElementScrollViewer;
        private UIElement ElementRelease;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshBox"/> class.
        /// </summary>
        public ListBoxUc()
        {
            DefaultStyleKey = typeof(ListBoxUc);
            this.MouseLeftButtonDown += RefreshBox_MouseLeftButtonDown;
            this.MouseMove += ListBoxUc_MouseMove;
        }

        void ListBoxUc_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.cBefore = e.GetPosition(null);//获取点击前鼠标坐标
            VerticalPullToRefreshDistance = pBefore.Y - cBefore.Y;
        }

        Point pBefore = new Point();//鼠标点击坐标
        Point cBefore = new Point();//鼠标移动坐标
        void RefreshBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.pBefore = e.GetPosition(null);//获取点击前鼠标坐标
        }

        /// <summary>
        /// Builds the visual tree for the <see cref="T:System.Windows.Controls.ListBox"/>
        /// control when a new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (ElementScrollViewer != null)
            {
                ElementScrollViewer.MouseMove -= viewer_MouseMove;
                ElementScrollViewer.ManipulationCompleted -= viewer_ManipulationCompleted;
            }
            ElementScrollViewer = GetTemplateChild("ScrollViewer") as ScrollViewer;
            if (ElementScrollViewer != null)
            {
                ElementScrollViewer.MouseMove += viewer_MouseMove;
                ElementScrollViewer.ManipulationCompleted += viewer_ManipulationCompleted;
                this.Loaded += RefreshBox_Loaded;
            }
            ElementRelease = GetTemplateChild("ReleaseElement") as UIElement;

            ChangeVisualState(false);
        }

        void RefreshBox_Loaded(object sender, RoutedEventArgs e)
        {
            RegisterScrollListBoxEvent();
        }

        public static List<T> GetVisualChildCollection<T>(object parent) where T : UIElement
        {
            List<T> visualCollection = new List<T>();
            GetVisualChildCollection(parent as DependencyObject, visualCollection);
            return visualCollection;
        }

        public static void GetVisualChildCollection<T>(DependencyObject parent, List<T> visualCollection) where T : UIElement
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                    visualCollection.Add(child as T);
                else if (child != null)
                    GetVisualChildCollection(child, visualCollection);
            }
        }

        private void RegisterScrollListBoxEvent()
        {
            List<ScrollBar> controlScrollBarList = GetVisualChildCollection<ScrollBar>(this);
            if (controlScrollBarList == null)
                return;

            foreach (ScrollBar queryBar in controlScrollBarList)
            {
                if (queryBar.Orientation == System.Windows.Controls.Orientation.Vertical)
                    queryBar.ValueChanged += queryBar_ValueChanged;
            }
        }

        void queryBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ScrollBar scrollBar = (ScrollBar)sender;
            object valueObj = scrollBar.GetValue(ScrollBar.ValueProperty);
            object maxObj = scrollBar.GetValue(ScrollBar.MaximumProperty);
            object minObj = scrollBar.GetValue(ScrollBar.MinimumProperty);

            if (valueObj != null && maxObj != null)
            {
                double value = (double)valueObj;
                double max = (double)maxObj;
                double min = (double)minObj;

                if (value >= max - 2)
                {
                    #region Load Old
                    MessageBox.Show("加载更多...");
                    #endregion
                }
            }
        }

        private void ChangeVisualState(bool useTransitions)
        {
            if (isPulling)
            {
                GoToState(useTransitions, "Pulling");
            }
            else
            {
                GoToState(useTransitions, "NotPulling");
            }
        }

        private bool GoToState(bool useTransitions, string stateName)
        {
            return VisualStateManager.GoToState(this, stateName, useTransitions);
        }

        private void viewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (VerticalOffset == 0)
            {
                var p = this.TransformToVisual(ElementRelease).Transform(new Point());
                //if (p.Y < -VerticalPullToRefreshDistance)
                if (130 < -VerticalPullToRefreshDistance)
                {
                    if (!isPulling)
                    {
                        isPulling = true;
                        if (EnteredPullRefreshThreshold != null)
                        {
                            EnteredPullRefreshThreshold(this, EventArgs.Empty);
                        }
                        ChangeVisualState(true);
                    }
                }
                else if (isPulling)
                {
                    isPulling = false;
                    if (LeftPullRefreshThreshold != null)
                    {
                        LeftPullRefreshThreshold(this, EventArgs.Empty);
                    }
                    ChangeVisualState(true);
                }
            }
        }

        private void viewer_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            var p = this.TransformToVisual(ElementRelease).Transform(new Point());
            //if (p.Y < -VerticalPullToRefreshDistance)
            if (130 < -VerticalPullToRefreshDistance)
            {
                if (PullRefresh != null)
                    PullRefresh(this, EventArgs.Empty);
                isPulling = false;
                ChangeVisualState(true);
            }
        }

        private double VerticalOffset
        {
            get
            {
                if (ElementScrollViewer == null) return double.NaN;
                return ElementScrollViewer.VerticalOffset;
            }
        }
        private double ScrollHeight
        {
            get
            {
                if (ElementScrollViewer == null) return double.NaN;
                return ElementScrollViewer.ScrollableHeight;
            }
        }

        /// <summary>
        /// Distance in pixels to pull down the RefreshBox before a refresh will get initiated.
        /// </summary>
        public double VerticalPullToRefreshDistance
        {
            get { return (double)GetValue(VerticalPullToRefreshDistanceProperty); }
            set { SetValue(VerticalPullToRefreshDistanceProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="VerticalPullToRefreshDistance"/> property
        /// </summary>
        public static readonly DependencyProperty VerticalPullToRefreshDistanceProperty =
            DependencyProperty.Register("VerticalPullToRefreshDistance", typeof(double), typeof(ListBoxUc), new PropertyMetadata(20d));

        /// <summary>
        /// Gets or sets the refresh text. Ie "Pull down to refresh".
        /// </summary>
        public string RefreshText
        {
            get { return (string)GetValue(RefreshTextProperty); }
            set { SetValue(RefreshTextProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="RefreshText"/> property
        /// </summary>
        public static readonly DependencyProperty RefreshTextProperty =
            DependencyProperty.Register("RefreshText", typeof(string), typeof(ListBoxUc), new PropertyMetadata("Pull down to refresh..."));

        /// <summary>
        /// Gets or sets the release text. Ie "Release to refresh".
        /// </summary>
        public string ReleaseText
        {
            get { return (string)GetValue(ReleaseTextProperty); }
            set { SetValue(ReleaseTextProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ReleaseText"/> property
        /// </summary>
        public static readonly DependencyProperty ReleaseTextProperty =
            DependencyProperty.Register("ReleaseText", typeof(string), typeof(ListBoxUc), new PropertyMetadata("Release to refresh..."));

        /// <summary>
        /// Sub text below Release/Refresh text. For example: Updated last: 12:34pm
        /// </summary>
        public string PullSubtext
        {
            get { return (string)GetValue(PullSubtextProperty); }
            set { SetValue(PullSubtextProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="PullSubtext"/> property
        /// </summary>
        public static readonly DependencyProperty PullSubtextProperty =
            DependencyProperty.Register("PullSubtext", typeof(string), typeof(ListBoxUc), null);

        /// <summary>
        /// Triggered when the user requested a refresh.
        /// </summary>
        public event EventHandler PullRefresh;
        /// <summary>
        /// If the user lets go of the screen after this event fires, a PullRefresh event is fired.
        /// </summary>
        public event EventHandler EnteredPullRefreshThreshold;
        /// <summary>
        /// If the user exited the "refresh" area without letting go of the screen.
        /// </summary>
        public event EventHandler LeftPullRefreshThreshold;
        /// <summary>
        /// load more data (load next page data)
        /// </summary>
        public event EventHandler LoadMoreData;
    }
}
