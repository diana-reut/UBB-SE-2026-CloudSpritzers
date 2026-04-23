using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudSpritzers1.Src.Repository;
using CloudSpritzers1.Src.Repository.Interfaces;
using CloudSpritzers1.Src.Model.Ticket;
using CloudSpritzers1.Src.Model;
using CloudSpritzers1Tests.Src.MockClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudSpritzers1Tests.Src.Repository
{
    [TestClass]
    public class TicketRepositoryTests
    {
        private ITicketRepository _ticketRepository;
        private User _testUser;
        private TicketCategory _testCategory;
        private TicketSubcategory _testSubcategory;

        [TestInitialize]
        public void Setup()
        {

            _ticketRepository = new InMemoryTicketRepository();

            _testUser = new User(1, "Dede", "dede@test.com");
            _testCategory = new TicketCategory(1, "IT", TicketUrgencyLevelEnum.HIGH);
            _testSubcategory = new TicketSubcategory(10, "Hardware", 101, _testCategory);
        }

        [TestMethod]
        public void GetById_WithExistingId_Succeeds()
        {
            var ticket = new Ticket(1, _testUser, TicketStatusEnum.OPEN, _testCategory, _testSubcategory, "Issue", "Desc", DateTime.Now);
            _ticketRepository.CreateNewEntity(ticket);

            var result = _ticketRepository.GetById(1);

            Assert.AreEqual(ticket.TicketId, result.TicketId);
            Assert.AreEqual(ticket.Subject, result.Subject);
        }

        [TestMethod]
        public void GetById_WithoutExistingId_ThrowsKeyNotFoundException()
        {
            Assert.ThrowsExactly<KeyNotFoundException>(() => _ticketRepository.GetById(999));
        }

        [TestMethod]
        public void CreateNewEntity_WithValidEntity_Succeeds()
        {
            var ticket = new Ticket(5, _testUser, TicketStatusEnum.OPEN, _testCategory, _testSubcategory, "New Ticket", "Description", DateTime.Now);

            var resultId = _ticketRepository.CreateNewEntity(ticket);

            Assert.AreEqual(5, resultId);
        }

        [TestMethod]
        public void CreateNewEntity_WhenTicketIsNull_ThrowsArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _ticketRepository.CreateNewEntity(null));
        }

        [TestMethod]
        public void AddTicket_WithDuplicateEntity_ThrowsArgumentException()
        {
            var ticket1 = new Ticket(1, _testUser, TicketStatusEnum.OPEN, _testCategory, _testSubcategory, "Original", "Desc", DateTime.Now);
            _ticketRepository.CreateNewEntity(ticket1);


            Assert.ThrowsExactly<ArgumentException>(() => _ticketRepository.CreateNewEntity(ticket1));
        }

        [TestMethod]
        public void UpdateById_WithExistingId_Succeeds()
        {
            var originalTicket = new Ticket(1, _testUser, TicketStatusEnum.OPEN, _testCategory, _testSubcategory, "Old Subject", "Old Desc", DateTime.Now);
            _ticketRepository.CreateNewEntity(originalTicket);

            var updatedTicket = new Ticket(1, _testUser, TicketStatusEnum.RESOLVED, _testCategory, _testSubcategory, "New Subject", "New Desc", DateTime.Now);

            _ticketRepository.UpdateById(1, updatedTicket);

            var result = _ticketRepository.GetById(1);
            Assert.AreEqual(TicketStatusEnum.RESOLVED, result.CurrentStatus);
            Assert.AreEqual("New Subject", result.Subject);
            Assert.AreEqual(updatedTicket, result);
        }

        [TestMethod]
        public void UpdateById_WithoutExistingId_ThrowsKeyNotFoundException()
        {
            var firstTicket = new Ticket(1, _testUser, TicketStatusEnum.OPEN, _testCategory, _testSubcategory, "Old Subject", "Old Desc", DateTime.Now);
            var updatedTicket = new Ticket(2, _testUser, TicketStatusEnum.RESOLVED, _testCategory, _testSubcategory, "New Subject", "New Desc", DateTime.Now);

            _ticketRepository.CreateNewEntity(firstTicket);

            Assert.ThrowsExactly<KeyNotFoundException>(() => _ticketRepository.UpdateById(updatedTicket.TicketId, updatedTicket));
        }


        [TestMethod]
        public void DeleteById_WithExistingId_Succeeds()
        {
            var ticket1 = new Ticket(1, _testUser, TicketStatusEnum.OPEN, _testCategory, _testSubcategory, "To Delete", "Desc", DateTime.Now);
            var ticket2 = new Ticket(2, _testUser, TicketStatusEnum.OPEN, _testCategory, _testSubcategory, "To Keep", "Desc", DateTime.Now);
            _ticketRepository.CreateNewEntity(ticket1);
            _ticketRepository.CreateNewEntity(ticket2); 

            _ticketRepository.DeleteById(1);

            var allTickets = _ticketRepository.GetAll().ToList();
            Assert.IsFalse(allTickets.Any(t => t.TicketId == 1));
        }


        [TestMethod]
        public void DeleteById_WithoutExistingId_ThrowsKeyNotFoundException()
        {
            var ticket = new Ticket(1, _testUser, TicketStatusEnum.OPEN, _testCategory, _testSubcategory, "To Delete", "Desc", DateTime.Now);
            _ticketRepository.CreateNewEntity(ticket);
            _ticketRepository.DeleteById(1);

            Assert.ThrowsExactly<KeyNotFoundException>(() => _ticketRepository.DeleteById(ticket.TicketId));
        }


        [TestMethod]
        public void GetAll_ReturnsAllEntities()
        {
            var ticket1 = new Ticket(1, _testUser, TicketStatusEnum.OPEN, _testCategory, _testSubcategory, "T1", "D1", DateTime.Now);
            var ticket2 = new Ticket(2, _testUser, TicketStatusEnum.OPEN, _testCategory, _testSubcategory, "T2", "D2", DateTime.Now);
            _ticketRepository.CreateNewEntity(ticket1);
            _ticketRepository.CreateNewEntity(ticket2);

            var result = _ticketRepository.GetAll().ToList();

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void GetAll_WhenEmpty_ReturnsEmptyCollection()
        {
            var result = _ticketRepository.GetAll().ToList();
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void UpdateStatus_ExecutesInternalLogic()
        {
            var ticket = new Ticket(1, _testUser, TicketStatusEnum.OPEN, _testCategory, _testSubcategory, "I forgot to make this test", "only 70% without this one is crazy bro", DateTime.Now);
            ticket.UpdateStatus(TicketStatusEnum.RESOLVED);
            Assert.AreEqual(TicketStatusEnum.RESOLVED, ticket.CurrentStatus);
        }

        [TestMethod]
        public void UpdateUrgencyLevel_ExecutesInternalLogic()
        {
            var ticket = new Ticket(1, _testUser, TicketStatusEnum.OPEN, _testCategory, _testSubcategory, "this one too", "descriptionnnnn", DateTime.Now, TicketUrgencyLevelEnum.LOW);
            ticket.UpdateUrgencyLevel(TicketUrgencyLevelEnum.HIGH);
            Assert.AreEqual(TicketUrgencyLevelEnum.HIGH, ticket.UrgencyLevel);
        }

        [TestMethod]
        public void Constructor_WhenUrgencyLevelIsNull_AssignsCategoryUrgency()
        {
            var categoryWithUrgency = new TicketCategory(2, "Urgent Category", TicketUrgencyLevelEnum.HIGH);
            var ticket = new Ticket(99, _testUser, TicketStatusEnum.OPEN, categoryWithUrgency, _testSubcategory, "Subject", "Desc", DateTime.Now, null);
            Assert.AreEqual(TicketUrgencyLevelEnum.HIGH, ticket.UrgencyLevel);
        }
    }


}