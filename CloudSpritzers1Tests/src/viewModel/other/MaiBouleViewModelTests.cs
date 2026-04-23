using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudSpritzers1.Src.ViewModel.General;

namespace CloudSpritzers1Tests.Src.ViewModel
{
    [TestClass]
    public class MaiBouleViewModelTests
    {
        private MaiBouleViewModel _viewModel;

        [TestInitialize]
        public void Setup()
        {
            _viewModel = new MaiBouleViewModel();
        }

        [TestMethod]
        public void Constructor_SetsDefaultValuesCorrectly()
        {
            // Assert
            Assert.AreEqual("Warning", _viewModel.Title);
            Assert.AreEqual("Oopsie Daisy! Boule.", _viewModel.Message);
        }

       
    }
}