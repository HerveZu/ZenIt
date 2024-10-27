using Domain.Gyms;
using NUnit.Framework;

namespace Domain.Tests;

internal sealed class GymTests
{
    [TestCase("")]
    [TestCase("5abc")]
    [TestCase("abc")]
    [TestCase("a_bc")]
    public void EnrollNewGym__WithInvalidCode__ShouldFail(string invalidCode)
    {
        Assert.Throws<InvalidOperationException>(
            () =>
            {
                Gym.EnrollNew(invalidCode, "Valid gym name");
            });
    }
    
    [TestCase("")]
    [TestCase("   ")]
    [TestCase("ts")]
    [TestCase("A Waaaaaaaaaaaaaaaaaaay tooooooooooooooo loooooooong nnnnnname")]
    public void EnrollNewGym__WithInvalidName__ShouldFail(string invalidName)
    {
        Assert.Throws<InvalidOperationException>(
            () =>
            {
                Gym.EnrollNew("VLID", invalidName);
            });
    }
    
    [TestCase("MGYM", "My gym")]
    public void EnrollNewGym__ShouldCreateGym(string code, string name)
    {
        Gym.EnrollNew(code, name);
    }
}