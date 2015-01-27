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
using System.Linq;
namespace PD.Controls
{
    public abstract class ControlOrientationProvider : IOrientationProvider
    {
        public DataGridMoveInputBehavior Behavior
        {
            get;
            set;
        }

        private FrameworkElement currentControl;

        public FrameworkElement CurrentControl
        {
            get { return currentControl; }
            set { currentControl = value; }
        }

        public virtual void SetOrientationProvider(FrameworkElement control)
        {
            if (control != null)
            {
                CurrentControl = control;
                control.AddHandler(UIElement.KeyDownEvent, new KeyEventHandler(HandleOwnerMouseRightButtonDown), true);
            }
        }

        protected virtual void HandleOwnerMouseRightButtonDown(object sender, KeyEventArgs e)
        {
            DataGridColumn moveToColumn = null;

            var currentColumn = Behavior.DataGrid.CurrentColumn;

            var currentColumnAction = Behavior.EditColumns[currentColumn];

            if (e.Key == Key.Left)
            {
                if (currentColumnAction != null)
                {
                    if (currentColumnAction.MoveSpan == MoveSpan.NextEditColumn)
                        moveToColumn = Behavior.EditColumns.LeftColumnItem(currentColumnAction).Column;
                    else
                        moveToColumn = Behavior.DataGrid.LeftColumnItem();
                }
                else
                    moveToColumn = Behavior.DataGrid.LeftColumnItem();

            }
            if (e.Key == Key.Right)
            {
                if (currentColumnAction != null)
                {
                    if (currentColumnAction.MoveSpan == MoveSpan.NextEditColumn)
                        moveToColumn = Behavior.EditColumns.RightColumnItem(currentColumnAction).Column;
                    else
                        moveToColumn = Behavior.DataGrid.RightColumnItem();
                }
                else
                    moveToColumn = Behavior.DataGrid.RightColumnItem();
            }
            if (moveToColumn != null)
            {
                Behavior.DataGrid.ScrollIntoView(Behavior.DataGrid.SelectedItem, moveToColumn);
                Behavior.DataGrid.CurrentColumn = moveToColumn;
            }
        }

        public virtual void SetFocus()
        {

        }

        public virtual void Dispose()
        {
            CurrentControl.RemoveHandler(UIElement.KeyDownEvent, new KeyEventHandler(HandleOwnerMouseRightButtonDown));
        }
    }
}
