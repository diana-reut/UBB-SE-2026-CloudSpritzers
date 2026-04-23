using CloudSpritzers1.Src.Converter;
using Microsoft.UI.Xaml;
namespace CloudSpritzers1Tests.src.converter;

[TestClass]
public class InverseBooleanToVisibilityConverterTests
{
    private InverseBooleanToVisibilityConverter _converter;

    [TestInitialize]
    public void Setup()
    {
        _converter = new InverseBooleanToVisibilityConverter();
    }

    [TestMethod]
    public void Convert_FalseToVisible_Succeeds()
    {
        var result = _converter.Convert(false, typeof(Visibility), null, null);
        Assert.AreEqual(Visibility.Visible, result);
    }

    [TestMethod]
    public void Convert_TrueToCollapsede_Succeeds()
    {
        var result = _converter.Convert(true, typeof(Visibility), null, null);
        Assert.AreEqual(Visibility.Collapsed, result);
    }

    [TestMethod()]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        Assert.ThrowsExactly<NotImplementedException>(() => _converter.ConvertBack(Visibility.Collapsed, typeof(bool), null, null));
    }
}
