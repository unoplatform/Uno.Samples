using Person = MVUX.Presentation.Person;

namespace MVUX.Models;

public record PersonMessage(EntityChange Change, Person Person);
