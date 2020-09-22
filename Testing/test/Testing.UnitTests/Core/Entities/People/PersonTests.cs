using System;
using System.Linq;
using Shouldly;
using Xunit;
using Zamin.Core.Domain.TacticalPatterns;

public class PersonTests
{
    [Fact]
    public void when_pass_valid_Input_value_expect_person_create()
    {
        string firstName = "mahdiye";
        string lastName = "hosseini";
        BusinessId personId = BusinessId.FromGuid(Guid.NewGuid());

        var person = Person.Create(personId, firstName, lastName);

        person.ShouldNotBeNull();
        person.Id.ShouldBe(personId);
        person.FirstName.ShouldBe(firstName);
        person.LastName.ShouldBe(lastName);
        person.GetEvents().Count().ShouldBe(1);
        var @event = person.GetEvents().First();
        @event.ShouldBeOfType<PersonCreated>();
    }

    [Fact]
    public void when_pass_invalid_id_expect_throw_invalid_person_id_exception()
    {
        string firstName = "mahdiye";
        string lastName = "hosseini";
        BusinessId personId = null;

        var exception = Record.Exception(() => Person.Create(personId, firstName, lastName));

        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<InvalidPersonIdException>();
    }

    [Fact]
    public void when_pass_invalid_first_name_expect_throw_invalid_first_name_exception()
    {
        string firstName = null;
        string lastName = "hosseini";
        BusinessId personId = Guid.NewGuid();

        var exception = Record.Exception(() => Person.Create(personId, firstName, lastName));

        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<InvalidFirstNameException>();
    }

    [Fact]
    public void when_pass_invalid_last_name_expect_throw_invalid_first_name_exception()
    {
        string firstName = "mahdiye";
        string lastName = "";
        BusinessId personId = Guid.NewGuid();

        var exception = Record.Exception(() => Person.Create(personId, firstName, lastName));

        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<InvalidLastNameException>();
    }
}

