using System;
using AutoMapper;
using CloudSpritzers1.Src.Dto;
using CloudSpritzers1.Src.Dto.MappingProfiles;
using CloudSpritzers1.Src.Model;
using CloudSpritzers1.Src.Model.Ticket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace CloudSpritzers1Tests.Dto.MappingProfiles;

[TestClass]
public class TicketMappingProfileTests
{

    private IMapper _mapper;

    [TestInitialize]
    public void Setup()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<TicketMappingProfile>());
        _mapper = config.CreateMapper();
    }


    [TestMethod]
    public void MapFromTicketToDTO_MapsCorrectly()
    {
        var creatorMock = new User(1, "John Doe", "johdoe@gmail.com");

        var categoryMock = new TicketCategory(1, "Software", TicketUrgencyLevelEnum.HIGH);

        var subcategoryMock = new TicketSubcategory(1, "Bug", 10, categoryMock);

        var ticket = new Ticket(1, creatorMock, TicketStatusEnum.IN_PROGRESS, categoryMock, subcategoryMock, "Issue with login", "Cannot login to the system", DateTime.UtcNow);

        var ticketDTO = _mapper.Map<TicketDTO>(ticket);
        Assert.AreEqual(ticket.TicketId, ticketDTO.ticketId);
        Assert.AreEqual(ticket.Creator.RetrieveUniqueDatabaseIdentifierForBot(), ticketDTO.creatorAccountId);
        Assert.AreEqual(ticket.Creator.RetrieveConfiguredEmailAddressForBotContact(), ticketDTO.creatorEmailAddress);
        Assert.AreEqual(ticket.UrgencyLevel, ticketDTO.urgencyLevel);
        Assert.AreEqual(ticket.CurrentStatus, ticketDTO.currentStatus);
        Assert.AreEqual(ticket.Category.CategoryId, ticketDTO.categoryId);
        Assert.AreEqual(ticket.Category.CategoryName, ticketDTO.categoryName);
        Assert.AreEqual(ticket.Subcategory.SubcategoryId, ticketDTO.subcategoryId);
        Assert.AreEqual(ticket.Subcategory.SubcategoryName, ticketDTO.subcategoryName);
        Assert.AreEqual(ticket.Subject, ticketDTO.subject);
        Assert.AreEqual(ticket.Description, ticketDTO.description);
        Assert.AreEqual(ticket.CreationTimestamp, ticketDTO.creationTimestamp);


    }
    
}