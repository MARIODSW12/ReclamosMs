
using Reclamos.Domain.Exceptions;

namespace Reclamos.Domain.ValueObjects
{
    public class VOSolution
    {
        public string Value { get; private set; }

        public VOSolution(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidSolutionException();

            Value = value;
        }

        public override string ToString() => Value;
    }
}
