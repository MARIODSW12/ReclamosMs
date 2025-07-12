
using Reclamos.Domain.Exceptions;

namespace Reclamos.Domain.ValueObjects
{
    public class VOStatus
    {
        private readonly List<string> _validStatus =
            ["pending", "solved", "opened"];
        public string Value { get; private set; }

        public VOStatus(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidStatusException();
            if (!_validStatus.Contains(value.ToLower()))
            {
                throw new InvalidStatusException();
            }

            Value = value;
        }

        public override string ToString() => Value;
    }
}
