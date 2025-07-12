using Reclamos.Domain.Exceptions;

namespace Reclamos.Domain.ValueObjects
{
    public class VOPrizeClaimId
    {
        public string Value { get; private set; }

        public VOPrizeClaimId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidPrizeClaimIdException();

            if (!Guid.TryParse(value, out _))
                throw new InvalidPrizeClaimIdException();

            Value = value;
        }

        //public static VOId Generate() => new VOId(Guid.NewGuid().ToString());

        public override string ToString() => Value;
    }
}
