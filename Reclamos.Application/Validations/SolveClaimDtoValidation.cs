using FluentValidation;

using Reclamos.Application.DTOs;

namespace Reclamos.Application.Validations
{

    public class SolveClaimDtoValidation : AbstractValidator<SolveClaimDto>
    {
        public SolveClaimDtoValidation()
        {
            RuleFor(x => x.ClaimId)
                .NotEmpty().WithMessage("El id del reclamo es obligatorio.");

            RuleFor(x => x.Solution)
                .NotEmpty().WithMessage("La solucion es obligatoria.");

            RuleFor(x => x.SolutionDetail)
                .NotEmpty().WithMessage("El detalle de la solucion es obligatorio.");

        }
    }
}
