﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PD.Controls
{
    public interface IOrientationProvider : IDisposable
    {
        DataGridMoveInputBehavior Behavior { get; set; }

        FrameworkElement CurrentControl { get; }

        void SetOrientationProvider(FrameworkElement control);

        void SetFocus();
    }
}
