using Application.Interfaces;

namespace Application.Validators
{
    public class ValidationPipeline<T>
    {
        private readonly IEnumerable<IValidationHandler<T>> _handlers;

        public ValidationPipeline(IEnumerable<IValidationHandler<T>> handlers)
        {
            _handlers = handlers;
        }

        public void Validate(T request)
        {
            foreach (var handler in _handlers)
            {
                handler.Handle(request);
            }
        }
    }
}
