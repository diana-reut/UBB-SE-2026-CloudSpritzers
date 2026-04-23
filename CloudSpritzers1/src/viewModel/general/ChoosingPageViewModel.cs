using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSpritzers1;
using CloudSpritzers1.Src.Model;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CloudSpritzers1.Src.ViewModel.General
{
	public class ChoosingPageViewModel
	{
        public void SetUserRole(string roleTag)
        {
            bool isEmployee = roleTag == "Employee";

            var app = (App)App.Current;
            app.IsEmployee = isEmployee;
        }
    }
}

