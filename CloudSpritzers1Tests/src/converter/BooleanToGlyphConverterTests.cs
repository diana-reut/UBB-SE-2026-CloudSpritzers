using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudSpritzers1.Src.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;

namespace CloudSpritzers1Tests.src.converter;

[TestClass()]
public class BooleanToGlyphConverterTests
{
    private BooleanToGlyphConverter _converter;

    [TestInitialize]
    public void Setup()
    {
        _converter = new BooleanToGlyphConverter();
    }

    [TestMethod()]
    public void ConvertIsExpanded_ReturnsCorrespondingString()
    {
        var result = _converter.Convert(true, typeof(string), null, null);
        Assert.AreEqual("\uE70D", result);
    }

    [TestMethod()]
    public void ConvertIsNotExpanded_ReturnsCorrespondingString()
    {
        var result = _converter.Convert(false, typeof(string), null, null);
        Assert.AreEqual("\uE76C", result);
    }

    [TestMethod()]
    public void ConvertBackT_ThrowsNotImplementedException()
    {
        
         Assert.ThrowsExactly<NotImplementedException>(() => _converter.ConvertBack("\uE70D", typeof(bool), null, null));
    }
}