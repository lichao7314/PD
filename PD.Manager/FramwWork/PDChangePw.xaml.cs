using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using PD.Controls;
using PD.ServiceClient.PDService;
using PD.ServiceClient;

namespace PD.Manager.FramwWork
{
    public partial class PDChangePw : BaseModalWindow
    {
        private PDServiceClient client = new ClientProxy().Client;

        public PDChangePw()
        {
            InitializeComponent();
            RegisterEvent();
        }

        public override void RegisterEvent()
        {
            btnSave.Click += (btnSave_Click);
            btnCancle.Click += (btnCancle_Click);
            tbName.KeyUp += (tbName_KeyUp);
            tbPw.KeyUp += (tbPw_KeyUp);
            tbNewPw.KeyUp += (tbNewPw_KeyUp);
            tbNewPwSumbit.KeyUp += (tbNewPwSumbit_KeyUp);
            client.ChangeUserPwCompleted += (client_ChangeUserPwCompleted);
        }

        void btnCancle_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        void Save()
        {
            if (string.IsNullOrEmpty(tbName.Text))
            {
                MessageBox.Show("用户名不能为空");
                return;
            }
            if (string.IsNullOrEmpty(this.tbPw.Password))
            {
                MessageBox.Show("旧密码不能为空");
                return;
            }
            if (string.IsNullOrEmpty(this.tbNewPw.Password))
            {
                MessageBox.Show("新密码不能为空");
                return;
            }
            if (!this.tbNewPw.Password.Equals(this.tbNewPwSumbit.Password))
            {
                MessageBox.Show("新密码两次输入不一致");
                return;
            }
            if (CommonMethod.GetLength(tbNewPw.Password) > 16)
            {
                MessageBox.Show("新密码长度过长");
                return;
            }
            client.ChangeUserPwAsync(tbName.Text, tbPw.Password, tbNewPw.Password);
            BeginWindowLoading(this);
        }

        public override void UnRegisterEvent()
        {
            btnSave.Click -= (btnSave_Click);
            btnCancle.Click -= (btnCancle_Click);
            tbName.KeyUp -= (tbName_KeyUp);
            tbPw.KeyUp -= (tbPw_KeyUp);
            tbNewPw.KeyUp -= (tbNewPw_KeyUp);
            tbNewPwSumbit.KeyUp -= (tbNewPwSumbit_KeyUp);
            client.ChangeUserPwCompleted -= (client_ChangeUserPwCompleted);
        }

        void client_ChangeUserPwCompleted(object sender, ChangeUserPwCompletedEventArgs e)
        {
            EndWindowLoading(this);
            if (e.Error == null)
            {
                if (string.IsNullOrEmpty(e.Result))
                    Close();
                else
                    MessageBox.Show(e.Result);
            }
            else
                MessageBox.Show("修改密码失败" + e.Error.Message);
        }

        void tbNewPwSumbit_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            Save();
        }

        void tbNewPw_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                tbNewPwSumbit.Focus();
        }

        void tbPw_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                tbNewPw.Focus();
        }

        void tbName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                tbPw.Focus();
        }

        public override void Dispose()
        {
            UnRegisterEvent();
        }
    }
}
