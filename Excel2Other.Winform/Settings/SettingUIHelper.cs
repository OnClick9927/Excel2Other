using Ookii.Dialogs.WinForms;
using Sunny.UI;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Excel2Other.Winform
{
    public class SettingUIHelper
    {
        /// <summary>
        /// 生成标题
        /// </summary>
        /// <param name="headerText">标题文字</param>
        /// <returns></returns>
        public static UILabel GetHeaderLabel(string headerText)
        {
            var headerLabel = new UILabel
            {
                Style = UIStyle.Custom,
                StyleCustomMode = true,
                Font = new Font("微软雅黑", 14.25f, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft,
                Size = new Size(500, 28),
                Text = headerText
            };
            return headerLabel;
        }

        /// <summary>
        /// 生成文本文字
        /// </summary>
        /// <param name="content">文本文字</param>
        /// <returns></returns>
        public static UILabel GetContentLabel(string content)
        {
            var headerLabel = new UILabel
            {
                Style = UIStyle.Custom,
                StyleCustomMode = true,
                Font = new Font("微软雅黑", 12f),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft,
                Size = new Size(500, 21),
                Text = content
            };
            return headerLabel;
        }

        /// <summary>
        /// 生成输入框
        /// </summary>
        /// <param name="def">默认值</param>
        /// <param name="stringType">文本框类型</param>
        /// <returns></returns>
        public static UITextBox GetInputBox(StringType stringType)
        {
            var inputBox = new UITextBox
            {
                Style = UIStyle.Custom,
                StyleCustomMode = true,
                TextAlignment = ContentAlignment.MiddleLeft
            };

            switch (stringType)
            {
                case StringType.Integer:
                    inputBox.Type = UITextBox.UIEditType.Integer;
                    inputBox.Watermark = "输入数字";
                    inputBox.Size = new Size(70, 30);
                    break;
                case StringType.Double:
                    inputBox.Type = UITextBox.UIEditType.Double;
                    inputBox.Watermark = "输入数字";
                    inputBox.Size = new Size(150, 30);
                    break;
                case StringType.Directory:
                    inputBox.Type = UITextBox.UIEditType.String;
                    inputBox.Size = new Size(400, 30);
                    inputBox.AllowDrop = true;
                    inputBox.Watermark = "双击选择路径或者拖入路径";
                    #region 添加事件
                    inputBox.DoubleClick += (sender, e) =>
                    {
                        VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            ((UITextBox)sender).Text = dialog.SelectedPath;
                        }
                    };

                    inputBox.DragEnter += (sender, e) =>
                    {
                        string[] dropData = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                        if (dropData != null && dropData.Length > 0 && Directory.Exists(dropData[0]))
                        {
                            e.Effect = DragDropEffects.All;
                        }
                        else
                        {
                            e.Effect = DragDropEffects.None;
                        }

                    };

                    inputBox.DragDrop += (sender, e) =>
                    {
                        string[] dropData = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                        if (dropData == null)
                        {
                            UIMessageTip.ShowError("你拖入了什么玩意？？？？？");
                            return;
                        }
                        //如果拖入的文件是多个，暂时不做处理直接报错提示
                        if (dropData.Length >= 2)
                        {
                            UIMessageTip.ShowError("请勿移入多个文件");
                            return;
                        }

                        if (Directory.Exists(dropData[0]))
                        {
                            ((UITextBox)sender).Text = dropData[0];
                        }
                        else
                        {
                            UIMessageTip.ShowError("请拖入文件夹");
                        }
                    };
                    #endregion
                    break;
                default:
                    inputBox.Type = UITextBox.UIEditType.String;
                    inputBox.Size = new Size(230, 30);
                    break;
            }

            return inputBox;
        }

        public static UISwitch GetSwith()
        {
            var uiSwitch = new UISwitch();
            return uiSwitch;
        }

        public static TabPage GetTabPage(string tabName)
        {
            var tabPage = new TabPage
            {
                BackColor = Color.FromArgb(30, 30, 30),
                Text = tabName,
                AutoScroll = true
            };
            return tabPage;
        }

        public static UIButton GetUIButton(string text)
        {
            var button = new UIButton
            {
                Style = UIStyle.Custom,
                StyleCustomMode = true,
                Text = text
            };

            return button;
        }

        public static UILabel GetUILabel()
        {
            var label = new UILabel();
            return label;
        }

    }
}
