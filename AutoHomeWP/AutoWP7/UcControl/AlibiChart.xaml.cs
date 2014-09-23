using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace AutoWP7.UcControl
{
    public partial class AlibiChart : UserControl
    {
        private const double MAX_VALUE = 5.0d;
        private const double COLUMN_HEIGHT_MAX = 200d;
        private const double COLUMN_HEIGHT_FACTOR = 40d; // 200d / 5.0d
        private const double COLUMN_HEAD_OFFSET = 24d;

        public AlibiChart()
        {
            InitializeComponent();
        }

        public void SetColumns(double v0, double v1, double v2, double v3, double v4, double v5, double v6, double v7)
        {
            column_0.Height = COLUMN_HEIGHT_FACTOR * v0;
            column_1.Height = COLUMN_HEIGHT_FACTOR * v1;
            column_2.Height = COLUMN_HEIGHT_FACTOR * v2;
            column_3.Height = COLUMN_HEIGHT_FACTOR * v3;
            column_4.Height = COLUMN_HEIGHT_FACTOR * v4;
            column_5.Height = COLUMN_HEIGHT_FACTOR * v5;
            column_6.Height = COLUMN_HEIGHT_FACTOR * v6;
            column_7.Height = COLUMN_HEIGHT_FACTOR * v7;

            columnHead_0.Text = v0.ToString("f2");
            columnHead_1.Text = v1.ToString("f2");
            columnHead_2.Text = v2.ToString("f2");
            columnHead_3.Text = v3.ToString("f2");
            columnHead_4.Text = v4.ToString("f2");
            columnHead_5.Text = v5.ToString("f2");
            columnHead_6.Text = v6.ToString("f2");
            columnHead_7.Text = v7.ToString("f2");

            columnHeadTransform_0.TranslateY = COLUMN_HEAD_OFFSET - COLUMN_HEIGHT_FACTOR * v0;
            columnHeadTransform_1.TranslateY = COLUMN_HEAD_OFFSET - COLUMN_HEIGHT_FACTOR * v1;
            columnHeadTransform_2.TranslateY = COLUMN_HEAD_OFFSET - COLUMN_HEIGHT_FACTOR * v2;
            columnHeadTransform_3.TranslateY = COLUMN_HEAD_OFFSET - COLUMN_HEIGHT_FACTOR * v3;
            columnHeadTransform_4.TranslateY = COLUMN_HEAD_OFFSET - COLUMN_HEIGHT_FACTOR * v4;
            columnHeadTransform_5.TranslateY = COLUMN_HEAD_OFFSET - COLUMN_HEIGHT_FACTOR * v5;
            columnHeadTransform_6.TranslateY = COLUMN_HEAD_OFFSET - COLUMN_HEIGHT_FACTOR * v6;
            columnHeadTransform_7.TranslateY = COLUMN_HEAD_OFFSET - COLUMN_HEIGHT_FACTOR * v7;

        }
    }
}
