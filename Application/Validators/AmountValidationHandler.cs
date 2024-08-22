using Application.DTO;

namespace Application.Validators
{
    public class AmountValidationHandler : ValidationHandler<TransactionRequest>
    {
        public override void Handle(TransactionRequest request)
        {
            if (request.Amount <= 0)
            {
                throw new ArgumentException("Сумма должна быть положительным числом.");
            }

            base.Handle(request);
        }
    }


}
