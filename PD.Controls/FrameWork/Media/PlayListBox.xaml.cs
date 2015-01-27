using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace PD.Controls.Media
{
    public partial class PlayListBox : UserControl
    {
        #region 属性
        public IList<PlayListItem> PlayListSource
        {
            get
            {
                return null;
            }
            set
            {
                if (value != null)
                {
                    int index = 0;
                    int total = value.Count;

                    foreach (PlayListItem item in value)
                    {
                        index++;
                        item.ItemIndex = string.Format("{0}/{1}", index, total);
                    }
                }
                this.listBPlayItem.ItemsSource = value;
            }
        }
        #endregion

        #region 委托事件
        public event Action<PlayListItem> SelectPlayItemChanged;
        #endregion

        public PlayListBox()
        {
            InitializeComponent();

            this.listBPlayItem.SelectionMode = SelectionMode.Single;
            this.listBPlayItem.SelectionChanged += new SelectionChangedEventHandler(listBPlayItem_SelectionChanged);
        }

        void listBPlayItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PlayListItem playListItem = (sender as ListBox).SelectedItem as PlayListItem;
            if (playListItem != null)
            {
                if (this.SelectPlayItemChanged != null)
                {
                    this.SelectPlayItemChanged(playListItem);
                }
            }
        }
    }
}