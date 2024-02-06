﻿using ConferenceOrganizers.Models;
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

            FI.Text = $"{organizer.Surname} {organizer.Name}";

            var currentTime = DateTime.Now.TimeOfDay;
            if (currentTime >= TimeSpan.FromHours(9) && currentTime < TimeSpan.FromHours(11))
            {
                NameTime.Text = "Доброе утро!";
            }
            else if (currentTime >= TimeSpan.FromHours(11.01) && currentTime < TimeSpan.FromHours(18))
            {
                NameTime.Text = "Добрый день!";
            }
            else if (currentTime >= TimeSpan.FromHours(18.01) && currentTime <= TimeSpan.FromHours(24))
            {
                NameTime.Text = "Добрый вечер!";
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
    }
}