using System.Net;
using System.Net.Http.Json;
using Domain.Gyms;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;
using WebApi.Common.Infrastructure;
using WebApi.Gyms;

namespace IntegrationTests;

internal sealed class EnrollGymTests : IntegrationTestsBase
{
    [CancelAfter(10_000)]
    [Test]
    public async Task EnrollGym__ShouldCreateGymInDb(CancellationToken cancellationToken)
    {
        var client = ApplicationFactory.CreateClient();
        
        var apiResponse = await client.PostAsync(
            "/gyms/enroll",
            JsonContent.Create(
                new EnrollNewGymRequest
                {
                    Code = "TEST",
                    Name = "Test gym"
                }),
            cancellationToken);

        await apiResponse.AssertIsSuccessful(cancellationToken);
        
        var gym = JsonConvert.DeserializeObject<EnrollNewGymResponse>(
            await apiResponse.Content.ReadAsStringAsync(cancellationToken))!;

        using var scope = ApplicationFactory.Services.CreateScope();
        
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var gymInDb = await dbContext.Set<Gym>().FindAsync([gym.Id], cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(gymInDb, Is.Not.Null);
            Assert.That(gymInDb!.Code.Value, Is.EqualTo(gym.Code));
            Assert.That(gymInDb.Name.Value, Is.EqualTo(gym.Name));
        });
    }
    
    [CancelAfter(10_000)]
    [TestCase("INVALID", "GYM")]
    [TestCase("VGYM", " ")]
    public async Task EnrollGym__WhenInvalid__ShouldBadRequest(string code, string name, CancellationToken cancellationToken)
    {
        var client = ApplicationFactory.CreateClient();
        
        var apiResponse = await client.PostAsync(
            "/gyms/enroll",
            JsonContent.Create(
                new EnrollNewGymRequest
                {
                    Code = code,
                    Name = name
                }),
            cancellationToken);

        Assert.That(apiResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
    
    [CancelAfter(10_000)]
    [TestCase("code")]
    [TestCase("CODE")]
    public async Task EnrollGym__WhenCodeIsNotUnique__ShouldBadRequest(string duplicatedCode, CancellationToken cancellationToken)
    {
        var client = ApplicationFactory.CreateClient();
        
        await client.PostAsync(
            "/gyms/enroll",
            JsonContent.Create(
                new EnrollNewGymRequest
                {
                    Code = duplicatedCode,
                    Name = "Original Gym"
                }),
            cancellationToken);
        
        var apiResponse = await client.PostAsync(
            "/gyms/enroll",
            JsonContent.Create(
                new EnrollNewGymRequest
                {
                    Code = duplicatedCode,
                    Name = "Thief Gym"
                }),
            cancellationToken);

        Assert.That(apiResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
}