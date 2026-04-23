using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSpritzers1;
using CloudSpritzers1.src.model;
using CommunityToolkit.Mvvm.ComponentModel;
using System;


namespace CloudSpritzers1.src.viewModel.general
{
	public class ChoosingPageViewModel
	{
        public void SetUserRole(string roleTag)
        {
            bool isEmployee = roleTag == "Employee";

            var app = (App)App.Current;
            app.isEmployee = isEmployee;
        }
    }
}

