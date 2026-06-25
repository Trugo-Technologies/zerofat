using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroFat.Application.Common.Interfaces;

namespace ZeroFat.Infrastructure.Core.Services;
public interface ICoreSeeder : ITransientService
{
    Task InitializeAsync(CancellationToken cancellationToken);
}
