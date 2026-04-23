using Microsoft.UI.Xaml;
using CloudSpritzers1.Src.Converter;
namespace CloudSpritzers1Tests.src.converter;

[TestClass]
public class BooleanToVisibilityConverterTests
{
    private BooleanToVisibilityConverter _converter;

    [TestInitialize]
    public void Setup()
    {
        _converter = new BooleanToVisibilityConverter();
    }

    [TestMethod]
    public void Convert_TrueToVisible_Succeeds()
    {
        var result = _converter.Convert(true, typeof(Visibility), null, null);
        Assert.AreEqual(Visibility.Visible, result);
    }

    [TestMethod]
    public void Convert_FalseToCollapsed_Succeeds()
    {
        var result = _converter.Convert(false, typeof(Visibility), null, null);
        Assert.AreEqual(Visibility.Collapsed, result);
    }

    [TestMethod]
    public void ConvertBack_VisibleToTrue_Succeeds()
    {
        var result = _converter.ConvertBack(Visibility.Visible, typeof(bool), null, null);
        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void ConvertBack_CollapsedToFalse_Succeeds()
    {
        var result = _converter.ConvertBack(Visibility.Collapsed, typeof(bool), null, null);
        Assert.AreEqual(false, result);
    }
}
