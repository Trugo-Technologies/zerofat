using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Infrastructure.Core.Services;

namespace ZeroFat.ClientPortal.Infrastructure.Services;
public interface IClientPortalSeeder : ITransientService
{
    Task InitializeAsync(CancellationToken cancellationToken);
}
