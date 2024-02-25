using ConferenceOrganizers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Логика взаимодействия для ModeratorProfil.xaml
    /// </summary>
    public partial class ModeratorProfil : Window
    {
        MainContext context = new MainContext();
        int moderatorId = 0;
        public ModeratorProfil(Moderator moderator)
        {
            InitializeComponent();
            LoadEvents();
            moderatorId = moderator.Id;


            DateTime dateTime = DateTime.Now;
            if (dateTime.Hour >= 9 && (dateTime.Hour < 11 || (dateTime.Hour <= 11 && dateTime.Minute == 0)))
            {
                Greeting.Text = $"Доброе утро!\n{moderator.Name} {moderator.Patronimic}";
            }
            else if ((dateTime.Hour >= 11 && dateTime.Minute == 1) && (dateTime.Hour < 18 || (dateTime.Hour <= 18 && dateTime.Minute == 0)))
            {
                Greeting.Text = $"Добрый день!\n{moderator.Name} {moderator.Patronimic}";
            }
            else
            {
                Greeting.Text = $"Добрый день!\n{moderator.Name} {moderator.Patronimic}";
            }


            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(moderator.Photo, UriKind.RelativeOrAbsolute);
            bitmap.EndInit();

            mainImage.Source = bitmap;
        }
        private void LoadEvents()
        {
            using (var context = new MainContext())
            {
                var events = context.Events.ToList();
                foreach (var eventes in events)
                {
                    EventModerator.Items.Add(new ComboBoxItem { Content = eventes.Name, Tag = eventes.Id });
                }
            }
        }

        private void EventModerator_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedEvent = (ComboBoxItem)EventModerator.SelectedItem;
            if (selectedEvent != null)
            {
                int eventId = (int)selectedEvent.Tag;

                LoadActivitiesForEvent(eventId);
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите мероприятие", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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

                ActivityModerator.Items.Clear();

                foreach (var activity in activities)
                {
                    ActivityModerator.Items.Add(new ComboBoxItem { Content = activity.Name, Tag = activity.Id });
                }
            }
        }

        private void ActivityModerator_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedActivity = (ComboBoxItem)ActivityModerator.SelectedItem;
            if (selectedActivity != null)
            {
                int activityId = (int)selectedActivity.Tag;
                string activityName = selectedActivity.Content.ToString();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите активность", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedEvent = (ComboBoxItem)EventModerator.SelectedItem;
            var selectedActivity = (ComboBoxItem)ActivityModerator.SelectedItem;

            if (selectedEvent != null && selectedActivity != null)
            {
                int eventId = (int)selectedEvent.Tag;
                int activityId = (int)selectedActivity.Tag;

        using (var context = new MainContext())
                {
                    var eventModerator = new EventModerator
                    {
                        EventId = eventId,
                        ModeratorId = moderatorId
                    };

                    var activityModerator = new ActivityModerator
                    {
                        ActivityId = activityId,
                        ModeratorId = moderatorId
                    };

                    context.EventModerators.Add(eventModerator);
                    context.ActivityModerators.Add(activityModerator);

                    context.SaveChanges();
                }

                MessageBox.Show("Регистрация успешно завершена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите мероприятие и активность", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            AppSettings.Default.userId = 0;
            AppSettings.Default.userPassword = null;
            AppSettings.Default.role = null;
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void OpenCatalog_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
