using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Infrastructure.Core.Services;

namespace ZeroFat.NutriPlan.Infrastructure.Services;
public interface INutriPlanSeeder : ITransientService
{
    Task InitializeAsync(CancellationToken cancellationToken);
}
