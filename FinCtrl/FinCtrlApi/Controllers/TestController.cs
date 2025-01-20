using FinCtrlApi.Databases.MongoDb.FinCtrl.Interfaces;
using FinCtrlLibrary.Models;
using FinCtrlLibrary.Utilities;
using Microsoft.AspNetCore.Mvc;
using static FinCtrlLibrary.Utilities.ProjEnumerators;
using System.IO;
using System.Globalization;

namespace FinCtrlApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet, Route("GetList")]
        public ActionResult<Category> GetList([FromServices] ICategory categoryRepo)
        {
            return Ok(categoryRepo.GetList());
        }

        [HttpPost, Route("InsertNewMock")]
        public ActionResult InsertNewMock([FromServices] ICategory categoryRepo)
        {
            categoryRepo.InsertNew(new Category(1, "New Fiesta"));
            return Ok();
        }

        [HttpDelete, Route("DeleteById/{id}")]
        public ActionResult InsertNewMock(int id, [FromServices] ICategory categoryRepo)
        {
            categoryRepo.DeleteById(id);
            return Ok();
        }

        [HttpGet, Route("CalculateWorkTimeForPayment/{paidValue}")]
        public ActionResult CalculateWorkTimeForPayment(double paidValue)
        {
            WorkHoursAndSalary workHoursAndSalary = new WorkHoursAndSalary(20, 8, 5000);
            return Ok(workHoursAndSalary.CalculateWorkTimeForPayment(paidValue));
        }

        [HttpGet, Route("GetSalaryByBaseTimePeriodAndValue/{period}/{value}")]
        public ActionResult GetSalaryByBaseTimePeriodAndValue(BaseTimePeriod period, double value)
        {
            WorkHoursAndSalary workHoursAndSalary = new(20, 8, 5000);
            MoneyByTimeCalculator moneyInputByTimeCalculator = new MoneyByTimeCalculator(workHoursAndSalary);
            moneyInputByTimeCalculator.UpdateInputValue(period, value);
            return Ok(moneyInputByTimeCalculator);
        }

        [HttpGet, Route("GetSalaryPercentageByValue/{value}")]
        public ActionResult GetSalaryPercentageByValue(double value)
        {
            WorkHoursAndSalary workHoursAndSalary = new(20, 8, 5000);
            return Ok(workHoursAndSalary.CalculateSalaryPercentageForPayment(value));
        }

        [HttpGet, Route("ProcessBrute")]
        public ActionResult ProcessBrute()
        {
            string[] fileContent = System.IO.File.ReadAllLines("C:\\Users\\carlos\\Documents\\Pessoal\\FinCtrl\\Brute.txt");

            List<Category> categories = [];
            List<Category> spendingCategories = [];
            List<Category> tags = [];
            List<SpendingRule> spendingRules =
                [
                    new (1, "DIV/2 PAIS", $"{SpendingRule.StringValueForReplacing} / 2"),
                    new (2, "DIV/2 TB", $"{SpendingRule.StringValueForReplacing} / 2"),
                    new (3, "DIV/5*3 PAIS", $"(({SpendingRule.StringValueForReplacing} / 5) * 3) / 2"
                )];
            List<SpendingRecord> spendingRecords = [];
            List<DiscountRecord> discountRecords = [];

            List<Category> discountCategories = [new Category(1, "Adiantamento de parcelas")];

            DateTime minDate = new(1900, 1, 1);

            int spendingRecordId = 0;

            foreach (string line in fileContent)
            {
                try
                {
                    string[] columns = line.Split('|');

                    string rawDate = columns[0];
                    string rawCategory = columns[1];
                    string rawTag = columns[2];
                    string rawDescription = columns[3];
                    string rawUnitValue = columns[4];
                    string rawDiscount = columns[5];
                    string rawInstallment = columns[6];
                    string rawRule = columns[7];
                    string rawSpendingCategory = columns[8];
                    string rawTotalValue = columns[9];


                    DateTime date = minDate.AddDays(int.Parse(rawDate) - 2); //TODO estudar o -2
                    string category = rawCategory;
                    string tag = rawTag;
                    string description = rawDescription;
                    double unitValue = string.IsNullOrEmpty(rawUnitValue) ? 0 : double.Parse(rawUnitValue, new CultureInfo("pt-BR"));
                    double discount = string.IsNullOrEmpty(rawDiscount) ? 0 : double.Parse(rawDiscount, new CultureInfo("pt-BR"));
                    int installment = string.IsNullOrEmpty(rawInstallment) ? 1 : int.Parse(rawInstallment, new CultureInfo("pt-BR"));
                    string rule = rawRule;
                    string spendingCategory = string.IsNullOrEmpty(rawSpendingCategory) ? "Não informado" : rawSpendingCategory;
                    double totalValue = string.IsNullOrEmpty(rawTotalValue) ? 0 : double.Parse(rawTotalValue, new CultureInfo("pt-BR"));

                    Category? categoryObj = HandleNewCategoryByName(category, ref categories);

                    Category spendingCategoryObj = HandleNewCategoryByName(spendingCategory, ref spendingCategories)!;

                    Category? tagObj = HandleNewCategoryByName(tag, ref tags);

                    SpendingRule? spendingRule = spendingRules.FirstOrDefault(x => x.Name == rule);

                    SpendingRecord spendingRecord = new(spendingRecordId++, date, spendingCategoryObj, installment, categoryObj, tagObj, description, null, unitValue, totalValue, spendingRule, true);

                    DiscountRecord discountRecord;

                    if (discount > 0)
                    {
                        long nextDiscountRecordId = discountRecords.Count == 0 ? 1 : discountRecords.Max(x => x.Id) + 1;

                        discountRecord = new DiscountRecord(nextDiscountRecordId, spendingRecordId, discountCategories[0].Id, discount)
                        {
                            DiscountCategory = discountCategories[0]
                        };

                        discountRecords.Add(discountRecord);

                        if(spendingRecord.DiscountRecordsIds == null || spendingRecord.DiscountRecords == null)
                        {
                            spendingRecord.DiscountRecords = [];
                            spendingRecord.DiscountRecordsIds = [];
                        }

                        spendingRecord.DiscountRecordsIds!.Add(discountRecord.Id);
                        spendingRecord.DiscountRecords!.Add(discountRecord);
                    }

                    if (spendingRecord.SpendingRule != null)
                        spendingRecord.OriginalValue = spendingRecord.SpendingRule.CalculateValueByRule(spendingRecord.UnitValue);

                    spendingRecords.Add(spendingRecord);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Falha no processamento da linha: {line}. {ex.Message}");
                }
            }

            return Ok(spendingRecords);
        }

        [NonAction]
        private Category? HandleNewCategoryByName(string categoryName, ref List<Category> category)
        {
            if (string.IsNullOrEmpty(categoryName))
                return null;

            Category? categoryObj = category.FirstOrDefault(x => x.Name == categoryName);

            if (categoryObj == null)
            {
                int newCategoryId = category.Count == 0 ? 1 : category.Max(x => x.Id) + 1;
                categoryObj = new(newCategoryId, categoryName);
                category.Add(categoryObj);
            }

            return categoryObj;
        }
    }
}
