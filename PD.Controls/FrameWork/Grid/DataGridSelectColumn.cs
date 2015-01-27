using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Markup;

namespace PD.Controls
{
    #region DataGridSelectColumn
    public class DataGridSelectColumn : DataGridTemplateColumn
    {
        #region PrimaryField
        /// <summary>
        /// 行记录的主键字段,不设置时使用整个行数据为Key，否则对应主键字段的值为Key
        /// </summary>
        public string PrimaryField
        {
            get;
            set;
        }
        #endregion

        /// <summary>
        /// 所有行记录对应的变量
        /// </summary>
        public Dictionary<object, MarkObject> _markObjects;
        /// <summary>
        /// 列头变量
        /// </summary>
        public MarkObject _markObject;

        private DataGrid _ownerDataGrid;

        

        /// <summary>
        /// 构造函数
        /// </summary>
        public DataGridSelectColumn()
        {
            IsReadOnly = true;
            _markObjects = new Dictionary<object, MarkObject>();
            DataGridSelectColumnHelper helper = new DataGridSelectColumnHelper() { SelectColumn = this };
            this.HeaderStyle = helper.HeaderStyle;
            this.CellTemplate = helper.CellTemplate;
            if (Application.Current != null)
            {
                //this.HeaderStyle.BasedOn = Application.Current.Resources["baseDataGridColumnHeader"] as Style;
            }
            _markObject = helper.MarkObject;
            //_markObject.PropertyChanged += (sender, e) =>
            //{
            //    if (_markObject.Selected)
            //        SelecteAll();
            //    else
            //        UnselectAll();
            //};
            _markObject.PropertyChanged += new PropertyChangedEventHandler(ColumnHedadSelectedPropertyChanged);
        }
        protected override void RefreshCellContent(FrameworkElement element, string propertyName)
        {
            base.RefreshCellContent(element, propertyName);
        }
        
        #region ColumnHedadSelectedPropertyChanged
        public void ColumnHedadSelectedPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                _markObject.PropertyChanged -= new PropertyChangedEventHandler(ColumnHedadSelectedPropertyChanged);
                if (_markObject.Selected)
                    SelecteAll();
                else
                    UnselectAll();
            }
            finally
            {
                _markObject.PropertyChanged += new PropertyChangedEventHandler(ColumnHedadSelectedPropertyChanged);
            }
        }
        #endregion

        #region OwnerDataGrid
        /// <summary>
        /// 拥有该行的DataGrid
        /// </summary>
        public DataGrid OwnerDataGrid
        {
            get
            {
                return _ownerDataGrid;
            }
            set
            {
                if (value == null)
                {
                    if (_ownerDataGrid != null)
                    {
                        _ownerDataGrid.LoadingRow -= OnDataGridLoadingRow;
                    }
                }
                _ownerDataGrid = value;
                if (_ownerDataGrid != null)
                {
                    _ownerDataGrid.LoadingRow -= OnDataGridLoadingRow;
                    _ownerDataGrid.LoadingRow += OnDataGridLoadingRow;
                }
            }
        }
        #endregion

        private void OnDataGridLoadingRow(object sender, DataGridRowEventArgs e)
        {
            object dataContext = e.Row.DataContext;
            FrameworkElement element = this.GetCellContent(e.Row);
            element.DataContext = GetMarkObject(dataContext);
            ((FrameworkElement)element.Parent).DataContext = element.DataContext;
        }

        private void SetAllSelectedStates(bool value)
        {
            if (OwnerDataGrid.ItemsSource == null)
                return;

            var enu = OwnerDataGrid.ItemsSource.GetEnumerator();
            while (enu.MoveNext())
            {
                GetMarkObject(enu.Current).Selected = value;
            }
        }

        #region GetMarkObjectPrimaryValue
        /// <summary>
        /// 获得MarkObject值
        /// </summary>
        /// <param name="obj">DataGrid行</param>
        /// <returns>MarkObject值</returns>
        public object GetMarkObjectPrimaryValue(Object obj)
        {
            if (string.IsNullOrEmpty(this.PrimaryField))
            {
                return obj;
            }
            else
            {
                return GetFieldValue(obj, this.PrimaryField);
            }
        }
        #endregion

        internal MarkObject GetMarkObject(Object obj)
        {
            object vPrimaryValue = GetMarkObjectPrimaryValue(obj);
            object vRowDataContext = obj;
            obj = vPrimaryValue;

            if (_markObjects.ContainsKey(obj) == false)
            {
                MarkObject markObject;
                markObject = new MarkObject();
                markObject.RowDataContext = vRowDataContext;
                _markObjects.Add(obj, markObject);
            }

            return _markObjects[obj];
        }

        /// <summary>
        /// 选择所有
        /// </summary>
        public void SelecteAll()
        {            
            //_markObject.Selected = true;
            SetAllSelectedStates(true);
        }

        /// <summary>
        /// 取消所有已选
        /// </summary>
        public void UnselectAll()
        {
            //_markObject.Selected = false;
            SetAllSelectedStates(false);
        }

        /// <summary>
        /// 获得已选项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetSelectedItems<T>()
        {
            List<T> result = new List<T>();
            if (OwnerDataGrid.ItemsSource != null)
            {
                var enu = OwnerDataGrid.ItemsSource.GetEnumerator();
                while (enu.MoveNext())
                {
                    if (GetMarkObject(enu.Current).Selected&&enu.Current is T) 
                        result.Add((T)enu.Current);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取所有勾选的行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetSelectedAllItems<T>()
        {
            List<T> result = new List<T>();

            foreach (var xDe in this._markObjects)
            {
                MarkObject _MarkObj = (MarkObject)xDe.Value;
                if (_MarkObj.Selected)
                {
                    result.Add((T)xDe.Key);
                }
            }          
            return result;
        }

        /// <summary>
        /// 设置是否勾选
        /// </summary>
        /// <param name="objEntity">要设置的项</param>
        /// <param name="vIsChecked">是否勾选</param>
        public void SetSelectedItem(object objEntity, bool vIsChecked)
        {
            if (OwnerDataGrid.ItemsSource != null)
            {
                GetMarkObject(objEntity).Selected = vIsChecked;
            }
        }

        /// <summary>
        /// 判断是否有选择项
        /// </summary>
        /// <returns>true:有已选项;false:无选择项</returns>
        public bool HasSelectedItems()
        {
            bool result = false;
            if (OwnerDataGrid.ItemsSource != null)
            {
                var enu = OwnerDataGrid.ItemsSource.GetEnumerator();
                while (enu.MoveNext())
                {
                    if (GetMarkObject(enu.Current).Selected)
                    {
                        return true;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 获取已选行
        /// </summary>
        /// <param name="dataGrid">DataGrid对象</param>
        /// <returns>已选行对象</returns>
        public static DataGridSelectColumn GetSelectColumn(DataGrid dataGrid)
        {
            DataGridSelectColumn result = null;
            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                result = dataGrid.Columns[i] as DataGridSelectColumn;
                if (result != null)
                    break;
            }
            return result;
        }

        /// <summary>
        /// 获取字段值
        /// </summary>
        /// <param name="ObjectInstace">DataGrid行对象</param>
        /// <param name="FieldName">文件名</param>
        /// <returns>字段值</returns>
        public static object GetFieldValue(object ObjectInstace, string FieldName)
        {
            object result = null;
            result = ObjectInstace.GetType().InvokeMember(FieldName, System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, ObjectInstace, null);
            return result;
        }
    }
    #endregion

    #region MarkObject
    /// <summary>
    /// 自定义MarkObject
    /// </summary>
    public class MarkObject : INotifyPropertyChanged
    {
        /// <summary>
        /// 绑定的行记录
        /// </summary>
        public object RowDataContext;

        //public bool EnabledNotifyPropertyChanged = true;//当执行全选时需要屏蔽列头对象的NotifyPropertyChanged事件,不要造成死循环

        public bool _selected;

        /// <summary>
        /// 是否勾选
        /// </summary>
        public bool Selected
        {
            get { return _selected; }
            set
            {
                //if (_selected == value)
                //    return;

                _selected = value;
                //if (EnabledNotifyPropertyChanged)
                NotifyPropertyChanged("Selected");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
    #endregion
}
