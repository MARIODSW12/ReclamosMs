
using Reclamos.Domain.Exceptions;

namespace Reclamos.Domain.ValueObjects
{
    public class VOSolutionDetail
    {
        public string Value { get; private set; }

        public VOSolutionDetail(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidSolutionDetailException();

            Value = value;
        }

        public override string ToString() => Value;
    }
}
