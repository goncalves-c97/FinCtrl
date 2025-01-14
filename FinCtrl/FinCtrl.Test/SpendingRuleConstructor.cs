using FinCtrlLibrary.Models;
using FinCtrlLibrary.Validators;
using static FinCtrlLibrary.Models.SpendingRule;

namespace FinCtrl.Test
{
    public class SpendingRuleConstructor
    {
        [Fact]
        public void ReturnsValidSpendingRuleWhenDataIsValid()
        {
            int id = 1;
            string spendingRuleName = "Divisão familiar";
            string rule = $"(({StringValueForReplacing} / 5) * 3) / 2"; // (({0} / 5) * 3) / 2
            bool validation = true;

            SpendingRule spendingRule = new(id, spendingRuleName, rule);

            Assert.Equal(validation, spendingRule.IsValid);
        }

        [Fact]
        public void ReturnsNegativeIdErrorWhenSpendingRuleIdIsNegative()
        {
            int id = -1;
            string spendingRuleName = "Divisão familiar";
            string rule = $"(({StringValueForReplacing} / 5) * 3) / 2"; // (({0} / 5) * 3) / 2
            bool validation = false;

            SpendingRule spendingRule = new(id, spendingRuleName, rule);

            Assert.Equal(validation, spendingRule.IsValid);
            Assert.True(spendingRule.ContainsError(GenericErrors.NegativeIdError, nameof(spendingRule.Id)));
        }

        [Fact]
        public void ReturnsEmptyStringErrorWhenSpendingRuleNameIsEmpty()
        {
            int id = 1;
            string spendingRuleName = string.Empty;
            string rule = $"(({StringValueForReplacing} / 5) * 3) / 2"; // (({0} / 5) * 3) / 2
            bool validation = false;

            SpendingRule spendingRule = new(id, spendingRuleName, rule);

            Assert.Equal(validation, spendingRule.IsValid);
            Assert.True(spendingRule.ContainsError(GenericErrors.EmptyStringError, nameof(spendingRule.Name)));
        }

        [Fact]
        public void ReturnsNoReplaceableTextForExpressionErrorWhenSpendingRuleDoesntHaveStringForReplacing()
        {
            int id = 1;
            string spendingRuleName = "Divisão familiar";
            string rule = $"((x / 5) * 3) / 2"; // (({0} / 5) * 3) / 2
            bool validation = false;

            SpendingRule spendingRule = new(id, spendingRuleName, rule);

            Assert.Equal(validation, spendingRule.IsValid);
            Assert.True(spendingRule.ContainsError(SpendingRuleErrors.NoReplaceableStringForExpressionError));
        }

        [Fact]
        public void ReturnsInfinityResultErrorWhenRuleForcesDivZeroResult()
        {
            int id = 1;
            string spendingRuleName = "Divisão familiar";
            string rule = $"10 / {StringValueForReplacing}"; // (({0} / 5) * 3) / 2
            bool validation = false;

            SpendingRule spendingRule = new(id, spendingRuleName, rule);

            Assert.Equal(validation, spendingRule.IsValid);
            Assert.True(spendingRule.ContainsError(SpendingRuleErrors.InifinityResultError));
        }
    }
}
