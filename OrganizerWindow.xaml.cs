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
    /// Логика взаимодействия для Organizer.xaml
    /// </summary>
    public partial class OrganizerWindow : Window
    {
        public OrganizerWindow(Organizer organizer)
        {
            InitializeComponent();


            DateTime dateTime = DateTime.Now;
            if (dateTime.Hour >= 9 && (dateTime.Hour < 11 || (dateTime.Hour <= 11 && dateTime.Minute == 0)))
            {
                Greeting.Text = $"Доброе утро!\n{organizer.Name} {organizer.Patronimic}";
            }
            else if ((dateTime.Hour >= 11 && dateTime.Minute == 1) && (dateTime.Hour < 18 || (dateTime.Hour <= 18 && dateTime.Minute == 0)))
            {
                Greeting.Text = $"Добрый день!\n{organizer.Name} {organizer.Patronimic}";
            }
            else
            {
                Greeting.Text = $"Добрый день!\n{organizer.Name} {organizer.Surname}";
            }


            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(organizer.Photo, UriKind.RelativeOrAbsolute);
            bitmap.EndInit();

            mainImage.Source = bitmap;
        }

        private void JuryButton_Click(object sender, RoutedEventArgs e)
        {
            Juri juri = new Juri();
            juri.Show();
        }

        private void EventButton_Click(object sender, RoutedEventArgs e)
        {
            CreateEvent createEvent = new CreateEvent();
            createEvent.Show();
        }

        private void ParticipantsButton_Click(object sender, RoutedEventArgs e)
        {
            ParticipantsWindow participantsWindow = new ParticipantsWindow();
            participantsWindow.Show();
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
