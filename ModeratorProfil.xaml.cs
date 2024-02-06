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


            var currentTime = DateTime.Now.TimeOfDay;
            if (currentTime >= TimeSpan.FromHours(9) && currentTime < TimeSpan.FromHours(11))
            {
                Greeting.Text = "Доброе утро!";
            }
            else if (currentTime >= TimeSpan.FromHours(11.01) && currentTime < TimeSpan.FromHours(18))
            {
                Greeting.Text = "Добрый день!";
            }
            else if (currentTime >= TimeSpan.FromHours(18.01) && currentTime <= TimeSpan.FromHours(24))
            {
                Greeting.Text = "Добрый вечер!";
            }
            Greeting.Text += $"\n{moderator.Surname} {moderator.Name}";


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
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }
    }
}
