using Model.Search;
using System.Windows;

namespace AutoWP7.UcControl.SearchResult
{
    public class NatureResultDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ArticleTemplate { get; set; }

        public DataTemplate TopicTemplate { get; set; }

        public DataTemplate VideoTemplate { get; set; }

        public DataTemplate LobbyistTemplate { get; set; }

        public DataTemplate LoadMoreTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            NaturalModel model = item as NaturalModel;
            if (model != null)
            {
                if (model.IsLoadMore)
                {
                    return LoadMoreTemplate;
                }
                else
                {
                    switch (model.MediaType)
                    {
                        //文章
                        case 1:
                            return this.ArticleTemplate;
                        //帖子
                        case 5:
                            return this.TopicTemplate;
                        //视频
                        case 3:
                            return this.VideoTemplate;
                        //说客
                        case 2:
                            return this.LobbyistTemplate;
                    }
                }
            }

            return base.SelectTemplate(item,container);
        }
    }
}
