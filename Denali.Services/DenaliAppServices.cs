using Denali.Services.Analysis;
using Denali.Services.FinnHub;
using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Services
{
    public class DenaliAppServices
    {
        public IServiceProvider ServiceProvider { get; }

        public DenaliAppServices(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            //TODO: Load base modules/services
        }
    }
}
