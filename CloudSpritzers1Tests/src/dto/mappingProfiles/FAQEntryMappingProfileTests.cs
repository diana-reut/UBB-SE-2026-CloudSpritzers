using AutoMapper;
using CloudSpritzers1.Src.Model.Faq;
using CloudSpritzers1.Src.Repository.Interfaces;
using NSubstitute;

namespace CloudSpritzers1.Src.Dto.MappingProfiles;
[TestClass]
public class FAQEntryMappingProfileTests
{
    private IMapper _mapper;

    [TestInitialize]
    public void Setup()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<FAQEntryMappingProfile>());
        _mapper = config.CreateMapper();
    }
    
    [TestMethod]
    public void MapFromEntryToDTO_MapsCorrectly()
    {
        var FAQEntry = new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0);
        var expectedFAQDto = new FAQEntryDTO(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0);

        var result = _mapper.Map<FAQEntryDTO>(FAQEntry);
        Assert.AreEqual(expectedFAQDto.Id, result.Id);
        Assert.AreEqual(expectedFAQDto.Question, result.Question);
        Assert.AreEqual(expectedFAQDto.Answer, result.Answer);
        Assert.AreEqual(expectedFAQDto.ViewCount, result.ViewCount);
        Assert.AreEqual(expectedFAQDto.HelpfulVotesCount, result.HelpfulVotesCount);
        Assert.AreEqual(expectedFAQDto.NotHelpfulVotesCount, result.NotHelpfulVotesCount);
    }

    [TestMethod]
    public void MapFromDtoToEntry_MapsCorrectly()
    {
        var FAQDto = new FAQEntryDTO(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0);
        var expectedFAQEntry = new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0);

        var result = _mapper.Map<FAQEntry>(FAQDto);
        Assert.AreEqual(expectedFAQEntry.Id, result.Id);
        Assert.AreEqual(expectedFAQEntry.Question, result.Question);
        Assert.AreEqual(expectedFAQEntry.Answer, result.Answer);
        Assert.AreEqual(expectedFAQEntry.ViewCount, result.ViewCount);
        Assert.AreEqual(expectedFAQEntry.HelpfulVotesCount, result.HelpfulVotesCount);
        Assert.AreEqual(expectedFAQEntry.NotHelpfulVotesCount, result.NotHelpfulVotesCount);
    }
}
