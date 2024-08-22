using Application.Interfaces;

namespace Application.Validators
{
    public abstract class ValidationHandler<T> : IValidationHandler<T>
    {
        private IValidationHandler<T>? _nextHandler;

        public IValidationHandler<T> SetNext(IValidationHandler<T> handler)
        {
            _nextHandler = handler;
            return handler;
        }

        public virtual void Handle(T request)
        {
            _nextHandler?.Handle(request);
        }
    }
}
