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
using System.Windows.Threading;

namespace ConferenceOrganizers
{
    /// <summary>
    /// Логика взаимодействия для ParticipantsWindow.xaml
    /// </summary>
    public partial class ParticipantsWindow : Window
    {
        MainContext context = new MainContext();
        public ParticipantsWindow()
        {
            InitializeComponent();

            RefreshData();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(30);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            RefreshData();
        }
        private void RefreshData()
        {
            var participants = context.Participants.ToList();
            GridAdmin.ItemsSource = participants.Select(p => new
            {
                Id = p.Id,
                FIO = $"{p.Name} {p.Surname} {p.Patronimic}",
                Email = p.Email,
                Phone = p.Phone,
                CountryId = p.CountryId,
                Birthday = p.Birthday.ToString("dd.MM.yyyy"),
            }).ToList();
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы точно хотите удалить участника?", "Подтверждение", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var selectedParticipant = (dynamic)GridAdmin.SelectedItem;
                    if (selectedParticipant != null)
                    {
                        var participant = context.Participants.Find(selectedParticipant.Id);
                        if (participant != null)
                        {
                            context.Participants.Remove(participant);
                            context.SaveChanges();
                            RefreshData();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Нельзя удалять победителя!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
