public class CreatePersonCommandHandler
{
    private readonly IPersonCommandRepository _commandRepository;

    public CreatePersonCommandHandler(IPersonCommandRepository commandRepository)
    {
        _commandRepository = commandRepository;
    }

    public void Handle(CreatePersonCommand command)
    {
        Person entity = Person.Create(command.Id, command.FirstName, command.LastName);
        _commandRepository.Add(entity);
    }
}