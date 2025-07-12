
using Reclamos.Domain.Exceptions;

namespace Reclamos.Domain.ValueObjects
{
    public class VOEvidence
    {
        public string Value { get; private set; }

        public VOEvidence(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidEvidenceException();

            Value = value;
        }

        public override string ToString() => Value;
    }
}
