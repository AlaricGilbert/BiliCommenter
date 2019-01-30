using BiliCommenter.API;
using BiliCommenter.Core;
using BiliCommenter.Models;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
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
    /// MainWindow.xaml Logic
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public List<CommentTask> TaskList = new List<CommentTask>();
        public Dictionary<string, CommentTask> TaskPair = new Dictionary<string, CommentTask>();
        private UserInfoModel.UserInfo UserInfo { get; set; }
        private List<BangumiInfo> Bangumis { get; set; }
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
            InitializeControls();
        }
        public async Task FreshUserInfo()
        {
            await Account.FreshStatusAsync();
            await Account.FreahUserInfoAsync();
            Avator.Source = CreateBI(Account.UserInfo.face);
            UsernameLabel.Content = Account.UserInfo.name;
        }
        public void InitializeControls()
        {

            Thread freshThread = new Thread(async () =>
            {
                if (!File.Exists("bili.ack"))
                    File.Create("bili.ack").Close();
                string access_key = File.ReadAllText("bili.ack");
                if (access_key != "")
                {
                    Account.AccessKey = access_key;
                    await Auth.FreshSSO();
                    await this.Invoke(async () => await FreshUserInfo());
                }
                else
                    this.Invoke(() => LoginFlyout.IsOpen = true);
            });
            freshThread.Start();
            Thread bangumiThread = new Thread(async () =>
            {
                Bangumis = await Bangumi.GetBangumiInfosAsync();
                List<string> titles = new List<string>();
                for (int i = 0; i < Bangumis.Count; i++)
                {
                    titles.Add(Bangumis[i].Title);
                }
                this.Invoke(() => BangumiListBox.ItemsSource = titles);
            });
            bangumiThread.Start();
            Thread emojiThread = new Thread(async () =>
            {

                var allEmojis = await Common.GetEmojisAsync();

                for (int i = 0; i < allEmojis.Data.Count; i++)
                {
                    var emojiPack = allEmojis.Data[i];
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
            });
            emojiThread.Start();
        }
        private void ChangeLoginFlyout(object sender, RoutedEventArgs e)
        {
            LoginFlyout.IsOpen = !LoginFlyout.IsOpen;
        }
        private void ChangeLoggedFlyout(object sender, RoutedEventArgs e)
        {
            LoggedFlyout.IsOpen = !LoggedFlyout.IsOpen;
        }
        private void ChangeFlyouts(object sender, RoutedEventArgs e)
        {
            if (Account.OnlineStatus)
                ChangeLoggedFlyout(sender,e);
            else
                ChangeLoginFlyout(sender,e);
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
                await Auth.LoginV3(username, password);
                File.WriteAllText("bili.ack", Account.AccessKey);
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
            UserInfo = null;
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
                TaskListBox.SelectedItem = curr.Title;
                MessageTextBox.Text = TaskPair[curr.Title].Message;
            }
            else
                MessageTextBox.Text = "";
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
                    MessageTextBox.Text, new Action<CommentTask>((tsk) =>
                    {
                        TaskList.Remove(tsk);
                        TaskPair.Remove(tsk.BangumiInfo.Title);
                        this.Invoke(() => TaskListBox.Items.Remove(tsk.BangumiInfo.Title));
                    }));
                TaskList.Add(task);
                TaskPair.Add(BangumiListBox.SelectedItem as string, task);
                TaskListBox.Items.Add(task.BangumiInfo.Title);
            }
        }
        private void Remove(object sender, RoutedEventArgs e)
        {
            if (TaskListBox.Items.Count == 0)
                return;
            var title = TaskListBox.SelectedItem as string;
            var tsk = TaskPair[title];
            tsk.Dispose();
            TaskList.Remove(tsk);
            TaskPair.Remove(tsk.BangumiInfo.Title);
            TaskListBox.Items.Remove(tsk.BangumiInfo.Title);
        }
    }
}
