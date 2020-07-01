using Forecaster.Client.MVVM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Client.MVVM.ViewModels
{
    public abstract class CloseableViewModel : BasicViewModel
    {
        public event EventHandler ClosingRequest;

        protected void OnClosingRequest()
        {
            ClosingRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}
