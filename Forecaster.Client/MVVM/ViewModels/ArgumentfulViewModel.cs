using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Client.MVVM.ViewModels
{
    public abstract class ArgumentfulViewModel : CloseableViewModel
    {
        private IEnumerable<object> args;
        public IEnumerable<object> Args 
        {
            get { return args; } 
            set 
            { 
                args = value;
                AssignArguments();
            } 
        }

        protected abstract void AssignArguments();
    }
}
