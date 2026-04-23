using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;


namespace CloudSpritzers1.src.viewModel.general
{
    /// <summary>
    /// ViewModel for confirmation dialogs. 
    /// Uses Source Generators to maintain clean, readable code.
    /// </summary>
    public sealed partial class YouSureViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _title = "Confirm Action";

        [ObservableProperty]
        private string _message = "Are you sure you want to proceed?";

        [ObservableProperty]
        private string _confirmButtonText = "Yes";

        [ObservableProperty]
        private string _cancelButtonText = "Cancel";
    }
}