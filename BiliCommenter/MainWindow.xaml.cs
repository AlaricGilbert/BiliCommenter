using BiliCommenter.API;
using BiliCommenter.Core;
using BiliCommenter.Models;
using BiliCommenter.Properties;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BiliCommenter
{
    /// <summary>
    /// Interactive logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        /// <summary>
        /// List of the current tasks.
        /// </summary>
        public List<CommentTask> TaskList = new List<CommentTask>();
        /// <summary>
        /// A map from the task's name to the task object instance.
        /// </summary>
        public Dictionary<string, CommentTask> TaskPair = new Dictionary<string, CommentTask>();
        /// <summary>
        /// List of the on-updating bangumis.
        /// </summary>
        private List<BangumiInfo> Bangumis { get; set; }
        /// <summary>
        /// Instance of the welcome window.
        /// </summary>
        private WelcomeWindow WelcomeWindow { get; } = new WelcomeWindow();
        /// <summary>
        /// A variable which marks the selected index number was changed by another,
        /// preventing the infinite loop call between TaskListBox and BangumiListBox.
        /// </summary>
        private bool ListBoxChangedByAnother = false;
        /// <summary>
        /// Create an BitmapImage uses the specified url.
        /// </summary>
        /// <param name="url">Image source</param>
        /// <returns>The created image.</returns>
        public BitmapImage CreateBI(string url)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(url);
            bi.EndInit();
            return bi;
        }
        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }
        public async Task FreshUserInfo()
        {
            await Account.FreshStatusAsync();
            await Account.FreahUserInfoAsync();
            Avator.Source = CreateBI(Account.UserInfo.face);
            UsernameLabel.Content = Account.UserInfo.name;
        }
        public void Initialize()
        {
            // Inherit settings.
            if (Settings.Default.IsUpgrateNeeded)
            {
                Settings.Default.Upgrade();
                Settings.Default.IsUpgrateNeeded = false;
                Settings.Default.Save();
            }

            // Show the welcome window and hide the main window.
            WelcomeWindow.Show();
            this.Visibility = Visibility.Hidden;

            // Inherit tasks.
            if (Settings.Default.IsInheritTasks)
                ReadTasks();

            #region Read log-in informations
            Thread freshThread = new Thread(async () =>
            {
                string access_key = Settings.Default.AccessKey;
                if (access_key != "")
                {
                    Account.AccessKey = access_key;
                    await Auth.FreshSSO();
                    await this.Invoke(async () => await FreshUserInfo());
                }
                else
                    this.Invoke(() => LoginFlyout.IsOpen = true);
            });
            if (Settings.Default.IsSaveAccessKey)
                freshThread.Start(); //Starts the thread only when Settings.Default.IsSaveAccessKey is set to True.
            #endregion

            #region Get bangumi informations.
            Thread bangumiThread = new Thread(async () =>
            {
                Bangumis = await Bangumi.GetBangumiInfosAsync();
                List<string> titles = new List<string>();
                for (int i = 0; i < Bangumis.Count; i++)
                    titles.Add(Bangumis[i].Title);
                this.Invoke(() => BangumiListBox.ItemsSource = titles);
            });
            bangumiThread.Start();
            #endregion

            #region Get bilibili emojis.
            Thread emojiThread = new Thread(async () =>
            {
                // Get the list of the emojis.
                var allEmojis = await Common.GetEmojisAsync();

                for (int i = 0; i < allEmojis.Data.Count; i++)
                {
                    var emojiPack = allEmojis.Data[i];
                    // These part of code is a piece of s**t, but works :).
                    this.Invoke(() =>
                    {
                        var item = new TabItem();
                        item.Header = emojiPack.Pname;
                        StackPanel panel = new StackPanel { Orientation = Orientation.Vertical, HorizontalAlignment = HorizontalAlignment.Center };
                        StackPanel stack = null;
                        for (int j = 0; j < emojiPack.Emojis.Count; j++)
                        {
                            var emoji = emojiPack.Emojis[j];
                            if (j % 9 == 0)
                            {
                                stack = new StackPanel { Orientation = Orientation.Horizontal };
                                panel.Children.Add(stack);
                            }
                            Button button = new Button
                            {
                                Width = 75,
                                Height = 75,
                                Content = new Image
                                {
                                    Width = 70,
                                    Height = 70,
                                    Source = CreateBI(emoji.Url),
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                    VerticalAlignment = VerticalAlignment.Center
                                },
                                Margin = new Thickness(0, 0, 20, 5)
                            };
                            button.Click += (sender, e) =>
                            {
                                string emoji_str = emoji.Name;
                                int index = MessageTextBox.CaretIndex;
                                string newtext = MessageTextBox.Text.Insert(index, emoji_str);
                                MessageTextBox.Text = newtext;
                                MessageTextBox.CaretIndex = index + emoji_str.Length;
                            };
                            stack.Children.Add(button);
                            item.Content = panel;
                        }
                        EmojiTabControl.Items.Add(item);
                    });
                }
                this.Invoke(() =>
                {
                    this.Visibility = Visibility.Visible;
                    WelcomeWindow.Close();
                });
            });
#if !DEBUG
            emojiThread.Start(); // do not load the emojis in the debug mode.
#else
            this.Visibility = Visibility.Visible;
            WelcomeWindow.Close();
#endif
            #endregion
        }
        private void ChangeLoginFlyout(object sender, RoutedEventArgs e) => LoginFlyout.IsOpen = !LoginFlyout.IsOpen;
        private void ChangeLoggedFlyout(object sender, RoutedEventArgs e) => LoggedFlyout.IsOpen = !LoggedFlyout.IsOpen;
        private void ChangeLogStatusFlyouts(object sender, RoutedEventArgs e)
        {
            if (Account.OnlineStatus)
                ChangeLoggedFlyout(sender,e);
            else
                ChangeLoginFlyout(sender,e);
        }
        private void ChangeSettingFlyout(object sender, RoutedEventArgs e)
        {
            IsSaveAccessKey.IsChecked = Settings.Default.IsSaveAccessKey;
            IsInheritTasks.IsChecked = Settings.Default.IsInheritTasks;
            SettingsFlyout.IsOpen = !SettingsFlyout.IsOpen;
        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            LeftAvatar.Source = CreateBI(@"pack://application:,,,/avatar/ic_22.png");
            RightAvatar.Source = CreateBI(@"pack://application:,,,/avatar/ic_33.png");
        }
        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            LeftAvatar.Source = CreateBI(@"pack://application:,,,/avatar/ic_22_hide.png");
            RightAvatar.Source = CreateBI(@"pack://application:,,,/avatar/ic_33_hide.png");
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text;
            string password = PasswordBox.Password;
            if (username == "" || password == "")
            {
                this.ShowMessageAsync("Login failed.", "Username and password can't be null.");
                ResultLabel.Content = "Login failed.\nUsername and password can't be null.";
                return;
            }
            Thread loginThread = new Thread(async () =>
            {
                // we should handle the exceptions that made it fail to login in the future version.
                await Auth.LoginV3(username, password);
                if (Settings.Default.IsSaveAccessKey)
                {
                    Settings.Default.AccessKey = Account.AccessKey;
                    Settings.Default.Save();
                }
                await this.Invoke(async () => {
                    await FreshUserInfo();
                    LoginFlyout.IsOpen = false;
                });
            });
            loginThread.Start();
        }
        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                LoginButton_Click(sender, e);
        }
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Account.AccessKey = "";
            Account.CookieString = "";
            Account.UserInfo = null;
            Account.OnlineStatus = false;
            ChangeLoggedFlyout(sender, e);
            ChangeLoginFlyout(sender, e);
        }
        private void BangumiListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var curr = Bangumis[BangumiListBox.SelectedIndex];
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(curr.Cover);
            bi.EndInit();
            BangumiCover.Source = bi;
            BangumiTitle.Content = $"番剧名称：{curr.Title}";
            BangumiSeason.Content = $"本周更新：第{curr.Index}集";
            BangumiDate.Content = $"更新日期：{curr.UpdateTime.ToLongDateString()}";
            BangumiTime.Content = $"更新时间：{curr.UpdateTime.ToLongTimeString()}";
            if (TaskPair.ContainsKey(curr.Title))
            {
            }
            else
                MessageTextBox.Text = "";
            if (ListBoxChangedByAnother)
                ListBoxChangedByAnother = false;
            else
            {
                if (TaskPair.ContainsKey(curr.Title))
                {
                    ListBoxChangedByAnother = true;
                    TaskListBox.SelectedIndex = TaskPair[curr.Title].TaskId;
                }
            }

        }
        private void TasksListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var currTask = TaskList[TaskListBox.SelectedIndex];
            MessageTextBox.Text = currTask.Message;
            if (ListBoxChangedByAnother)
                ListBoxChangedByAnother = false;
            else
            {
                ListBoxChangedByAnother = true;
                BangumiListBox.SelectedIndex = currTask.BangumiListId;
            }
        }
        private void MessageTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var l = MessageTextBox.Text.Length;
            if (l > 1000)
                MessageTBStatus.Foreground = Brushes.Red;
            else
                MessageTBStatus.Foreground = Brushes.White;
            MessageTBStatus.Content = $"{l}/1000 chars.";
        }
        private void EmojiButton_Click(object sender, RoutedEventArgs e)
        {
            EmojiFlyout.IsOpen = !EmojiFlyout.IsOpen;
            if (EmojiFlyout.IsOpen)
            {
                int delta = 40;
                for (int i = 0; i < 400 / delta; i++)
                    Height += delta;
            }
            else
            {
                Thread th = new Thread(() => {
                    Thread.Sleep(500);
                    int delta = 40;
                    for (int i = 0; i < 400 / delta; i++)
                        this.Invoke(() => Height -= delta);
                });
                th.Start();
            }
        }
        private void AddOrUpdate(object sender, RoutedEventArgs e)
        {
            ReadTasks();
            if(TaskPair.ContainsKey(BangumiListBox.SelectedItem as string))
            {
                TaskPair[BangumiListBox.SelectedItem as string].Message = MessageTextBox.Text;
            }
            else
            {
                CommentTask task = new CommentTask(
                    Bangumis[BangumiListBox.SelectedIndex].UpdateTime,
                    BangumiListBox.SelectedIndex,
                    TaskList.Count,
                    Bangumis[BangumiListBox.SelectedIndex],
                    MessageTextBox.Text,
                    TaskCallback
                    );
                TaskList.Add(task);
                TaskPair.Add(BangumiListBox.SelectedItem as string, task);
                TaskListBox.Items.Add(task.BangumiInfo.Title);
                task.Start();
            }
            SaveCurrentTasks();
        }
        private void TaskCallback(int taskid)
        {
            this.Invoke(() =>
            {
                var title = TaskList[taskid].BangumiInfo.Title;
                TaskList.RemoveAt(taskid);
                TaskPair.Remove(title);
                TaskListBox.Items.Remove(title);
            });
        }
        private void Remove(object sender, RoutedEventArgs e)
        {
            ReadTasks();
            if (TaskListBox.Items.Count == 0)
                return;
            var title = TaskListBox.SelectedItem as string;
            var tsk = TaskPair[title];
            tsk.Dispose();
            TaskList.Remove(tsk);
            TaskPair.Remove(tsk.BangumiInfo.Title);
            TaskListBox.Items.Remove(tsk.BangumiInfo.Title);
            SaveCurrentTasks();
        }
        private void SaveCurrentTasks()
        {
            var json = JsonConvert.SerializeObject(TaskList);
            File.WriteAllText("tasks.json", json);
        }
        private void ReadTasks()
        {
            foreach (var task in TaskList)
                task.Dispose();
            TaskPair.Clear();
            TaskListBox.Items.Clear();
            if (File.Exists("tasks.json"))
                TaskList = JsonConvert.DeserializeObject<List<CommentTask>>(File.ReadAllText("tasks.json"));
            if (TaskList == null)
                TaskList = new List<CommentTask>();
            foreach (var task in TaskList)
            {
                try
                {
                    task.TaskId = TaskListBox.Items.Count;
                    task.Start();
                    TaskListBox.Items.Add(task.BangumiInfo.Title);
                    TaskPair.Add(task.BangumiInfo.Title, task);
                    task.Callback = TaskCallback;
                }
                catch (ArgumentOutOfRangeException) // when target time is before the current time, Noticer throws.
                {
                    // remove it from
                    TaskList.Remove(task);
                    // there should be a logger system..
                    task.Dispose();
                }
            }
        }
        private void UpdateSettings(object sender, RoutedEventArgs e)
        {
            Settings.Default.IsSaveAccessKey = IsSaveAccessKey.IsChecked == true;
            Settings.Default.IsInheritTasks = IsInheritTasks.IsChecked == true;
            Settings.Default.Save();
        }
    }
}
