using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSpritzers1.Src.Converter;

namespace CloudSpritzers1Tests.src.converter;

[TestClass]
public class DateTimeToLocalConverterTests
{
    private DateTimeToLocalConverter _converter;

    [TestInitialize]
    public void Setup()
    {
        _converter = new DateTimeToLocalConverter();
    }

    [TestMethod]
    public void Convert_ValidDateTimeOffset_ReturnsFormattedString()
    {
        var input = new DateTimeOffset(2024, 1, 10, 12, 30, 0, TimeSpan.Zero);

        var result = _converter.Convert(input, typeof(string), null, null);

        Assert.IsInstanceOfType(result, typeof(string));
    }

    [TestMethod]
    public void Convert_ValidDateTimeOffset_FormatIsCorrect()
    {
        var input = new DateTimeOffset(2024, 1, 10, 12, 30, 0, TimeSpan.Zero);

        var result = _converter.Convert(input, typeof(string), null, null) as string;

        Assert.IsTrue(result.Contains("Jan") || result.Contains("Feb") || result.Contains("Mar")
            || result.Contains("Apr") || result.Contains("May") || result.Contains("Jun")
            || result.Contains("Jul") || result.Contains("Aug") || result.Contains("Sep")
            || result.Contains("Oct") || result.Contains("Nov") || result.Contains("Dec"));
    }

    [TestMethod]
    public void Convert_InvalidType_ReturnsSameValue()
    {
        var input = "not a date";

        var result = _converter.Convert(input, typeof(string), null, null);

        Assert.AreEqual(input, result);
    }

    [TestMethod]
    public void Convert_Null_ReturnsNull()
    {
        var result = _converter.Convert(null, typeof(string), null, null);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        Assert.ThrowsExactly<NotImplementedException>(() =>
            _converter.ConvertBack("Jan 10, 12:30", typeof(DateTimeOffset), null, null));
    }
}