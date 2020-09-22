public class CreatePersonCommand
{
    public CreatePersonCommand(string firstName, string lastName, Guid id)
    {
        FirstName = firstName;
        LastName = lastName;
        Id = id;
    }

    public string FirstName { get; }
    public string LastName { get; }
    public Guid Id { get; }

}