﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Client.MVVM.Async
{
    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }
}