using BoticaPOS.Domain.Entities;
using FluentValidation;

namespace BoticaPOS.Application.Validators;

public sealed class MedicamentoValidator : AbstractValidator<Medicamento>
{
    public MedicamentoValidator()
    {
        RuleFor(x => x.NombreComercial).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PrecioVenta).GreaterThan(0);
        RuleFor(x => x.StockMinimo).GreaterThanOrEqualTo(0);
    }
}
