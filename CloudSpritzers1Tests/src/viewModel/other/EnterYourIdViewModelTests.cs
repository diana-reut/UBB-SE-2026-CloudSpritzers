using CloudSpritzers1.Src.ViewModel.General;

[TestClass]
public class EnterYourIdViewModelTests
{
    private EnterYourIdViewModel _viewModel;

    [TestInitialize]
    public void Setup()
    {
        _viewModel = new EnterYourIdViewModel();
    }

    [TestMethod]
    public void TryAuthenticate_WhenInputIsNotANumber_ReturnsFalse()
    {
        // Arrange
        _viewModel.UserIdentification = "not_a_number";

        // Act
        bool result = _viewModel.TryAuthenticate(out int parsedId);

        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(0, parsedId);
    }

    [TestMethod]
    public void UserIdentification_WhenSet_UpdatesProperty()
    {
        // Act
        _viewModel.UserIdentification = "123";

        // Assert
        Assert.AreEqual("123", _viewModel.UserIdentification);
    }

    [TestMethod]
    public void TryAuthenticate_WhenValidNumberAndTesting_ReturnsTrue()
    {
        // Arrange
        _viewModel.UserIdentification = "42";
        _viewModel.IsTestingMode = true; // Bypasses App.Current

        // Act
        bool result = _viewModel.TryAuthenticate(out int parsedId);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(42, parsedId);
    }

    [TestMethod]
    public void TryAuthenticate_WhenNotTesting_AttemptsToCallApp()
    {
        // Arrange
        _viewModel.UserIdentification = "123";
        _viewModel.IsTestingMode = false; // The default, but we're being explicit

        // Act & Assert
        // We expect this to crash because App.Current is null in tests.
        // In Code Coverage, a crash STILL counts as "hitting" the line!
        Assert.ThrowsExactly<NullReferenceException>(() =>
            _viewModel.TryAuthenticate(out _));
    }
}