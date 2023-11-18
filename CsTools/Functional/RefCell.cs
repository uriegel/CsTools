namespace CsTools.Functional;

public class RefCell<T>
{
    public T? Value;

    public RefCell() {}
    public RefCell(T? value) => Value = value;
}
