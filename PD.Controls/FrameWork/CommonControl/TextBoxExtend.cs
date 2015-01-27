using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PD.Controls
{
    public class TextBoxExtend : TextBox, IDisposable
    {
        /// <summary>
        /// 在文本框中的内容更改时发生。
        /// </summary>
        public new event TextChangedEventHandler TextChanged;

        /// <summary>
        /// 文本框控件
        /// </summary>
        public TextBoxExtend()
        {
            RegisterEvent();
            _flag = true;
        }
        #region IsToDbc 是否转半角

        /// <summary>
        /// IsToDbc 依赖属性
        /// </summary>
        public static readonly DependencyProperty IsToDbcProperty =
            DependencyProperty.Register("IsToDbc", typeof(bool), typeof(TextBoxExtend),
                new PropertyMetadata(true,
                    OnIsToDbcChanged));

        /// <summary>
        /// 是否转半角
        /// </summary>
        [Category("自定义属性")
        , Description("是否转半角，默认true")]
        public bool IsToDbc
        {
            get
            {
                return (bool)GetValue(IsToDbcProperty);
            }
            set
            {
                SetValue(IsToDbcProperty, value);
            }
        }

        /// <summary>
        /// 处理的IsToDbc属性的变化 是否转半角
        /// </summary>
        private static void OnIsToDbcChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextBoxExtend)d).OnIsToDbcChanged(e);
        }

        /// <summary>
        /// 处理的IsToDbc属性的变化 是否转半角
        /// </summary>
        protected virtual void OnIsToDbcChanged(DependencyPropertyChangedEventArgs e)
        {
            var value = (bool)e.NewValue;
            if (value)
            {
                AsyncSetText(PublicMethod.ToDbc(base.Text), this.SelectionStart);
            }
        }

        #endregion

        #region InputNullRuleMess 为空验证
        /*
        /// <summary>
        /// InputNullRuleMess 依赖属性
        /// </summary>
        public static readonly DependencyProperty InputNullRuleMessProperty =
            DependencyProperty.Register("InputNullRuleMess", typeof(string), typeof(TextBoxExtend),
                new PropertyMetadata("",
                    OnInputNullRuleMessChanged));

        /// <summary>
        ///  获取或设置InputNullRuleMess属性
        ///  此显示依赖项属性
        /// </summary>
        [Category("自定义属性")
        , Description("为空验证，传空不验证")]
        public string InputNullRuleMess
        {
            get
            {
                return (string)GetValue(InputNullRuleMessProperty);
            }
            set
            {
                SetValue(InputNullRuleMessProperty, value);
            }
        }

        /// <summary>
        /// 处理的InputNullRuleMess属性的变化 为空验证
        /// </summary>
        private static void OnInputNullRuleMessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextBoxExtend)d).OnInputNullRuleMessChanged(e);
        }

        /// <summary>
        /// 处理的InputNullRuleMess属性的变化 为空验证
        /// </summary>
        protected virtual void OnInputNullRuleMessChanged(DependencyPropertyChangedEventArgs e)
        {
           if(!string.IsNullOrEmpty(e.NewValue+""))
           {

           }
        }
        */
        #endregion

        #region MaxByteLength 最大字节长度

        /// <summary>
        /// MaxByteLength依赖属性
        /// </summary>
        public static readonly DependencyProperty MaxByteLengthProperty =
            DependencyProperty.Register("MaxByteLength", typeof(int), typeof(TextBoxExtend), new PropertyMetadata(0,
                  null));
        /// <summary>
        ///获取或设置MaxByteLength属性。
        ///此显示依赖项属性
        /// 最大字节长度
        /// </summary>
        [Category("自定义属性")
        , Description("最大字节长度")]
        public int MaxByteLength
        {
            get { return (int)GetValue(MaxByteLengthProperty); }
            set { SetValue(MaxByteLengthProperty, value); }
        }

        #endregion

        #region InputType 输入类型

        /// <summary>
        /// InputType 依赖属性
        /// </summary>
        public static readonly DependencyProperty InputTypeProperty =
            DependencyProperty.Register("InputType", typeof(InputType), typeof(TextBoxExtend),
                new PropertyMetadata(InputType.Text,
                    OnInputTypeChanged));

        /// <summary>
        ///  获取或设置InputType属性
        ///  此显示依赖项属性
        /// 输入类型
        /// </summary>
        [Category("自定义属性")
        , Description("输入类型")]
        public InputType InputType
        {
            get
            {
                return (InputType)GetValue(InputTypeProperty);
            }
            set
            {
                SetValue(InputTypeProperty, value);
            }
        }

        /// <summary>
        /// 处理的InputType属性的变化 文本框改变
        /// </summary>
        private static void OnInputTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextBoxExtend)d).OnInputTypeChanged(e);
        }

        /// <summary>
        /// 处理的InputType属性的变化 文本框改变
        /// </summary>
        protected virtual void OnInputTypeChanged(DependencyPropertyChangedEventArgs e)
        {
            var inputType = (InputType)e.NewValue;
            if (inputType == InputType.Digit && !string.IsNullOrEmpty(base.Text) && base.Text.Any(c => !char.IsDigit(c)))
            {
                Text = "";
            }
            //switch (inputType)
            //{
            //    case InputType.Text:
            //        TextAlignment = TextAlignment.Left;
            //        break;
            //    case InputType.Digit:
            //    case InputType.DecimalPlaces:
            //        TextAlignment = TextAlignment.Right;
            //        break;
            //}
        }

        #endregion

        #region Fractional 小数位前后多少位

        /// <summary>
        /// Fractional 依赖属性
        /// </summary>
        public static readonly DependencyProperty FractionalProperty =
            DependencyProperty.Register("Fractional", typeof(DecimalMedian), typeof(TextBoxExtend),
                new PropertyMetadata(new DecimalMedian(5, 2),
                    FractionalChanged));

        /// <summary>
        ///  获取或设置Fractional属性
        ///  小数位前后多少位
        /// </summary>
        [Category("自定义属性")
        , Description("小数位前后多少位")]
        public DecimalMedian Fractional
        {
            get
            {
                var v = new DecimalMedian(GetValue(FractionalProperty));
                return v;
            }
            set
            {
                SetValue(FractionalProperty, value);
            }
        }

        /// <summary>
        /// 处理的Fractional属性的变化 文本框改变
        /// </summary>
        private static void FractionalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextBoxExtend)d).OnFractionalChangedChanged(e);
        }

        /// <summary>
        /// 处理的Fractional属性的变化 文本框改变
        /// </summary>
        protected virtual void OnFractionalChangedChanged(DependencyPropertyChangedEventArgs e)
        {
            this.Text = "";
        }

        #endregion

        #region Text 获取或设置文本框的文本内容
        /// <summary>
        /// Fractional 依赖属性
        /// </summary>
        public new static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TextBoxExtend),
                new PropertyMetadata(string.Empty,
                    ValueChanged));

        /// <summary>
        ///  获取或设置Text属性
        ///  此显示依赖项属性
        /// 获取或设置文本框的文本内容
        /// </summary>
        [Category("自定义属性")
        , Description("获取或设置文本框的文本内容。")]
        public new string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        /// <summary>
        /// 处理的Fractional属性的变化 文本框改变
        /// </summary>
        private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextBoxExtend)d).OnTextChanged(e);
        }

        /// <summary>
        /// 处理的Fractional属性的变化 文本框改变
        /// </summary>
        protected virtual void OnTextChanged(DependencyPropertyChangedEventArgs e)
        {
            if (InputType == InputType.DecimalPlaces || InputType == InputType.PlusAndMinusDecimalPlaces
                )
            {
                if ((e.NewValue + "").Trim() == "")
                {
                    Text = null;
                }
            }
            base.Text = Text + "";
        }

        #endregion

        #region Text 获取或设置文本框的绑定字段

        /// <summary>
        /// TextField 依赖属性
        /// </summary>
        public static readonly DependencyProperty TextFieldProperty =
            DependencyProperty.Register("TextField", typeof(string), typeof(TextBoxExtend),
                                        new PropertyMetadata(string.Empty, OnTextFieldChanged));

        /// <summary>
        ///  获取或设置TextField属性绑定字段
        ///  此显示依赖项属性
        /// 获取或设置文本框绑定字段
        /// </summary>
        [Category("自定义属性")
        , Description("获取或设置文本框绑定字段。")]
        public string TextField
        {
            get
            {
                return (string)GetValue(TextFieldProperty);
            }
            set
            {
                SetValue(TextFieldProperty, value);
            }
        }

        /// <summary>
        /// 处理的TextField属性的变化 文本框绑定字段改变
        /// </summary>
        private static void OnTextFieldChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextBoxExtend)d).OnTextFieldChanged(e);
        }

        /// <summary>
        /// 处理的TextField属性的变化 文本框绑定字段改变
        /// </summary>
        protected virtual void OnTextFieldChanged(DependencyPropertyChangedEventArgs e)
        {
            SetBinding(TextProperty, new Binding(e.NewValue + "") { Mode = BindingMode.TwoWay });
        }

        #endregion

        #region HandleEvents
        /// <summary>
        /// 是否第一次加载
        /// </summary>
        private bool _flag;

        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            if (InputType == InputType.DecimalPlaces
                 || InputType == InputType.Digit
                   )
            {
                if (e.Text != ".")
                {
                    if (e.Text.Any(c => !char.IsDigit(c)))
                    {
                        return;
                    }
                }
            }
            if (InputType == InputType.PlusAndMinusDecimalPlaces

                   )
            {
                if (e.Text != "." && e.Text != "-" && e.Text != "+")
                {
                    if (e.Text.Any(c => !char.IsDigit(c)))
                    {
                        return;
                    }
                }
            }
            base.OnTextInput(e);
        }
        protected override void OnTextInputStart(TextCompositionEventArgs e)
        {
            if (_flag)
            {
                _lastValidText = base.Text;
                _flag = false;
            }
            base.OnTextInputStart(e);
        }
        protected override void OnTextInputUpdate(TextCompositionEventArgs e)
        {
            base.OnTextInputUpdate(e);
        }

        /// <summary>
        /// 文本框改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ReTextBoxInputControlTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                // 先检查输入的正确性
                CheckInputType();
                switch (InputType)
                {
                    case InputType.DecimalPlaces:

                        CheckDecimal();

                        break;
                    case InputType.PlusAndMinusDecimalPlaces:

                        CheckPlusAndMinusDecimal();

                        break;
                    // 再检查长度的正确性
                }
                CheckMaxByteLength();
                if (TextChanged != null)
                {
                    TextChanged(this, e);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ReTextBoxInputControlTextChanged事件" + ex.Message);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.None)
            {
                _isInputWithKeyboard = true;
            }
            if (InputType == InputType.DecimalPlaces)
            {
                if (base.Text.Contains("."))
                {
                    if (e.PlatformKeyCode == 190)
                    {
                        e.Handled = true;
                        return;
                    }
                }
            }
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            _isInputWithKeyboard = false;
            base.OnKeyUp(e);
        }

        /// <summary>
        /// 验证小数位
        /// </summary>
        /// <returns></returns>
        private void CheckDecimal()
        {
            try
            {
                if (InputType == InputType.DecimalPlaces)
                {
                    if (string.IsNullOrEmpty(base.Text))
                    {
                        return;
                    }
                    var startIndex = this.SelectionStart == 0 ? this.SelectionStart : this.SelectionStart - 1;
                    var textarrary = base.Text.Split('.');
                    if (textarrary.Length > 2)
                    {
                        AsyncSetText(_lastValidText, startIndex);
                        return;
                    }
                    try
                    {
                        //验证是否为数字
                        if (textarrary.Any(s => s.Any(c => !char.IsDigit(c))))
                        {
                            AsyncSetText(_lastValidText, startIndex);
                            return;
                        }
                        // AsyncSetText(Convert.ToDecimal(Text).ToString("G0"), startIndex, this);
                        if (base.Text.Contains('.'))
                        {
                            // if (result.SelectionStart > Text.Trim().IndexOf(".", System.StringComparison.Ordinal))
                            // {
                            var tempTxt = base.Text.Substring(base.Text.Trim().IndexOf(".", System.StringComparison.Ordinal) + 1);
                            if (tempTxt.Length > Fractional.AfterPoint)
                            {
                                AsyncSetText(_lastValidText, startIndex);
                                return;
                            }
                            //   }
                            //   else
                            //   {
                            if (base.Text.Substring(0, base.Text.Trim().IndexOf(".", System.StringComparison.Ordinal)).Length >
                                Fractional.BeforePoint)
                            {
                                AsyncSetText(_lastValidText, startIndex);
                                return;
                            }
                            //   }
                        }
                        else
                        {
                            if (base.Text.Length > Fractional.BeforePoint)
                            {
                                AsyncSetText(_lastValidText, startIndex);
                                return;
                            }
                        }
                    }
                    catch
                    {
                        AsyncSetText(_lastValidText, startIndex);
                        return;
                    }
                    _lastValidText = base.Text;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("CheckDecimal方法");
            }
        }

        /// <summary>
        /// 验证正负小数位
        /// </summary>
        /// <returns></returns>
        private void CheckPlusAndMinusDecimal()
        {
            if (InputType == InputType.PlusAndMinusDecimalPlaces)
            {
                if (string.IsNullOrEmpty(base.Text))
                {
                    return;
                }
                string tempText = "";
                string tempSymbol = base.Text.Substring(0, 1);
                if (tempSymbol.Equals("+") || tempSymbol.Equals("-"))
                {
                    tempText = base.Text.Substring(1);
                }
                else
                {
                    tempText = base.Text;
                    tempSymbol = "";
                }
                var startIndex = SelectionStart == 0 ? SelectionStart : SelectionStart - 1;
                var textarrary = tempText.Split('.');
                if (textarrary.Length > 2)
                {
                    AsyncSetText(_lastValidText, startIndex);
                    return;
                }
                //验证是否为数字
                if (textarrary.Any(s => s.Any(c => !char.IsDigit(c))))
                {
                    AsyncSetText(_lastValidText, startIndex);
                    return;
                }
                try
                {
                    if (tempText.Contains('.'))
                    {
                        //  if (result.SelectionStart > tempText.Trim().IndexOf(".", System.StringComparison.Ordinal))
                        //   {
                        if (tempText.Substring(tempText.Trim().IndexOf(".", System.StringComparison.Ordinal) + 1).Length >
                            Fractional.AfterPoint)
                        {
                            AsyncSetText(_lastValidText, startIndex);
                            return;
                        }
                        //  }
                        //  else
                        //  {
                        if (tempText.Substring(0, tempText.Trim().IndexOf(".", System.StringComparison.Ordinal)).Length >
                            Fractional.BeforePoint)
                        {
                            AsyncSetText(_lastValidText, startIndex);
                            return;
                        }
                        // }
                    }
                    else
                    {
                        if (tempText.Length > Fractional.BeforePoint)
                        {
                            AsyncSetText(_lastValidText, startIndex);
                            return;
                        }
                    }
                }
                catch
                {
                    AsyncSetText(_lastValidText, startIndex);
                    return;
                }
                _lastValidText = tempSymbol + tempText;
            }
        }

        #endregion

        /// <summary>
        /// 上个插入位置索引。
        /// </summary>
        private int _lastCaretIndex = 0;
        /// <summary>
        /// 上个插入前的值
        /// </summary>
        private string _lastValidText = "";
        /// <summary>
        /// 是否键盘输入
        /// </summary>
        private bool _isInputWithKeyboard = false;

        #region CheckInputType

        public void CheckInputType()
        {
            try
            {
                if (IsToDbc)
                {
                    AsyncSetText(PublicMethod.ToDbc(base.Text), this.SelectionStart);
                }

                // VerifyShapedChar();
                if (InputType == InputType.Text || _lastValidText == base.Text
                    || InputType == InputType.DecimalPlaces || InputType == InputType.PlusAndMinusDecimalPlaces
                    )
                    return;
                if (base.Text.Any(c => !char.IsDigit(c)))
                {
                    AsyncSetText(_lastValidText, _lastCaretIndex);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("CheckInputType方法");
            }
        }

        #endregion

        #region CheckMaxByteLength
        /// <summary>
        /// 检查长度的正确性
        /// </summary>
        /// <param name="txt"></param>
        private void CheckMaxByteLength()
        {
            try
            {
                var text = base.Text;
                if (text == _lastValidText)
                    return;
                var maxByteLength = this.MaxByteLength;
                if (maxByteLength == 0)
                {
                    _lastValidText = text;
                    _lastCaretIndex = this.SelectionStart;
                    return;
                }
                _lastValidText = PublicMethod.TruncatedStringForMaxLength(text, _lastValidText, maxByteLength,
                                                                          _isInputWithKeyboard);
                _lastCaretIndex = this.SelectionStart;
                AsyncSetText(_lastValidText, _lastCaretIndex);
            }
            catch (Exception)
            {
                MessageBox.Show("CheckMaxByteLength方法");
            }

            #region old

            /*
            //var encoding = Encoding.Unicode;
            //var tempBytes = encoding.GetBytes(text);
            // var textByteLength = encoding.GetBytes(text).Count(v => v != 0);
            int textByteLength = GetMaxByteLength(text);
            //for (var i = 0; i < tempBytes.Length; i++)
            //{
            //    if(IsParity(i))
            //    {
            //        if(tempBytes[i]!=0)
            //        {
            //            textByteLength++;
            //        }
            //    }
            //    else
            //    {
            //        textByteLength++;
            //    }
            //}

            // 如果字节数大于最大字节数，要截断
            if (textByteLength > maxByteLength)
            {
                // 如果是通过键盘输入的
                if (_isInputWithKeyboard)
                {
                    AsyncSetText(_lastValidText, _lastCaretIndex, this);
                    return;
                }

                // 如果是其他途径，比如复制粘贴，或者是代码直接赋值的，需要截断
                var endPos = text.Length;
                var startPos = 0;
                var validLength = 0;

                // 二分法截断
                while (startPos <= endPos)
                {
                    var trimLength = (startPos + endPos) / 2 + (startPos + endPos) % 2;
                    var currentText = text.Substring(0, trimLength);
                    //var currentByteLength = encoding.GetBytes(currentText).Count(v => v != 0);
                    var currentByteLength = GetMaxByteLength(currentText);

                    if (currentByteLength > maxByteLength)
                    {
                        endPos = trimLength - 1;
                    }
                    else if (currentByteLength < maxByteLength)
                    {
                        startPos = trimLength + 1;
                        validLength = trimLength;
                    }
                    else
                    {
                        validLength = trimLength;
                        break;
                    }
                }
                var validText = text.Substring(0, validLength);
                _lastValidText = validText;
                _lastCaretIndex = text.Length;

                AsyncSetText(_lastValidText, _lastCaretIndex, this);
            }
            else
            {
                _lastValidText = text;
                _lastCaretIndex = this.SelectionStart;
            }
             */

            #endregion


        }

        #endregion


        #region IModule 成员

        public void RegisterEvent()
        {
            base.TextChanged += ReTextBoxInputControlTextChanged;
        }

        public void UnRegisterEvent()
        {
            base.TextChanged -= ReTextBoxInputControlTextChanged;
        }

        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            UnRegisterEvent();

            GC.Collect();
            // base.Dispose();
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="caretIndex">插入位置索引</param>
        /// <param name="txt"></param>
        public void AsyncSetText(string text, int caretIndex)
        {
            UnRegisterEvent();
            Text = text;
            SelectionStart = caretIndex < 0 ? 0 : caretIndex;
            RegisterEvent();
        }

    }

    /// <summary>
    /// 公共方法
    /// </summary>
    public static partial class PublicMethod
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="caretIndex">插入位置索引</param>
        /// <param name="txt"></param>
        public static void AsyncSetText(string text, int caretIndex, System.Windows.Controls.TextBox txt)
        {
            txt.Text = text;
            txt.SelectionStart = caretIndex < 0 ? 0 : caretIndex;
        }
        /// <summary>   
        /// 转半角的函数(DBC case)     
        /// </summary> 
        /// <param name="input">任意字符串</param>   
        /// <returns>半角字符串</returns>   
        ///<remarks>   
        ///全角空格为12288，半角空格为32    
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248     
        ///</remarks>    
        public static string ToDbc(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }

        /// <summary>
        /// 根据最大字符长度截断
        /// </summary>
        /// <param name="text">新字符串（需截断）</param>
        /// <param name="oldText">原来的字符串 </param>
        /// <param name="maxByteLength">保留长度</param>
        /// <param name="isInputWithKeyboard"></param>
        /// <returns></returns>
        public static string TruncatedStringForMaxLength(string text, string oldText, int maxByteLength, bool isInputWithKeyboard)
        {
            int textByteLength = GetMaxByteLength(text);
            // 如果字节数大于最大字节数，要截断
            if (textByteLength > maxByteLength)
            {
                // 如果是通过键盘输入的
                if (isInputWithKeyboard)
                {
                    return oldText;
                }
                // 如果是其他途径，比如复制粘贴，或者是代码直接赋值的，需要截断
                var endPos = text.Length;
                var startPos = 0;
                var validLength = 0;

                // 二分法截断
                while (startPos <= endPos)
                {
                    var trimLength = (startPos + endPos) / 2 + (startPos + endPos) % 2;
                    var currentText = text.Substring(0, trimLength);
                    var currentByteLength = GetMaxByteLength(currentText);

                    if (currentByteLength > maxByteLength)
                    {
                        endPos = trimLength - 1;
                    }
                    else if (currentByteLength < maxByteLength)
                    {
                        startPos = trimLength + 1;
                        validLength = trimLength;
                    }
                    else
                    {
                        validLength = trimLength;
                        break;
                    }
                }
                var validText = text.Substring(0, validLength);
                return validText;
            }
            return text;
        }
        /// <summary>
        /// 根据最大字符长度截断
        /// </summary>
        /// <param name="text">新字符串（需截断）</param>
        /// <param name="oldText">原来的字符串 </param>
        /// <param name="maxByteLength">保留长度</param>
        /// <param name="isInputWithKeyboard"></param>
        /// <param name="isTruncated">是否截断 </param>
        /// <returns></returns>
        public static string TruncatedStringForMaxLength(string text, string oldText, int maxByteLength, bool isInputWithKeyboard, out bool isTruncated)
        {
            int textByteLength = GetMaxByteLength(text);
            // 如果字节数大于最大字节数，要截断
            if (textByteLength > maxByteLength)
            {
                // 如果是通过键盘输入的
                if (isInputWithKeyboard)
                {
                    isTruncated = true;
                    return oldText;
                }
                // 如果是其他途径，比如复制粘贴，或者是代码直接赋值的，需要截断
                var endPos = text.Length;
                var startPos = 0;
                var validLength = 0;

                // 二分法截断
                while (startPos <= endPos)
                {
                    var trimLength = (startPos + endPos) / 2 + (startPos + endPos) % 2;
                    var currentText = text.Substring(0, trimLength);
                    var currentByteLength = GetMaxByteLength(currentText);

                    if (currentByteLength > maxByteLength)
                    {
                        endPos = trimLength - 1;
                    }
                    else if (currentByteLength < maxByteLength)
                    {
                        startPos = trimLength + 1;
                        validLength = trimLength;
                    }
                    else
                    {
                        validLength = trimLength;
                        break;
                    }
                }
                var validText = text.Substring(0, validLength);
                isTruncated = true;
                return validText;
            }
            isTruncated = false;
            return text;
        }


        /// <summary>
        /// 获取字段最大字节数区分中英文函数
        /// </summary>
        /// <param name="str">当前字符串</param>
        /// <returns>最大值</returns>
        public static int GetMaxByteLength(string str)
        {
            if (str == null) str = "";
            byte[] tempBytes = Encoding.Unicode.GetBytes(str.Trim());
            int textByteLength = 0;
            for (var i = 0; i < tempBytes.Length; i++)
            {
                if (IsParity(i))
                {
                    if (tempBytes[i] != 0)
                    {
                        textByteLength++;
                    }
                }
                else
                {
                    textByteLength++;
                }
            }
            return textByteLength;
        }
        /// <summary>
        /// 判断奇偶数
        /// </summary>
        /// <param name="n"></param>
        /// <returns>true 奇 false 偶</returns>
        public static bool IsParity(int n)
        {
            return Convert.ToBoolean(n % 2);
        }

        /// <summary>
        /// 清空清除勾选列的行信息
        /// </summary>
        /// <param name="dataGrid"></param>
        public static void MarkObjectClear(DataGrid dataGrid)
        {
            try
            {
                DataGridSelectColumn.GetSelectColumn(dataGrid)._markObject.Selected = false;
                // DataGridSelectColumn.GetSelectColumn(dataGrid)._markObjects.Clear();
            }
            catch { }
        }
    }

    public enum InputType
    {
        /// <summary>
        /// 文字
        /// </summary>
        Text = 0,
        /// <summary>
        /// 数字
        /// </summary>
        Digit = 1,
        /// <summary>
        /// 小数位
        /// </summary>
        DecimalPlaces = 2,
        /// <summary>
        /// 正负小数位
        /// </summary>
        PlusAndMinusDecimalPlaces = 3,
        #region 未实现
        /// <summary>
        /// 正负整数
        /// </summary>
        //  PlusAndMinusInteger = 4,
        /// <summary>
        /// 只能输入字符不包含数字和异型字符 
        /// </summary>
        //  StringType = 5,
        #endregion
    }
    /// <summary>
    /// 小数位数
    /// </summary>
    [TypeConverter(typeof(DecimalMedianTypeConverter))]
    public struct DecimalMedian : IFormattable
    {
        public DecimalMedian(int beforePoint, int afterPoint)
        {
            BeforePoint = beforePoint;
            AfterPoint = afterPoint;
        }
        public DecimalMedian(string param)
        {
            var result = param.Split(',');
            if (result.Length == 2)
            {
                BeforePoint = Convert.ToInt32(result[0]);
                AfterPoint = Convert.ToInt32(result[1]);
            }
            else
            {
                MessageBox.Show("正确格式为5,2");
                BeforePoint = 5;
                AfterPoint = 2;
            }
        }
        public DecimalMedian(object param)
        {
            var result = param.ToString().Split(',');
            if (result.Length == 2)
            {
                BeforePoint = Convert.ToInt32(result[0]);
                AfterPoint = Convert.ToInt32(result[1]);
            }
            else
            {
                MessageBox.Show("正确格式为5,2");
                BeforePoint = 5;
                AfterPoint = 2;
            }
        }

        /// <summary>
        /// 小数前位数
        /// </summary>
        public int BeforePoint;
        /// <summary>
        /// 小数点后位数
        /// </summary>
        public int AfterPoint;

        /// <summary>
        ///  创建此 DecimalMedian 的 System.String 表示形式。
        /// </summary>
        /// <returns> 一个 System.String，它包含此 DecimalMedian 结构的 DecimalMedian.BeforePoint 和 DecimalMedian.AfterPoint
        /// 值。</returns>
        public override string ToString()
        {
            return BeforePoint + "," + AfterPoint;
        }

        public override bool Equals(object obj)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region IFormattable 成员

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return BeforePoint + "," + AfterPoint;
        }

        #endregion
    }

    public class DecimalMedianTypeConverter : TypeConverter
    {
        #region 重写TypeConverter

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if ((destinationType == typeof(string)))
            {
                return true;
            }
            else
            {
                return base.CanConvertTo(context, destinationType);
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value != null)
            {
                var result = (DecimalMedian)value;
                return result.BeforePoint + "," + result.AfterPoint;
            }
            return "5,2"; //base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value == null)
            {
                return new DecimalMedian(5, 2);
            }
            if (value is string)
            {
                return new DecimalMedian(value);
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }

        #endregion
    }
}
