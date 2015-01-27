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
using System.Collections.Generic;
using PD.Controls;
using System.Collections;

namespace PD.Controls
{
    public static class DataGridExpand
    {
        #region 标准扩展
        public static void IsTemplate(this string grid)
        {

        }

        /// <summary>
        /// 列是否为模板列
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static bool IsTemplate(this DataGrid grid, System.Windows.Controls.DataGridColumn column)
        {
            if (column == null)
                return false;
            return column is System.Windows.Controls.DataGridTemplateColumn;
        }

        /// <summary>
        /// 列是否存在单元格编辑模板
        /// </summary>
        /// <param name="column"></param>
        public static bool IsCellEditingTemplate(this DataGrid grid, System.Windows.Controls.DataGridColumn column)
        {
            if (grid.IsTemplate(column))
            {
                return (column as System.Windows.Controls.DataGridTemplateColumn).CellEditingTemplate != null;
            }
            return false;
        }

        /// <summary>
        /// 列是否存在单元格模板
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static bool IsCellTemplate(this DataGrid grid, System.Windows.Controls.DataGridColumn column)
        {
            if (grid.IsTemplate(column))
            {
                return (column as System.Windows.Controls.DataGridTemplateColumn).CellTemplate != null;
            }
            return false;
        }

        /// <summary>
        /// 是下一个单元格编辑
        /// </summary>
        /// <param name="grid"></param>
        public static void NextCellToEdit(this DataGrid grid, System.Windows.Controls.DataGridColumn column, object item)
        {
            grid.CurrentColumn = column;
            grid.SelectedItem = item;
            grid.BeginEdit();
        }

        /// <summary>
        /// 获取网格上一行数据
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static object LastRowItem(this DataGrid grid)
        {
            if (grid.ItemsSource == null)
                return null;
            return GetGridItemInChangeIndex(grid, -1);
        }

        /// <summary>
        /// 获取下一行数据
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static object NextRowItem(this DataGrid grid)
        {
            if (grid.ItemsSource == null)
                return null;
            return GetGridItemInChangeIndex(grid, 1);
        }

        /// <summary>
        /// 获取左边的列
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static System.Windows.Controls.DataGridColumn LeftColumnItem(this DataGrid grid)
        {
            return GetItemOrDefault(grid.Columns.ToList().OfType<object>().ToList(), grid.Columns.IndexOf(grid.CurrentColumn) - 1, grid.CurrentColumn) as System.Windows.Controls.DataGridColumn;
        }

        /// <summary>
        /// 获取右边的列
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static System.Windows.Controls.DataGridColumn RightColumnItem(this DataGrid grid)
        {
            return GetItemOrDefault(grid.Columns.ToList().OfType<object>().ToList(), grid.Columns.IndexOf(grid.CurrentColumn) + 1, grid.CurrentColumn) as System.Windows.Controls.DataGridColumn;
        }

        /// <summary>
        /// 先左找一个列信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static EditColumn RightColumnItem(this List<EditColumn> source, EditColumn t)
        {
            return GetItem(source.OfType<object>().ToList(), source.IndexOf(t) + 1, t) as EditColumn;

        }
        /// <summary>
        /// 先右找一个列信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static EditColumn LeftColumnItem(this List<EditColumn> source, EditColumn t)
        {
            return GetItem(source.OfType<object>().ToList(), source.IndexOf(t) - 1, t) as EditColumn;
        }

        /// <summary>
        /// 获取网格项目针对当前网格选中的索引
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static object GetGridItemInChangeIndex(DataGrid grid, int index)
        {
            var source = CopySource(grid);

            var gridSelect = grid.SelectedItem;

            if (gridSelect == null && source.Count > 0)
            {
                return source[0];
            }
            else
            {
                var currentIndex = source.IndexOf(gridSelect);

                if (index == 1 && source.Count > 1)
                {
                    return GetItemOrDefault(source, currentIndex + index, gridSelect);
                }
                if (currentIndex == 0 || source.Count == 1)
                {
                    return gridSelect;
                }
                else
                {
                    return GetItemOrDefault(source, currentIndex + index, gridSelect);
                }
            }
        }

        private static object GetItemOrDefault(List<object> source, int index, object defaultItem)
        {
            if (source.Count > index && index >= 0)
                return source[index];
            return defaultItem;
        }

        private static object GetItem(List<object> source, int index, object defaultItem)
        {
            var currentIndex = source.IndexOf(defaultItem);

            if (source.Count > index && index >= 0)
                return source[index];
            if (index <= 0 && source.Count > 0)
                return source[source.Count - 1];
            if (index >= currentIndex && source.Count > 0)
                return source[0];
            return defaultItem;
        }

        private static List<object> CopySource(DataGrid grid)
        {
            return CopySource(grid.ItemsSource);
        }
        #endregion

        #region c1扩展

 
        private static List<object> CopySource(IEnumerable grid)
        {
            if (grid == null)
                return null;

            var source = grid.GetEnumerator();

            List<object> copySource = new List<object>();

            while (source.MoveNext())
            {
                copySource.Add(source.Current);
            }
            return copySource;
        }
        #endregion
    }
}
