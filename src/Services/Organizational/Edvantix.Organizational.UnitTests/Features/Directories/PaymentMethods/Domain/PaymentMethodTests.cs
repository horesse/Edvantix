using Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate;

namespace Edvantix.Organizational.UnitTests.Features.Directories.PaymentMethods.Domain;

public sealed class PaymentMethodTests
{
    private static readonly Guid OrgId = Guid.CreateVersion7();
    private static readonly Guid UserId = Guid.CreateVersion7();

    [Test]
    public void GivenValidParams_WhenCreating_ThenShouldSetProperties()
    {
        var pm = new PaymentMethod(OrgId, "Карта", "card", true, false, 1, UserId);

        pm.OrganizationId.ShouldBe(OrgId);
        pm.Name.ShouldBe("Карта");
        pm.Code.ShouldBe("card");
        pm.IsCashless.ShouldBeTrue();
        pm.RequiresContract.ShouldBeFalse();
        pm.Order.ShouldBe(1);
        pm.IsArchived.ShouldBeFalse();
    }

    [Test]
    public void GivenCodeWithSpaces_WhenCreating_ThenShouldTrimCode()
    {
        var pm = new PaymentMethod(OrgId, "Карта", "  card  ", true, false);

        pm.Code.ShouldBe("card");
    }

    [Test]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenEmptyCode_WhenCreating_ThenShouldThrow(string code)
    {
        Should.Throw<ArgumentException>(() => new PaymentMethod(OrgId, "Карта", code, true, false));
    }

    [Test]
    public void GivenCodeExceedingMaxLength_WhenCreating_ThenShouldThrow()
    {
        var longCode = new string('x', PaymentMethod.MaxCodeLength + 1);

        Should.Throw<ArgumentException>(() => new PaymentMethod(OrgId, "Карта", longCode, true, false));
    }

    [Test]
    public void GivenCodeAtMaxLength_WhenCreating_ThenShouldBeAllowed()
    {
        var maxCode = new string('x', PaymentMethod.MaxCodeLength);

        var pm = new PaymentMethod(OrgId, "Карта", maxCode, true, false);

        pm.Code.ShouldBe(maxCode);
    }

    [Test]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenEmptyName_WhenCreating_ThenShouldThrow(string name)
    {
        Should.Throw<Exception>(() => new PaymentMethod(OrgId, name, "card", true, false));
    }

    [Test]
    public void GivenNameExceeding120Chars_WhenCreating_ThenShouldThrow()
    {
        var longName = new string('А', 121);

        Should.Throw<Exception>(() => new PaymentMethod(OrgId, longName, "card", true, false));
    }

    [Test]
    public void GivenRequiresContractTrue_WhenCreating_ThenShouldBeStored()
    {
        var pm = new PaymentMethod(OrgId, "Рассрочка", "installment", false, true);

        pm.RequiresContract.ShouldBeTrue();
        pm.IsCashless.ShouldBeFalse();
    }

    [Test]
    public void GivenValidUpdateParams_WhenUpdating_ThenShouldUpdateProperties()
    {
        var pm = new PaymentMethod(OrgId, "Карта", "card", true, false, 0, UserId);

        pm.Update("Перевод", "transfer", true, false, 2, UserId);

        pm.Name.ShouldBe("Перевод");
        pm.Code.ShouldBe("transfer");
        pm.IsCashless.ShouldBeTrue();
        pm.RequiresContract.ShouldBeFalse();
        pm.Order.ShouldBe(2);
    }

    [Test]
    public void GivenUpdateWithEmptyCode_WhenUpdating_ThenShouldThrow()
    {
        var pm = new PaymentMethod(OrgId, "Карта", "card", true, false);

        Should.Throw<ArgumentException>(() => pm.Update("Карта", "", true, false, 0, UserId));
    }

    [Test]
    public void GivenUpdateWithTooLongCode_WhenUpdating_ThenShouldThrow()
    {
        var pm = new PaymentMethod(OrgId, "Карта", "card", true, false);
        var longCode = new string('x', PaymentMethod.MaxCodeLength + 1);

        Should.Throw<ArgumentException>(() => pm.Update("Карта", longCode, true, false, 0, UserId));
    }

    [Test]
    public void GivenActivePaymentMethod_WhenArchiving_ThenIsArchivedShouldBeTrue()
    {
        var pm = new PaymentMethod(OrgId, "Карта", "card", true, false);

        pm.Archive(UserId);

        pm.IsArchived.ShouldBeTrue();
    }

    [Test]
    public void GivenAlreadyArchivedPaymentMethod_WhenArchivingAgain_ThenShouldBeIdempotent()
    {
        var pm = new PaymentMethod(OrgId, "Карта", "card", true, false);
        pm.Archive(UserId);

        pm.Archive(UserId);

        pm.IsArchived.ShouldBeTrue();
    }

    [Test]
    public void GivenArchivedPaymentMethod_WhenRestoring_ThenIsArchivedShouldBeFalse()
    {
        var pm = new PaymentMethod(OrgId, "Карта", "card", true, false);
        pm.Archive(UserId);

        pm.Restore(UserId);

        pm.IsArchived.ShouldBeFalse();
    }

    [Test]
    public void GivenActivePaymentMethod_WhenRestoringAgain_ThenShouldBeIdempotent()
    {
        var pm = new PaymentMethod(OrgId, "Карта", "card", true, false);

        pm.Restore(UserId);

        pm.IsArchived.ShouldBeFalse();
    }
}
