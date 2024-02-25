using ConferenceOrganizers.Models;
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
    /// Логика взаимодействия для ParticipantProfil.xaml
    /// </summary>
    public partial class ParticipantProfil : Window
    {
        private Participant participant;
        public ParticipantProfil(Participant participant)
        {
            InitializeComponent();

            this.DataContext = participant;


            DateTime dateTime = DateTime.Now;
            if (dateTime.Hour >= 9 && (dateTime.Hour < 11 || (dateTime.Hour <= 11 && dateTime.Minute == 0)))
            {
                Greeting.Text = $"Доброе утро!\n{participant.Name} {participant.Patronimic}";
            }
            else if ((dateTime.Hour >= 11 && dateTime.Minute == 1) && (dateTime.Hour < 18 || (dateTime.Hour <= 18 && dateTime.Minute == 0)))
            {
                Greeting.Text = $"Добрый день!\n{participant.Name} {participant.Patronimic}";
            }
            else
            {
                Greeting.Text = $"Добрый день!\n{participant.Name} {participant.Patronimic}";
            }


            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(participant.Photo, UriKind.RelativeOrAbsolute);
            bitmap.EndInit();

            mainImage.Source = bitmap;

            RegButton.IsEnabled = false;
            this.participant = participant;
        }

        private void ImageBut_Click(object sender, RoutedEventArgs e)
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

        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            string surname = Surname.Text;
            string name = Name.Text;
            string patronimic = Patronimic.Text;
            string email = Email.Text;
            string phone = Phone.Text;
            string password = Password.Password;

            if (string.IsNullOrWhiteSpace(surname) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(patronimic) ||
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            participant.Surname = surname;
            participant.Name = name;
            participant.Patronimic = patronimic;
            participant.Email = email;
            participant.Phone = phone;
            participant.Password = password;
            participant.Photo = mainImage.Source.ToString();

            using (var context = new MainContext())
            {
                context.Participants.Update(participant);
                context.SaveChanges();
            }


            MessageBox.Show("Данные успешно обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
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

        private void Password_LostFocus(object sender, RoutedEventArgs e)
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
                    RegButton.IsEnabled = true;
                }
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
