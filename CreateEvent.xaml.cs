using ConferenceOrganizers.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// Логика взаимодействия для CreateEvent.xaml
    /// </summary>
    public partial class CreateEvent : Window
    {
        MainContext context = new MainContext();
        public CreateEvent()
        {
            InitializeComponent();

            LoadCities();
            LoadEvents();

            datePicker.DisplayDateStart = DateTime.Now;
        }

        private void datePicker_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 || e.Key >= Key.A && e.Key <= Key.Z)
            {
                e.Handled = true;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Создать новое мероприятие?", "Сообщение", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);
            if (result == MessageBoxResult.Yes)
            {
                DateTime date = datePicker.SelectedDate ?? DateTime.Today;

                DateTime? selectedTime = time.SelectedTime;
                if (!selectedTime.HasValue)
                {
                    MessageBox.Show("Вы не выбрали время", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                DateTime dateTime = new DateTime(date.Year, date.Month, date.Day, selectedTime.Value.Hour, selectedTime.Value.Minute, 0);
                DateTime currentDateTime = DateTime.Now;
                if (dateTime >= currentDateTime)
                {
                    var selectedCity = (string)CityEvent.SelectedItem;
                    int cityId = 0;
                    if (selectedCity != null)
                    {
                        using (var context = new MainContext())
                        {
                            var city = context.Cities.FirstOrDefault(c => c.Name == selectedCity);
                            if (city != null)
                            {
                                cityId = city.Id;
                            }
                        }
                    }

                    int eventId;
                    var eventName = NameEvent.Text;
                    using (var context = new MainContext())
                    {
                        var eventes = context.Events.FirstOrDefault(ev => ev.Name == eventName);
                        if (eventes == null)
                        {
                            if (!string.IsNullOrWhiteSpace(eventName) && date != null && !string.IsNullOrWhiteSpace(DayEvent.Text) && cityId != 0 && mainImage.Source != null)
                            {
                                eventes = new Event
                                {
                                    Name = eventName,
                                    Date = date,
                                    Days = int.Parse(DayEvent.Text),
                                    CityId = cityId,
                                    WinnerId = null,
                                    Image = mainImage.Source.ToString()
                                };
                                context.Events.Add(eventes);
                                context.SaveChanges();
                            }
                            else
                            {
                                MessageBox.Show("Пожалуйста, заполните все поля для мероприятия.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                        }
                        eventId = eventes.Id;
                    }
                

                    int activityId;
                    var activityName = NameAct.Text;
                    if (!string.IsNullOrWhiteSpace(activityName) && dateTime != null && !string.IsNullOrWhiteSpace(DayAct.Text))
                    {
                        Activity activity = new Activity
                        {
                            Name = activityName,
                            StartTime = dateTime,
                            ModeratorId = 1,
                            Day = int.Parse(DayAct.Text),
                        };
                        using (var context = new MainContext())
                        {
                            context.Activities.Add(activity);
                            context.SaveChanges();
                            activityId = activity.Id;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Пожалуйста, заполните все поля для активности.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    ActivityEvent activityEvent = new ActivityEvent
                    {
                        ActivityId = activityId,
                        EventId = eventId
                    };
                    using (var context = new MainContext())
                    {
                        context.ActivityEvents.Add(activityEvent);
                        context.SaveChanges();
                    }

                    MessageBox.Show("Вы создали мероприятие!", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Нельзя ввести прошедшее время!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void LoadCities()
        {
            using (var context = new MainContext())
            {
                var cities = context.Cities.ToList();
                foreach (var city in cities)
                {
                    CityEvent.Items.Add(city.Name);
                }
            }
        }
        private void CityEvent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCity = (string)CityEvent.SelectedItem;
            if (selectedCity != null)
            {
                using (var context = new MainContext())
                {
                    var city = context.Cities.FirstOrDefault(c => c.Name == selectedCity);
                    if (city != null)
                    {
                        int cityId = city.Id;
                        string cityName = city.Name;
                    }
                }
            }
            else
            {
                
            }
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

        private void DayAct_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {

            }
            else
            {
                e.Handled = true;
            }
        }

        private void DayEvent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {

            }
            else
            {
                e.Handled = true;
            }
        }

        private void LoadEvents()
        {
            using (var context = new MainContext())
            {
                var events = context.Events.ToList();
                foreach (var eventes in events)
                {
                    NameEvent.Items.Add(new ComboBoxItem { Content = eventes.Name, Tag = eventes.Id });
                }
            }
        }
        private void NameEvent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedEvent = NameEvent.SelectedItem as string;
            if (selectedEvent != null)
            {
                using (var context = new MainContext())
                {
                    var eventes = context.Events.FirstOrDefault(ev => ev.Name == selectedEvent);
                    if (eventes != null)
                    {

                    }
                }
            }
            else
            {

            }
        }

        private void time_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
        }

        private void time_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space || e.Key == Key.Back)
            {
                e.Handled = true;
            }
        }
    }
}
