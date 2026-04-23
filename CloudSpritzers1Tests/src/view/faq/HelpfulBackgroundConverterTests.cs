using CloudSpritzers1.src.view.faq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.UI.Dispatching;
using System;
using System.Threading.Tasks;
namespace CloudSpritzers1.src.view.faq;

[TestClass]
public class HelpfulBackgroundConverterTests
{
    private HelpfulBackgroundConverter _converter;

    [TestInitialize]
    public void Setup()
    {
        _converter = new HelpfulBackgroundConverter();
    }

    //[TestMethod]
    //public void ConvertHelpfulVoteToHelpfulVoteBackground()
    //{
    //    var result = _converter.Convert(true, typeof(SolidColorBrush), null, null);
    //    Assert.AreEqual(Color.FromArgb(255, 232, 247, 236), result);
    //}

    //[TestMethod]
    //public async Task ConvertHelpfulVoteToHelpfulVoteBackground()
    //{
    //    await Task.Run(() =>
    //    {
    //        DispatcherQueue.GetForCurrentThread().TryEnqueue(() =>
    //        {
    //            var result = _converter.Convert(true, typeof(SolidColorBrush), null, null);
    //            Assert.AreEqual(Color.FromArgb(255, 232, 247, 236), result);
    //        });
    //    });
    //}


    //[TestMethod]
    //public async Task ConvertDefaultToDefaultBackground()
    //{
    //    await Task.Run(() =>
    //    {
    //        DispatcherQueue.GetForCurrentThread().TryEnqueue(() =>
    //        {
    //            var result = _converter.Convert(true, typeof(SolidColorBrush), null, null);
    //            Assert.AreEqual(Color.FromArgb(255, 248, 249, 251), result);
    //        });
    //    });
    //}

    [TestMethod()]
    public void ConvertBack_ThrowsNotImplementedException()
    {

        Assert.ThrowsExactly<NotImplementedException>(() => _converter.ConvertBack(Color.FromArgb(255, 248, 249, 251), typeof(bool), null, null));
    }

}
