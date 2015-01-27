// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Linq;
namespace PD.Controls
{
    /// <summary>
    /// 提供一个能在父窗口上显示并能与其交互的窗口.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplatePart(Name = PART_Chrome, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PART_CloseButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_ResetButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_MinButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_MaxButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_ContentPresenter, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PART_ContentRoot, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PART_Overlay, Type = typeof(Panel))]
    [TemplatePart(Name = PART_Root, Type = typeof(FrameworkElement))]
    [TemplateVisualState(Name = VSMSTATE_StateClosed, GroupName = VSMGROUP_Window)]
    [TemplateVisualState(Name = VSMSTATE_StateOpen, GroupName = VSMGROUP_Window)]
    public class CustomChildWindow : ContentControl
    {
        #region ControlColor
        /// <summary>
        /// 窗体边框颜色
        /// </summary>
        public Brush WindowBorderBrush
        {
            get { return (Brush)GetValue(WindowBorderBrushProperty); }
            set { SetValue(WindowBorderBrushProperty, value); }
        } /// <summary>
        /// 窗体背景颜色
        /// </summary>
        public Brush WindowBackBrush
        {
            get { return (Brush)GetValue(WindowBackBrushProperty); }
            set { SetValue(WindowBackBrushProperty, value); }
        } /// <summary>
        /// 窗体内容边框颜色
        /// </summary>
        public Brush WindowContentBackBrush
        {
            get { return (Brush)GetValue(WindowContentBackBrushProperty); }
            set { SetValue(WindowContentBackBrushProperty, value); }
        }

        private static readonly DependencyProperty WindowBorderBrushProperty = DependencyProperty.Register(
           "WindowBorderBrush", typeof(Brush), typeof(CustomChildWindow), new PropertyMetadata(BrushChanged));

        private static readonly DependencyProperty WindowBackBrushProperty = DependencyProperty.Register(
           "WindowBackBrush", typeof(Brush), typeof(CustomChildWindow), new PropertyMetadata(BrushChanged));

        private static readonly DependencyProperty WindowContentBackBrushProperty = DependencyProperty.Register(
            "WindowContentBackBrush", typeof(Brush), typeof(CustomChildWindow), new PropertyMetadata(BrushChanged));

        private static void BrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CustomChildWindow cw = (CustomChildWindow)d;

            if (cw.CloseButton != null)
            {
                var contentBorder = cw.GetTemplateChild("_2") as Border;
                var windowBorder = cw.GetTemplateChild("_3") as Border;
                if (contentBorder != null)
                {
                    contentBorder.BorderBrush = cw.WindowContentBackBrush;
                }
                if (windowBorder != null)
                {
                    windowBorder.Background = cw.WindowBackBrush;
                    windowBorder.BorderBrush = cw.WindowBorderBrush;
                }
            }
        }
        #endregion

        #region Static Fields and Constants

        /// <summary>
        /// The name of the Chrome template part.
        /// </summary>
        private const string PART_Chrome = "Chrome";

        /// <summary>
        /// The name of the CloseButton template part.
        /// </summary>
        private const string PART_CloseButton = "CloseButton";

        /// <summary>
        /// 最小化按钮
        /// </summary>
        private const string PART_MinButton = "MinButton";
        /// <summary>
        /// 最大化按钮
        /// </summary>
        private const string PART_MaxButton = "MaxButton";
        /// <summary>
        /// 还原按钮
        /// </summary>
        private const string PART_ResetButton = "ResetButton";

        /// <summary>
        /// The name of the ContentPresenter template part.
        /// </summary>
        private const string PART_ContentPresenter = "ContentPresenter";

        /// <summary>
        /// The name of the ContentRoot template part.
        /// </summary>
        private const string PART_ContentRoot = "ContentRoot";

        /// <summary>
        /// The name of the Overlay template part.
        /// </summary>
        private const string PART_Overlay = "Overlay";

        /// <summary>
        /// The name of the Root template part.
        /// </summary>
        private const string PART_Root = "Root";

        /// <summary>
        /// The name of the WindowStates VSM group.
        /// </summary>
        private const string VSMGROUP_Window = "WindowStates";

        /// <summary>
        /// The name of the Closing VSM state.
        /// </summary>
        private const string VSMSTATE_StateClosed = "Closed";

        /// <summary>
        /// The name of the Opening VSM state.
        /// </summary>
        private const string VSMSTATE_StateOpen = "Open";

        /// <summary>
        /// Stores the previous value of RootVisual.IsEnabled.
        /// </summary>
        private static bool RootVisual_PrevEnabledState = true;

        /// <summary>
        /// Stores a count of the number of open ChildWindow instances.
        /// </summary>
        private static int OpenChildWindowCount = 0;

        #region public bool HasCloseButton

        /// <summary>
        /// System.Windows.Controls.ChildWindow是否有关闭按钮
        /// </summary>
        /// <value>
        /// 如果有关闭按钮为True; 否则为false. 默认为true.
        /// </value>
        public bool HasCloseButton
        {
            get { return (bool)GetValue(HasCloseButtonProperty); }
            set { SetValue(HasCloseButtonProperty, value); }
        }

        /// <summary>
        /// System.Windows.Controls.ChildWindow.HasCloseButton依赖属性.
        /// </summary>
        /// <value>
        /// System.Windows.Controls.ChildWindow.HasCloseButton依赖属性标识.
        /// </value>
        public static readonly DependencyProperty HasCloseButtonProperty =
            DependencyProperty.Register(
            "HasCloseButton",
            typeof(bool),
            typeof(CustomChildWindow),
            new PropertyMetadata(true, OnHasCloseButtonPropertyChanged));

        /// <summary>
        /// HasCloseButtonProperty PropertyChangedCallback call back static function.
        /// </summary>
        /// <param name="d">ChildWindow object whose HasCloseButton property is changed.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs which contains the old and new values.</param>
        private static void OnHasCloseButtonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CustomChildWindow cw = (CustomChildWindow)d;

            if (cw.CloseButton != null)
            {
                if ((bool)e.NewValue)
                {
                    cw.CloseButton.Visibility = Visibility.Visible;
                }
                else
                {
                    cw.CloseButton.Visibility = Visibility.Collapsed;
                }
            }
        }

        #endregion public bool HasCloseButton

        #region public Brush OverlayBrush

        /// <summary>
        /// 当子窗口打开时覆盖父窗口的虚拟画刷.
        /// </summary>
        /// <value>
        /// 默认为null.
        /// </value>
        public Brush OverlayBrush
        {
            get { return (Brush)GetValue(OverlayBrushProperty); }
            set { SetValue(OverlayBrushProperty, value); }
        }

        /// <summary>
        /// System.Windows.Controls.ChildWindow.OverlayBrush依赖属性.
        /// </summary>
        /// <value>
        /// System.Windows.Controls.ChildWindow.OverlayBrush依赖属性标识.
        /// </value>
        public static readonly DependencyProperty OverlayBrushProperty =
            DependencyProperty.Register(
            "OverlayBrush",
            typeof(Brush),
            typeof(CustomChildWindow),
            new PropertyMetadata(OnOverlayBrushPropertyChanged));

        /// <summary>
        /// OverlayBrushProperty PropertyChangedCallback call back static function.
        /// </summary>
        /// <param name="d">ChildWindow object whose OverlayBrush property is changed.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs which contains the old and new values.</param>
        private static void OnOverlayBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CustomChildWindow cw = (CustomChildWindow)d;

            if (cw.Overlay != null)
            {
                cw.Overlay.Background = (Brush)e.NewValue;
            }
        }

        #endregion public Brush OverlayBrush

        #region public double OverlayOpacity

        /// <summary>
        /// 覆盖画刷透明度.
        /// </summary>
        /// <value>
        /// 默认为 1.0.
        /// </value>
        public double OverlayOpacity
        {
            get { return (double)GetValue(OverlayOpacityProperty); }
            set { SetValue(OverlayOpacityProperty, value); }
        }

        /// <summary>
        /// System.Windows.Controls.ChildWindow.OverlayOpacity依赖属性.
        /// </summary>
        /// <value>
        /// System.Windows.Controls.ChildWindow.OverlayOpacity依赖属性标识.
        /// </value>
        public static readonly DependencyProperty OverlayOpacityProperty =
            DependencyProperty.Register(
            "OverlayOpacity",
            typeof(double),
            typeof(CustomChildWindow),
            new PropertyMetadata(OnOverlayOpacityPropertyChanged));

        /// <summary>
        /// OverlayOpacityProperty PropertyChangedCallback call back static function.
        /// </summary>
        /// <param name="d">ChildWindow object whose OverlayOpacity property is changed.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs which contains the old and new values.</param>
        private static void OnOverlayOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CustomChildWindow cw = (CustomChildWindow)d;

            if (cw.Overlay != null)
            {
                cw.Overlay.Opacity = (double)e.NewValue;
            }
        }

        #endregion public double OverlayOpacity

        #region private static Control RootVisual

        /// <summary>
        /// Gets the root visual element.
        /// </summary>
        private static Control RootVisual
        {
            get
            {
                return Application.Current == null ? null : (Application.Current.RootVisual as Control);
            }
        }

        #endregion private static Control RootVisual

        #region public object Title

        /// <summary>
        /// System.Windows.Controls.ChildWindow标题.
        /// </summary>
        /// <value>
        /// 默认为 null.
        /// </value>
        public object Title
        {
            get { return GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// System.Windows.Controls.ChildWindow.Title依赖属性.
        /// </summary>
        /// <value>
        /// System.Windows.Controls.ChildWindow.Title依赖属性标识.
        /// </value>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
            "Title",
            typeof(object),
            typeof(CustomChildWindow),
            null);

        #endregion public object Title

        #endregion Static Fields and Constants

        #region Member Fields

        private DoubleEventHelper doubleEvent = new DoubleEventHelper();

        private WindowStateManager _windowStateManager = new WindowStateManager();

        private WindowSizeManager _windowSizeManager = new WindowSizeManager();

        private WindowResizeManager _windowResizeManager = new WindowResizeManager();

        /// <summary>
        /// Private accessor for the Chrome.
        /// </summary>
        private FrameworkElement _chrome;

        /// <summary>
        /// Private accessor for the click point on the chrome.
        /// </summary>
        private Point _clickPoint;

        /// <summary>
        /// Private accessor for the Closing storyboard.
        /// </summary>
        private Storyboard _closed;

        /// <summary>
        /// Private accessor for the ContentPresenter.
        /// </summary>
        private FrameworkElement _contentPresenter;

        /// <summary>
        /// Private accessor for the translate transform that needs to be applied on to the ContentRoot.
        /// </summary>
        private TranslateTransform _contentRootTransform;

        /// <summary>
        /// Private accessor for the Dialog Result property.
        /// </summary>
        private bool? _dialogresult;

        /// <summary>
        /// Private accessor for the ChildWindow InteractionState.
        /// </summary>
        private WindowInteractionState _interactionState;

        /// <summary>
        /// Boolean value that specifies whether the application is exit or not.
        /// </summary>
        private bool _isAppExit;

        /// <summary>
        /// Boolean value that specifies whether the window is in closing state or not.
        /// </summary>
        private bool _isClosing;

        /// <summary>
        /// Boolean value that specifies whether the window is opened.
        /// </summary>
        private bool _isOpen;

        /// <summary>
        /// Private accessor for the Opening storyboard.
        /// </summary>
        private Storyboard _opened;

        /// <summary>
        /// Boolean value that specifies whether the mouse is captured or not.
        /// </summary>
        private bool _isMouseCaptured;

        private bool _isMoveMouseCaptured;

        /// <summary>
        /// Boolean value that specifies whether we are listening to RootVisual.GotFocus.
        /// </summary>
        private bool _attachedRootVisualListener;

        /// <summary>
        /// Private accessor for the Root of the window.
        /// </summary>
        private FrameworkElement _root;

        private Button _minButton;

        private Button _maxButton;

        private Button _resetButton;

        #endregion Member Fields

        #region Constructors

        /// <summary>
        /// System.Windows.Controls.ChildWindow构造函数.
        /// </summary>
        public CustomChildWindow()
        {
            this.DefaultStyleKey = this.GetType();
            this.InteractionState = WindowInteractionState.NotResponding;
            this._windowResizeManager.ResizingArea = 4;

        }


        #endregion Constructors

        #region Events

        /// <summary>
        /// System.Windows.Controls.ChildWindow关闭后触发.
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// System.Windows.Controls.ChildWindow关闭时触发.
        /// </summary>
        public event EventHandler<CancelEventArgs> Closing;

        #endregion Events

        #region Properties and DependencyProperty

        #region 显示最小化
        /// <summary>
        /// 是否显示最小化按钮
        /// </summary>
        public bool ShowMinButton
        {
            get { return (bool)GetValue(ShowMinButtonProperty); }
            set { SetValue(ShowMinButtonProperty, value); }
        }


        public static readonly DependencyProperty ShowMinButtonProperty = DependencyProperty.Register("ShowMinButton", typeof(bool), typeof(CustomChildWindow), new PropertyMetadata(true, new PropertyChangedCallback(ShowMinButtonChanged)));

        private static void ShowMinButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomChildWindow)
            {
                var window = d as CustomChildWindow;
                window.SetControlVisibility((bool)e.NewValue, window._minButton);
            }
        }
        #endregion

        #region 最大化
        /// <summary>
        /// 是否显示最大化按钮
        /// </summary>
        public bool ShowMaxButton
        {
            get { return (bool)GetValue(ShowMaxButtonProperty); }
            set { SetValue(ShowMaxButtonProperty, value); }
        }

        public static readonly DependencyProperty ShowMaxButtonProperty = DependencyProperty.Register("ShowMaxButton", typeof(bool), typeof(CustomChildWindow), new PropertyMetadata(true, new PropertyChangedCallback(ShowMaxButtonChanged)));

        private static void ShowMaxButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomChildWindow)
            {
                var window = d as CustomChildWindow;
                window.SetControlVisibility((bool)e.NewValue, window._maxButton);
            }
        }
        #endregion

        #region 图标
        /// <summary>
        /// Icon图标
        /// </summary>
        public Image Icon
        {
            get { return (Image)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(Image), typeof(CustomChildWindow), null);
        #endregion

        #region 是否拖动
        /// <summary>
        /// 是否拖动
        /// </summary>
        public bool Move
        {
            get { return (bool)GetValue(MoveProperty); }
            set { SetValue(MoveProperty, value); }
        }

        public static readonly DependencyProperty MoveProperty = DependencyProperty.Register("Move", typeof(bool), typeof(CustomChildWindow), new PropertyMetadata(true, new PropertyChangedCallback(MoveChanged)));

        private static void MoveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomChildWindow)
            {
                var window = d as CustomChildWindow;
                window.SetControlVisibility((bool)e.NewValue, window._chrome);
            }
        }

        #endregion

        #region 是否调整大小
        /// <summary>
        /// 是否调整大小
        /// </summary>
        public bool Resize
        {
            get { return (bool)GetValue(ResizeProperty); }
            set { SetValue(ResizeProperty, value); }
        }

        public static readonly DependencyProperty ResizeProperty = DependencyProperty.Register("Resize", typeof(bool), typeof(CustomChildWindow),
             new PropertyMetadata(new PropertyChangedCallback(ResizeChanged)));

        private static void ResizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomChildWindow)
            {
                var window = d as CustomChildWindow;
                if (window._chrome != null)
                {
                    if (window.Resize)
                    {
                        window._chrome.Margin = new Thickness(4, 4, 4, 0);
                    }
                    else
                    {
                        window._chrome.Margin = new Thickness(0);
                    }
                }
            }
        }
        #endregion

        #region 是否模态对话框
        /// <summary>
        /// 是否模态对话框
        /// </summary>
        public bool Modal
        {
            get { return (bool)GetValue(ModalProperty); }
            set { SetValue(ModalProperty, value); }
        }

        public static readonly DependencyProperty ModalProperty = DependencyProperty.Register("Modal", typeof(bool), typeof(CustomChildWindow), new PropertyMetadata(false, new PropertyChangedCallback(ModalChanged)));

        private static void ModalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomChildWindow)
            {
                var window = d as CustomChildWindow;
                if (window.IsOpen)
                {
                    window.UpdateOverlaySize();
                    if ((bool)e.NewValue)
                    {
                        window.DisableRootVisual();
                    }
                    else
                    {
                        window.EnableRootVisual();
                    }
                }
            }
        }
        #endregion

        #region
        /// <summary>
        /// 最小化是距离底部的间距
        /// </summary>
        public double MinVertical { get; set; }
        #endregion
        /// <summary>
        /// Gets the internal accessor for the ContentRoot of the window.
        /// </summary>
        internal FrameworkElement ContentRoot
        {
            get;
            private set;
        }

        /// <summary>
        /// System.Windows.Controls.ChildWindow是 accepted还是canceled.
        /// </summary>
        /// <value>
        /// True表示accepted; false表示canceled.默认为null.
        /// </value>
        [TypeConverter(typeof(NullableBoolConverter))]
        public bool? DialogResult
        {
            get
            {
                return this._dialogresult;
            }

            set
            {
                if (this._dialogresult != value)
                {
                    this._dialogresult = value;
                    this.Close();
                }
            }
        }

        /// <summary>
        /// Gets the internal accessor for the PopUp of the window.
        /// </summary>
        internal System.Windows.Controls.Primitives.Popup ChildWindowPopup
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the internal accessor for the close button of the window.
        /// </summary>
        internal ButtonBase CloseButton
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the InteractionState for the ChildWindow.
        /// </summary>
        internal WindowInteractionState InteractionState
        {
            get
            {
                return this._interactionState;
            }
            private set
            {
                if (this._interactionState != value)
                {
                    WindowInteractionState oldValue = this._interactionState;
                    this._interactionState = value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the PopUp is open or not.
        /// </summary>
        private bool IsOpen
        {
            get
            {
                return (this.ChildWindowPopup != null && this.ChildWindowPopup.IsOpen);
            }
        }

        /// <summary>
        /// Gets the internal accessor for the overlay of the window.
        /// </summary>
        internal Panel Overlay
        {
            get;
            private set;
        }

        #endregion Properties

        #region Static Methods

        /// <summary>
        /// Inverts the input matrix.
        /// </summary>
        /// <param name="matrix">The matrix values that is to be inverted.</param>
        /// <returns>Returns a value indicating whether the inversion was successful or not.</returns>
        private static bool InvertMatrix(ref Matrix matrix)
        {
            double determinant = (matrix.M11 * matrix.M22) - (matrix.M12 * matrix.M21);

            if (determinant == 0.0)
            {
                return false;
            }

            Matrix matCopy = matrix;
            matrix.M11 = matCopy.M22 / determinant;
            matrix.M12 = -1 * matCopy.M12 / determinant;
            matrix.M21 = -1 * matCopy.M21 / determinant;
            matrix.M22 = matCopy.M11 / determinant;
            matrix.OffsetX = ((matCopy.OffsetY * matCopy.M21) - (matCopy.OffsetX * matCopy.M22)) / determinant;
            matrix.OffsetY = ((matCopy.OffsetX * matCopy.M12) - (matCopy.OffsetY * matCopy.M11)) / determinant;

            return true;
        }

        #endregion Static Methods

        #region Methods
        public void SkinUpdate()
        {

        }

        /// <summary>
        /// Executed when the application is exited.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Event args.</param>
        internal void Application_Exit(object sender, EventArgs e)
        {
            if (this.IsOpen)
            {
                this._isAppExit = true;
                try
                {
                    this.Close();
                }
                finally
                {
                    this._isAppExit = false;
                }
            }
        }

        /// <summary>
        /// Changes the visual state of the ChildWindow.
        /// </summary>
        private void ChangeVisualState()
        {
            if (this._isClosing)
            {
                VisualStateManager.GoToState(this, VSMSTATE_StateClosed, true);
            }
            else
            {
                VisualStateManager.GoToState(this, VSMSTATE_StateOpen, true);
            }
        }

        /// <summary>
        /// 关闭System.Windows.Controls.ChildWindow.
        /// </summary>
        public void Close()
        {
            if (this.InteractionState == WindowInteractionState.Closing)
                return;
            // AutomationPeer returns "Closing" when Close() is called
            // but the window is not closed completely:
            this.InteractionState = WindowInteractionState.Closing;
            CancelEventArgs e = new CancelEventArgs();
            this.OnClosing(e);

            // On ApplicationExit, close() cannot be cancelled
            if (!e.Cancel || this._isAppExit)
            {
                if (Modal && this.IsOpen)
                {
                    EnableRootVisual();
                }


                // Close Popup
                if (this.IsOpen)
                {
                    if (this._closed != null)
                    {
                        // Popup will be closed when the storyboard ends
                        this._isClosing = true;
                        try
                        {
                            this.ChangeVisualState();
                        }
                        finally
                        {
                            this._isClosing = false;
                        }
                    }
                    else
                    {
                        // If no closing storyboard is defined, close the Popup
                        this.ChildWindowPopup.IsOpen = false;
                    }

                    if (!this._dialogresult.HasValue)
                    {
                        // If close action is not happening because of DialogResult property change action,
                        // Dialogresult is always false:
                        this._dialogresult = false;
                    }

                    this.OnClosed(EventArgs.Empty);
                    this.UnSubscribeFromEvents();
                    this.UnsubscribeFromTemplatePartEvents();

                    if (Application.Current.RootVisual != null && Modal)
                    {
                        Application.Current.RootVisual.GotFocus -= new RoutedEventHandler(this.RootVisual_GotFocus);
                        _attachedRootVisualListener = false;
                    }
                }
            }
            else
            {
                // If the Close is cancelled, DialogResult should always be NULL:
                this._dialogresult = null;
                this.InteractionState = WindowInteractionState.Running;
            }
        }

        /// <summary>
        /// Executed when the Closing storyboard ends.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event args.</param>
        private void Closing_Completed(object sender, EventArgs e)
        {
            if (this.ChildWindowPopup != null)
            {
                this.ChildWindowPopup.IsOpen = false;
            }

            // AutomationPeer returns "NotResponding" when the ChildWindow is closed:
            this.InteractionState = WindowInteractionState.NotResponding;

            if (this._closed != null)
            {
                this._closed.Completed -= new EventHandler(this.Closing_Completed);
            }
        }

        /// <summary>
        /// Executed when the a key is presses when the window is open.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Key event args.</param>
        private void ChildWindow_KeyDown(object sender, KeyEventArgs e)
        {
            CustomChildWindow ew = sender as CustomChildWindow;
            Debug.Assert(ew != null, "ChildWindow instance is null.");

            // Ctrl+Shift+F4 closes the ChildWindow
            if (e != null && !e.Handled && e.Key == Key.F4 &&
                ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) &&
                ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift))
            {
                ew.Close();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Executed when the window loses focus.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Routed event args.</param>
        private void ChildWindow_LostFocus(object sender, RoutedEventArgs e)
        {
            // If the ChildWindow loses focus but the popup is still open,
            // it means another popup is opened. To get the focus back when the
            // popup is closed, we handle GotFocus on the RootVisual
            // TODO: Something else could get focus and handle the GotFocus event right.  
            // Try listening to routed events that were Handled (new SL 3 feature)
            // Blocked by Jolt bug #29419
            if (this.IsOpen && Application.Current != null && Application.Current.RootVisual != null)
            {
                this.InteractionState = WindowInteractionState.BlockedByModalWindow;
                if (!_attachedRootVisualListener && Modal)
                {
                    Application.Current.RootVisual.GotFocus += new RoutedEventHandler(this.RootVisual_GotFocus);
                    _attachedRootVisualListener = true;
                }
            }
        }

        #region 拖动子窗体
        /// <summary>
        /// Executed when mouse left button is down on the chrome.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Mouse button event args.</param>
        private void Chrome_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetTop();
            if (this._chrome != null && EnableUpdateContentRoot())
            {
                e.Handled = true;
                if (this.CloseButton != null && !this.CloseButton.IsTabStop)
                {
                    this.CloseButton.IsTabStop = true;
                    try
                    {
                        this.Focus();
                    }
                    finally
                    {
                        this.CloseButton.IsTabStop = false;
                    }
                }
                else
                {
                    this.Focus();
                }
                this._chrome.CaptureMouse();
                this._isMouseCaptured = true;
                this._clickPoint = e.GetPosition(ContentRoot);
            }
        }

        /// <summary>
        /// Executed when mouse left button is up on the chrome.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Mouse button event args.</param>
        private void Chrome_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this._chrome != null && EnableUpdateContentRoot())
            {
                e.Handled = true;
                this._chrome.ReleaseMouseCapture();
                this._isMouseCaptured = false;
            }
        }

        /// <summary>
        /// Executed when mouse moves on the chrome.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Mouse event args.</param>
        private void Chrome_MouseMove(object sender, MouseEventArgs e)
        {
            if (this._isMouseCaptured && this.ContentRoot != null && Application.Current != null && Application.Current.RootVisual != null && EnableUpdateContentRoot())
            {
                Point position = e.GetPosition(Application.Current.RootVisual);

                GeneralTransform gt = this.ContentRoot.TransformToVisual(Application.Current.RootVisual);

                if (gt != null)
                {
                    Point p = gt.Transform(this._clickPoint);
                    UpdatePosition();

                    if (position.X < 0)
                    {
                        double Y = FindPositionY(p, position, 0);
                        position = new Point(0, Y);
                    }

                    if (position.X > this.Width)
                    {
                        double Y = FindPositionY(p, position, this.Width);
                        position = new Point(this.Width, Y);
                    }

                    if (position.Y < 0)
                    {
                        double X = FindPositionX(p, position, 0);
                        position = new Point(X, 0);
                    }

                    if (position.Y > this.Height)
                    {
                        double X = FindPositionX(p, position, this.Height);
                        position = new Point(X, this.Height);
                    }

                    double x = position.X - p.X;
                    double y = position.Y - p.Y;

                    // Take potential RightToLeft layout into account
                    FrameworkElement fe = Application.Current.RootVisual as FrameworkElement;
                    if (fe != null && fe.FlowDirection == FlowDirection.RightToLeft)
                    {
                        x = -x;
                    }
                    UpdateContentRootTransform(x, y);
                }
            }
        }
        #endregion

        #region 拖动窗体大小
        void ContentRoot_MouseMove(object sender, MouseEventArgs e)
        {
            if (EnableUpdateContentRoot() && Resize)
            {
                if (_isMoveMouseCaptured && _windowResizeManager.anchor != ResizeAnchor.None)
                {
                    var p = e.GetPosition(ContentRoot);
                    var root = e.GetPosition(Application.Current.RootVisual);
                    if (root.X <= 0 || root.X > GetRootVisualSize().Width)
                    {
                        p.X = 0;
                    }
                    if (root.Y <= 0 || root.Y > GetRootVisualSize().Height)
                    {
                        p.Y = 0;
                    }
                    _windowResizeManager.Resize(p.X, p.Y);
                    _windowResizeManager.ResizeTramfrom(p.X, p.Y, _contentRootTransform);
                }
                else
                {
                    _windowResizeManager.SetCursor(e.GetPosition(ContentRoot));
                }
            }
        }

        void ContentRoot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Resize)
            {
                ContentRoot.ReleaseMouseCapture();
                _isMoveMouseCaptured = false;
            }
        }

        void ContentRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (EnableUpdateContentRoot() && Resize)
            {
                _isMoveMouseCaptured = true;
                ContentRoot.CaptureMouse();
                UpdatePosition();
            }
        }
        #endregion

        /// <summary>
        /// Executed when the ContentPresenter size changes.
        /// </summary>
        /// <param name="sender">Content Presenter object.</param>
        /// <param name="e">SizeChanged event args.</param>
        private void ContentPresenter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (EnableUpdateContentRoot())
            {
                this.UpdatePosition();
            }
        }

        /// <summary>
        /// Executed when the page resizes.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event args.</param>
        private void Page_Resized(object sender, EventArgs e)
        {
            if (this.ChildWindowPopup != null)
            {
                if (_windowStateManager.CurrentState == DialogSizeState.Maximized)
                {
                    SetMaxStateSize();
                    UpdateOverlaySize();
                }
                else if (_windowStateManager.CurrentState == DialogSizeState.Minimized)
                {
                    UpdateWindowMin();
                }
                else
                {
                    UpdateNormalSize();
                    UpdateOverlaySize();
                }
            }
        }
        /// <summary>
        /// Finds the X coordinate of a point that is defined by a line.
        /// </summary>
        /// <param name="p1">Starting point of the line.</param>
        /// <param name="p2">Ending point of the line.</param>
        /// <param name="y">Y coordinate of the point.</param>
        /// <returns>X coordinate of the point.</returns>
        private static double FindPositionX(Point p1, Point p2, double y)
        {
            if (y == p1.Y || p1.X == p2.X)
            {
                return p2.X;
            }

            Debug.Assert(p1.Y != p2.Y, "Unexpected equal Y coordinates");

            return (((y - p1.Y) * (p1.X - p2.X)) / (p1.Y - p2.Y)) + p1.X;
        }

        /// <summary>
        /// Finds the Y coordinate of a point that is defined by a line.
        /// </summary>
        /// <param name="p1">Starting point of the line.</param>
        /// <param name="p2">Ending point of the line.</param>
        /// <param name="x">X coordinate of the point.</param>
        /// <returns>Y coordinate of the point.</returns>
        private static double FindPositionY(Point p1, Point p2, double x)
        {
            if (p1.Y == p2.Y || x == p1.X)
            {
                return p2.Y;
            }

            Debug.Assert(p1.X != p2.X, "Unexpected equal X coordinates");

            return (((p1.Y - p2.Y) * (x - p1.X)) / (p1.X - p2.X)) + p1.Y;
        }

        /// <summary>
        /// 当新的模版被应用的时候，为System.Windows.Controls.ChildWindow构建visual tree(虚拟树).
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "No need to split the code into two parts.")]
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.UnsubscribeFromTemplatePartEvents();

            this.CloseButton = GetTemplateChild(PART_CloseButton) as ButtonBase;

            this._maxButton = GetTemplateChild(PART_MaxButton) as Button;

            this._minButton = GetTemplateChild(PART_MinButton) as Button;

            this._resetButton = GetTemplateChild(PART_ResetButton) as Button;

            this._chrome = GetTemplateChild(PART_Chrome) as FrameworkElement;

            if (this._chrome != null)
            {
                SetControlVisibility(Move, _chrome);
                ResizeChanged(this, new DependencyPropertyChangedEventArgs());
            }

            if (this._minButton != null)
            {
                SetControlVisibility(this.ShowMinButton, this._minButton);
            }

            if (this._maxButton != null)
            {
                SetControlVisibility(this.ShowMaxButton, this._maxButton);
            }

            if (this.CloseButton != null)
            {
                if (this.HasCloseButton)
                {
                    this.CloseButton.Visibility = Visibility.Visible;
                }
                else
                {
                    this.CloseButton.Visibility = Visibility.Collapsed;
                }
            }

            if (this._closed != null)
            {
                this._closed.Completed -= new EventHandler(this.Closing_Completed);
            }

            if (this._opened != null)
            {
                this._opened.Completed -= new EventHandler(this.Opening_Completed);
            }

            this._root = GetTemplateChild(PART_Root) as FrameworkElement;

            if (this._root != null)
            {
                IList groups = VisualStateManager.GetVisualStateGroups(this._root);

                if (groups != null)
                {
                    IList states = null;

                    foreach (VisualStateGroup vsg in groups)
                    {
                        if (vsg.Name == CustomChildWindow.VSMGROUP_Window)
                        {
                            states = vsg.States;
                            break;
                        }
                    }

                    if (states != null)
                    {
                        foreach (VisualState state in states)
                        {
                            if (state.Name == CustomChildWindow.VSMSTATE_StateClosed)
                            {
                                this._closed = state.Storyboard;
                            }

                            if (state.Name == CustomChildWindow.VSMSTATE_StateOpen)
                            {
                                this._opened = state.Storyboard;
                            }
                        }
                    }
                }
            }
            this.ContentRoot = GetTemplateChild(PART_ContentRoot) as FrameworkElement;
            this._windowResizeManager.element = ContentRoot;

            this.Overlay = GetTemplateChild(PART_Overlay) as Panel;
            SetOverlayStyle();

            this._contentPresenter = GetTemplateChild(PART_ContentPresenter) as FrameworkElement;

            this.SubscribeToTemplatePartEvents();
            this.SubscribeToStoryBoardEvents();
            this._windowSizeManager.DesiredMargin = this.Margin;
            this.Margin = new Thickness(0);

            // Update overlay size
            if (this.IsOpen)
            {
                this.UpdateNormalSize();
                this.UpdateOverlaySize();
                this.UpdateRenderTransform();
                this.ChangeVisualState();
                this.ContentRoot.LayoutUpdated += new EventHandler(ContentRoot_LayoutUpdated);
            }
        }

        void ContentRoot_LayoutUpdated(object sender, EventArgs e)
        {
            if (ContentRoot.ActualHeight > 0)
            {
                this.ContentRoot.LayoutUpdated -= new EventHandler(ContentRoot_LayoutUpdated);
                this.UpdateContentCenter();
            }
        }

        /// <summary>
        /// Raises the
        /// <see cref="E:System.Windows.Controls.ChildWindow.Closed" /> event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnClosed(EventArgs e)
        {
            EventHandler handler = this.Closed;

            if (null != handler)
            {
                handler(this, e);
            }

            this._isOpen = false;
        }

        /// <summary>
        /// Raises the
        /// <see cref="E:System.Windows.Controls.ChildWindow.Closing" /> event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnClosing(CancelEventArgs e)
        {
            EventHandler<CancelEventArgs> handler = this.Closing;

            if (null != handler)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// This method is called every time a
        /// <see cref="T:System.Windows.Controls.ChildWindow" /> is displayed.
        /// </summary>
        protected virtual void OnOpened()
        {
            this._isOpen = true;

            if (this.Overlay != null)
            {
                SetOverlayStyle();
            }

            if (!this.Focus())
            {
                this.IsTabStop = true;
                this.Focus();
            }
        }

        private void SetOverlayStyle()
        {
            if (Overlay != null)
            {
                this.Overlay.Opacity = this.OverlayOpacity;
                this.Overlay.Background = this.OverlayBrush;
            }
        }

        /// <summary>
        /// Executed when the opening storyboard finishes.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event args.</param>
        private void Opening_Completed(object sender, EventArgs e)
        {
            if (this._opened != null)
            {
                this._opened.Completed -= new EventHandler(this.Opening_Completed);
            }
            this.InteractionState = WindowInteractionState.ReadyForUserInteraction;
            this.OnOpened();
            this.UpdatePosition();
        }

        /// <summary>
        /// Executed when the root visual gets focus.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Routed event args.</param>
        private void RootVisual_GotFocus(object sender, RoutedEventArgs e)
        {
            this.Focus();
            this.InteractionState = WindowInteractionState.ReadyForUserInteraction;
        }

        private void MainMouseDown(object sender, MouseButtonEventArgs e)
        {
            UpdatePosition();
            SetTop();
        }

        private void SetTop()
        {
            if (Modal == false)
            {
                var popups = VisualTreeHelper.GetOpenPopups();
                if (popups.Count() > 0)
                {
                    if (popups.ElementAt(0) != this.ChildWindowPopup)
                    {
                        ChildWindowPopup.IsOpen = false;
                        ChildWindowPopup.IsOpen = true;
                    }
                }
            }
        }
        /// <summary>
        /// 打开System.Windows.Controls.ChildWindow.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">
        /// 子窗体已经在visual tree(虚拟树)中.
        /// </exception>
        public void Show()
        {
            // AutomationPeer returns "Running" when Show() is called
            // but the ChildWindow is not ready for user interaction:
            this.InteractionState = WindowInteractionState.Running;

            this.SubscribeToEvents();
            this.SubscribeToTemplatePartEvents();
            this.SubscribeToStoryBoardEvents();

            if (this.ChildWindowPopup == null)
            {
                this.ChildWindowPopup = new System.Windows.Controls.Primitives.Popup();

                try
                {
                    this.ChildWindowPopup.Child = this;
                    this.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                }
                catch (ArgumentException e)
                {
                    this.InteractionState = WindowInteractionState.NotResponding;
                    throw e;
                }
            }
            // MaxHeight and MinHeight properties should not be overwritten:
            this.MaxHeight = double.PositiveInfinity;
            this.MaxWidth = double.PositiveInfinity;
            if (Modal && !this.IsOpen)
            {
                DisableRootVisual();
            }
            if (this.ChildWindowPopup != null && Application.Current.RootVisual != null)
            {
                this.ChildWindowPopup.IsOpen = true;
                // while the ChildWindow is open, the DialogResult is always NULL:
                this._dialogresult = null;
            }
            // if the template is already loaded, display loading visuals animation
            if (this.ContentRoot != null)
            {
                this.ChangeVisualState();
            }
        }

        #region 页面对象树根节点状态
        /// <summary>
        ///  禁用能操作页面根对象
        /// </summary>
        private void DisableRootVisual()
        {
            // disable the underlying UI
            if (RootVisual != null)
            {
                if (OpenChildWindowCount == 0)
                {
                    // Save current value to restore it upon closing the last window
                    RootVisual_PrevEnabledState = RootVisual.IsEnabled;
                }
                ++OpenChildWindowCount;

              //  RootVisual.IsEnabled = false;
            }
        }
        /// <summary>
        ///  启用能操作页面根对象
        /// </summary>
        private void EnableRootVisual()
        {
            if (RootVisual != null)
            {
                --OpenChildWindowCount;
                if (OpenChildWindowCount == 0)
                {
                    // Restore the value saved when the first window was opened
                  //  RootVisual.IsEnabled = RootVisual_PrevEnabledState;
                }
            }
        }
        #endregion

        #region 事件注册销毁
        /// <summary>
        /// Subscribes to events when the ChildWindow is opened.
        /// </summary>
        private void SubscribeToEvents()
        {
            if (Application.Current != null && Application.Current.Host != null && Application.Current.Host.Content != null)
            {
                Application.Current.Exit += new EventHandler(this.Application_Exit);
                Application.Current.Host.Content.Resized += new EventHandler(this.Page_Resized);
            }

            this.KeyDown += new KeyEventHandler(this.ChildWindow_KeyDown);
            this.LostFocus += new RoutedEventHandler(this.ChildWindow_LostFocus);
        }

        /// <summary>
        /// Subscribes to events that are on the storyboards. 
        /// Unsubscribing from these events happen in the event handlers individually.
        /// </summary>
        private void SubscribeToStoryBoardEvents()
        {
            if (this._closed != null)
            {
                this._closed.Completed += new EventHandler(this.Closing_Completed);
            }

            if (this._opened != null)
            {
                this._opened.Completed += new EventHandler(this.Opening_Completed);
            }
        }

        /// <summary>
        /// Subscribes to events on the template parts.
        /// </summary>
        private void SubscribeToTemplatePartEvents()
        {

            this.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(MainMouseDown), false);
            if (this.CloseButton != null)
            {
                this.CloseButton.Click += this.CloseButton_Click;
            }

            if (this._chrome != null)
            {
                doubleEvent.RegisterEvent(_chrome);
                doubleEvent.DoubleEvent += Title_DoubleClick;

                this._chrome.MouseLeftButtonDown += this.Chrome_MouseLeftButtonDown;
                this._chrome.MouseLeftButtonUp += this.Chrome_MouseLeftButtonUp;
                this._chrome.MouseMove += this.Chrome_MouseMove;
            }

            if (this._maxButton != null)
            {
                _maxButton.Click += MaxButton_Click;
            }
            if (this._resetButton != null)
            {
                _resetButton.Click += ResetButton_Click;
            }

            if (this._minButton != null)
            {
                _minButton.Click += MinButton_Click;
            }
            if (this.ContentRoot != null)
            {
                this.ContentRoot.MouseLeftButtonDown += ContentRoot_MouseLeftButtonDown;
                this.ContentRoot.MouseLeftButtonUp += ContentRoot_MouseLeftButtonUp;
                this.ContentRoot.MouseMove += ContentRoot_MouseMove;
            }
            if (this._contentPresenter != null)
            {
                this._contentPresenter.SizeChanged += this.ContentPresenter_SizeChanged;
            }
        }

        /// <summary>
        /// Unsubscribe from events when the ChildWindow is closed.
        /// </summary>
        private void UnSubscribeFromEvents()
        {
            if (Application.Current != null && Application.Current.Host != null && Application.Current.Host.Content != null)
            {
                Application.Current.Exit -= new EventHandler(this.Application_Exit);
                Application.Current.Host.Content.Resized -= new EventHandler(this.Page_Resized);
            }

            this.KeyDown -= new KeyEventHandler(this.ChildWindow_KeyDown);
            this.LostFocus -= new RoutedEventHandler(this.ChildWindow_LostFocus);
        }

        /// <summary>
        /// Unsubscribe from the events that are subscribed on the template part elements.
        /// </summary>
        private void UnsubscribeFromTemplatePartEvents()
        {
            this.RemoveHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(MainMouseDown));
            if (this.CloseButton != null)
            {
                this.CloseButton.Click -= this.CloseButton_Click;
            }

            if (this._chrome != null)
            {
                doubleEvent.RemoveEvent(_chrome);
                doubleEvent.DoubleEvent -= Title_DoubleClick;
                this._chrome.MouseLeftButtonDown -= this.Chrome_MouseLeftButtonDown;
                this._chrome.MouseLeftButtonUp -= this.Chrome_MouseLeftButtonUp;
                this._chrome.MouseMove -= this.Chrome_MouseMove;
            }
            if (this._maxButton != null)
            {
                _maxButton.Click -= MaxButton_Click;
            }
            if (this._resetButton != null)
            {
                _resetButton.Click -= ResetButton_Click;
            }
            if (this.ContentRoot != null)
            {
                this.ContentRoot.MouseLeftButtonDown -= ContentRoot_MouseLeftButtonDown;
                this.ContentRoot.MouseLeftButtonUp -= ContentRoot_MouseLeftButtonUp;
                this.ContentRoot.MouseMove -= ContentRoot_MouseMove;
            }
            if (this._minButton != null)
            {
                _minButton.Click -= MinButton_Click;
            }
            if (this._contentPresenter != null)
            {
                this._contentPresenter.SizeChanged -= this.ContentPresenter_SizeChanged;
            }
        }
        #endregion

        #region 窗体状态改变
        internal void Title_DoubleClick(object sender, EventArgs e)
        {
            if (_windowStateManager.CurrentState != DialogSizeState.Normal)
            {
                ResetButton_Click(null, null);
            }
            else
            {

                MaxButton_Click(null, null);
            }
        }
        internal void MaxButton_Click(object sender, RoutedEventArgs e)
        {
            if (ShowMaxButton)
            {
                _windowStateManager.SetMaximized();
                UpdateNormalSize();
                UpdatePosition();
                SetMaxStateSize();
                UpdateOverlaySize();
                if (this._contentRootTransform != null)
                {
                    UpdateContentRootTransform(-this._contentRootTransform.X, -this._contentRootTransform.Y);
                }
                else
                {
                    UpdateContentRootTransform(0, 0);
                }
                CurrentStateButtonChange();
            }
        }

        internal void MinButton_Click(object sender, RoutedEventArgs e)
        {
            _windowStateManager.SetMinimized();
            UpdatePosition();
            UpdateWindowMin();
            UpdateOverlaySize();
            CurrentStateButtonChange();
        }

        internal void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            _windowStateManager.SetRestored();
            CurrentStateButtonChange();
            UpdateOverlaySize();
            _windowStateManager.SetNormal();
            if (this.ContentRoot != null && Application.Current != null && Application.Current.RootVisual != null && _isOpen)
            {
                GeneralTransform gt = this.ContentRoot.TransformToVisual(Application.Current.RootVisual);
                if (gt != null)
                {
                    Point p = gt.Transform(new Point(0, 0));
                    double x = this._windowSizeManager.WindowPosition.X - p.X;
                    double y = this._windowSizeManager.WindowPosition.Y - p.Y;
                    UpdateContentRootTransform(x, y);
                }
            }
        }

        /// <summary>
        /// Executed when the CloseButton is clicked.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Routed event args.</param>
        internal void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion

        #region 更新布局
        private void UpdateNormalSize()
        {
            if (ContentRoot != null)
            {
                _windowSizeManager.NormalDesiredContentHeight = ContentRoot.Height;
                _windowSizeManager.NormalDesiredContentWidth = ContentRoot.Width;
            }
        }

        private void UpdateWindowMin()
        {
            _windowSizeManager.MinDesiredContentWidth = 150;
            _windowSizeManager.MinDesiredContentHeight = 30;
            var y = 0.0;
            var x = 0.0;
            if (this._contentRootTransform != null)
            {
                x = -_contentRootTransform.X;
                y = GetRootVisualSize().Height - _contentRootTransform.Y - _windowSizeManager.MinDesiredContentHeight;
            }
            else
            {
                y = GetRootVisualSize().Height - _windowSizeManager.MinDesiredContentHeight;
            }
            UpdateContentRootTransform(x, y - MinVertical);
        }

        private void UpdateContentCenter()
        {
            if (_windowStateManager.CurrentState == DialogSizeState.Normal)
            {
                var x = 0.0;
                var y = 0.0;
                if (_contentRootTransform == null)
                {
                    x = (GetRootVisualSize().Width - ContentRoot.ActualWidth) / 2;
                    y = (GetRootVisualSize().Height - ContentRoot.ActualHeight) / 2;
                }
                else
                {
                    x = (GetRootVisualSize().Width - ContentRoot.ActualWidth) / 2 - _contentRootTransform.X;
                    y = (GetRootVisualSize().Height - ContentRoot.ActualHeight) / 2 - _contentRootTransform.Y;
                }
                UpdateContentRootTransform(Math.Floor(x), Math.Floor(y));
            }
        }

        /// <summary>
        /// Updates the size of the overlay of the window.
        /// </summary>
        private void UpdateOverlaySize()
        {
            if (this.Overlay != null && Application.Current != null && Application.Current.Host != null && Application.Current.Host.Content != null)
            {
                Size size = MathSize(GetRootVisualSize());
                this.Height = size.Height;
                this.Width = size.Width;

                if (Modal)
                {
                    this.Overlay.Height = size.Height;
                    this.Overlay.Width = size.Width;
                }

                if (this.ContentRoot != null)
                {
                    if (_windowStateManager.CurrentState == DialogSizeState.Maximized)
                    {
                        this.ContentRoot.Width = this._windowSizeManager.MaxDesiredContentWidth;
                        this.ContentRoot.Height = this._windowSizeManager.MaxDesiredContentHeight;
                    }
                    else if (_windowStateManager.CurrentState == DialogSizeState.Minimized)
                    {
                        this.ContentRoot.Height = this._windowSizeManager.MinDesiredContentHeight;
                        this.ContentRoot.Width = this._windowSizeManager.MinDesiredContentWidth;
                    }
                    else
                    {
                        this.ContentRoot.Width = this._windowSizeManager.NormalDesiredContentWidth;
                        this.ContentRoot.Height = this._windowSizeManager.NormalDesiredContentHeight;
                        this.ContentRoot.Margin = this._windowSizeManager.DesiredMargin;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the position of the window in case the size of the content changes.
        /// This allows ChildWindow only scale from right and bottom.
        /// </summary>
        private void UpdatePosition()
        {
            if (this.ContentRoot != null && Application.Current != null && Application.Current.RootVisual != null)
            {
                GeneralTransform gt = this.ContentRoot.TransformToVisual(Application.Current.RootVisual);
                if (gt != null && _windowStateManager.CurrentState == DialogSizeState.Normal)
                {
                    this._windowSizeManager.WindowPosition = gt.Transform(new Point(0, 0));
                }
            }
        }

        /// <summary>
        /// Updates the render transform applied on the overlay.
        /// </summary>
        private void UpdateRenderTransform()
        {
            if (this._root != null && this.ContentRoot != null)
            {
                // The Overlay part should not be affected by the render transform applied on the
                // ChildWindow. In order to achieve this, we adjust an identity matrix to represent
                // the _root's transformation, invert it, apply the inverted matrix on the _root, so that 
                // nothing is affected by the rendertransform, and apply the original transform only on the Content
                GeneralTransform gt = this._root.TransformToVisual(null);
                if (gt != null)
                {
                    Point p10 = new Point(1, 0);
                    Point p01 = new Point(0, 1);
                    Point transform10 = gt.Transform(p10);
                    Point transform01 = gt.Transform(p01);

                    Matrix transformToRootMatrix = Matrix.Identity;
                    transformToRootMatrix.M11 = transform10.X;
                    transformToRootMatrix.M12 = transform10.Y;
                    transformToRootMatrix.M21 = transform01.X;
                    transformToRootMatrix.M22 = transform01.Y;

                    MatrixTransform original = new MatrixTransform();
                    original.Matrix = transformToRootMatrix;

                    InvertMatrix(ref transformToRootMatrix);
                    MatrixTransform mt = new MatrixTransform();
                    mt.Matrix = transformToRootMatrix;

                    TransformGroup tg = this._root.RenderTransform as TransformGroup;

                    if (tg != null)
                    {
                        tg.Children.Add(mt);
                    }
                    else
                    {
                        this._root.RenderTransform = mt;
                    }

                    tg = this.ContentRoot.RenderTransform as TransformGroup;

                    if (tg != null)
                    {
                        tg.Children.Add(original);
                    }
                    else
                    {
                        this.ContentRoot.RenderTransform = original;
                    }
                }
            }
        }

        /// <summary>
        /// 计算面积大小(保证容器大小不小于窗体大小)
        /// </summary>
        /// <param name="size"></param>
        private Size MathSize(Size size)
        {
            var normalHeight = this._windowSizeManager.NormalDesiredContentHeight;
            var normalWidth = this._windowSizeManager.NormalDesiredContentWidth;

            size.Width = size.Width < normalWidth ? normalWidth : size.Width;
            size.Height = size.Height < normalHeight ? normalHeight : size.Height;
            return size;
        }

        /// <summary>
        /// 是否弹出
        /// </summary>
        /// <param name="show"></param>
        public void Popup(bool show)
        {
            if (Modal)
            {
                if (show == false)
                    EnableRootVisual();
                else
                    DisableRootVisual();
            }
            ChildWindowPopup.IsOpen = show;
        }

        /// <summary>
        /// 设置控件可见性
        /// </summary>
        /// <param name="visibility"></param>
        /// <param name="element"></param>
        private void SetControlVisibility(bool visibility, UIElement element)
        {
            if (element != null)
            {
                element.Visibility = visibility ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Updates the ContentRootTranslateTransform.
        /// </summary>
        /// <param name="X">X coordinate of the transform.</param>
        /// <param name="Y">Y coordinate of the transform.</param>
        private void UpdateContentRootTransform(double X, double Y)
        {
            if (this._contentRootTransform == null)
            {
                this._contentRootTransform = new TranslateTransform();
                this._contentRootTransform.X = X;
                this._contentRootTransform.Y = Y;

                TransformGroup transformGroup = this.ContentRoot.RenderTransform as TransformGroup;

                if (transformGroup == null)
                {
                    transformGroup = new TransformGroup();
                    transformGroup.Children.Add(this.ContentRoot.RenderTransform);
                }
                transformGroup.Children.Add(this._contentRootTransform);
                this.ContentRoot.RenderTransform = transformGroup;
            }
            else
            {
                this._contentRootTransform.X += X;
                this._contentRootTransform.Y += Y;
            }
        }

        /// <summary>
        /// 是否可以更新窗体布局
        /// </summary>
        /// <returns></returns>
        private bool EnableUpdateContentRoot()
        {
            return _windowStateManager.CurrentState == DialogSizeState.Normal || _windowStateManager.CurrentState == DialogSizeState.Restored;
        }

        private void CurrentStateButtonChange()
        {
            if (_windowStateManager.CurrentState == DialogSizeState.Maximized)
            {
                _maxButton.Visibility = System.Windows.Visibility.Collapsed;
                _resetButton.Visibility = System.Windows.Visibility.Visible;
            }
            else if (_windowStateManager.CurrentState == DialogSizeState.Minimized)
            {
                _maxButton.Visibility = System.Windows.Visibility.Collapsed;
                _minButton.Visibility = System.Windows.Visibility.Collapsed;
                _resetButton.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                if (ShowMaxButton)
                    _maxButton.Visibility = System.Windows.Visibility.Visible;
                if (ShowMinButton)
                    _minButton.Visibility = System.Windows.Visibility.Visible;
                _resetButton.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void SetMaxStateSize()
        {
            var size = GetRootVisualSize();
            this._windowSizeManager.MaxDesiredContentWidth = size.Width;
            this._windowSizeManager.MaxDesiredContentHeight = size.Height;
        }

        private Size GetRootVisualSize()
        {
            Size size = new Size();
            if (Application.Current != null && Application.Current.Host != null && Application.Current.Host.Content != null)
            {
                size.Height = Application.Current.Host.Content.ActualHeight;
                size.Width = Application.Current.Host.Content.ActualWidth;
                if (Application.Current.Host.Settings.EnableAutoZoom)
                {
                    double zoomFactor = Application.Current.Host.Content.ZoomFactor;
                    if (zoomFactor != 0)
                    {
                        size.Height /= zoomFactor;
                        size.Width /= zoomFactor;
                    }
                }
            } return size;
        }
        #endregion
        #endregion Methods

    }

    public enum ResizeAnchor
    {
        None = 0,
        TopLeft = 1,
        Top = 2,
        TopRight = 3,
        Right = 4,
        BottomRight = 5,
        Bottom = 6,
        BottomLeft = 7,
        Left = 9
    }

    public enum DialogSizeState
    {
        Normal,
        Maximized,
        Minimized,
        Restored,
        Resize,
    }

    public class WindowResizeManager
    {
        #region Attribute



        /// <summary>
        /// Thickness of resizing area.
        /// </summary>
        private const double ResizingAreaDefaultValue = 6;

        /// <summary>
        /// 缩放窗体的最小宽度.
        /// </summary>
        public double MinResizedWidth = 100;

        public double MinResizedHeight = 35;

        /// <summary>
        /// 正在缩放的边界.
        /// </summary>
        private Rect bounds;

        /// <summary>
        /// 当缩放开始后的初始元素大小.
        /// </summary>
        private Size initialSize;

        /// <summary>
        /// Initial position of the element when resizing started.
        /// </summary>
        private Point initialPosition;

        /// <summary>
        /// 用于缩放的元素的一角或者边缘.
        /// </summary>
        public ResizeAnchor anchor = ResizeAnchor.None;

        /// <summary>
        /// 实现了 IResizableElement 接口的元素.
        /// </summary>
        public FrameworkElement element;

        /// <summary>
        /// 是否能开始缩放.
        /// </summary>
        /// <value>
        /// true表示能开始缩放; false表示不能缩放.
        /// </value>
        public bool CanResize
        {
            get
            {
                return anchor != ResizeAnchor.None;
            }
        }

        /// <summary>
        /// 缩放区域宽度.
        /// </summary>
        /// <value>缩放区域宽度.默认为 6.</value>
        public double ResizingArea { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance can be resized horizontally.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can be resized horizontally; otherwise, <c>false</c>.
        /// </value>
        private bool CanResizeHorizontally
        {
            get
            {
                return !(element.MinWidth == element.MaxWidth && element.MinWidth != 0);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can be resized vertically.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can be resized vertically; otherwise, <c>false</c>.
        /// </value>
        private bool CanResizeVertically
        {
            get
            {
                return !(element.MinHeight == element.MaxHeight && element.MinHeight != 0);
            }
        }
        #endregion

        /// <summary>
        /// 缩放元素.
        /// </summary>
        /// <param name="dx">当前鼠标移动的坐标相对当前移动对象X</param>
        /// <param name="dy">当前鼠标移动的坐标相对当前移动对象Y</param>
        public void Resize(double dx, double dy)
        {
            switch (anchor)
            {
                case ResizeAnchor.BottomRight:
                    ResizeBottomRight(dx, dy);
                    break;
                case ResizeAnchor.Right:
                    ResizeBottomRight(dx, 0);//ok
                    break;
                case ResizeAnchor.Bottom:
                    ResizeBottomRight(0, dy);//ok
                    break;

                case ResizeAnchor.BottomLeft:
                    ResizeBottomLeft(dx, dy);
                    break;

                case ResizeAnchor.TopRight:
                    ResizeTopRight(dx, dy);
                    break;

                case ResizeAnchor.TopLeft:
                    ResizeTopLeft(dx, dy);//ok
                    break;
                case ResizeAnchor.Top:
                    ResizeTopLeft(0, dy);//ok
                    break;
                case ResizeAnchor.Left:
                    ResizeTopLeft(dx, 0);//ok
                    break;


            }
        }

        public void ResizeTramfrom(double dx, double dy, TranslateTransform transform)
        {
            switch (anchor)
            {
                case ResizeAnchor.Top:
                    ResizeTramTopLeft(0, dy, transform);
                    break;
                case ResizeAnchor.Left:
                    ResizeTramTopLeft(dx, 0, transform);
                    break;
                case ResizeAnchor.TopLeft:
                    ResizeTramTopLeft(dx, dy, transform);
                    break;
                case ResizeAnchor.TopRight:
                    ResizeTramTopLeft(0, dy, transform);
                    break;
                case ResizeAnchor.BottomLeft:
                    ResizeTramTopLeft(dx, 0, transform);
                    break;
            }
        }

        /// <summary>
        /// 设置鼠标位置.
        /// </summary>
        /// <param name="p">鼠标坐标.</param>
        public void SetCursor(Point p)
        {
            if (p.Y < ResizingArea && p.X < ResizingArea)
            {
                anchor = ResizeAnchor.TopLeft;
                element.Cursor = Cursors.SizeNWSE;
            }
            else if (p.Y < ResizingArea && p.X >= (element.ActualWidth - ResizingArea))
            {
                anchor = ResizeAnchor.TopRight;
                element.Cursor = Cursors.SizeNESW;
            }
            else if (p.Y < ResizingArea)
            {
                if (CanResizeVertically)
                {
                    anchor = ResizeAnchor.Top;
                    element.Cursor = Cursors.SizeNS;
                }
            }
            else if (p.X < ResizingArea && p.Y >= (element.ActualHeight - ResizingArea))
            {
                anchor = ResizeAnchor.BottomLeft;
                element.Cursor = Cursors.SizeNESW;
            }
            else if (p.X < ResizingArea)
            {
                if (CanResizeHorizontally)
                {
                    anchor = ResizeAnchor.Left;
                    element.Cursor = Cursors.SizeWE;
                }
            }
            else if (p.X >= (element.ActualWidth - ResizingArea) && p.Y >= (element.ActualHeight - ResizingArea))
            {
                anchor = ResizeAnchor.BottomRight;
                element.Cursor = Cursors.SizeNWSE;
            }
            else if (p.X >= (element.ActualWidth - ResizingArea))
            {
                if (CanResizeHorizontally)
                {
                    anchor = ResizeAnchor.Right;
                    element.Cursor = Cursors.SizeWE;
                }
            }
            else if (p.Y >= (element.ActualHeight - ResizingArea))
            {
                if (CanResizeVertically)
                {
                    anchor = ResizeAnchor.Bottom;
                    element.Cursor = Cursors.SizeNS;
                }
            }
            else
            {
                anchor = ResizeAnchor.None;
                element.Cursor = null;
            }
        }

        private void ResizeTramTopLeft(double dx, double dy, TranslateTransform transform)
        {
            if (element.Height > MinResizedHeight)
            {
                var y = transform.Y + dy;
                transform.Y += y > 0 ? Math.Floor(dy) : 0;
            }
            if (element.Width > MinResizedWidth)
            {
                var x = transform.X + dx;
                transform.X += x > 0 ? Math.Floor(dx) : 0;
            }
        }

        /// <summary>
        /// Resizes the window by the bottom right corner of the window.
        /// </summary>
        /// <param name="dx">Increment by X-coordinate.</param>
        /// <param name="dy">Increment by Y-coordinate.</param>
        private void ResizeBottomRight(double dx, double dy)
        {
            SetElementSize(dx, dy);
        }

        /// <summary>
        /// Resizes the window by the top left corner of the window.
        /// </summary>
        /// <param name="dx">Increment by X-coordinate.</param>
        /// <param name="dy">Increment by Y-coordinate.</param>
        private void ResizeTopLeft(double dx, double dy)
        {

            var height = double.IsNaN(element.Height) ? element.ActualHeight : element.Height - dy;
            var width = double.IsNaN(element.Width) ? element.ActualWidth : element.Width - dx;
            SetElementSize(width, height);
        }

        /// <summary>
        /// Resizes the window by the lower left corner of the window.
        /// </summary>
        /// <param name="dx">Increment by X-coordinate.</param>
        /// <param name="dy">Increment by Y-coordinate.</param>
        private void ResizeBottomLeft(double dx, double dy)
        {
            var width = element.Width - dx;
            SetElementSize(width, dy);
        }

        /// <summary>
        /// Resizes the window by the top right corner of the window.
        /// </summary>
        /// <param name="dx">Increment by X-coordinate.</param>
        /// <param name="dy">Increment by Y-coordinate.</param>
        private void ResizeTopRight(double dx, double dy)
        {
            var height = element.Height - dy;
            SetElementSize(dx, height);
        }

        private void SetElementSize(double Width, double Height)
        {
            if (Width >= MinResizedWidth && Width < RootContentWidth)
            {
                this.element.Width = Width;
            }
            if (Height >= MinResizedHeight && Height < RootContentHeight)
            {
                this.element.Height = Height;
            }
        }

        private double RootContentWidth
        {
            get
            {
                return Application.Current.Host.Content.ActualWidth;
            }
        }

        private double RootContentHeight
        {
            get
            {
                return Application.Current.Host.Content.ActualHeight;
            }
        }


    }

    public class WindowStateManager
    {
        private DialogSizeState currentState = DialogSizeState.Normal;

        public DialogSizeState CurrentState
        {
            get
            {
                return currentState;
            }
        }

        public void SetMaximized()
        {
            currentState = DialogSizeState.Maximized;
        }

        public void SetMinimized()
        {
            currentState = DialogSizeState.Minimized;
        }

        public void SetNormal()
        {
            currentState = DialogSizeState.Normal;
        }

        public void SetRestored()
        {
            currentState = DialogSizeState.Restored;
        }
    }

    public class WindowSizeManager
    {
        /// <summary>
        ///Min Content area desired width.
        /// </summary>
        public double MinDesiredContentWidth { get; set; }

        /// <summary>
        ///Min Content area desired height.
        /// </summary>
        public double MinDesiredContentHeight { get; set; }


        /// <summary>
        ///Max Content area desired width.
        /// </summary>
        public double MaxDesiredContentWidth { get; set; }

        /// <summary>
        ///Max Content area desired height.
        /// </summary>
        public double MaxDesiredContentHeight { get; set; }

        /// <summary>
        ///Normal Content area desired width.
        /// </summary>
        public double NormalDesiredContentWidth { get; set; }

        /// <summary>
        ///Normal Content area desired height.
        /// </summary>
        public double NormalDesiredContentHeight { get; set; }

        /// <summary>
        ///History Desired margin for the window.
        /// </summary>
        public Thickness DesiredMargin { get; set; }

        /// <summary>
        /// Private accessor for the position of the window with respect to RootVisual.
        /// </summary>
        public Point WindowPosition { get; set; }
    }
}
