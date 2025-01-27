﻿using System.Collections;
using System.Text;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;
using MongoDB.Bson;

namespace FinCtrlLibrary.Validators
{
    public class Errors : IEnumerable<Error>
    {
        private readonly ICollection<Error> errors = [];

        public void RegisterError(Enum errorEnum, string message) => errors.Add(new Error(errorEnum, message));
        public void RegisterError(Enum errorEnum, string message, params string? [] propertyName) => errors.Add(new Error(errorEnum, message, propertyName));

        public IEnumerator<Error> GetEnumerator()
        {
            return errors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return errors.GetEnumerator();
        }

        [BsonIgnore]
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

        public override string ToString()
        {
            return $"{Summary}";
        }
    }

    public class Error()
    {
        [BsonIgnore]
        public Enum ErrorEnum { get; set; }
        [BsonIgnore]
        public string?[] PropertiesNames { get; set; }
        [BsonIgnore]
        public string Message { get; set; }
        [BsonIgnore]
        public string PropertiesNamesSummary
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                if (PropertiesNames == null || PropertiesNames.Length == 0)
                    return string.Empty;

                foreach (string propertyName in PropertiesNames)
                {
                    sb.Append($", {propertyName}");
                }

                return sb.ToString()[2..];
            }
        }
        public Error(Enum errorEnum, string message) : this()
        {
            ErrorEnum = errorEnum;
            Message = message;
        }

        public Error(Enum errorEnum, string message, params string[] propertyName) : this()
        {
            ErrorEnum = errorEnum;
            PropertiesNames = propertyName;
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
        NullValueError,
        InvalidObjectError,
        InvalidBsonIdError,
        SameDateTimeError,
        StartBiggerThanEndDateTimeError
    }

    public abstract class ValidatorClass
    {
        private readonly Errors errors = [];

        [BsonIgnore, JsonIgnore]
        public bool IsValid => !errors.Any();
        [BsonIgnore, JsonIgnore]
        public Errors Errors => errors;

        protected abstract void Validate();

        public bool ContainsError(Enum errorEnum) => errors.Any(x => x.ErrorEnum.ToString() == errorEnum.ToString());
        public bool ContainsError(Enum errorEnum, string propertyName) => errors.Any(x => x.ErrorEnum.ToString() == errorEnum.ToString() && x.PropertiesNames.Contains(propertyName));
        public bool ContainsError(int enumValue, string propertyName) => errors.Any(x => Convert.ToInt32(x.ErrorEnum) == enumValue && x.PropertiesNames.Contains(propertyName));

        protected void IdValidation(int id, string? propertyName = null, bool validateZero = false)
        {
            string propertyNameToConsider = propertyName != null ? propertyName : "Id";

            if (!int.IsPositive(id))
                Errors.RegisterError(GenericErrors.NegativeIdError, $"{propertyNameToConsider} não pode ser negativo.", propertyName);

            if (validateZero && id == 0)
                Errors.RegisterError(GenericErrors.IdZeroError, $"{propertyNameToConsider} não pode ser 0", propertyName);
        }

        protected void IdValidation(long id, string? propertyName = null, bool validateZero = false)
        {
            string propertyNameToConsider = propertyName != null ? propertyName : "Id";

            if (!long.IsPositive(id))
                Errors.RegisterError(GenericErrors.NegativeIdError, $"{propertyNameToConsider} não pode ser negativo.", propertyName);

            if (validateZero && id == 0)
                Errors.RegisterError(GenericErrors.IdZeroError, $"{propertyNameToConsider} não pode ser 0", propertyName);
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

        protected void NotNullValueValidation(object obj, string propertyName)
        {
            if (obj == null)
                Errors.RegisterError(GenericErrors.NullValueError, $"'{propertyName}' não pode ser nulo!", propertyName);
        }

        protected void ValidObjectValidation(ValidatorClass objWithValidator, string propertyName)
        {
            if (!objWithValidator.IsValid)
            {
                Errors.RegisterError(GenericErrors.InvalidObjectError, $"'{propertyName}' possui {objWithValidator.Errors.Count()} erro(s)");

                foreach (var erro in objWithValidator.Errors)
                {
                    Errors.RegisterError(erro.ErrorEnum, $"'{propertyName}' -> " + erro.Message, erro.PropertiesNamesSummary);
                }
            }
        }

        protected void ObjectIdValidation(string propertyName, string bsonId)
        {
            if (!ObjectId.TryParse(bsonId, out _))
                Errors.RegisterError(GenericErrors.InvalidBsonIdError, $"'{propertyName}' não é um ObjectId válido!", propertyName);
        }

        protected void StartEndDateTimeValidation(DateTime startDateTime, string startDateTimePropertyName, DateTime endDateTime, string endDateTimePropertyName, bool validateSameDateTimes = false)
        {
            if (validateSameDateTimes && startDateTime == endDateTime)
                Errors.RegisterError(GenericErrors.SameDateTimeError, $"'{startDateTimePropertyName}' e '{endDateTimePropertyName}' são iguais!", startDateTimePropertyName, endDateTimePropertyName);

            if (startDateTime > endDateTime)
                Errors.RegisterError(GenericErrors.StartBiggerThanEndDateTimeError, $"'{startDateTime}' é maior que '{endDateTime}'", startDateTimePropertyName, endDateTimePropertyName);
        }
    }
}
