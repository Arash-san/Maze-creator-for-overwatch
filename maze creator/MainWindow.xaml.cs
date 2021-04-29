using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ModernWpf;
using ModernWpf.Controls;


namespace maze_creator
{
    public class DataItem
    {
        public int num { get; set; }
        public string text { get; set; }

    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<int> minutes { get; } = new List<int>()
        {

        };
        const string title = "Maze creator for overwatch";
        int type = 0;
        int flag = 0, count;
        int num_row = 29, num_col = 29, gridlen = 20, borderSize = 11;
        string stLinePos = "", finLinPos = "", colorReturn;
        List<StackPanel> recs = new List<StackPanel>();
        List<string> meiPos = new List<string>();
        List<DataItem> story = new List<DataItem>();
        List<string> toBeSaved = new List<string>();
        List<string> toBeLoaded = new List<string>();
        Border stLine, finLine;
        public static MainWindow reff;
        public bool isMazeChanged = false;


        public MainWindow()
        {

            InitializeComponent();
            this.Title = title;
            if (ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark)
            {
                darkModeToggle.IsOn = true;
            }
            else
            {
                darkModeToggle.IsOn = false;
            }
            var data = new DataItem { num = 1, text = "Find a way out!" };
            story.Add(data);
            dataGrid.ItemsSource = story;


            for (int i = 0; i < num_row; i++)
            {
                RowDefinition row = new RowDefinition
                {
                    Height = new GridLength(gridlen),
                };
                ColumnDefinition col = new ColumnDefinition
                {
                    Width = new GridLength(gridlen),
                };
                mainGrid.RowDefinitions.Add(row);
                mainGrid.ColumnDefinitions.Add(col);


            }
            reff = this;
            for (int i = 0; i < num_row; i++)
            {
                for (int j = 0; j < num_col; j++)
                {
                    Border border = new Border
                    {

                        Width = borderSize,
                        Height = borderSize,
                        CornerRadius = new CornerRadius(50),
                        BorderThickness = new Thickness(1),
                        Tag = (num_row - 1 - j) * 4 + "," + (num_row - 1 - i) * 4
                    };

                    border.SetResourceReference(Border.BackgroundProperty, "SystemControlBackgroundAltHighBrush");
                    border.SetResourceReference(Border.BorderBrushProperty, "SystemControlBackgroundBaseMediumLowBrush");


                    mainGrid.Children.Add(border);
                    Grid.SetRow(border, i);
                    Grid.SetColumn(border, j);
                    border.MouseLeftButtonUp += new MouseButtonEventHandler(clickOnMeSenpai);
                }
            }
            stLine = new Border
            {
                Width = 17,
                Height = 17,
                CornerRadius = new CornerRadius(50),
                Background = Brushes.MediumSeaGreen
            };
            mainGrid.Children.Add(stLine);
            Grid.SetRow(stLine, num_row - 2);
            Grid.SetColumn(stLine, num_col - 2);

            finLine = new Border
            {
                Width = 17,
                Height = 17,
                CornerRadius = new CornerRadius(50),
                Background = Brushes.DodgerBlue
            };
            mainGrid.Children.Add(finLine);
            Grid.SetRow(finLine, 1);
            Grid.SetColumn(finLine, 1);

            stLinePos = "(4,0,4)";
            finLinPos = $"({(num_col - 2) * 4},0,{(num_row - 2) * 4})";

            colorReturn = "(0,200,255,255)";

            Color tempColor = new Color();
            tempColor.R = 0;
            tempColor.G = 200;
            tempColor.B = 255;
            tempColor.A = 255;
            colorPrew.Background = new SolidColorBrush(tempColor);

            for (int j = 0; j < 60; j++)
            {
                minutes.Add(j);
            }
            combo1.ItemsSource = minutes;
            combo2.ItemsSource = minutes;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Delete the last wall
            try
            {
                mainGrid.Children.Remove(recs[recs.Count - 1]);
                recs.RemoveAt(recs.Count - 1);
                meiPos.RemoveAt(meiPos.Count - 1);
                countButton.Label = "Count: " + --count;
            } catch (Exception) { }

        }



        private async void copyButton_Click(object sender, RoutedEventArgs e)
        {
            string first = "actions\n{\n\tGlobal.meiPos = Array(", second = "", final = "";

            for (int i = 0; i < meiPos.Count; i++)
            {
                second += "Vector" + meiPos[i];
                if (i != meiPos.Count - 1)
                    second += ",";
            }
            final = first + second + ");\n";
            final += $"\tGlobal.workshopFlashing = Workshop Setting Toggle(Custom String(\"Settings\"), Custom String(\"Enable constant flashing to black\"), {(flashChkbox.IsChecked == true ? "True" : "False")}, 0);\n";
            final += $"\tGlobal.enableStory = Workshop Setting Toggle(Custom String(\"Settings\"), Custom String(\"Enable story\"),  {(storyChkbox.IsChecked == true ? "True" : "False")}, 1);\n";
            final += $"\tGlobal.isEffectEnabled = Workshop Setting Toggle(Custom String(\"Settings\"), Custom String(\"Effects in the vicinity of the player\"), {(effectChkbox.IsChecked == true ? "True" : "False")}, 2);\n";
            final += $"\tGlobal.isLineEnabled = Workshop Setting Toggle(Custom String(\"Settings\"), Custom String(\"Draw lines on the ground\"), {(effectChkbox.IsChecked == true ? "True" : "False")}, 2);\n";
            final += $"\tGlobal.colorOfEffect = Custom Color{colorReturn};\n";
            final += $"\tGlobal.gameLength = {combo1.SelectedIndex * 60 + combo2.SelectedIndex};\n";
            final += "\tGlobal.texts = Empty Array;\n";
            foreach (var item in story)
            {
                final += $"\tGlobal.texts = Append To Array(Global.texts, Custom String(\"{item.text}\"));\n";
            }
            final += $"\tGlobal.otherPos[2] = Vector{stLinePos};\n";
            final += $"\tGlobal.finishPos = Vector{finLinPos};\n";
            final += "\n}";
            Clipboard.SetText(final);
            MyMessage msg = new MyMessage();
            var result = await msg.ShowAsync();

        }


        private void effectChkbox_Click(object sender, RoutedEventArgs e)
        {
            if (effectChkbox.IsChecked == true)
            {
                chooseColorButton.IsEnabled = true;
            }
            else
            {
                chooseColorButton.IsEnabled = false;
            }

        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //Add button
            var data = new DataItem { num = story.Count + 1, text = "" };
            story.Add(data);
            dataGrid.Items.Refresh();
            isMazeChanged = true;
        }

        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            //Delete button
            try
            {
                int selIndex = dataGrid.SelectedIndex;
                story.RemoveAt(selIndex);
                for (int i = selIndex; i < story.Count; i++)
                {
                    story[i].num = i + 1;
                }
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = story;
                isMazeChanged = true;

            } catch (Exception)
            {

            }

        }

        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (tabControl.SelectedIndex == 1)
            {

                if (e.Key == Key.Delete)
                {
                    delTextButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

                }

            }
        }
        public void saveButton_Click(object sender, RoutedEventArgs e)
        {


            try
            {

                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.DefaultExt = ".maze";
                dlg.Filter = "Maze Files (*.maze)|*.maze|All files|*.*";
                Nullable<bool> result = dlg.ShowDialog();
                string filename = "", temp = "", vector = "";
                if (result == true)
                {
                    toBeSaved.RemoveRange(0, toBeSaved.Count);
                    filename = dlg.FileName;
                    //MessageBox.Show(filename);
                    for (int i = 0; i < recs.Count; i++)
                    {
                        temp = (string)recs[i].Name;
                        string[] data = temp.Split('_');
                        vector += data[1] + "," + data[2] + "," + data[3];
                        if (i != recs.Count - 1)
                            vector += "_";
                    }
                    toBeSaved.Add(vector);
                    toBeSaved.Add("StartLine_" + stLinePos);
                    toBeSaved.Add("FinishLine_" + finLinPos);

                    toBeSaved.Add("flashing_" + flashChkbox.IsChecked);
                    toBeSaved.Add("story_" + storyChkbox.IsChecked);
                    toBeSaved.Add("effect_" + effectChkbox.IsChecked);
                    toBeSaved.Add("lineOnTheGround_" + lineChkbox.IsChecked);
                    toBeSaved.Add("effectColor_" + colorReturn);
                    toBeSaved.Add("gameLength_" + (combo1.SelectedIndex * 60 + combo2.SelectedIndex));
                    toBeSaved.Add("##AYAYA##");
                    foreach (var item in story)
                    {
                        toBeSaved.Add(item.text);
                    }

                    TextWriter tw = new StreamWriter(filename, false);

                    foreach (String s in toBeSaved)
                        tw.WriteLine(s);

                    tw.Close();
                    this.Title = title + $" ({Path.GetFileName(filename)})";
                    isMazeChanged = false;

                }
            } catch (Exception)
            {
                new ContentDialog()
                {
                    Title = "Failed to save the file",
                    Content = "Maybe another application or proccess is using this file. try to close them to solve the problem",
                    PrimaryButtonText = "OK",
                    DefaultButton = ContentDialogButton.Primary
                }.ShowAsync();
            }

        }

        private async void openButton_Click(object sender, RoutedEventArgs e)
        {
            if (isMazeChanged)
            {
                saveTheProjectFfs msg = new saveTheProjectFfs();
                var result = await msg.ShowAsync();
                if (result == ModernWpf.Controls.ContentDialogResult.Primary)
                {
                    reff.saveButton_Click(null, null);

                }
                else if (result == ModernWpf.Controls.ContentDialogResult.Secondary)
                {

                }
                else
                {
                    return;
                }


            }
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".maze";
                dlg.Filter = "Maze Files (*.maze)|*.maze|All files|*.*";
                Nullable<bool> result = dlg.ShowDialog();
                string filename = "", temp = "";
                if (result == true)
                {

                    toBeLoaded.RemoveRange(0, toBeLoaded.Count);
                    filename = dlg.FileName;
                    var lines = File.ReadLines(filename);
                    foreach (var line in lines)
                        toBeLoaded.Add(line);
                    if (toBeLoaded[9] != "##AYAYA##")
                    {
                        throw new FileFormatException();
                    }
                    for (int i = 0; i < recs.Count; i++)
                    {
                        mainGrid.Children.Remove(recs[i]);
                    }
                    recs.RemoveRange(0, recs.Count);
                    meiPos.RemoveRange(0, meiPos.Count);

                    string[] vectors = toBeLoaded[0].Split('_');
                    int backFlag = flag, backType = type; type = 0;
                    foreach (var item in vectors)
                    {
                        string[] vector = item.Split(',');
                        flag = Int32.Parse(vector[2]);
                        var currentBorder = mainGrid.Children[Int32.Parse(vector[0]) * num_row + Int32.Parse(vector[1])] as Border;
                        clickOnMeSenpai(currentBorder, null);
                    }
                    flag = backFlag; type = backType;
                    ////////////////////////////////////
                    temp = toBeLoaded[1].Split('_')[1];
                    stLinePos = temp;
                    temp = temp.Substring(1, temp.Length - 2);
                    string[] nums = temp.Split(',');
                    int value1 = num_row - Int32.Parse(nums[0]) / 4;

                    int value2 = num_col - Int32.Parse(nums[2]) / 4;
                    Grid.SetRow(stLine, value2 - 1);
                    Grid.SetColumn(stLine, value1 - 1);
                    /////////////////////////////////////////////
                    temp = toBeLoaded[2].Split('_')[1];
                    finLinPos = temp;
                    temp = temp.Substring(1, temp.Length - 2);
                    nums = temp.Split(',');
                    value1 = num_row - Int32.Parse(nums[0]) / 4;
                    value2 = num_col - Int32.Parse(nums[2]) / 4;
                    Grid.SetRow(finLine, value2 - 1);
                    Grid.SetColumn(finLine, value1 - 1);
                    /////////////////////////////////////////////
                    flashChkbox.IsChecked = toBeLoaded[3].Split('_')[1] == "True" ? true : false;
                    storyChkbox.IsChecked = toBeLoaded[4].Split('_')[1] == "True" ? true : false;
                    effectChkbox.IsChecked = toBeLoaded[5].Split('_')[1] == "True" ? true : false;
                    lineChkbox.IsChecked = toBeLoaded[6].Split('_')[1] == "True" ? true : false;
                    //////////////////////////f///////////////////
                    temp = toBeLoaded[7].Split('_')[1];
                    colorReturn = temp;
                    temp = temp.Substring(1, temp.Length - 2);
                    nums = temp.Split(',');
                    Color tempCol = new Color();
                    tempCol.R = Byte.Parse(nums[0]); tempCol.G = Byte.Parse(nums[1]); tempCol.B = Byte.Parse(nums[2]);
                    tempCol.A = Byte.Parse(nums[3]);
                    colorPrew.Background = new SolidColorBrush(tempCol);
                    /////////////////////////////////////////////
                    temp = toBeLoaded[8].Split('_')[1];
                    combo1.SelectedIndex = Int32.Parse(temp) / 60;
                    combo2.SelectedIndex = Int32.Parse(temp) % 60;
                    /////////////////////////////////////////////
                    story.RemoveRange(0, story.Count);
                    DataItem something = new DataItem();
                    for (int i = 10; i < toBeLoaded.Count; i++)
                    {
                        story.Add(new DataItem { num = i - 9, text = toBeLoaded[i] });
                    }
                    dataGrid.Items.Refresh();
                    countButton.Label = "Count: " + recs.Count;
                    count = recs.Count;
                    this.Title = title + $" ({Path.GetFileName(filename)})";
                    isMazeChanged = false;
                }
            } catch (Exception)
            {
                await new ContentDialog()
                {
                    Title = "Invalid file",
                    Content = "We cannot open this file because this file isn't a maze project or modified in a wrong way",
                    PrimaryButtonText = "OK",
                    DefaultButton = ContentDialogButton.Primary
                }.ShowAsync();

            }

        }

        private void chkBox_Click(object sender, RoutedEventArgs e)
        {
            if (chkBox.IsChecked == true)
            {
                flag = 0;
            }
            else
            {
                flag = 1;
            }
        }

        private void link1_Click(object sender, RoutedEventArgs e)
        {
            //About
            string url = "";
            string urlType = (string)((Button)sender).Tag;
            if (urlType == "Twitter")
            {
                url = "https://twitter.com/user_arash";
            }
            else if (urlType == "Instagram")
            {
                url = "https://www.instagram.com/theyorha/";
            }
            if (urlType == "Youtube")
            {
                url = "https://www.youtube.com/channel/UC8zytkOHzujwp6eYvLkAqvg";
            }
            else if (urlType == "Github")
            {
                url = "https://github.com/arash-san";
            }

            System.Diagnostics.Process.Start("explorer.exe", url);

        }



        private void startLine_Checked(object sender, RoutedEventArgs e)
        {
            finsihLine.IsChecked = false;
            if (startLine.IsChecked == true)
            {
                type = 1;
            }
            else
            {
                type = 0;
            }
        }

        private async void foo_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            if (isMazeChanged)
            {
                saveTheProjectFfs msg = new saveTheProjectFfs();
                var result = await msg.ShowAsync();
                if (result == ModernWpf.Controls.ContentDialogResult.Primary)
                {
                    reff.saveButton_Click(null, null);
                    System.Windows.Application.Current.Shutdown();
                }
                else if (result == ModernWpf.Controls.ContentDialogResult.Secondary)
                {
                    System.Windows.Application.Current.Shutdown();
                }
            }
            else
            {
                System.Windows.Application.Current.Shutdown();
            }



        }

        private void finsihLine_Checked(object sender, RoutedEventArgs e)
        {
            startLine.IsChecked = false;
            if (finsihLine.IsChecked == true)
            {
                type = 2;
            }
            else
            {
                type = 0;
            }
        }


        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (darkModeToggle.IsOn)
            {

                ThemeManager.SetRequestedTheme(this, ElementTheme.Dark);
            }
            else
            {

                ThemeManager.SetRequestedTheme(this, ElementTheme.Light);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            //String res = ColorPickers.showWindow();
            colorReturn = String.Format("({0},{1},{2},{3})", mainColor.SelectedColor.R, mainColor.SelectedColor.G, mainColor.SelectedColor.B, mainColor.SelectedColor.A);

            if (colorReturn != "")
            {
                colorPrew.Background = new SolidColorBrush(mainColor.SelectedColor);
                isMazeChanged = true;
            }

            Flyout f = FlyoutService.GetFlyout(chooseColorButton) as Flyout;
            if (f != null)
            {
                f.Hide();
            }
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (tabControl.SelectedIndex == 0)
            {
                if (e.Key == Key.D)
                    delButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                if (e.Key == Key.V)
                {
                    if (chkBox.IsChecked == true)
                        chkBox.IsChecked = false;
                    else
                        chkBox.IsChecked = true;
                }
            }
            else if (tabControl.SelectedIndex == 1)
            {

                if (e.Key == Key.Insert)
                {
                    addTextButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
            }



        }

        private bool delDuplicate(string Data)
        {
            for (int i = 0; i < recs.Count; i++)
            {
                //MessageBox.Show($"{(string)recs[i].Tag} == {Data}");
                if ((string)recs[i].Tag == Data)
                {
                    return true;
                }
            }
            return false;

        }

        private void removeSelf(object sender, MouseButtonEventArgs e)
        {
            StackPanel item = (StackPanel)sender;
            int index = recs.IndexOf(item);
            mainGrid.Children.Remove(item);
            recs.Remove(item);
            meiPos.RemoveAt(index);
            countButton.Label = "Count: " + --count;
            isMazeChanged = true;
        }

        private void clickOnMeSenpai(object sender, MouseButtonEventArgs e)
        {
            string Data = (string)((Border)sender).Tag;
            String[] infos = Data.Split(',');
            int value1 = num_row - Int32.Parse(infos[0]) / 4;
            int value2 = num_col - Int32.Parse(infos[1]) / 4;
            if (delDuplicate(Data))
            {
                return;
            }
            if (type == 1)
            {
                Grid.SetRow(stLine, value2 - 1);
                Grid.SetColumn(stLine, value1 - 1);
                stLinePos = "(" + (Int32.Parse(infos[0])) + ",0," + (Int32.Parse(infos[1])) + ")";
            }
            else if (type == 2)
            {
                Grid.SetRow(finLine, value2 - 1);
                Grid.SetColumn(finLine, value1 - 1);
                finLinPos = "(" + (Int32.Parse(infos[0])) + ",0," + (Int32.Parse(infos[1])) + ")";
            }
            else if (type == 0)
            {
                StackPanel rec = new StackPanel
                {
                    Name = "n_" + (value2 - 1) + "_" + (value1 - 1) + "_" + flag,
                    Width = (gridlen * 2) - 2,
                    Height = 6,
                    Background = (SolidColorBrush)Application.Current.Resources["SystemAccentColorLight1Brush"],
                    Tag = infos[0] + "," + infos[1]
                };
                rec.MouseLeftButtonUp += new MouseButtonEventHandler(removeSelf);
                meiPos.Add("(" + (Int32.Parse(infos[0])) + "," + flag + "," + (Int32.Parse(infos[1])) + ")");
                //posTextBlock.Text = "(" + (Int32.Parse(infos[0])) + "," + flag + "," + (Int32.Parse(infos[1])) + ")";
                try
                {
                    mainGrid.Children.Add(rec);
                    if (flag == 0)
                    {
                        //MessageBox.Show((value2)+","+ (value1));
                        Grid.SetRow(rec, value2 - 1);
                        Grid.SetColumn(rec, value1 - 2);
                        Grid.SetColumnSpan(rec, 3);

                    }
                    else if (flag == 1)
                    {
                        rec.Width = 6;
                        rec.Height = (gridlen * 2) - 2;
                        Grid.SetRow(rec, value2 - 2);
                        Grid.SetColumn(rec, value1 - 1);
                        Grid.SetRowSpan(rec, 3);

                    }
                    recs.Add(rec);
                    countButton.Label = "Count: " + ++count;
                    isMazeChanged = true;
                } catch (Exception)
                {
                    mainGrid.Children.Remove(rec);
                }
            }

        }

    }




}
