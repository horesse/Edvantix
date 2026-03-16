namespace Edvantix.Persona.UnitTests.Fakers;

public sealed class ProfileFaker : Faker<Profile>
{
    public ProfileFaker()
    {
        Randomizer.Seed = new Random(Edvantix.Constants.Other.Seeder.DefaultSeed);
        CustomInstantiator(f => new Profile(
            Guid.CreateVersion7(),
            f.Internet.UserName(),
            f.PickRandom<Gender>(),
            DateOnly.FromDateTime(f.Date.Past(30, DateTime.UtcNow.AddYears(-18))),
            f.Name.FirstName(),
            f.Name.LastName()
        ));
    }

    public Profile Generate() => base.Generate(1)[0];
}
