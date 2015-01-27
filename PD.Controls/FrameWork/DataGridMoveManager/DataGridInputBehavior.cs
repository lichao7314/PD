using System;
using System.Net;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using PD.Controls;

namespace PD.Controls
{
    /// <summary>
    /// 网格输入行为控制
    /// </summary>
    public class DataGridMoveInputBehavior:IDisposable
    {
        private DataGrid dataGrid = null;

        private OrientationProviderCollection providerCollection = new OrientationProviderCollection();

        /// <summary>
        /// 需要进行输入的网格
        /// </summary>
        public DataGrid DataGrid
        {
            get { return dataGrid; }
            set
            {
                UnRegisterEvent();
                dataGrid = value;
                RegisterEvent();
            }
        }

        private EditColumnCollection editColumns =new EditColumnCollection();

        /// <summary>
        /// 编辑行
        /// </summary>
        internal EditColumnCollection EditColumns
        {
            get { return editColumns; }
        }

        /// <summary>
        /// 初始化网格输入行为
        /// </summary>
        public void Init(EditColumnCollection editIndexs)
        {
            if (DataGrid == null)
                throw new NullReferenceException("没有找到网格对象实例");

            editIndexs.OrderBy(index=>index.ColumnIndex).ToList().ForEach(index =>
            {
                if (DataGrid.Columns.Count <= index.ColumnIndex)
                {
                    throw new ArgumentException("没有找到第" + index + "列，列不再网格列集合中");
                }

                index.Column = DataGrid.Columns[index.ColumnIndex];

                editColumns.Add(index);
            });
        }

       /// <summary>
       /// 单元格选择改变
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void CurrentCellSelectChanged(object sender, EventArgs e)
        {
            if (ContainColumn(DataGrid.CurrentColumn)&&DataGrid.SelectedItem != null)
            {
                DataGrid.BeginEdit();

                SetCurrentProvider(DataGrid.CurrentColumn, DataGrid.SelectedItem);

                SetCellControlFocus(DataGrid.CurrentColumn.GetCellContent(DataGrid.SelectedItem));
            }
        }

        /// <summary>
        /// 设置当前单元格控件的方向提供者
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        private void SetCurrentProvider(DataGridColumn column, object row)
        {
            if (row == null) return;
            var control = column.GetCellContent(row);

            if (providerCollection[control] == null)
            {
                var orientationProvider = ManagerOrientationProvider.InitControlPrivider(control);

                if (orientationProvider != null)
                {
                    orientationProvider.SetOrientationProvider(control);

                    orientationProvider.Behavior = this;

                    providerCollection.Add(orientationProvider);
                }
            }
        }

        /// <summary>
        /// 对元素设置Provider的设置焦点方法
        /// </summary>
        /// <param name="control"></param>
        private void SetCellControlFocus(FrameworkElement control)
        {
            var provider = providerCollection[control];

            if (provider != null)
            {
                provider.SetFocus();
            }
        }

        /// <summary>
        /// 是否在编辑列表中包含某个列
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private bool ContainColumn(DataGridColumn column)
        {
            return editColumns.FirstOrDefault(item=>item.Column==column)!=null;
        }

        #region 事件资源管理
        private void RegisterEvent()
        {
            DataGrid.CurrentCellChanged += (CurrentCellSelectChanged);
        }

        private void UnRegisterEvent()
        {
            if (dataGrid != null)
            {
                dataGrid.CurrentCellChanged -= (CurrentCellSelectChanged);
            }
        }

        public void Dispose()
        {
            UnRegisterEvent();
            providerCollection.ForEach(provider =>
            {
                provider.Dispose();
            });
            editColumns.Clear();
        }
        #endregion
    }
}
