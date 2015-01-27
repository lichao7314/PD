using System;

namespace PD.Controls.Media
{
    public class PlayListItem
    {
        /// <summary>
        /// 资源地址
        /// </summary>
        public Uri MediaSource { get; set; }

        public string DisplayName { get; set; }

        public string ItemIndex { get; set; }
    }
}
