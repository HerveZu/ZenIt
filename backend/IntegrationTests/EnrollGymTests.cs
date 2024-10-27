using System.Net;
using System.Net.Http.Json;
using NUnit.Framework;
using WebApi.GymManagement;
using WebApi.GymManagement.Contracts;

namespace IntegrationTests;

internal sealed class EnrollGymTests : IntegrationTestsBase
{
    [CancelAfter(10_000)]
    [TestCase("TEST", "Test gym")]
    public async Task GetGym__WhenEnrolled__ShouldReturnEnrolledGym(string code, string name, CancellationToken cancellationToken)
    {
        var client = ApplicationFactory.CreateClient();
        
        var enrollmentResponse = await client.PostAsync(
            "/gyms/enroll",
            JsonContent.Create(
                new EnrollNewGymRequest
                {
                    Code = code,
                    Name = name
                }),
            cancellationToken);

        await enrollmentResponse.AssertIsSuccessful();
        
        var gymResponse = await client.GetAsync(enrollmentResponse.Headers.Location, cancellationToken);
        await gymResponse.AssertIsSuccessful();
        var gym = await gymResponse.Content.ReadFromJsonAsync<GymDto>(cancellationToken);
        
        Assert.Multiple(() =>
        {
            Assert.That(gym!.Code, Is.EqualTo(code));
            Assert.That(gym.Name, Is.EqualTo(name));
        });
    }
    
    [CancelAfter(10_000)]
    [TestCase("INVALID", "GYM")]
    [TestCase("VGYM", " ")]
    [TestCase("GYM2", "GYM")]
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

        await apiResponse.AssertIs(HttpStatusCode.BadRequest);
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

        await apiResponse.AssertIs(HttpStatusCode.BadRequest);
    }
}