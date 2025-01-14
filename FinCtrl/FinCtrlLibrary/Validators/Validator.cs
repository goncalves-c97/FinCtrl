using System.Collections;
using System.Text;
using System.Linq;

namespace FinCtrlLibrary.Validators
{
    public class Errors : IEnumerable<Error>
    {
        private readonly ICollection<Error> errors = [];

        public void RegisterError(Enum errorEnum, string message) => errors.Add(new Error(errorEnum, message));
        public void RegisterError(Enum errorEnum, string message, string? propertyName) => errors.Add(new Error(errorEnum, message, propertyName));

        public IEnumerator<Error> GetEnumerator()
        {
            return errors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return errors.GetEnumerator();
        }

        public string Summary
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var item in errors)
                    sb.AppendLine($"{item.ErrorEnum} - {item.Message}");
                return sb.ToString();
            }
        }
    }

    public class Error()
    {
        public Enum ErrorEnum { get; set; }
        public string? PropertyName { get; set; }
        public string Message { get; set; }

        public Error(Enum errorEnum, string message) : this()
        {
            ErrorEnum = errorEnum;
            Message = message;
        }

        public Error(Enum errorEnum, string message, string? propertyName) : this()
        {
            ErrorEnum = errorEnum;
            PropertyName = propertyName;
            Message = message;
        }
    }

    public enum GenericErrors
    {
        NegativeIdError,
        IdZeroError,
        NegativeValueError,
        ValueZeroError,
        EmptyStringError,
        StringSizeExcedeedError,
        StringMinSizeNotReachedError,
        StringOutOfSizeRangeError,
    }

    public abstract class Validator
    {
        private readonly Errors errors = [];

        public bool IsValid => !errors.Any();
        public Errors Errors => errors;
        protected abstract void Validate();

        public bool ContainsError(Enum errorEnum) => errors.Any(x => x.ErrorEnum.ToString() == errorEnum.ToString());
        public bool ContainsError(Enum errorEnum, string propertyName) => errors.Any(x => x.ErrorEnum.ToString() == errorEnum.ToString() && x.PropertyName == propertyName);
        public bool ContainsError(int enumValue, string propertyName) => errors.Any(x => Convert.ToInt32(x.ErrorEnum) == enumValue && x.PropertyName == propertyName);
        protected void IdValidation(int id, string? propertyName = null, bool validateZero = false)
        {
            if (!int.IsPositive(id))
                Errors.RegisterError(GenericErrors.NegativeIdError, "Id não pode ser negativo.", propertyName);

            if (validateZero && id == 0)
                Errors.RegisterError(GenericErrors.IdZeroError, "Id não pode ser 0", propertyName);
        }

        protected void IdValidation(long id, string? propertyName = null, bool validateZero = false)
        {
            if (!long.IsPositive(id))
                Errors.RegisterError(GenericErrors.NegativeIdError, "Id não pode ser negativo.", propertyName);

            if (validateZero && id == 0)
                Errors.RegisterError(GenericErrors.IdZeroError, "Id não pode ser 0", propertyName);
        }

        protected void PositiveValueValidation(string propertyName, int value, bool validateZero = false)
        {
            if (!int.IsPositive(value))
                Errors.RegisterError(GenericErrors.NegativeValueError, $"'{propertyName}' não pode ser negativo(a).", propertyName);

            if (validateZero && value == 0)
                Errors.RegisterError(GenericErrors.ValueZeroError, $"'{propertyName}' não pode ser 0", propertyName);
        }

        protected void PositiveValueValidation(string propertyName, long value, bool validateZero = false)
        {
            if (!long.IsPositive(value))
                Errors.RegisterError(GenericErrors.NegativeValueError, $"'{propertyName}' não pode ser negativo(a).", propertyName);

            if (validateZero && value == 0)
                Errors.RegisterError(GenericErrors.ValueZeroError, $"'{propertyName}' não pode ser 0", propertyName);
        }

        protected void PositiveValueValidation(string propertyName, double value, bool validateZero = false)
        {
            if (!double.IsPositive(value))
                Errors.RegisterError(GenericErrors.NegativeValueError, $"'{propertyName}' não pode ser negativo(a).", propertyName);

            if (validateZero && value == 0)
                Errors.RegisterError(GenericErrors.ValueZeroError, $"'{propertyName}' não pode ser 0", propertyName);
        }

        protected void NotEmptyStringValidation(string propertyName, string propertyValue)
        {
            if (string.IsNullOrEmpty(propertyValue))
                Errors.RegisterError(GenericErrors.EmptyStringError, $"'{propertyName}' não informado(a)!", propertyName);
        }

        protected void NotEmptyStringLengthValidation(string propertyName, string propertyValue, int maxLength)
        {
            NotEmptyStringValidation(propertyName, propertyValue);
            
            if (propertyValue.Length > maxLength)
                Errors.RegisterError(GenericErrors.StringSizeExcedeedError, $"'{propertyName}' deve possuir até {maxLength} caracteres", propertyName);
        }

        protected void StringLengthValidation(string propertyName, string propertyValue, int minLength, int maxLength)
        {
            if (minLength > maxLength)
                throw new ArgumentException("O valor mínimo deve ser menor ou igual ao valor máximo.", propertyName);

            if (propertyValue.Length != minLength && minLength == maxLength)
                Errors.RegisterError(GenericErrors.StringMinSizeNotReachedError, $"'{propertyName}' deve possuir {minLength} caracteres.", propertyName);

            if (propertyValue.Length < minLength || propertyValue.Length > maxLength)
                Errors.RegisterError(GenericErrors.StringOutOfSizeRangeError, $"'{propertyName}' deve possuir entre {minLength} e {maxLength} caracteres.", propertyName);
        }
    }
}
