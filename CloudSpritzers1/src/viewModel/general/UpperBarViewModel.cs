using CloudSpritzers1.src.model;
using CloudSpritzers1.src.model.employee;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSpritzers1.src.viewModel.general
{
    public sealed partial class UpperBarViewModel : ObservableObject
    {
        private User _user;
        private Employee _employee;
        public UpperBarViewModel()
        {
            _user = ((App)App.Current).User;
            _employee = ((App)App.Current).Employee;
        }

        private bool _isEmployee;
        public bool IsEmployee
        {
            get => _isEmployee;
            set => SetProperty(ref _isEmployee, value); // This triggers the UI update
        }

        public string UserId 
        {
            get {
                if(IsEmployee) {
                    return $"ID: {_user.GetId()}";
                }
                return IsEmployee ? $"ID: {_employee.GetId()}" : "Not signed in";
            }
         }
    }
}
