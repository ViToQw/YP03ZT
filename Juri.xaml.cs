using ConferenceOrganizers.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ConferenceOrganizers
{
    /// <summary>
    /// Логика взаимодействия для Juri.xaml
    /// </summary>
    public partial class Juri : Window
    {
        MainContext context = new MainContext();
        public Juri()
        {
            InitializeComponent();
            GenerateId();
            LoadCourses();
            LoadEvents();

            Event.IsEnabled = false;
            Activity.IsEnabled = false;
            OkButton.IsEnabled = false;
        }

        private void GenerateId()
        {
            Random random = new Random();
            int id;
            do
            {
                id = random.Next(100000, 999999);
            }
            while (CheckIdExists(id));

            IdNumber.Text = id.ToString();
        }

        private bool CheckIdExists(int id)
        {
            List<int> juryIds = GetJuryIds();
            List<int> moderatorIds = GetModeratorIds();

            // Проверка, существует ли ID в этих списках
            if (juryIds.Contains(id) || moderatorIds.Contains(id))
            {
                return true;
            }

            return false;
        }

        private List<int> GetJuryIds()
        {
            using (var context = new MainContext())
            {
                return context.Juries.Select(j => j.Id).ToList();
            }
        }

        private List<int> GetModeratorIds()
        {
            using (var context = new MainContext())
            {
                return context.Moderators.Select(m => m.Id).ToList();
            }
        }

        private void Gender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Role_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void LoadCourses()
        {
            using (var context = new MainContext())
            {
                var courses = context.Courses.ToList();
                foreach (var course in courses)
                {
                    Course.Items.Add(new ComboBoxItem { Content = course.Name, Tag = course.Id });
                }
            }
        }
        private void Course_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCourse = (ComboBoxItem)Course.SelectedItem;
            if (selectedCourse != null)
            {
                int courseId = (int)selectedCourse.Tag;
                string courseName = selectedCourse.Content.ToString();
            }
            else
            {
                
            }
        }

        private void LoadEvents()
        {
            using (var context = new MainContext())
            {
                var events = context.Events.ToList();
                foreach (var eventes in events)
                {
                    Event.Items.Add(new ComboBoxItem { Content = eventes.Name, Tag = eventes.Id });
                }
            }
        }

        private void Event_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedEvent = (ComboBoxItem)Event.SelectedItem;
            if (selectedEvent != null)
            {
                int eventId = (int)selectedEvent.Tag;

                // Загрузка активностей для выбранного мероприятия
                LoadActivitiesForEvent(eventId);
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите мероприятие", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Зарегистрировать нового жюри/модератора?", "Сообщение", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);
            if (result == MessageBoxResult.Yes)
            {
                var selectedRole = (ComboBoxItem)Role.SelectedItem;
                if (selectedRole != null)
                {
                    string roleName = selectedRole.Content.ToString();

                    switch (roleName)
                    {
                        case "Жюри":
                            string[] splitFIO = FIO.Text.Split(' ');
                            if (splitFIO.Length == 3 && !string.IsNullOrWhiteSpace(IdNumber.Text) && !string.IsNullOrWhiteSpace(Email.Text) &&
                                !string.IsNullOrWhiteSpace(Phone.Text) && mainImage.Source != null && Gender.SelectedItem != null &&
                                (PasswordCheckBox.IsChecked == true ? !string.IsNullOrWhiteSpace(VisiblePassword.Text) : !string.IsNullOrWhiteSpace(HiddenPassword.Password)))
                            {
                                ComboBoxItem selectedCourseItem = Course.SelectedItem as ComboBoxItem;

                                string courseName = selectedCourseItem != null ? selectedCourseItem.Content.ToString() : Course.Text;

                                int courseId;

                                using (var context = new MainContext())
                                {
                                    var course = context.Courses.FirstOrDefault(c => c.Name == courseName);
                                    if (course == null)
                                    {
                                        // Курса нет в базе данных, создаем новый
                                        course = new Course { Name = courseName };
                                        context.Courses.Add(course);
                                        context.SaveChanges();
                                    }

                                    courseId = course.Id;
                                }

                                Jury jury = new Jury
                                {
                                    Id = int.Parse(IdNumber.Text),
                                    Surname = splitFIO[0],
                                    Name = splitFIO[1],
                                    Patronimic = splitFIO[2],
                                    CourseId = courseId,
                                    CountryId = 643,
                                    Email = Email.Text,
                                    Password = PasswordCheckBox.IsChecked == true ? VisiblePassword.Text : HiddenPassword.Password,
                                    Phone = Phone.Text,
                                    Photo = mainImage.Source.ToString(),
                                    Gender = ((ComboBoxItem)Gender.SelectedItem).Content.ToString(),
                                    Birthday = DateTime.Now,
                                };
                                context.Database.OpenConnection();
                                int juryId;
                                try
                                {
                                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Juries] ON");
                                    context.Juries.AddRange(jury);
                                    context.SaveChanges();
                                    juryId = jury.Id;
                                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Juries] OFF");
                                }
                                finally
                                {
                                    context.Database.CloseConnection();
                                }

                                var selectedActivity = (ComboBoxItem)Activity.SelectedItem;
                                if (selectedActivity != null)
                                {
                                    int activityId = (int)selectedActivity.Tag;

                                    var activityJury = new ActivityJury
                                    {
                                        ActivityId = activityId,
                                        JuryId = juryId
                                    };

                                    context.ActivityJuries.Add(activityJury);
                                    context.SaveChanges();
                                }

                                MessageBox.Show("Жюри успешно добавлен!", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("Пожалуйста, введите ФИО в формате 'Фамилия Имя Отчество' и пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            break;
                        case "Модератор":
                            string[] splitFIOMod = FIO.Text.Split(' ');
                            if (splitFIOMod.Length == 3 && !string.IsNullOrWhiteSpace(IdNumber.Text) && !string.IsNullOrWhiteSpace(Email.Text) &&
                                !string.IsNullOrWhiteSpace(Phone.Text) && mainImage.Source != null && Gender.SelectedItem != null &&
                                (PasswordCheckBox.IsChecked == true ? !string.IsNullOrWhiteSpace(VisiblePassword.Text) : !string.IsNullOrWhiteSpace(HiddenPassword.Password)))
                            {
                                ComboBoxItem selectedCourseItem = Course.SelectedItem as ComboBoxItem;

                                string courseName = selectedCourseItem != null ? selectedCourseItem.Content.ToString() : Course.Text;

                                int courseId;

                                using (var context = new MainContext())
                                {
                                    var course = context.Courses.FirstOrDefault(c => c.Name == courseName);
                                    if (course == null)
                                    {
                                        // Курса нет в базе данных, создаем новый
                                        course = new Course { Name = courseName };
                                        context.Courses.Add(course);
                                        context.SaveChanges();
                                    }

                                    courseId = course.Id;
                                }
                                int moderatorId;
                                Moderator moderator = new Moderator // Запись в БД Модератор
                                {
                                    Id = int.Parse(IdNumber.Text),
                                    Surname = splitFIOMod[0],
                                    Name = splitFIOMod[1],
                                    Patronimic = splitFIOMod[2],
                                    CourseId = courseId,
                                    CountryId = 643,
                                    Email = Email.Text,
                                    Password = PasswordCheckBox.IsChecked == true ? VisiblePassword.Text : HiddenPassword.Password,
                                    Phone = Phone.Text,
                                    Photo = mainImage.Source.ToString(),
                                    Gender = ((ComboBoxItem)Gender.SelectedItem).Content.ToString(),
                                    Birthday = null,
                                };
                                context.Database.OpenConnection();
                                try
                                {
                                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Moderators] ON");
                                    context.Moderators.AddRange(moderator);
                                    context.SaveChanges();
                                    moderatorId = moderator.Id;
                                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Moderators] OFF");
                                }
                                finally
                                {
                                    context.Database.CloseConnection();
                                }

                                var selectedEvent = (ComboBoxItem)Event.SelectedItem; // Запись в БД Мероприятия
                                if (selectedEvent != null) 
                                {
                                    int eventId = (int)selectedEvent.Tag;

                                    var eventModerator = new EventModerator
                                    {
                                        EventId = eventId,
                                        ModeratorId = moderatorId
                                    };

                                    context.EventModerators.Add(eventModerator);
                                    context.SaveChanges();
                                }

                                var selectedActivity = (ComboBoxItem)Activity.SelectedItem; // Запись в БД Активности
                                if (selectedActivity != null)
                                {
                                    int activityId = (int)selectedActivity.Tag;

                                    var activityModerator = new ActivityModerator
                                    {
                                        ActivityId = activityId,
                                        ModeratorId = moderatorId
                                    };

                                    context.ActivityModerators.Add(activityModerator);
                                    context.SaveChanges();
                                }

                                MessageBox.Show("Модератор успешно добавлен!", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("Пожалуйста, введите ФИО в формате 'Фамилия Имя Отчество' и пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            break;
                        default:
                            MessageBox.Show("Выбрана неизвестная роль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите роль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void PhotoButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "All Files (*.*)|*.*|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = System.IO.Path.GetFileNameWithoutExtension(dlg.FileName);
                string extension = System.IO.Path.GetExtension(dlg.FileName);
                string file = filename + "_" + DateTime.Now.Ticks + extension;
                string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image\\");

                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }

                System.IO.File.Copy(dlg.FileName.Replace("\\", "/"), path + file);

                mainImage.Source = new BitmapImage(new Uri(path + file));
            }
        }

        private void LoadActivitiesForEvent(int eventId)
        {
            using (var context = new MainContext())
            {
                var activities = context.ActivityEvents
                    .Where(ae => ae.EventId == eventId)
                    .Select(ae => ae.Activity)
                    .ToList();

                Activity.Items.Clear();

                foreach (var activity in activities)
                {
                    Activity.Items.Add(new ComboBoxItem { Content = activity.Name, Tag = activity.Id });
                }
            }
        }

        private void LoadActivities()
        {
            using (var context = new MainContext())
            {
                var activities = context.Activities.ToList();
                foreach (var activity in activities)
                {
                    Activity.Items.Add(new ComboBoxItem { Content = activity.Name, Tag = activity.Id });
                }
            }
        }

        private void Activity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedActivity = (ComboBoxItem)Activity.SelectedItem;
            if (selectedActivity != null)
            {
                int activityId = (int)selectedActivity.Tag;
                string activityName = selectedActivity.Content.ToString();
            }
            else
            {
                
            }
        }

        private void IsEvent_Checked(object sender, RoutedEventArgs e)
        {
            Event.IsEnabled = true;
            Activity.IsEnabled = true;
            EventNameText.Foreground = Brushes.Black;
        }

        private void IsEvent_Unchecked(object sender, RoutedEventArgs e)
        {
            Event.IsEnabled = false;
            Activity.IsEnabled = false;
            EventNameText.Foreground = Brushes.Gray;
        }

        private void Phone_LostFocus(object sender, RoutedEventArgs e)
        {
            var phoneTextBox = sender as TextBox;
            if (phoneTextBox != null)
            {
                var phoneText = phoneTextBox.Text;
                var phoneRegex = new Regex(@"^\+7\(\d{3}\)-\d{3}-\d{2}-\d{2}$");
                if (!phoneRegex.IsMatch(phoneText))
                {
                    MessageBox.Show("Неверный формат телефонного номера. Пожалуйста, используйте формат +7(999)-099-90-90", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void PasswordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            PasswordTB.Foreground = Brushes.Black;

            VisiblePassword.Text = HiddenPassword.Password;
            VisiblePassword.Visibility = Visibility.Visible;
            HiddenPassword.Visibility = Visibility.Collapsed;

            VisibleBorder.Visibility = Visibility.Visible;
            HiddenBorder.Visibility = Visibility.Collapsed;

            VisibleRePassword.Text = HiddenPassword.Password;
            VisibleRePassword.Visibility = Visibility.Visible;
            HiddenRePassword.Visibility = Visibility.Collapsed;

            VisibleReBorder.Visibility = Visibility.Visible;
            HiddenReBorder.Visibility = Visibility.Collapsed;
        }

        private void PasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            PasswordTB.Foreground = Brushes.Gray;

            HiddenPassword.Password = VisiblePassword.Text;
            HiddenPassword.Visibility = Visibility.Visible;
            VisiblePassword.Visibility = Visibility.Collapsed;

            VisibleBorder.Visibility = Visibility.Collapsed;
            HiddenBorder.Visibility = Visibility.Visible;

            HiddenRePassword.Password = VisiblePassword.Text;
            HiddenRePassword.Visibility = Visibility.Visible;
            VisibleRePassword.Visibility = Visibility.Collapsed;

            VisibleReBorder.Visibility = Visibility.Collapsed;
            HiddenReBorder.Visibility = Visibility.Visible;
        }

        private void HiddenPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {
                var passwordText = passwordBox.Password;
                var passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d\s]).{6,}$");
                if (!passwordRegex.IsMatch(passwordText))
                {
                    MessageBox.Show("Пароль не соответствует требованиям. Пожалуйста, убедитесь, что ваш пароль содержит не менее 6 символов, включает заглавные и строчные буквы, не менее одного спецсимвола и не менее одной цифры.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    OkButton.IsEnabled = true;
                }
            }
        }

        private void VisiblePassword_LostFocus(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as TextBox;
            if (passwordBox != null)
            {
                var passwordText = passwordBox.Text;
                var passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d\s]).{6,}$");
                if (!passwordRegex.IsMatch(passwordText))
                {
                    MessageBox.Show("Пароль не соответствует требованиям. Пожалуйста, убедитесь, что ваш пароль содержит не менее 6 символов, включает заглавные и строчные буквы, не менее одного спецсимвола и не менее одной цифры.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    OkButton.IsEnabled = true;
                }
            }
        }

        private void VisibleRePassword_LostFocus(object sender, RoutedEventArgs e)
        {
            var rePasswordBox = sender as TextBox;
            if (rePasswordBox != null)
            {
                var rePasswordText = rePasswordBox.Text;
                if (rePasswordText != VisiblePassword.Text)
                {
                    MessageBox.Show("Пароли не совпадают. Пожалуйста, убедитесь, что ваш пароль и повторный пароль совпадают.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void HiddenRePassword_LostFocus(object sender, RoutedEventArgs e)
        {
            var rePasswordBox = sender as PasswordBox;
            if (rePasswordBox != null)
            {
                var rePasswordText = rePasswordBox.Password;
                if (rePasswordText != HiddenPassword.Password)
                {
                    MessageBox.Show("Пароли не совпадают. Пожалуйста, убедитесь, что ваш пароль и повторный пароль совпадают.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Phone_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 || e.Key == Key.OemPlus || e.Key == Key.OemMinus)
            {

            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
