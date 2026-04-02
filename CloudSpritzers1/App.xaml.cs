using AutoMapper;
using CloudSpritzers1.src;
using CloudSpritzers1.src.dto;
using CloudSpritzers1.src.dto.mappingProfiles;
using CloudSpritzers1.src.model;
using CloudSpritzers1.src.model.chat;
using CloudSpritzers1.src.repository;
using CloudSpritzers1.src.service;
using CloudSpritzers1.src.service.bot;
using CloudSpritzers1.src.service.bot.strategy;
using CloudSpritzers1.src.viewmodel;
using CloudSpritzers1.src.viewModel.chat;
using CloudSpritzers1.src.viewModel.review;
using CloudSpritzers1.src.model.employee;
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

        public void SetUser(int userId)
        {
            if (User != null || Employee != null)
                return;
            if (isEmployee)
            {
                Employee = Services.GetService<EmployeeService>().GetById(userId);
                return;
            }
            else
            {
                User = Services.GetService<UserService>().GetById(userId);
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

            services.AddSingleton<MessageDBRepository>();
            services.AddSingleton<MessageService>();

            services.AddSingleton<ChatDBRepository>();
            services.AddSingleton<ChatService>();

            services.AddSingleton<ReviewRepository>();
            services.AddSingleton<ReviewService>();

            services.AddSingleton<EmployeeRepository>();
            services.AddSingleton<EmployeeService>();

            services.AddSingleton<UserRepository>();
            services.AddSingleton<UserService>();

            services.AddTransient<LandingViewModel>();
            services.AddTransient<AllReviewsViewModel>();
            services.AddTransient<AddReviewViewModel>();
            services.AddTransient<ChatViewModel>();
            
            services.AddTransient<UpperBarViewModel>();
            
            services.AddSingleton<TicketRepository>();
            services.AddSingleton<TicketCategoryRepository>();
            services.AddSingleton<TicketSubcategoryRepository>();

            services.AddSingleton<TicketService>();
            services.AddSingleton<TicketCategoryService>();
            services.AddSingleton<TicketSubcategoryService>();

            services.AddTransient<TicketsViewModel>();

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
