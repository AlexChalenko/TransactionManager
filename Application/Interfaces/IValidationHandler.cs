namespace Application.Interfaces
{
    public interface IValidationHandler<T>
    {
        void Handle(T request);
    }
}
