
using Reclamos.Domain.Exceptions;

namespace Reclamos.Domain.ValueObjects
{
    public class VOMotive
    {
        public string Value { get; private set; }

        public VOMotive(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidMotiveException();

            Value = value;
        }

        public override string ToString() => Value;
    }
}
