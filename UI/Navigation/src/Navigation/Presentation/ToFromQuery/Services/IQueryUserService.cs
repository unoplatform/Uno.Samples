namespace Navigation.Presentation;

public interface IQueryUserService
{
	QueryUser? GetById(Guid id);
}
