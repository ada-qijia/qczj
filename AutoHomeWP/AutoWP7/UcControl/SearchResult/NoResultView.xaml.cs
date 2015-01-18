using System.Windows.Controls;

namespace AutoWP7.UcControl.SearchResult
{
    public partial class NoResultView : UserControl
    {
        public NoResultView()
        {
            InitializeComponent();
        }

        public void SetContent(string keyword, string contentType)
        {
            this.promptTextBlock.Text = string.Format("没有找到和\"{0}\"相符的{1}", keyword, contentType);
        }

        public void SetContent(string content)
        {
            this.promptTextBlock.Text = content;
        }
    }
}
