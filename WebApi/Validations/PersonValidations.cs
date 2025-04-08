using FluentValidation;
using WebApi.Models;

namespace WebApi.Validations
{
    public class PersonValidations :AbstractValidator<Person>

    {
        public PersonValidations() {
            
            RuleFor(x=> x.Name).NotEmpty().MinimumLength(3).MaximumLength(16).WithMessage("El campo {PropertyName} es requerido");
            RuleFor(x => x.Age).NotEmpty().InclusiveBetween(18, 100);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }

    }
}
