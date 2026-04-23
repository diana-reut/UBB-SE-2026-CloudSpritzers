using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using AutoMapper;
using CloudSpritzers1.Src;
using CloudSpritzers1.Src.Dto;
using CloudSpritzers1.Src.Dto.MappingProfiles;
using CloudSpritzers1.Src.Model;
using CloudSpritzers1.Src.Model.Chats;
using CloudSpritzers1.Src.Model.Review;
using CloudSpritzers1.Src.Model.Employee;
using CloudSpritzers1.Src.Repository;
using CloudSpritzers1.Src.Repository.Implementation;
using CloudSpritzers1.Src.Repository.Interfaces;
using CloudSpritzers1.Src.Service;
using CloudSpritzers1.Src.Service.Bot;
using CloudSpritzers1.Src.Service.Bot.Strategy;
using CloudSpritzers1.Src.Service.Implementation;
using CloudSpritzers1.Src.Service.Interfaces;
using CloudSpritzers1.Src.ViewModel;
using CloudSpritzers1.Src.ViewModel;
using CloudSpritzers1.Src.ViewModel.Chats;
using CloudSpritzers1.Src.ViewModel.Faq;
using CloudSpritzers1.Src.ViewModel.General;
using CloudSpritzers1.Src.ViewModel.Review;
using CloudSpritzers1.Src.Repository.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using CloudSpritzers1.Src.ViewModel.General;
using CloudSpritzers1.Src.ViewModel;
using CloudSpritzers1.Src.Repository.Interfaces;
using CloudSpritzers1.Src.Service.Interfaces;
using CloudSpritzers1.Src.Model.Review;
using CloudSpritzers1.Src.Model.Faq.Bot;
using CloudSpritzers1.Src.Model.Message;

namespace CloudSpritzers1
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; }
        private Window? window;
        public User User { get; private set; }
        public Employee Employee { get; private set; }
        public bool IsEmployee = false;

        public App()
        {
            Services = ConfigureServices();
            InitializeComponent();
        }

        /// <summary>
        /// Attempts to find and set the active user or employee.
        /// Returns true if the ID was found; otherwise, false.
        /// </summary>
        // Updated SetUser in App.xaml.cs
        public bool SetUser(int userId)
        {
            User = null;
            Employee = null;

            try
            {
                if (IsEmployee)
                {
                    Employee = Services.GetService<IEmployeeService>().GetEmployeeById(userId);
                    return Employee != null;
                }
                else
                {
                    User = Services.GetService<IUserService>().GetById(userId);
                    return User != null;
                }
            }
            catch (KeyNotFoundException)
            {
                return false; // Safely return false so the UI shows the error
            }
        }

        private static IServiceProvider ConfigureServices()
        {
            DotNetEnv.Env.Load(System.IO.Path.Combine(AppContext.BaseDirectory, ".env"));

            var services = new ServiceCollection();
            services.AddAutoMapper(
                typeof(UserMappingProfile).Assembly,
                typeof(EmployeeMappingProfile).Assembly,
                typeof(MessageMappingProfile).Assembly,
                typeof(FAQEntryMappingProfile).Assembly,
                typeof(ReviewMappingProfile).Assembly,
                typeof(TicketMappingProfile).Assembly);

            services.AddSingleton<DecisionTreeRepository>();
            services.AddSingleton<IRepository<int, FAQNode>>(provider => provider.GetRequiredService<DecisionTreeRepository>());
            services.AddTransient<IBotStrategy, DecisionTreeStrategy>(); // I am not sure this is the way to do it :(
            services.AddTransient<BotEngine>();

            services.AddSingleton<MessageDatabaseRepository>();
            services.AddSingleton<IRepository<int, Message>>(provider => provider.GetRequiredService<MessageDatabaseRepository>());
            services.AddSingleton<MessageService>();

            services.AddSingleton<ChatDatabaseRepository>();
            services.AddSingleton<IRepository<int, Chat>>(provider => provider.GetRequiredService<ChatDatabaseRepository>());
            services.AddSingleton<ChatService>();

            services.AddSingleton<ReviewRepository>();
            services.AddSingleton<IRepository<int, Review>>(provider => provider.GetRequiredService<ReviewRepository>());
            services.AddSingleton<ReviewService>();

            services.AddSingleton<IEmployeeRepository, EmployeeRepository>();
            services.AddSingleton<IEmployeeService, EmployeeService>();

            services.AddSingleton<UserRepository>();
            services.AddSingleton<IUserRepository>(provider => provider.GetRequiredService<UserRepository>());
            services.AddSingleton<IRepository<int, User>>(provider => provider.GetRequiredService<UserRepository>());

            services.AddSingleton<IUserService, UserService>();

            services.AddTransient<LandingViewModel>();
            services.AddTransient<AllReviewsViewModel>();
            services.AddTransient<AddReviewViewModel>();
            services.AddTransient<ChatViewModel>();

            // Register the ViewModel
            services.AddTransient<UpperBarViewModel>();

            services.AddSingleton<ITicketRepository, TicketRepository>();
            services.AddSingleton<ITicketCategoryRepository, TicketCategoryRepository>();
            services.AddSingleton<ITicketSubcategoryRepository, TicketSubcategoryRepository>();

            services.AddSingleton<ITicketService, TicketService>();
            services.AddSingleton<ITicketCategoryService, TicketCategoryService>();
            services.AddSingleton<ITicketSubcategoryService, TicketSubcategoryService>();

            services.AddTransient<TicketsViewModel>();

            services.AddSingleton<IFAQRepository, FAQRepository>();
            services.AddSingleton<IFAQService, FAQService>();

            services.AddTransient<FAQViewModel>();

            return services.BuildServiceProvider();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            window = new MainWindow();

            var frame = new Frame();
            frame.Navigate(typeof(CloudSpritzers1.Src.View.General.ChoosingPage));
            window.Content = frame;

            window.Activate();
        }
    }
}
