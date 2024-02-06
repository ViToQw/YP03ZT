using ConferenceOrganizers.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// Логика взаимодействия для RegParticipantWin.xaml
    /// </summary>
    public partial class RegParticipantWin : Window
    {
        MainContext context = new MainContext();
        public RegParticipantWin()
        {
            InitializeComponent();
            LoadCountry();
            GenerateId();
            OkButton.IsEnabled = false;
        }

        private void GenerateId()
        {
            Random random = new Random();
            int id;
            do
            {
                id = random.Next(1000000, 9999999);
            }
            while (CheckIdExists(id));

            IdNumberText.Text = id.ToString();
        }

        private bool CheckIdExists(int id)
        {
            List<int> participantIds = GetParticipantIds();

            // Проверка, существует ли ID в таблице Participant
            if (participantIds.Contains(id))
            {
                return true;
            }

            return false;
        }

        private List<int> GetParticipantIds()
        {
            using (var context = new MainContext())
            {
                return context.Participants.Select(j => j.Id).ToList();
            }
        }

        private void GenderText_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void PhoneText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 || e.Key == Key.OemPlus || e.Key == Key.OemMinus)
            {

            }
            else
            {
                e.Handled = true;
            }
        }

        private void PhoneText_LostFocus(object sender, RoutedEventArgs e)
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

        private void datePicker_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 || e.Key >= Key.A && e.Key <= Key.Z)
            {
                e.Handled = true;
            }
        }

        private void LoadCountry()
        {
            using (var context = new MainContext())
            {
                var country = context.Countries.ToList();
                foreach (var countries in country)
                {
                    CountryText.Items.Add(new ComboBoxItem { Content = countries.Name, Tag = countries.Id });
                }
            }
        }
        private void CountryText_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCounry = (ComboBoxItem)CountryText.SelectedItem;
            if (selectedCounry != null)
            {
                int countryId = (int)selectedCounry.Tag;
                string countryName = selectedCounry.Content.ToString();
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

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime date = datePicker.SelectedDate ?? DateTime.Today;
            var selectedCountry = ((ComboBoxItem)CountryText.SelectedItem).Content.ToString();
            int countryId = 0;
            if (selectedCountry != null)
            {
                using (var context = new MainContext())
                {
                    var country = context.Countries.FirstOrDefault(c => c.Name == selectedCountry);
                    if (country != null)
                    {
                        countryId = country.Id;
                    }
                }
            }
            string[] splitFIO = FIOText.Text.Split(' ');
            if (splitFIO.Length == 3)
            {
                Participant participant = new Participant
                {
                    Id = int.Parse(IdNumberText.Text),
                    Surname = splitFIO[0],
                    Name = splitFIO[1],
                    Patronimic = splitFIO[2],
                    CountryId = countryId,
                    Email = EmailText.Text,
                    Password = PasswordText.Password,
                    Phone = PhoneText.Text,
                    Photo = mainImage.Source.ToString(),
                    Gender = ((ComboBoxItem)GenderText.SelectedItem).Content.ToString(),
                    Birthday = date
                };
                context.Database.OpenConnection();
                try
                {
                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Participants] ON");
                    context.Participants.AddRange(participant);
                    context.SaveChanges();
                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Participants] OFF");
                }
                finally
                {
                    context.Database.CloseConnection();
                }

                MessageBox.Show("Вы зарегистрированы!", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите ФИО в формате 'Фамилия Имя Отчество' и пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void PasswordText_LostFocus(object sender, RoutedEventArgs e)
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
    }
}
