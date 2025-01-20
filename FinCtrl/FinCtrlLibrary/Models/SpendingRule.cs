using FinCtrlLibrary.Exceptions;
using FinCtrlLibrary.Validators;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Xml.XPath;

namespace FinCtrlLibrary.Models
{
    public class SpendingRule : Validator
    {
        private const int _nameMaxLength = 30;
        private const int _ruleMaxLength = 100;
        private const string _stringValueForReplacing = "{0}";

        public int Id { get; set; }
        public string Name { get; set; }
        public string Rule { get; set; }
        public static string StringValueForReplacing => _stringValueForReplacing;

        public SpendingRule(int id, string name, string rule)
        {
            Id = id;
            Name = name;
            Rule = rule;
            Validate();
        }

        public enum SpendingRuleCustomErrors
        {
            NoReplaceableStringForExpressionError,
            InvalidExpressionError,
            ExpressionCalculationError,
            InifinityResultError
        }

        public double CalculateValueByRule(double value)
        {
            string expressionToBeCalculated = GetExpressionToBeCalculated(value);

            XPathDocument xPathDocument = new(new StringReader("<r/>"));

            double resultDoubleValue = (double)xPathDocument.CreateNavigator().Evaluate(string.Format("number({0})", expressionToBeCalculated));

            return resultDoubleValue;
        }

        private static double CalculateValueByExpression(string expressionToBeCalculated)
        {
            XPathDocument xPathDocument = new(new StringReader("<r/>"));

            double resultDoubleValue = (double)xPathDocument.CreateNavigator().Evaluate(string.Format("number({0})", expressionToBeCalculated));

            if (double.IsInfinity(resultDoubleValue) || double.IsNegativeInfinity(resultDoubleValue))
                throw new InfinityValueException("Resultado da conta gerou valor infinito");
            
            return resultDoubleValue;
        }

        private string GetExpressionToBeCalculated(double value)
        {
            string baseExpression = Rule.Replace(_stringValueForReplacing, value.ToString()).Replace(',', '.');
            
            Regex mathSymbolsRegex = new(@"([\+\-\*])");

            string expressionToBeCalculated = mathSymbolsRegex
                .Replace(baseExpression, " ${1} ")
                .Replace("/", " div ")
                .Replace("%", " mod ")
                .Replace("x", " * ");

            return expressionToBeCalculated;
        }

        protected override void Validate()
        {
            IdValidation(Id, nameof(Id));
            NotEmptyStringLengthValidation(nameof(Name), Name, _nameMaxLength);
            NotEmptyStringLengthValidation(nameof(Rule), Rule, _ruleMaxLength);

            if (!Rule.Contains(_stringValueForReplacing))
                Errors.RegisterError(SpendingRuleCustomErrors.NoReplaceableStringForExpressionError, "A regra deve conter o texto de substituição para cálculo.");

            if (!Errors.Any())
            {
                string expressionToBeCalculatedWithInput0 = GetExpressionToBeCalculated(0);

                try
                {
                    CalculateValueByExpression(expressionToBeCalculatedWithInput0);
                }
                catch (InfinityValueException)
                {
                    Errors.RegisterError(SpendingRuleCustomErrors.InifinityResultError, "Resultado do teste resultou em valor infinito.");
                }
                catch (XPathException ex)
                {
                    Errors.RegisterError(SpendingRuleCustomErrors.InvalidExpressionError, $"Expressão ({expressionToBeCalculatedWithInput0}) gerada pela regra possui sintaxe ou caracteres inválidos! {ex.Message}");
                }
                catch (Exception ex)
                {
                    Errors.RegisterError(SpendingRuleCustomErrors.ExpressionCalculationError, $"Houve algum erro para calcular a expressão: {expressionToBeCalculatedWithInput0}. {ex.Message}");
                }

                string expressionToBeCalculatedWithInput100 = GetExpressionToBeCalculated(100);

                try
                {
                    CalculateValueByExpression(expressionToBeCalculatedWithInput100);
                }
                catch (InfinityValueException)
                {
                    Errors.RegisterError(SpendingRuleCustomErrors.InifinityResultError, "Resultado do teste resultou em valor infinito.");
                }
                catch (XPathException ex)
                {
                    Errors.RegisterError(SpendingRuleCustomErrors.InvalidExpressionError, $"Expressão ({expressionToBeCalculatedWithInput100}) gerada pela regra possui sintaxe ou caracteres inválidos! {ex.Message}");
                }
                catch (Exception ex)
                {
                    Errors.RegisterError(SpendingRuleCustomErrors.ExpressionCalculationError, $"Houve algum erro para calcular a expressão: {expressionToBeCalculatedWithInput100}. {ex.Message}");
                }
            }
        }
    }
}
