using DevExpressApp.Data;

namespace DevExpressApp.Presentation;

public class DataGridViewModel
{
    public IReadOnlyList<Employee> Employees { get; }

    public DataGridViewModel()
    {
        Employees = new EmployeeData().Employees;
    }
}
