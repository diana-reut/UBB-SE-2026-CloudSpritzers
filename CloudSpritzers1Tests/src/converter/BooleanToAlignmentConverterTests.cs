using CloudSpritzers1.Src.Converter;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSpritzers1Tests.src.converter;

[TestClass]
public class BooleanToAlignmentConversterTests
{
    private BooleanToAlignmentConverter _converter;

    [TestInitialize]
    public void Setup()
    {
        _converter = new BooleanToAlignmentConverter();
    }

    [TestMethod]
    public void Convert_TrueToRight_Succeeds()
    {
        var result = _converter.Convert(true, typeof(HorizontalAlignment), null, null);
        Assert.AreEqual(HorizontalAlignment.Right, result);
    }

    [TestMethod]
    public void Convert_FalseToLeft_Succeeds()
    {
        var result = _converter.Convert(false, typeof(HorizontalAlignment), null, null);
        Assert.AreEqual(HorizontalAlignment.Left, result);
    }

    [TestMethod]
    public void Convert_NullToLeft_Succeeds()
    {
        var result = _converter.Convert(null, typeof(HorizontalAlignment), null, null);
        Assert.AreEqual(HorizontalAlignment.Left, result);
    }

    [TestMethod]
    public void Convert_InvalidTypeToLeft_Succeeds()
    {
        var result = _converter.Convert("invalid", typeof(HorizontalAlignment), null, null);
        Assert.AreEqual(HorizontalAlignment.Left, result);
    }

    [TestMethod]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        Assert.ThrowsExactly<NotImplementedException>(() => _converter.ConvertBack(HorizontalAlignment.Left, typeof(bool), null, null));
    }

}
