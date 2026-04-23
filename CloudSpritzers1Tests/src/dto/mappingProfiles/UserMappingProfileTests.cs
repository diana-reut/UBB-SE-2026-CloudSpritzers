using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoMapper;
using CloudSpritzers1.Src.Dto;
using CloudSpritzers1.Src.Dto.MappingProfiles;
using CloudSpritzers1.Src.Model;

namespace CloudSpritzers1Tests.src.dto.mappingprofiles;

[TestClass]
public class UserMappingProfileTests
{
    private IMapper _mapper;

    [TestInitialize]
    public void Setup()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<UserMappingProfile>());

        _mapper = config.CreateMapper();
    }

    [TestMethod]
    public void Map_UserToUserDTO_Succeeds()
    {
        var user = new User(1, "Alex", "alex@mail.com");

        var result = _mapper.Map<UserDTO>(user);

        Assert.AreEqual(user.RetrieveConfiguredDisplayFullNameForBot(), result.name);
        Assert.AreEqual(user.RetrieveConfiguredEmailAddressForBotContact(), result.email);
    }

    [TestMethod]
    public void Configuration_IsValid()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<UserMappingProfile>());

        config.AssertConfigurationIsValid();
    }
}