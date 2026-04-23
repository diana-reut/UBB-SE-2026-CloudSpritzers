using CloudSpritzers1;
using CloudSpritzers1.Src.Model;
using CloudSpritzers1.Src.Model.Employee;
using CloudSpritzers1.Src.View.Review;
using CloudSpritzers1.Src.View.Ticket;
using CloudSpritzers1.Src.ViewModel.General;
using Microsoft.UI.Xaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CloudSpritzers1Tests.Src.ViewModel.General
{
    [TestClass]
    public class UpperBarViewModelTests
    {
        private const int TEST_USER_ID = 123;
        private const int TEST_EMPLOYEE_ID = 999;

        [TestMethod]
        public void Constructor_WhenUserIsProvided_SetsIsClientViewToTrue()
        {
            var user = new User(TEST_USER_ID, "John Doe", "john@example.com");

            var viewModel = new UpperBarViewModel(currentUser: user, currentEmployee: null);

            Assert.IsTrue(viewModel.IsClientView, "IsClientView should be true when a User is provided.");
            Assert.AreEqual($"ID: {TEST_USER_ID}", viewModel.UserDisplayLabel);
        }

        [TestMethod]
        public void Constructor_WhenEmployeeIsProvided_SetsIsClientViewToFalse()
        {
            var employee = new Employee(TEST_EMPLOYEE_ID, "Jane Staff", "jane@hospital.com", default);

            var viewModel = new UpperBarViewModel(currentUser: null, currentEmployee: employee);

            Assert.IsFalse(viewModel.IsClientView, "IsClientView should be false when an Employee is provided (and User is null).");
            Assert.AreEqual($"ID: {TEST_EMPLOYEE_ID}", viewModel.UserDisplayLabel);
        }




        [TestMethod]
        public void TicketsPageType_WhenIsClient_ReturnsTicketsView()
        {
            var user = new User(TEST_USER_ID, "John Doe", "john@example.com");
            var viewModel = new UpperBarViewModel(user, null);

            var pageType = viewModel.TicketsPageType;

            Assert.AreEqual(typeof(TicketsView), pageType, "Clients should be directed to TicketsView.");
        }

        [TestMethod]
        public void TicketsPageType_WhenIsEmployee_ReturnsTicketEmployeeView()
        {
            var employee = new Employee(TEST_EMPLOYEE_ID, "Jane Staff", "jane@hospital.com", default);
            var viewModel = new UpperBarViewModel(null, employee);

            var pageType = viewModel.TicketsPageType;

            Assert.AreEqual(typeof(TicketEmployeeView), pageType, "Employees should be directed to TicketEmployeeView.");
        }

        [TestMethod]
        public void ReviewsPageType_NavigationLogic_CorrectForBothRoles()
        {
            var clientVm = new UpperBarViewModel(new User(1, "A", "B"), null);
            Assert.AreEqual(typeof(ReviewPage), clientVm.ReviewsPageType);

            var employeeVm = new UpperBarViewModel(null, new Employee(2, "C", "D", default));
            Assert.AreEqual(typeof(EmployeeSeeReviews), employeeVm.ReviewsPageType);
        }

        [TestMethod]
        public void UserDisplayLabel_WhenNoOneIsSignedIn_ReturnsDefaultMessage()
        {

            var viewModel = new UpperBarViewModel(null, null);

            var label = viewModel.UserDisplayLabel;

            Assert.AreEqual("Not signed in", label);
        }

        [TestMethod]
        public void UserDisplayLabel_WhenUserIsNullAndEmployeeIsSet_ReturnsEmployeeIdentifier()
        {
            var employee = new Employee(999, "Employee Test", "emp@test.com", default);
            var viewModel = new UpperBarViewModel(currentUser: null, currentEmployee: employee);

            var result = viewModel.UserDisplayLabel;

            Assert.AreEqual("ID: 999", result);
        }

        [TestMethod]
        public void UserDisplayLabel_WhenUserIsClient_ReturnsUserIdentifier()
        {
            var user = new User(123, "Client Test", "client@test.com");
            var viewModel = new UpperBarViewModel(currentUser: user, currentEmployee: null);

            var result = viewModel.UserDisplayLabel;

            Assert.AreEqual("ID: 123", result);
        }


        [TestMethod]
        public void IsClientView_Setter_TriggersNotifications()
        {
            var viewModel = new UpperBarViewModel(null, null);
            bool propertyChangedRaised = false;

            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.UserDisplayLabel))
                {
                    propertyChangedRaised = true;
                }
            };

            var prop = typeof(UpperBarViewModel).GetProperty(nameof(viewModel.IsClientView));

            prop?.SetValue(viewModel, true);

            Assert.IsTrue(viewModel.IsClientView, "Proprietatea ar trebui să fie true acum.");
            Assert.IsTrue(propertyChangedRaised, "OnPropertyChanged ar fi trebuit să fie apelat pentru UserDisplayLabel.");
        }

        [TestMethod]
        public void StaticPages_AlwaysReturnCorrectTypes()
        {

            var viewModel = new UpperBarViewModel(null, null);

            Assert.AreEqual(typeof(CloudSpritzers1.Src.View.General.LandingPage), viewModel.LandingPageType);
            Assert.AreEqual(typeof(CloudSpritzers1.Src.View.Faq.FAQView), viewModel.FAQPageType);
            Assert.AreEqual(typeof(CloudSpritzers1.Src.View.General.ChoosingPage), viewModel.ChoosingPageType);
            Assert.AreEqual(typeof(CloudSpritzers1.Src.View.Chat.ChatPage), viewModel.ChatPageType);
        }



    }
}