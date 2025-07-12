using FluentValidation;

using Reclamos.Application.DTOs;

namespace Reclamos.Application.Validations
{

    public class CreateClaimDtoValidation : AbstractValidator<CreateClaimDto>
    {
        public CreateClaimDtoValidation()
        {
            RuleFor(x => x.Motive)
                .NotEmpty().WithMessage("El motivo es obligatorio.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("El ID del usuario es obligatorio.");

            RuleFor(x => x.AuctionId)
                .NotEmpty().WithMessage("El ID de la subasta es obligatorio.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La Descripcion es obligatoria.");

        }
    }
}
