using MediatR;
using Microsoft.EntityFrameworkCore;
using ZeroFat.Application.Common.Models;
using ZeroFat.Domain.Enums;
using ZeroFat.Users.Application.Statistics;
using ZeroFat.Users.Infrastructure.Persistence.Context;

namespace ZeroFat.Users.Infrastructure.Statistics;

public class GetStatisticsRequestHandler(UsersContext db) : IRequestHandler<GetStatisticsRequest, Result<StatisticsDto>>
{
    private readonly UsersContext _db = db;

    public async Task<Result<StatisticsDto>> Handle(GetStatisticsRequest request, CancellationToken cancellationToken)
    {

        return await Result<StatisticsDto>.SuccessAsync(new StatisticsDto
        {
            Users = await _db.Users.CountAsync(x => x.DeletedOn == null && x.IsActive, cancellationToken),
            AdminUsers = await _db.Users.CountAsync(x => x.UserType == UserType.Admin && x.DeletedOn == null && x.IsActive, cancellationToken),
            ClientUsers = await _db.Users.CountAsync(x => x.UserType == UserType.Client && x.DeletedOn == null && x.IsActive, cancellationToken),
            Devices = await _db.Devices.CountAsync(cancellationToken),
            TrustedDevices = await _db.Devices.CountAsync(x => x.IsTrusted, cancellationToken),
            FailedLoginAttempts = await _db.FailedLoginAttempts.CountAsync(cancellationToken),
            UsersByType = await GetUsersByType(),
            DevicesByType = await GetDevicesByType(),
            DevicesByUser = await GetTopTenDevicesByUser(),
            FailedLoginAttemptsByUser = await GetFailedLoginAttemptsByUser()
        });
    }


    private async Task<Dictionary<string, int>> GetUsersByType()
    {
        return await _db.Users.Where(x => x.DeletedOn == null && x.IsActive)
                               .GroupBy(x => x.UserType)
                               .Select(x => new
                               {
                                   key = x.Key.ToString(),
                                   value = x.Count()
                               })
                               .ToDictionaryAsync(x => x.key, x => x.value);

    }

    private async Task<Dictionary<string, int>> GetFailedLoginAttemptsByUser()
    {
        return await _db.FailedLoginAttempts
                               .GroupBy(x => x.ApplicationUserId)
                               .Select(x => new
                               {
                                   key = x.Key.ToString(),
                                   value = x.Count()
                               })
                               .ToDictionaryAsync(x => x.key, x => x.value);

    }

    private async Task<Dictionary<string, int>> GetDevicesByType()
    {
        return await _db.Devices.GroupBy(x => x.DeviceType)
                               .Select(x => new
                               {
                                   key = x.Key.ToString(),
                                   value = x.Count()
                               })
                               .ToDictionaryAsync(x => x.key, x => x.value);

    }

    private async Task<Dictionary<string, int>> GetTopTenDevicesByUser()
    {
        var devices = await _db.Devices.Where(x => x.UserPublicId != null)
                               .GroupBy(x => x.UserPublicId!.Value)
                               .Select(x => new
                               {
                                   key = x.Key,
                                   value = x.Count()
                               })
                               .Take(10)
                               .ToListAsync();

        var dic = new Dictionary<string, int>();
        foreach (var device in devices)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.PublicId == device.key);
            if (user is not null)
                dic.Add(user.FullName ?? user.UserName, device.value);
        }

        return dic;
    }
}
