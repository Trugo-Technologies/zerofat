using System;
using System.Collections.Generic;
using System.Linq;
using ZeroFat.Application.Common.Interfaces;

namespace ZeroFat.GymUp.Infrastructure.Services;
public interface IWorkoutSeeder : ITransientService
{
    Task InitializeAsync(CancellationToken cancellationToken);
}
