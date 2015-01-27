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
using System.Collections.Generic;

namespace PD.Controls
{
    public class OrientationProviderCollection : List<IOrientationProvider>
    {
        public IOrientationProvider this[FrameworkElement control]
        {
            get 
            {
                foreach (var provider in this)
                {
                    if (provider.CurrentControl != null)
                    {
                        if (provider.CurrentControl.Equals(control))
                        {
                            return provider;
                        }
                    }
                }
                return null;
            }
        }
    }

    /// <summary>
    /// 需要方向键控制编辑的列
    /// </summary>
    public class EditColumnCollection : List<EditColumn>
    {
        public EditColumn this[DataGridColumn column]
        {
            get
            {
                if (column == null)
                    return null;
                foreach (var editColumn in this)
                {
                    if (editColumn.Column .Equals(column))
                    {
                        return editColumn;
                    }
                }
                return null;
            }
        }
    }
}
