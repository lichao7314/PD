using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
namespace PD.Controls
{
    [TemplatePart(Name = PART_dataPager, Type = typeof(TextBlock))]
    public class FormDataGrid : DataGrid, IDisposable
    {
        internal DataGridTextColumn numberTextColumn = new DataGridTextColumn()
        {
            IsReadOnly = true,
            Header = "序号",
            CanUserReorder = false,
            CanUserSort = false,
            MinWidth = 40 
        };

        internal DataGridSelectColumn checkBoxColumn = new DataGridSelectColumn();

        /// <summary>
        /// 获取序号列
        /// </summary>
        public DataGridTextColumn NumberColumn { get { return numberTextColumn; } }

        private const string PART_dataPager = "dataPager";

        private FormDataPager formDataPager = null;

        #region 分页
        /// <summary>
        /// 在 PageIndex 更改后发生。
        /// </summary>
        public event EventHandler<EventArgs> PageIndexChanged;
        /// <summary>
        /// 在 PageIndex 正在更改时发生。
        /// </summary>
        public event EventHandler<CancelEventArgs> PageIndexChanging;
        /// <summary>
        /// 获取或设置当前页的索引
        /// </summary>
        public int PageIndex
        {
            get
            {
                if (formDataPager != null)
                    return formDataPager.PageIndex;
                return pageIndex;
            }
            set { pageIndex = value; if (formDataPager != null)formDataPager.PageIndex = value; }
        }
        /// <summary>
        /// 获取或设置一个值，该值指示在页面上显示的项的数目。
        /// </summary>
        public int PageSize { get { return pageSize; } set { pageSize = value; if (formDataPager != null) formDataPager.PageSize = value; } }

        private IEnumerable pageSource;
        private int pageSize = 10;
        private int pageIndex;

        /// <summary>
        /// 获取或设置 System.Windows.Controls.DataPager 为其控制分页的数据集合。
        /// </summary>
        public IEnumerable PageSource
        {
            get { return pageSource; }
            set { pageSource = value; if (formDataPager != null)formDataPager.Source = value; }
        }
        /// <summary>
        /// 分页控件
        /// </summary>
        public  FormDataPager DataPager
        {
            get
            {
                return formDataPager;
            }
        }
        #endregion

        /// <summary>
        /// 列头选中改变事件
        /// </summary>
        public event EventHandler<CheckEventArgs> HeaderChecked;
        /// <summary>
        /// 列复选框选中改变事件
        /// </summary>
        public event EventHandler<RowCheckEventArgs> Checked;


        /// <summary>
        /// 序号列的文本头
        /// </summary>
        public string NumberHeader
        {
            get { return (string)GetValue(NumberHeaderProperty); }
            set { SetValue(NumberHeaderProperty, value); }
        }
        public static readonly DependencyProperty NumberHeaderProperty = DependencyProperty.Register("NumberHeader", typeof(string), typeof(FormDataGrid), new PropertyMetadata("", (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {
            var gridView = (d as FormDataGrid);
            gridView.ColumnManager();
        }));

        /// <summary>
        /// 是否现在序号列
        /// </summary>
        public bool ShowNumberColumn
        {
            get { return (bool)GetValue(ShowNumberColumnProperty); }
            set { SetValue(ShowNumberColumnProperty, value); }
        }

        public static readonly DependencyProperty ShowNumberColumnProperty = DependencyProperty.Register("ShowNumberColumn", typeof(bool), typeof(FormDataGrid), new PropertyMetadata(false, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {
            var gridView = (d as FormDataGrid);
            gridView.ColumnManager();
        }));

        /// <summary>
        /// 是否显示分页控件
        /// </summary>
        public bool ShowDataPager
        {
            get { return (bool)GetValue(ShowDataPagerProperty); }
            set { SetValue(ShowDataPagerProperty, value); }
        }

        public static readonly DependencyProperty ShowDataPagerProperty = DependencyProperty.Register("ShowDataPager", typeof(bool), typeof(FormDataGrid), new PropertyMetadata(true, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {
            var gridView = (d as FormDataGrid);
            gridView.DisplayDataPager();
        }));

        /// <summary>
        /// 是否显示复选框列
        /// </summary>
        public bool ShowCheckBoxColumn
        {
            get { return (bool)GetValue(ShowCheckBoxColumnProperty); }
            set { SetValue(ShowCheckBoxColumnProperty, value); }
        }

        public static readonly DependencyProperty ShowCheckBoxColumnProperty = DependencyProperty.Register("ShowCheckBoxColumn", typeof(bool), typeof(FormDataGrid), new PropertyMetadata(false, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {
            var gridView = (d as FormDataGrid);
            gridView.ColumnManager();
        }));
        /// <summary>
        /// 获取或者设置数据源
        /// </summary>
        public new IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
                checkBoxColumn._markObjects.Values.ToList().ForEach(item =>
                {
                    item.PropertyChanged -= RowSelectChange;
                });
                this.checkBoxColumn._markObjects.Clear();
            }
        }

        public FormDataGrid()
        {
            DefaultStyleKey = typeof(FormDataGrid);
            checkBoxColumn._markObject.PropertyChanged += HeaderCheckChange;
            numberTextColumn.Binding = new Binding { Converter = new NumberConverter { DataGrid = this } };
        }

        /// <summary>
        /// 设置选中项目
        /// </summary>
        /// <param name="objEntity"></param>
        /// <param name="vIsChecked"></param>
        public void SetCheckItem(object objEntity, bool vIsChecked)
        {
            if (ShowCheckBoxColumn)
            {
                checkBoxColumn.SetSelectedItem(objEntity, vIsChecked);
                RegisterRowCheckEvent(true);
            }
        }
        /// <summary>
        /// 设置列头是否选中
        /// </summary>
        /// <param name="check">是否选中</param>
        public void SetHeaderCheck(bool check)
        {
            if (ShowCheckBoxColumn)
            {
                checkBoxColumn._markObject.Selected = check;
            }
        }
        /// <summary>
        /// 获取存在Checkbox列选中的列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetCheckedItems<T>()
        {
            if (ShowCheckBoxColumn)
            {
                return checkBoxColumn.GetSelectedItems<T>();
            }
            return default(List<T>);
        }

        private void HeaderCheckChange(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (HeaderChecked != null)
            {
                HeaderChecked(this, new CheckEventArgs { Check = checkBoxColumn._markObject.Selected });
            }
        }

        protected override void OnLoadingRow(DataGridRowEventArgs e)
        {
            base.OnLoadingRow(e);

            RegisterRowCheckEvent(true);
        }

        private void RowSelectChange(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (Checked != null)
            {
                var data = (( MarkObject)(sender));

                Checked(this, new RowCheckEventArgs { Check = data.Selected, DataContext = data.RowDataContext });
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild(PART_dataPager) is FormDataPager)
            {
                formDataPager = GetTemplateChild(PART_dataPager) as FormDataPager; DisplayDataPager();
                formDataPager.PageIndexChanged += FormPageIndexChanged;
                formDataPager.PageIndexChanging += FormPageIndexChanging;
                formDataPager.PageSize = pageSize;
                formDataPager.Source = PageSource;
                if (PageSource != null)
                    formDataPager.PageIndex = pageIndex;
            }
        }

        void FormPageIndexChanging(object sender, CancelEventArgs e)
        {
            if (ShowDataPager && PageIndexChanging != null)
            {
                PageIndexChanging(sender, e);
            }
        }

        void FormPageIndexChanged(object sender, EventArgs e)
        {
            if (ShowDataPager && PageIndexChanged != null)
            {
                PageIndexChanged(sender, e);
            }
        }

        private void DisplayDataPager()
        {
            if (formDataPager != null)
            {
                formDataPager.Visibility = ShowDataPager ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void ColumnManager()
        {
            numberTextColumn.Header = NumberHeader;
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool) {
                numberTextColumn.HeaderStyle = Application.Current.Resources["DataGridColumnHeaderCenter"] as Style;
                numberTextColumn.CellStyle = Application.Current.Resources["DataGridCellCenter"] as Style;
            }
            int displayIndex = 0;
            if (ShowCheckBoxColumn)
            {
                if (!Columns.Contains(checkBoxColumn))
                {
                    Columns.Insert(displayIndex, checkBoxColumn);
                    checkBoxColumn.OwnerDataGrid = this;
                }
                checkBoxColumn.DisplayIndex = displayIndex;
                displayIndex++;
            }
            else
            {
                if (Columns.Contains(checkBoxColumn))
                {
                    checkBoxColumn.OwnerDataGrid = null;
                    Columns.Remove(checkBoxColumn);
                }
            }
            if (ShowNumberColumn)
            {
                if (!Columns.Contains(numberTextColumn))
                    Columns.Insert(displayIndex, numberTextColumn);
                numberTextColumn.DisplayIndex = displayIndex;
            }
            else
            {
                if (Columns.Contains(numberTextColumn))
                    Columns.Remove(numberTextColumn);
            }
        }

        private void RegisterRowCheckEvent(bool register)
        {
            checkBoxColumn._markObjects.Values.ToList().ForEach(item =>
            {
                item.PropertyChanged -= RowSelectChange;
                if (register)
                    item.PropertyChanged += RowSelectChange;
            });
        }

        public void Dispose()
        {
            RegisterRowCheckEvent(false);
            checkBoxColumn._markObject.PropertyChanged-= HeaderCheckChange;
            this.checkBoxColumn._markObjects.Clear();
        }
    }

  

    public class CheckEventArgs : EventArgs
    {
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool Check { get; internal set; }
    }

    public class RowCheckEventArgs : CheckEventArgs
    {
        /// <summary>
        /// 数据源
        /// </summary>
        public object DataContext { get; internal set; }
    }

    public class NumberConverter : IValueConverter
    {
        public FormDataGrid DataGrid { get; set; }
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int i=0;
            var dataItems = DataGrid.ItemsSource.GetEnumerator();
            while (dataItems.MoveNext())
            {
                i++;
                if (dataItems.Current == value)
                    return i;
            }
            return i;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

}
