using Application.DTO;
using Domain.Models;

namespace Application.Validators
{
    public class DateValidationHandler : ValidationHandler<TransactionRequest>
    {
        public override void Handle(TransactionRequest request)
        {
            if (request.TransactionDate == default)
            {
                throw new ArgumentException("Дата не может быть пустой или некорректной.");
            }

            base.Handle(request);
        }
    }


}
