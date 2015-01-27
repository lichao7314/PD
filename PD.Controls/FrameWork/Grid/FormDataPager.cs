using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections;

namespace PD.Controls
{
    /// <summary>
    ///  DataPager类 (带总数)
    /// </summary>
    public class FormDataPager : DataPager, IDisposable
    {
        //定义变量
        TextBlock tbCurrentPagePrefix;
        TextBlock tbCurrentPageSuffix;
        Button btnNextPageButton;
        Button btnFirstPageButton;
        Button btnLastPageButton;
        Button btnPreviousPageButton;
        TextBox txtCurrentPageTextBox;

        /// <summary>
        /// 取得数据总数
        /// </summary>
        public int DataCount
        {
            get { return (int)GetValue(DataCountProperty); }
            private set { SetValue(DataCountProperty, value); }
        }

        public static readonly DependencyProperty DataCountProperty = DependencyProperty.Register("DataCount", typeof(int), typeof(FormDataPager), new PropertyMetadata(((DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {
            (d as FormDataPager).ExtendItem();
        })));

        /// <summary>
        /// 重写  当应用新模板时生成 System.Windows.Controls.DataPager 控件的可视化树。
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //通过名称取得模板中的元素
            tbCurrentPagePrefix = GetTemplateChild("CurrentPagePrefixTextBlock") as TextBlock;
            tbCurrentPageSuffix = GetTemplateChild("CurrentPageSuffixTextBlock") as TextBlock;
            btnNextPageButton = GetTemplateChild("NextPageButton") as Button;
            btnFirstPageButton = GetTemplateChild("FirstPageButton") as Button;
            btnLastPageButton = GetTemplateChild("LastPageButton") as Button;
            btnPreviousPageButton = GetTemplateChild("PreviousPageButton") as Button;
            txtCurrentPageTextBox = GetTemplateChild("CurrentPageTextBox") as TextBox;

            //重写以下元素的事件
            if (btnNextPageButton != null)
                btnNextPageButton.Click += ChangePageIndex;
            if (btnFirstPageButton != null)
                btnFirstPageButton.Click += ChangePageIndex;
            if (btnLastPageButton != null)
                btnLastPageButton.Click += ChangePageIndex;
            if (btnPreviousPageButton != null)
                btnPreviousPageButton.Click += ChangePageIndex;
            if (txtCurrentPageTextBox != null)
                txtCurrentPageTextBox.LostFocus += ChangePageIndex;
            if (txtCurrentPageTextBox != null)
                txtCurrentPageTextBox.KeyDown += KeyChangePageIndex;
            ExtendItem();
        }

        void KeyChangePageIndex(object sender, KeyEventArgs e)
        {
            ExtendItem();
        }

        void ChangePageIndex(object sender, RoutedEventArgs e)
        {
            ExtendItem();
        }
        /// <summary>
        /// 设置数据源
        /// </summary>
        /// <param name="source">数据源</param>
        private void SetSource(IEnumerable source)
        {

            if (source != null)
            {
                int i = 0;
                var enumerator = source.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    i++;
                }
                enumerator.Reset();
                base.Source = source;
                this.DataCount = i;
            }
        }
        /// <summary>
        /// 获取或设置 System.Windows.Controls.DataPager 为其控制分页的数据集合。
        /// </summary>
        public new IEnumerable Source
        {
            get { return base.Source; }
            set
            {
                SetSource(value);
            }
        }
        private void ExtendItem()
        {
            if (tbCurrentPagePrefix != null)
            {
                tbCurrentPagePrefix.Text = "总数:" + DataCount + "行" + " 页";
            }
        }
        /// <summary>
        /// 是否分页控件资源
        /// </summary>
        public void Dispose()
        {
            if (btnNextPageButton != null)
                btnNextPageButton.Click -= ChangePageIndex;
            if (btnFirstPageButton != null)
                btnFirstPageButton.Click -= ChangePageIndex;
            if (btnLastPageButton != null)
                btnLastPageButton.Click -= ChangePageIndex;
            if (btnPreviousPageButton != null)
                btnPreviousPageButton.Click -= ChangePageIndex;
            if (txtCurrentPageTextBox != null)
                txtCurrentPageTextBox.LostFocus -= ChangePageIndex;
            if (txtCurrentPageTextBox != null)
                txtCurrentPageTextBox.KeyDown -= KeyChangePageIndex;
        }
    }
}
