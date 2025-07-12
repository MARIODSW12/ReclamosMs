using Reclamos.Domain.Exceptions;

namespace Reclamos.Domain.ValueObjects
{
    public class VOAuctionId
    {
        public string Value { get; private set; }

        public VOAuctionId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidAuctionIdException();

            if (!Guid.TryParse(value, out _))
                throw new InvalidAuctionIdException();

            Value = value;
        }

        //public static VOId Generate() => new VOId(Guid.NewGuid().ToString());

        public override string ToString() => Value;
    }
}
