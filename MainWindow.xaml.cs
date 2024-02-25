using ConferenceOrganizers.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ConferenceOrganizers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainContext context = new MainContext();
        public ObservableCollection<Event> lists { get; set; }
        public List<Event> fullList { get; set; }
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();

            //ParsingCountry();
            //ParsingCourse();
            //ParsingCity();

            //ParsingJury();
            //ParsingModerator();
            //ParsingOrganizer();
            //ParsingParticipant();

            //ParsingActivity();
            //ParsingEvent();

            //ParsingActivityEvent();
            //ParsingActivityJury();
            var userId = AppSettings.Default.userId;
            var userPassword = AppSettings.Default.userPassword;
            var role = AppSettings.Default.role;
            if(userId!=0 && userPassword!=null) {
                RemoveClickEvent(LogIn);
                LogIn.Click += OpenProfile;
                switch (role)
                {
                    case "participant":
                        LogIn.Content = "Профиль";
                        break;
                    case "organizer":
                        LogIn.Content = "Профиль";
                        break;
                    case "moderator":
                        LogIn.Content = "Профиль";
                        break;
                    case "jury":
                        LogIn.Content = "Профиль";
                        break;
                    default:
                        MessageBox.Show("Ошибка", "Мы не смогли разспознать вашу роль в системе, простите(:");
                        break;
                }
                    
            }

            fullList = context.Events.ToList();
            fullList = context.Events.Include(e => e.City).ToList();
            lists = new ObservableCollection<Event>(fullList);

            services.ItemsSource = lists;
            UpdateEvents();
        }
        private void RemoveClickEvent(Button b)
        {
            var routedEventHandlers = GetRoutedEventHandlers(b, ButtonBase.ClickEvent);
            foreach (var routedEventHandler in routedEventHandlers)
                b.Click -= (RoutedEventHandler)routedEventHandler.Handler;
        }
        public static RoutedEventHandlerInfo[] GetRoutedEventHandlers(UIElement element, RoutedEvent routedEvent)
        {
            // Get the EventHandlersStore instance which holds event handlers for the specified element.
            // The EventHandlersStore class is declared as internal.
            var eventHandlersStoreProperty = typeof(UIElement).GetProperty(
                "EventHandlersStore", BindingFlags.Instance | BindingFlags.NonPublic);
            object eventHandlersStore = eventHandlersStoreProperty.GetValue(element, null);

            // Invoke the GetRoutedEventHandlers method on the EventHandlersStore instance 
            // for getting an array of the subscribed event handlers.
            var getRoutedEventHandlers = eventHandlersStore.GetType().GetMethod(
                "GetRoutedEventHandlers", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var routedEventHandlers = (RoutedEventHandlerInfo[])getRoutedEventHandlers.Invoke(
                eventHandlersStore, new object[] { routedEvent });

            return routedEventHandlers;
        }

        private void OpenProfile(object sender, RoutedEventArgs e)
        {
            var userId = AppSettings.Default.userId;
            var userPassword = AppSettings.Default.userPassword;
            var role = AppSettings.Default.role;
            if (userId != 0 && userPassword != null)
            {
                switch (role)
                {
                    case "participant":
                        var participant = context.Participants.FirstOrDefault(p => p.Id == userId && p.Password == userPassword);
                        if (participant != null) { 
                            ParticipantProfil participantsWindow = new ParticipantProfil(participant);
                            participantsWindow.Show();
                            this.Close();
                        }

                        break;
                    case "organizer":
                        var organizer = context.Organizers.FirstOrDefault(o => o.Id == userId && o.Password == userPassword);
                        if (organizer!=null)
                        {
                            OrganizerWindow organizerWindow = new OrganizerWindow(organizer);
                            organizerWindow.Show();
                            this.Close();
                        }
                        break;
                    case "moderator":
                        var moderator = context.Moderators.FirstOrDefault(m => m.Id == userId && m.Password == userPassword);
                        if (moderator!=null)
                        {
                            ModeratorProfil moderatorProfil = new ModeratorProfil(moderator);
                            moderatorProfil.Show();
                            this.Close();
                        }
                        break;
                    case "jury":
                        var jury = context.Juries.FirstOrDefault(j => j.Id == userId && j.Password == userPassword);
                        if (jury!=null)
                        {
                            JuryProfil juryProfil = new JuryProfil(jury);
                            juryProfil.Show();
                            this.Close();
                        }
                        break;
                    default:
                        MessageBox.Show("Ошибка", "Мы не смогли разспознать вашу роль в системе, простите(:");
                        break;
                }

            }
        }

        private void UpdateEvents()
        {
            fullList = context.Events.Include(e => e.City).ToList();
            lists = new ObservableCollection<Event>(fullList);

            services.ItemsSource = lists;
        }

        public void ParsingEvent()
        {
            var lines = File.ReadAllLines(@"..\..\ParsingFiles\Events.csv").ToList();

            lines.RemoveAt(0);
            var events = new List<Event>();

            foreach (var line in lines)
            {
                var columns = line.Split(';').Select(x => x.Trim()).ToArray();

                var id = int.Parse(columns[0]);
                var name = columns[1];
                var date = DateTime.Parse(columns[2]);
                var days = int.Parse(columns[3]);
                var cityId = int.Parse(columns[4]);
                int? winnerId = int.Parse(columns[5]);
                winnerId = (winnerId == 0) ? null : winnerId;
                var image = columns[6];
                events.Add(new Event
                {
                    Id = id,
                    Name = name,
                    Date = date,
                    Days = days,
                    CityId = cityId,
                    WinnerId = winnerId,
                    Image = image
                });
            }

            context.Database.OpenConnection();
            try
            {
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Events] ON");
                context.Events.AddRange(events);
                context.SaveChanges();
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Events] OFF");
            }
            finally
            {
                context.Database.CloseConnection();
            }
        }

        public void ParsingActivityEvent()
        {
            var lines = File.ReadAllLines(@"..\..\ParsingFiles\ActivityEvents.csv").ToList();

            lines.RemoveAt(0);
            var activityEvents = new List<ActivityEvent>();

            foreach (var line in lines)
            {
                var columns = line.Split(';').Select(x => x.Trim()).ToArray();

                var id = int.Parse(columns[0]);
                var activityId = int.Parse(columns[1]);
                var eventId = int.Parse(columns[2]);
                activityEvents.Add(new ActivityEvent
                {
                    Id = id,
                    ActivityId = activityId,
                    EventId = eventId,
                });
            }

            context.Database.OpenConnection();
            try
            {
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[ActivityEvents] ON");
                context.ActivityEvents.AddRange(activityEvents);
                context.SaveChanges();
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[ActivityEvents] OFF");
            }
            finally
            {
                context.Database.CloseConnection();
            }
        }

        public void ParsingActivityJury()
        {
            var lines = File.ReadAllLines(@"..\..\ParsingFiles\ActivityJuries.csv").ToList();

            lines.RemoveAt(0);
            var activityJuries = new List<ActivityJury>();

            foreach (var line in lines)
            {
                var columns = line.Split(';').Select(x => x.Trim()).ToArray();

                var id = int.Parse(columns[0]);
                var activityId = int.Parse(columns[1]);
                var juryId = int.Parse(columns[2]);
                activityJuries.Add(new ActivityJury
                {
                    Id = id,
                    ActivityId = activityId,
                    JuryId = juryId,
                });
            }

            context.Database.OpenConnection();
            try
            {
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[ActivityJuries] ON");
                context.ActivityJuries.AddRange(activityJuries);
                context.SaveChanges();
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[ActivityJuries] OFF");
            }
            finally
            {
                context.Database.CloseConnection();
            }
        }

        public void ParsingParticipant()
        {
            var lines = File.ReadAllLines(@"..\..\ParsingFiles\Participants.csv").ToList();

            lines.RemoveAt(0);
            var participants = new List<Participant>();

            foreach (var line in lines)
            {
                var columns = line.Split(';').Select(x => x.Trim()).ToArray();

                var id = int.Parse(columns[0]);
                var name = columns[1];
                var surname = columns[2];
                var patronimic = columns[3];
                var email = columns[4];
                var birthday = DateTime.Parse(columns[5]);
                var countryId = int.Parse(columns[6]);
                var password = columns[7];
                var phone = columns[8];
                var photo = columns[9];
                var gender = columns[10];
                participants.Add(new Participant
                {
                    Id = id,
                    Name = name,
                    Surname = surname,
                    Patronimic = patronimic,
                    Email = email,
                    Birthday = birthday,
                    CountryId = countryId,
                    Password = password,
                    Phone = phone,
                    Photo = photo,
                    Gender = gender,
                });
            }

            context.Database.OpenConnection();
            try
            {
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Participants] ON");
                context.Participants.AddRange(participants);
                context.SaveChanges();
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Participants] OFF");
            }
            finally
            {
                context.Database.CloseConnection();
            }
        }

        public void ParsingOrganizer()
        {
            var lines = File.ReadAllLines(@"..\..\ParsingFiles\Organizers.csv").ToList();

            lines.RemoveAt(0);
            var organizers = new List<Organizer>();

            foreach (var line in lines)
            {
                var columns = line.Split(';').Select(x => x.Trim()).ToArray();

                var id = int.Parse(columns[0]);
                var name = columns[1];
                var surname = columns[2];
                var patronimic = columns[3];
                var email = columns[4];
                var countryId = int.Parse(columns[5]);
                var password = columns[6];
                var phone = columns[7];
                var photo = columns[8];
                var gender = columns[9];
                var birthday = DateTime.Parse(columns[10]);
                organizers.Add(new Organizer
                {
                    Id = id,
                    Name = name,
                    Surname = surname,
                    Patronimic = patronimic,
                    Email = email,
                    CountryId = countryId,
                    Password = password,
                    Phone = phone,
                    Photo = photo,
                    Gender = gender,
                    Birthday = birthday,
                });
            }

            context.Database.OpenConnection();
            try
            {
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Organizers] ON");
                context.Organizers.AddRange(organizers);
                context.SaveChanges();
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Organizers] OFF");
            }
            finally
            {
                context.Database.CloseConnection();
            }
        }

        public void ParsingModerator()
        {
            var lines = File.ReadAllLines(@"..\..\ParsingFiles\Moderators.csv").ToList();

            lines.RemoveAt(0);
            var moderators = new List<Moderator>();

            foreach (var line in lines)
            {
                var columns = line.Split(';').Select(x => x.Trim()).ToArray();

                var id = int.Parse(columns[0]);
                var name = columns[1];
                var surname = columns[2];
                var patronimic = columns[3];
                var courseId = int.Parse(columns[4]);
                var email = columns[5];
                var countryId = int.Parse(columns[6]);
                var password = columns[7];
                var phone = columns[8];
                var photo = columns[9];
                var gender = columns[10];
                var birthday = DateTime.Parse(columns[11]);
                moderators.Add(new Moderator
                {
                    Id = id,
                    Name = name,
                    Surname = surname,
                    Patronimic = patronimic,
                    CourseId = courseId,
                    Email = email,
                    CountryId = countryId,
                    Password = password,
                    Phone = phone,
                    Photo = photo,
                    Gender = gender,
                    Birthday = birthday,
                });
            }
            context.Database.OpenConnection();
            try
            {
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Moderators] ON");
                context.Moderators.AddRange(moderators);
                context.SaveChanges();
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Moderators] OFF");
            }
            finally
            {
                context.Database.CloseConnection();
            }
        }

        public void ParsingJury()
        {
            var lines = File.ReadAllLines(@"..\..\ParsingFiles\Juries.csv").ToList();

            lines.RemoveAt(0);
            var juries = new List<Jury>();

            foreach (var line in lines)
            {
                var columns = line.Split(';').Select(x => x.Trim()).ToArray();

                var id = int.Parse(columns[0]);
                var name = columns[1];
                var surname = columns[2];
                var patronimic = columns[3];
                var courseId = int.Parse(columns[4]);
                var email = columns[5];
                var countryId = int.Parse(columns[6]);
                var password = columns[7];
                var phone = columns[8];
                var photo = columns[9];
                var gender = columns[10];
                var birthday = DateTime.Parse(columns[11]);
                juries.Add(new Jury
                {
                    Id = id,
                    Name = name,
                    Surname = surname,
                    Patronimic = patronimic,
                    CourseId = courseId,
                    Email = email,
                    CountryId = countryId,
                    Password = password,
                    Phone = phone,
                    Photo = photo,
                    Gender = gender,
                    Birthday = birthday,
                });
            }

            context.Database.OpenConnection();
            try
            {
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Juries] ON");
                context.Juries.AddRange(juries);
                context.SaveChanges();
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Juries] OFF");
            }
            finally
            {
                context.Database.CloseConnection();
            }
        }

        public void ParsingCity()
        {
            var lines = File.ReadAllLines(@"..\..\ParsingFiles\Cities.csv").ToList();

            lines.RemoveAt(0);
            var cities = new List<City>();

            foreach (var line in lines)
            {
                var columns = line.Split(';').Select(x => x.Trim()).ToArray();

                var id = int.Parse(columns[0]);
                var name = columns[1];
                cities.Add(new City
                {
                    Id = id,
                    Name = name,
                });
            }

            context.Database.OpenConnection();
            try
            {
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Cities] ON");
                context.Cities.AddRange(cities);
                context.SaveChanges();
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Cities] OFF");
            }
            finally
            {
                context.Database.CloseConnection();
            }
        }

        public void ParsingCountry()
        {
            var lines = File.ReadAllLines(@"..\..\ParsingFiles\Countries.csv").ToList();

            lines.RemoveAt(0);
            var countries = new List<Country>();

            foreach (var line in lines)
            {
                var columns = line.Split(';').Select(x => x.Trim()).ToArray();

                var id = int.Parse(columns[0]);
                var acronym = columns[1];
                var name = columns[2];
                var nameEng = columns[3];
                countries.Add(new Country
                {
                    Id = id,
                    Acronym = acronym,
                    Name = name,
                    NameEng = nameEng,
                });
            }
            context.Database.OpenConnection();
            try
            {
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Countries] ON");
                context.Countries.AddRange(countries);
                context.SaveChanges();
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Countries] OFF");
            }
            finally
            {
                context.Database.CloseConnection();
            }
        }

        public void ParsingCourse()
        {
            var lines = File.ReadAllLines(@"..\..\ParsingFiles\Courses.csv").ToList();

            lines.RemoveAt(0);
            var courses = new List<Course>();

            foreach (var line in lines)
            {
                var columns = line.Split(';').Select(x => x.Trim()).ToArray();

                var name = columns[1];
                courses.Add(new Course
                {
                    Name = name,
                });
            }

            context.Courses.AddRange(courses);
            context.SaveChanges();
        }

        public void ParsingActivity()
        {
            var lines = File.ReadAllLines(@"..\..\ParsingFiles\Activities.csv").ToList();

            lines.RemoveAt(0);
            var activities = new List<Activity>();

            foreach (var line in lines)
            {
                var columns = line.Split(';').Select(x => x.Trim()).ToArray();

                var id = int.Parse(columns[0]);
                var time = DateTime.Parse(columns[2]);
                var moderId = int.Parse(columns[3]);
                var day = int.Parse(columns[4]);
                activities.Add(new Activity
                {
                    Id = id,
                    Name = columns[1],
                    StartTime = time,
                    ModeratorId = moderId,
                    Day = day,
                });
            }

            context.Database.OpenConnection();
            try
            {
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Activities] ON");
                context.Activities.AddRange(activities);
                context.SaveChanges();
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Activities] OFF");
            }
            finally
            {
                context.Database.CloseConnection();
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            
            Authorization authorization = new Authorization();
            authorization.Show();
            this.Close();
        }

        private void SortFilter()
        {
            lists.Clear();
            var Filter = fullList.Where(x => x.Name.ToLower().Contains(StrokaPoiska.Text.ToLower()));

            switch (DiscountFilter2.SelectedIndex)
            {
                case 0:
                    Filter = Filter.OrderBy(x => x.Name);
                    break;
                case 1:
                    Filter = Filter.OrderByDescending(x => x.Date);
                    break;
                case 2:
                    Filter = Filter.OrderBy(x => x.Date);
                    break;
                case 3:
                    Filter = Filter.OrderBy(x => x.CityName);
                    break;
            }
            foreach (var item in Filter)
            {
                lists.Add(item);
            }
        }

        private void StrokaPoiska_TextChanged(object sender, TextChangedEventArgs e)
        {
            SortFilter();
        }

        private void services_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void DiscountFilter2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortFilter();
        }

        
    }
}
