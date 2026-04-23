using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CloudSpritzers1.Src.ViewModel.General
{
    /// <summary>
    /// ViewModel for confirmation dialogs.
    /// Uses Source Generators to maintain clean, readable code.
    /// </summary>
    public sealed partial class YouSureViewModel : ObservableObject
    {
        [ObservableProperty]
        private string title = "Confirm Action";

        [ObservableProperty]
        private string message = "Are you sure you want to proceed?";

        [ObservableProperty]
        private string confirmButtonText = "Yes";

        [ObservableProperty]
        private string cancelButtonText = "Cancel";
    }
}