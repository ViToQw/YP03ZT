using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
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
using ConferenceOrganizers.Models;

namespace ConferenceOrganizers
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Window
    {
        MainContext context = new MainContext();

        private Random random = new Random();
        private string captchaText;
        public Authorization()
        {
            InitializeComponent();
            CaptchaImage.Source = GenerateCaptcha();
         
        }

        private BitmapImage GenerateCaptcha()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            captchaText = new string(Enumerable.Repeat(chars, 4)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            Bitmap bitmap = new Bitmap(200, 50);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                Font font = new Font("Arial", 24);
                graphics.DrawString(captchaText, font, System.Drawing.Brushes.DarkRed, new PointF(10, 10));

                for (int i = 0; i < 4000; i++)
                {
                    int x = random.Next(bitmap.Width);
                    int y = random.Next(bitmap.Height);
                    bitmap.SetPixel(x, y, System.Drawing.Color.Black);
                }
            }

            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }

        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            RegParticipantWin regParticipantWin = new RegParticipantWin();
            regParticipantWin.Show();
        }

        private int failedAttempts = 0;
        private async void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            if (CaptchaText.Text != captchaText)
            {
                MessageBox.Show("Вы неверно ввели Сaptcha!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                CaptchaImage.Source = GenerateCaptcha();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(IdNumberText.Text) || string.IsNullOrWhiteSpace(PasswordText.Password))
                {
                    MessageBox.Show("Пожалуйста, введите ID и пароль!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int id = int.Parse(IdNumberText.Text);
                string password = PasswordText.Password;

                using (var context = new MainContext())
                {
                    var participant = context.Participants.FirstOrDefault(p => p.Id == id && p.Password == password);
                    var organizer = context.Organizers.FirstOrDefault(o => o.Id == id && o.Password == password);
                    var moderator = context.Moderators.FirstOrDefault(m => m.Id == id && m.Password == password);
                    var jury = context.Juries.FirstOrDefault(j => j.Id == id && j.Password == password);

                    if (participant != null)
                    {
                        ParticipantProfil participantsWindow = new ParticipantProfil(participant);
                        participantsWindow.Show();
                        this.Close();
                    }
                    else if (organizer != null)
                    {
                        OrganizerWindow organizerWindow = new OrganizerWindow(organizer);
                        organizerWindow.Show();
                        this.Close();
                    }
                    else if (moderator != null)
                    {
                        ModeratorProfil moderatorProfil = new ModeratorProfil(moderator);
                        moderatorProfil.Show();
                        this.Close();
                    }
                    else if (jury != null)
                    {
                        JuryProfil juryProfil = new JuryProfil(jury);
                        juryProfil.Show();
                        this.Close();
                    }
                    else
                    {
                        failedAttempts++; // Увеличить счетчик при неудачной попытке

                        if (failedAttempts >= 3)
                        {
                            MessageBox.Show("Слишком много неудачных попыток входа. Пожалуйста, подождите 10 секунд перед следующей попыткой.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                            await Task.Delay(10000); // Задержка в 10 секунд
                            failedAttempts = 0; // Сбросить счетчик после задержки
                        }
                        else
                        {
                            MessageBox.Show("Неверный ID или пароль!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }

        private void IdNumberText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {

            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
