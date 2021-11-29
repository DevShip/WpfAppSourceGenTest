using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Generators;
using WpfApp.Models;

namespace WpfApp
{
    public partial class ViewModelBase : BindableBase
    {
        [AutoNotify]
        private string _title;


        public ViewModelBase()
        {
            Title = "The test app";
        }
    }
}
