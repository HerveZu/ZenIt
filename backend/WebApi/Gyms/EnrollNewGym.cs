using System.Net;
using Domain.Gyms;
using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebApi.Common.Infrastructure;

namespace WebApi.Gyms;

[Serializable]
internal sealed record EnrollNewGymRequest
{
    public required string Code { get; init; }
    public required string Name { get; init; }
}

[Serializable]
internal sealed record EnrollNewGymResponse
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
}

internal sealed class EnrollNewGymValidator : Validator<EnrollNewGymRequest>
{
    public EnrollNewGymValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .Length(4);

        RuleFor(x => x.Name)
            .NotEmpty();
    }
}

internal sealed class EnrollNewGym(AppDbContext context) : Endpoint<EnrollNewGymRequest, EnrollNewGymResponse>
{
    public override void Configure()
    {
        Post("/gyms/enroll");
        AllowAnonymous();
    }

    public override async Task HandleAsync(EnrollNewGymRequest req, CancellationToken ct)
    {
        var isCodeDuplicated = await context.Set<Gym>()
            .AnyAsync(gym => gym.Code == req.Code, ct);

        if (isCodeDuplicated)
        {
            ThrowError(x => x.Code, "Gym code is not unique.");
        }
        
        var gym = Gym.EnrollNew(req.Code, req.Name);

        await context.Set<Gym>().AddAsync(gym, ct);
        await context.SaveChangesAsync(ct);

        await SendAsync(
            new EnrollNewGymResponse
            {
                Id = gym.Id,
                Code = gym.Code,
                Name = gym.Name
            },
            (int)HttpStatusCode.Created,
            ct);
    }
}