using AutoMapper;
using CloudSpritzers1.src;
using CloudSpritzers1.src.dto;
using CloudSpritzers1.src.dto.mappingProfiles;
using CloudSpritzers1.src.model;
using CloudSpritzers1.src.model.chat;
using CloudSpritzers1.src.model.employee;
using CloudSpritzers1.src.repository;
using CloudSpritzers1.src.repository.implementations;
using CloudSpritzers1.src.repository.interfaces;
using CloudSpritzers1.src.service;
using CloudSpritzers1.src.service.bot;
using CloudSpritzers1.src.service.bot.strategy;
using CloudSpritzers1.src.service.implementation;
using CloudSpritzers1.src.service.interfaces;
using CloudSpritzers1.src.viewmodel;
using CloudSpritzers1.src.viewModel;
using CloudSpritzers1.src.viewModel.chat;
using CloudSpritzers1.src.viewModel.faq;
using CloudSpritzers1.src.viewModel.general;
using CloudSpritzers1.src.viewModel.review;
using CloudSpritzers1.src.repository.database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using CloudSpritzers1.src.viewModel.general;
using CloudSpritzers1.src.viewModel;
using CloudSpritzers1.src.repository.interfaces;
using CloudSpritzers1.src.service.interfaces;
using CloudSpritzers1.src.model.review;

namespace CloudSpritzers1
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; }
        private Window? _window;
        public User User { get; private set; }
        public Employee Employee { get; private set; }
        public bool isEmployee = false;

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
                if (isEmployee)
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
                typeof(TicketMappingProfile).Assembly
            );


            services.AddSingleton<DecisionTreeRepository>();
            services.AddTransient<IBotStrategy, DecisionTreeStrategy>(); // I am not sure this is the way to do it :(
            services.AddTransient<BotEngine>();

            services.AddSingleton<MessageDatabaseRepository>();
            services.AddSingleton<MessageService>();

            services.AddSingleton<ChatDatabaseRepository>();
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

            services.AddSingleton<ITicketRepository,TicketRepository>();
            services.AddSingleton<ITicketCategoryRepository, TicketCategoryRepository>();
            services.AddSingleton<ITicketSubcategoryRepository, TicketSubcategoryRepository>();

            services.AddSingleton<ITicketService, TicketService>();
            services.AddSingleton<ITicketCategoryService,TicketCategoryService>();
            services.AddSingleton<ITicketSubcategoryService, TicketSubcategoryService>();

            services.AddTransient<TicketsViewModel>();

            services.AddSingleton<IFAQRepository, FAQRepository>();
            services.AddSingleton<IFAQService, FAQService>();

            services.AddTransient<FAQViewModel>();

            return services.BuildServiceProvider();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();

            var frame = new Frame();
            frame.Navigate(typeof(CloudSpritzers1.src.view.general.ChoosingPage));
            _window.Content = frame;

            _window.Activate();
        }
    }
}
