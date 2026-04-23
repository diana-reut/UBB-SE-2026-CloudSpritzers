using Microsoft.UI.Xaml.Media;
using Windows.UI;
using CloudSpritzers1.Src.Converter;

namespace CloudSpritzers1Tests.src.converter;

[TestClass]
public class HelpfulForegroundConverterTests
{
    private HelpfulForegroundConverter _converter;

    [TestInitialize]
    public void Setup()
    {
        _converter = new HelpfulForegroundConverter();
    }

    //[TestMethod]
    //public void ConvertHelpfulVoteToHelpfulVoteForeground()
    //{
    //    var result = _converter.Convert(true, typeof(SolidColorBrush), null, null);
    //    Assert.AreEqual(Color.FromArgb(255, 21, 128, 61), result);
    //}

    //[TestMethod]
    //public void ConvertDefaultToDefaultForeground()
    //{
    //    var result = _converter.Convert(false, typeof(SolidColorBrush), null, null);
    //    Assert.AreEqual(Color.FromArgb(255, 107, 114, 128), result);
    //}

    [TestMethod()]
    public void ConvertBack_ThrowsNotImplementedException()
    {

        Assert.ThrowsExactly<NotImplementedException>(() => _converter.ConvertBack(Color.FromArgb(255, 248, 249, 251), typeof(bool), null, null));
    }
}
