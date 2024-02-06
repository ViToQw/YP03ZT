using ConferenceOrganizers.Models;
using Microsoft.EntityFrameworkCore;
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
    /// Логика взаимодействия для JuryProfil.xaml
    /// </summary>
    public partial class JuryProfil : Window
    {
        private Jury jury;
        public JuryProfil(Jury jury)
        {
            InitializeComponent();

            this.jury = jury;

            FI.Text = $"{jury.Surname} {jury.Name}";

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
            bitmap.UriSource = new Uri(jury.Photo, UriKind.RelativeOrAbsolute);
            bitmap.EndInit();

            mainImage.Source = bitmap;

            LoadActivities();
        }

        private void LoadActivities()
        {
            using (var context = new MainContext())
            {
                var activityJuries = context.ActivityJuries
                    .Include(aj => aj.Activity)
                    .ThenInclude(a => a.ActivityEvents)
                    .ThenInclude(ae => ae.Event)
                    .Where(aj => aj.JuryId == jury.Id)
                    .ToList();

                var activityObjects = activityJuries.Select(aj => new
                {
                    Id = aj.Activity.Id,
                    Name = aj.Activity.Name,
                    StartTime = aj.Activity.StartTime.ToString("HH:mm"),
                    Day = aj.Activity.Day,
                    EventName = aj.Activity.ActivityEvents.First().Event.Name
                }).ToList();

                GridAdmin.ItemsSource = activityObjects;
            }
        }
    }
}
