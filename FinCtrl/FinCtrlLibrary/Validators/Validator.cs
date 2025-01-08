using System.Collections;
using System.Text;
using System.Xml.Linq;

namespace FinCtrlLibrary.Validators
{
    public class Errors : IEnumerable<Error>
    {
        private readonly ICollection<Error> errors = [];

        public void RegisterError(string message) => errors.Add(new Error(message));

        public IEnumerator<Error> GetEnumerator()
        {
            return errors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return errors.GetEnumerator();
        }

        public string Sumario
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var item in errors)
                    sb.AppendLine(item.Message);
                return sb.ToString();
            }
        }
    }

    public record Error(string Message);

    public abstract class Validator
    {
        private readonly Errors errors = [];

        public bool IsValid => !errors.Any();
        public Errors Errors => errors;
        protected abstract void Validate();

        protected void IdValidation(int id, bool validateZero = false)
        {
            if (!int.IsPositive(id))
                Errors.RegisterError("ID não pode ser negativo.");

            if (validateZero && id == 0)
                Errors.RegisterError("ID não pode ser 0");
        }

        protected void IdValidation(long id, bool validateZero = false)
        {
            if (!long.IsPositive(id))
                Errors.RegisterError("ID não pode ser negativo.");

            if (validateZero && id == 0)
                Errors.RegisterError("ID não pode ser 0");
        }

        protected void PositiveValueValidation(string propertyName, int value, bool validateZero = false)
        {
            if (!int.IsPositive(value))
                Errors.RegisterError($"'{propertyName}' não pode ser negativo(a).");

            if (validateZero && value == 0)
                Errors.RegisterError($"'{propertyName}' não pode ser 0");
        }

        protected void PositiveValueValidation(string propertyName, long value, bool validateZero = false)
        {
            if (!long.IsPositive(value))
                Errors.RegisterError($"'{propertyName}' não pode ser negativo(a).");

            if (validateZero && value == 0)
                Errors.RegisterError($"'{propertyName}' não pode ser 0");
        }

        protected void PositiveValueValidation(string propertyName, double value, bool validateZero = false)
        {
            if (!double.IsPositive(value))
                Errors.RegisterError($"'{propertyName}' não pode ser negativo(a).");

            if (validateZero && value == 0)
                Errors.RegisterError($"'{propertyName}' não pode ser 0");
        }

        protected void NotEmptyStringValidation(string propertyName, string propertyValue)
        {
            if (string.IsNullOrEmpty(propertyValue))
                Errors.RegisterError($"'{propertyName}' não informado(a)!");
        }

        protected void NotEmptyStringLengthValidation(string propertyName, string propertyValue, int maxLength)
        {
            NotEmptyStringValidation(propertyName, propertyValue);
            
            if (propertyValue.Length > maxLength)
                Errors.RegisterError($"'{propertyName}' deve possuir até {maxLength} caracteres");
        }

        protected void StringLengthValidation(string propertyName, string propertyValue, int minLength, int maxLength)
        {
            if (minLength > maxLength)
                throw new ArgumentException("O valor mínimo deve ser menor ou igual ao valor máximo.");

            if (propertyValue.Length != minLength && minLength == maxLength)
                Errors.RegisterError($"'{propertyName}' deve possuir {minLength} caracteres.");

            if (propertyValue.Length < minLength || propertyValue.Length > maxLength)
                Errors.RegisterError($"'{propertyName}' deve possuir entre {minLength} e {maxLength} caracteres.");
        }
    }
}
