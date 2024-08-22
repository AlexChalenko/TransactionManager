using Application.DTO;

namespace Application.Validators
{
    public class IdValidationHandler : ValidationHandler<TransactionRequest>
    {
        public override void Handle(TransactionRequest request)
        {
            if (request.Id <= 0)
            {
                throw new ArgumentException("Id должен быть положительным числом.");
            }

            base.Handle(request);
        }
    }


}
