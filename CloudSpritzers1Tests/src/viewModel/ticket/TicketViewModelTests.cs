using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CloudSpritzers1.Src.Dto;
using CloudSpritzers1.Src.Model;
using CloudSpritzers1.Src.Model.Ticket;
using CloudSpritzers1.Src.Service.Interfaces;
using CloudSpritzers1.Src.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace CloudSpritzers1Tests.Src.ViewModel
{
    [TestClass]
    public class TicketsViewModelTests
    {
        private ITicketService _ticketService;
        private ITicketCategoryService _categoryService;
        private ITicketSubcategoryService _subcategoryService;
        private IUserService _userService;
        private IMapper _mapper;
        private TicketsViewModel _ticketsViewModel;


        private TicketCategory _testCategory;
        private TicketSubcategory _testSubcategory;
        private User _testUser;

        [TestInitialize]
        public void Setup()
        {

            _ticketService = Substitute.For<ITicketService>();
            _categoryService = Substitute.For<ITicketCategoryService>();
            _subcategoryService = Substitute.For<ITicketSubcategoryService>();
            _userService = Substitute.For<IUserService>();
            _mapper = Substitute.For<IMapper>();

            _testCategory = new TicketCategory(1, "Hardware", TicketUrgencyLevelEnum.MEDIUM);
            _testSubcategory = new TicketSubcategory(10, "Monitor", 100, _testCategory);
            _testUser = new User(42, "Dede", "dedeee@airport.com");


            _mapper.Map<TicketDTO>(Arg.Any<Ticket>()).Returns(callInfo => MapToDto((Ticket)callInfo[0]));

            var initialTickets = new List<Ticket>
            {
                new Ticket(1, _testUser, TicketStatusEnum.OPEN, _testCategory, _testSubcategory, "Issue 1", "Desc 1", DateTime.Now)
            };

            _ticketService.GetAllTickets().Returns(initialTickets);
            _categoryService.GetAllCategories().Returns(new List<TicketCategory> { _testCategory });


            _ticketsViewModel = new TicketsViewModel(_ticketService, _categoryService, _subcategoryService, _userService, _mapper);
        }

        [TestMethod]
        public void Constructor_ShouldInitializeCollectionsAndLoadData()
        {

            Assert.AreEqual(1, _ticketsViewModel.AllTickets.Count);
            Assert.AreEqual(1, _ticketsViewModel.Categories.Count);
            _ticketService.Received(1).GetAllTickets();
        }

        [TestMethod]
        public void GetAllTickets_ShouldReturnCurrentAllTicketsCollection()
        {

            var result = _ticketsViewModel.GetAllTickets();
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(_ticketsViewModel.AllTickets.First(), result.First());
        }

        [TestMethod]
        public void GetTotalTicketCount_ShouldReturnCountOfAllTickets()
        {
            var count = _ticketsViewModel.GetTotalTicketCount();
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void CreateTicket_ShouldExecuteFlowCorrectly()
        {
            var dto = new TicketDTO(
                101, 42, "dede_the_racoon@gmail.com",
                TicketUrgencyLevelEnum.HIGH, TicketStatusEnum.OPEN,
                1, "Hardware", 10, "Monitor",
                "Broken Screen", "The screen is crackedddd", DateTime.Now);

            _userService.GetById(42).Returns(_testUser);
            _categoryService.GetCategoryById(1).Returns(_testCategory);
            _subcategoryService.GetSubcategoryById(10).Returns(_testSubcategory);


            _ticketsViewModel.CreateTicket(dto);


            _ticketService.Received(1).AddTicket(Arg.Is<Ticket>(t =>
                t.TicketId == 101 &&
                t.Subject == "Broken Screen" &&
                t.Creator.UserId == 42));


            _ticketService.Received(2).GetAllTickets();
        }

        [TestMethod]
        public void UpdateStatus_ShouldTriggerServiceUpdate()
        {
            _ticketsViewModel.UpdateStatus(1, TicketStatusEnum.RESOLVED);

            _ticketService.Received(1).UpdateStatus(1, TicketStatusEnum.RESOLVED);
            _ticketService.Received(2).GetAllTickets();
        }


        [TestMethod]
        public void UpdateUrgencyLevel_ShouldCallServiceAndUpdateLocalList()
        {
            int targetTicketId = 1;
            var newUrgency = TicketUrgencyLevelEnum.HIGH;

            _ticketsViewModel.UpdateUrgencyLevel(targetTicketId, newUrgency);
            _ticketService.Received(1).UpdateUrgencyLevel(targetTicketId, newUrgency);
            _ticketService.Received(2).GetAllTickets();
        }

        [TestMethod]
        public void FilterByStatus_ShouldUpdateFilteredDisplayCollection()
        {
            var filteredResults = new List<TicketDTO> { _ticketsViewModel.AllTickets[0] };
            _ticketService.FilterTicketsByStatus(Arg.Any<IEnumerable<TicketDTO>>(), TicketFilterStatusEnum.OPEN).Returns(filteredResults);

            _ticketsViewModel.SelectedFilterStatus = TicketFilterStatusEnum.OPEN;

            Assert.AreEqual(1, _ticketsViewModel.FilteredTicketsForDisplay.Count);
            _ticketService.Received().FilterTicketsByStatus(Arg.Any<IEnumerable<TicketDTO>>(), TicketFilterStatusEnum.OPEN);
        }


        [TestMethod]
        public void SelectedFilterString_SetValidValue_UpdatesEnumStatus()
        {

            _ticketsViewModel.SelectedFilterString = "RESOLVED";

            Assert.AreEqual(TicketFilterStatusEnum.RESOLVED, _ticketsViewModel.SelectedFilterStatus);

            _ticketService.Received().FilterTicketsByStatus(Arg.Any<IEnumerable<TicketDTO>>(), TicketFilterStatusEnum.RESOLVED);
        }

        [TestMethod]
        public void SelectedFilterString_Get_ReturnsEnumNameAsString()
        {
            _ticketsViewModel.SelectedFilterStatus = TicketFilterStatusEnum.IN_PROGRESS;

            var result = _ticketsViewModel.SelectedFilterString;
            Assert.AreEqual("IN_PROGRESS", result);
        }

        [TestMethod]
        public void LoadSubcategories_ShouldPopulateCorrectCategory()
        {
            var subList = new List<TicketSubcategory> { _testSubcategory };
            _subcategoryService.GetSubcategoriesByCategoryId(1).Returns(subList);

            _ticketsViewModel.LoadSubcategories(1);


            Assert.AreEqual(1, _ticketsViewModel.Subcategories.Count);
            Assert.AreEqual("Monitor", _ticketsViewModel.Subcategories[0].SubcategoryName);
        }

        private static TicketDTO MapToDto(Ticket ticket)
        {
            return new TicketDTO(
                ticket.TicketId,
                ticket.Creator.UserId,
                ticket.Creator.RetrieveConfiguredEmailAddressForBotContact(),
                ticket.UrgencyLevel,
                ticket.CurrentStatus,
                ticket.Category.CategoryId,
                ticket.Category.CategoryName,
                ticket.Subcategory.SubcategoryId,
                ticket.Subcategory.SubcategoryName,
                ticket.Subject,
                ticket.Description,
                ticket.CreationTimestamp
            );
        }
    }
}