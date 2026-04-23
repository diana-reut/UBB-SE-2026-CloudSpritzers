using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoMapper;
using CloudSpritzers1.Src.Dto;
using CloudSpritzers1.Src.Dto.MappingProfiles;
using CloudSpritzers1.Src.Model.Chats;

namespace CloudSpritzers1Tests.src.dto.mappingprofiles;

[TestClass]
public class ChatMappingProfileTests
{
    private IMapper _mapper;

    [TestInitialize]
    public void Setup()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<ChatMappingProfile>());

        _mapper = config.CreateMapper();
    }

    [TestMethod]
    public void Map_ChatToChatDTO_Succeeds()
    {
        var chat = new Chat(1, 10, ChatStatus.Active);

        var result = _mapper.Map<ChatDTO>(chat);

        Assert.AreEqual(chat.ChatId, result.chatId);
        Assert.AreEqual(chat.UserId, result.userId);
        Assert.AreEqual(chat.Status, result.status);
        Assert.AreEqual(0, result.messageCount); 
    }

    [TestMethod]
    public void Configuration_IsValid()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<ChatMappingProfile>());

        config.AssertConfigurationIsValid();
    }
}