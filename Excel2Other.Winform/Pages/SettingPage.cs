using Sunny.UI;
using System;
using System.Reflection;

namespace Excel2Other.Winform
{
    public partial class SettingPage : UIPage
    {
        public SettingPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 打开对应设置
        /// </summary>
        /// <param name="tabName">标签页名</param>
        public void OpenSetting(string tabName)
        {
            for (int i = 0; i < tabSettings.TabPages.Count; i++)
            {
                if (tabName.Equals(tabSettings.TabPages[i].Text.Trim()))
                {
                    tabSettings.SelectTab(i);
                    return;
                }
            }
        }

        /// <summary>
        /// 传入设置属性和转换名，自动生成设置页面
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="tabName"></param>
        public void CreateSettingTab(ISetting setting, string tabName, Type entityType = null)
        {
            var settingType = setting.GetType();
            var fields = SettingHelper.GetFields(setting.GetType());
            if (fields.Count == 0) return;
            fields.RemoveAll((f) => { return f.GetCustomAttribute<SettingAttribute>() == null; });
            fields.Sort((x, y) =>
            {
                int xPriority = x.GetCustomAttribute<SettingAttribute>().priority;
                int yPriority = y.GetCustomAttribute<SettingAttribute>().priority;
                return xPriority - yPriority;
            });
            var tabPage = SettingUIHelper.GetTabPage(tabName);
            tabSettings.TabPages.Add(tabPage);
            int location = 10; //控件位置
            foreach (var field in fields)
            {
                var attr = field.GetCustomAttribute<SettingAttribute>();
                //生成标题
                var title = SettingUIHelper.GetHeaderLabel(attr.name);
                var content = SettingUIHelper.GetContentLabel(attr.des);
                //生成内容
                //分为两种  一个是切换按钮的  一个是填框框的，框框的文本框类型需要考虑
                tabPage.Controls.Add(title);
                title.Location = new System.Drawing.Point(30, location);
                location += 30;

                if (field.FieldType == typeof(bool))
                {
                    //生成切换按钮
                    var uiSwitch = SettingUIHelper.GetSwith();
                    tabPage.Controls.Add(uiSwitch);
                    tabPage.Controls.Add(content);
                    uiSwitch.Location = new System.Drawing.Point(30, location);
                    content.Location = new System.Drawing.Point(110, location + 4);
                    location += 30;
                    uiSwitch.Active = (bool)field.GetValue(setting);
                    uiSwitch.ActiveChanged += (sender, e) =>
                    {
                        field.SetValue(setting, uiSwitch.Active);
                        SaveAndRefreshSetting(setting, entityType);
                    };
                }
                else
                {
                    UITextBox inputBox = null;
                    if (field.FieldType == typeof(string))
                    {
                        inputBox = SettingUIHelper.GetInputBox(attr.textType);

                        inputBox.Text = (string)field.GetValue(setting).ToString();
                        inputBox.Leave += (sender, e) =>
                        {
                            field.SetValue(setting, inputBox.Text);
                            SettingHelper.SaveSetting(setting);
                            SaveAndRefreshSetting(setting, entityType);
                        };
                    }
                    else if (field.FieldType == typeof(int))
                    {
                        //这里针对行号+1的问题处理……后续会改
                        inputBox = SettingUIHelper.GetInputBox(StringType.Integer);
                        inputBox.Text = (string)((int)field.GetValue(setting) + 1).ToString();
                        inputBox.Leave += (sender, e) =>
                        {
                            int.TryParse(inputBox.Text, out int num);
                            field.SetValue(setting, -1);
                            SaveAndRefreshSetting(setting, entityType);
                        };
                    }

                    tabPage.Controls.Add(inputBox);
                    tabPage.Controls.Add(content);

                    content.Location = new System.Drawing.Point(30, location);
                    inputBox.Location = new System.Drawing.Point(30, location + 30);
                    location += 60;
                }
                location += 30;

            }

            if (!tabName.Equals("通用"))
            {
                //生成按钮
                var loadButton = SettingUIHelper.GetUIButton("设置读取");
                var saveButton = SettingUIHelper.GetUIButton("设置另存为");
                tabPage.Controls.Add(loadButton);
                tabPage.Controls.Add(saveButton);

                //按钮绑定方法
                loadButton.Click += (sender, e) =>
                {
                    SettingHelper.LoadSetting(settingType, true);
                };
                saveButton.Click += (sender, e) =>
                {
                    SettingHelper.SaveSetting(entityType, true);
                };

                loadButton.Location = new System.Drawing.Point(30, location);
                saveButton.Location = new System.Drawing.Point(150, location);

                location += 50;
                var blankLabel = SettingUIHelper.GetUILabel();
                tabPage.Controls.Add(blankLabel);
                blankLabel.Location = new System.Drawing.Point(0, location);
            }
            
            tabSettings.Refresh();
        }

        private void SaveAndRefreshSetting(ISetting setting, Type entityType)
        {
            SettingHelper.SaveSetting(setting);
            if (entityType != null)
            {
                ExcelHelper.GetEntity(entityType).SetSetting(setting);
                ExcelHelper.SetAllDirty(entityType);
            }
        }
    }
}
