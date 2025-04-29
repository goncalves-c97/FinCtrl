using FinCtrlApi.Databases.MongoDb.FinCtrl.Interfaces;
using FinCtrlLibrary.Models;
using FinCtrlLibrary.Utilities;
using Microsoft.AspNetCore.Mvc;
using static FinCtrlLibrary.Utilities.ProjEnumerators;
using System.IO;
using System.Globalization;
using System.Net;
using FinCtrlLibrary.Models.GenericModels;
using System.Linq;
using DnsClient.Protocol;
using Newtonsoft.Json;

namespace FinCtrlApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
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
            try
            {
                return Ok(GetRawData());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut, Route("PopulateDatabase")]
        public async Task<ActionResult> PopulateDatabase([FromServices] IGenericRepository<SpendingCategory> spendingCategoryRepo,
            [FromServices] IGenericRepository<PaymentCategory> paymentCategoryRepo,
            [FromServices] IGenericRepository<TagCategory> tagCategoryRepo,
            [FromServices] IGenericRepository<SpendingRule> spendingRuleRepo,
            [FromServices] ISpendingRecordRepository spendingRecordRepo)
        {
            try
            {
                List<SpendingCategory> spendingCategoriesDtb = await spendingCategoryRepo.GetListAsync();
                List<TagCategory> tagCategoriesDtb = await tagCategoryRepo.GetListAsync();
                List<PaymentCategory> paymentCategoryDtb = await paymentCategoryRepo.GetListAsync();
                List<SpendingRule> spendingRulesDtb = await spendingRuleRepo.GetListAsync();
                List<SpendingRecord> spendingRecordsDtb = await spendingRecordRepo.GetListAsync();

                List<SpendingRecord> spendingRecords = GetRawData();

                List<SpendingCategory> spendingCategories = spendingRecords
                    .Select(x => x.SpendingCategory)
                    .DistinctBy(x => x.Id)
                    .ToList();

                foreach (SpendingCategory category in spendingCategories)
                {
                    if (spendingCategoriesDtb.FirstOrDefault(x => x.Name.Equals(category.Name, StringComparison.InvariantCultureIgnoreCase)) == null)
                    {
                        await spendingCategoryRepo.InsertNewAsync(category);
                        spendingCategoriesDtb.Add(category);
                    }
                }

                List<TagCategory> tagCategories = spendingRecords
                    .SelectMany(x => x.TagCategories)
                    .DistinctBy(x => x.Id)
                    .ToList();

                foreach (TagCategory category in tagCategories)
                {
                    if (tagCategoriesDtb.FirstOrDefault(x => x.Name.Equals(category.Name, StringComparison.InvariantCultureIgnoreCase)) == null)
                    {
                        await tagCategoryRepo.InsertNewAsync(category);
                        tagCategoriesDtb.Add(category);
                    }
                }

                List<PaymentCategory> paymentCategories = spendingRecords
                    .Select(x => x.PaymentCategory)
                    .DistinctBy(x => x.Id)
                    .ToList();

                foreach (PaymentCategory category in paymentCategories)
                {
                    if (paymentCategoryDtb.FirstOrDefault(x => x.Name.Equals(category.Name, StringComparison.InvariantCultureIgnoreCase)) == null)
                    {
                        await paymentCategoryRepo.InsertNewAsync(category);
                        paymentCategoryDtb.Add(category);
                    }
                }

                List<SpendingRule> spendingRules = spendingRecords
                    .Where(x => x.SpendingRule != null)
                    .Select(x => x.SpendingRule)
                    .DistinctBy(x => x.Id)
                    .ToList();

                foreach (SpendingRule rule in spendingRules)
                {
                    if (spendingRulesDtb.FirstOrDefault(x => x.Name.Equals(rule.Name, StringComparison.InvariantCultureIgnoreCase)) == null)
                    {
                        await spendingRuleRepo.InsertNewAsync(rule);
                        spendingRulesDtb.Add(rule); 
                    }
                }

                foreach (SpendingRecord record in spendingRecords)
                {
                    if (record.SpendingCategory != null)
                    {
                        SpendingCategory? recordCategory = spendingCategoriesDtb.FirstOrDefault(x => x.Name.Equals(record.SpendingCategory.Name));
                        record.CategoryBsonId = recordCategory._id.ToString();
                    }

                    if (record.TagCategories != null && record.TagCategories.Count > 0)
                    {
                        TagCategory? tagCategory = tagCategoriesDtb.FirstOrDefault(x => x.Name.Equals(record.TagCategories[0].Name));
                        record.TagBsonIds = [tagCategory._id.ToString()];
                    }

                    if (record.PaymentCategory != null)
                    {
                        PaymentCategory? paymentCategory = paymentCategoryDtb.FirstOrDefault(x => x.Name.Equals(record.PaymentCategory.Name));
                        record.PaymentCategoryBsonId = paymentCategory._id.ToString();
                    }

                    if (spendingRecordsDtb.FirstOrDefault(x => x.Equals(record)) == null)
                        await spendingRecordRepo.InsertNewAsync(record);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, Route("GetSpendingRecordsStatistics")]
        public async Task<ActionResult> GetSpendingRecordsStatistics(
            [FromServices] IGenericRepository<SpendingCategory> spendingCategoryRepo,
            [FromServices] IGenericRepository<PaymentCategory> paymentCategoryRepo,
            [FromServices] IGenericRepository<TagCategory> tagCategoryRepo,
            [FromServices] IGenericRepository<SpendingRule> spendingRuleRepo, 
            [FromServices] ISpendingRecordRepository spendingRecordRepo)
        {
            try
            {
                List<SpendingRecord> spendingRecords = await spendingRecordRepo.GetListWithIncludesAsync(spendingCategoryRepo, paymentCategoryRepo, tagCategoryRepo, spendingRuleRepo);

                string totalSpent = GetRealValue(spendingRecords.Sum(x => x.OriginalValue));
                string averageSpent = GetRealValue(spendingRecords.Average(x => x.OriginalValue));

                var valuesPerMonth = spendingRecords
                    .GroupBy(r => new { r.DateTime.Year, r.DateTime.Month })
                    .Select(g => new
                    {
                        g.Key.Year,
                        g.Key.Month,
                        TotalValue = GetRealValue(g.Sum(r => r.OriginalValue))
                    })
                    .OrderByDescending(x => x.Year)
                    .ThenByDescending(x => x.Month)
                    .ToList();

                string averageValuePerMonth = GetRealValue(spendingRecords.Sum(x => x.OriginalValue) / valuesPerMonth.Count());

                List<SpendingRecord> top10Buys = spendingRecords.OrderByDescending(x => x.OriginalValue).Take(10).ToList();

                var top10Categories = spendingRecords
                    .GroupBy(r => r.SpendingCategory)
                    .Select(g => new
                    {
                        Category = g.Key,
                        TotalValue = g.Sum(r => r.OriginalValue),
                        TotalValueFormatted = GetRealValue(g.Sum(r => r.OriginalValue))
                    })
                    .OrderByDescending(x => x.TotalValue)
                    .Take(10);

                return Ok(JsonConvert.SerializeObject(new
                {
                    totalSpent,
                    valuesPerMonth,
                    averageValuePerMonth,
                    averageSpent,
                    top10Buys,
                    top10Categories
                }, Formatting.Indented, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore }));
            }
            catch(Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [NonAction]
        public string GetRealValue(double value)
        {
            return value.ToString("C", new CultureInfo("pt-BR"));
        }
        [NonAction]
        public List<SpendingRecord> GetRawData()
        {
            string[] fileContent = System.IO.File.ReadAllLines("C:\\Users\\carlo\\OneDrive\\Área de Trabalho\\Brute.txt");

            List<SpendingCategory> categories = [];
            List<PaymentCategory> paymentCategories = [];
            List<TagCategory> tags = [];
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
                    string rawPaymentCategory = columns[8];
                    string rawTotalValue = columns[9];

                    DateTime date = minDate.AddDays(int.Parse(rawDate) - 2); //TODO estudar o -2
                    string category = rawCategory;
                    string tag = rawTag;
                    string description = rawDescription;
                    double unitValue = string.IsNullOrEmpty(rawUnitValue) ? 0 : double.Parse(rawUnitValue, new CultureInfo("pt-BR"));
                    double discount = string.IsNullOrEmpty(rawDiscount) ? 0 : double.Parse(rawDiscount, new CultureInfo("pt-BR"));
                    int installment = string.IsNullOrEmpty(rawInstallment) ? 1 : int.Parse(rawInstallment, new CultureInfo("pt-BR"));
                    string rule = rawRule;
                    string paymentCategory = string.IsNullOrEmpty(rawPaymentCategory) ? "Não informado" : rawPaymentCategory;
                    double totalValue = string.IsNullOrEmpty(rawTotalValue) ? 0 : double.Parse(rawTotalValue, new CultureInfo("pt-BR"));

                    SpendingCategory? categoryObj = HandleNewCategoryByName(category, ref categories);

                    PaymentCategory paymentCategoryObj = HandleNewPaymentCategoryByName(paymentCategory, ref paymentCategories)!;

                    TagCategory? tagObj = HandleNewTagCategoryByName(tag, ref tags);

                    SpendingRule? spendingRule = spendingRules.FirstOrDefault(x => x.Name == rule);

                    SpendingRecord spendingRecord = new(spendingRecordId++, date, paymentCategoryObj, installment, categoryObj, tagObj, description, null, unitValue, totalValue, spendingRule, true);

                    DiscountRecord discountRecord;

                    if (discount > 0)
                    {
                        long nextDiscountRecordId = discountRecords.Count == 0 ? 1 : discountRecords.Max(x => x.Id) + 1;

                        discountRecord = new DiscountRecord(nextDiscountRecordId, spendingRecordId, discountCategories[0].Id, discount)
                        {
                            DiscountCategory = discountCategories[0]
                        };

                        discountRecords.Add(discountRecord);

                        if (spendingRecord.DiscountRecordsIds == null || spendingRecord.DiscountRecords == null)
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
                    throw new Exception($"Falha no processamento da linha: {line}. {ex.Message}");
                }
            }

            return spendingRecords;
        }

        [NonAction]
        private SpendingCategory? HandleNewCategoryByName(string categoryName, ref List<SpendingCategory> category)
        {
            if (string.IsNullOrEmpty(categoryName))
                return null;

            SpendingCategory? categoryObj = category.FirstOrDefault(x => x.Name == categoryName);

            if (categoryObj == null)
            {
                int newCategoryId = category.Count == 0 ? 1 : category.Max(x => x.Id) + 1;
                categoryObj = new(newCategoryId, categoryName);
                category.Add(categoryObj);
            }

            return categoryObj;
        }

        [NonAction]
        private TagCategory? HandleNewTagCategoryByName(string categoryName, ref List<TagCategory> category)
        {
            if (string.IsNullOrEmpty(categoryName))
                return null;

            TagCategory? categoryObj = category.FirstOrDefault(x => x.Name == categoryName);

            if (categoryObj == null)
            {
                int newCategoryId = category.Count == 0 ? 1 : category.Max(x => x.Id) + 1;
                categoryObj = new(newCategoryId, categoryName);
                category.Add(categoryObj);
            }

            return categoryObj;
        }

        [NonAction]
        private PaymentCategory? HandleNewPaymentCategoryByName(string categoryName, ref List<PaymentCategory> category)
        {
            if (string.IsNullOrEmpty(categoryName))
                return null;

            PaymentCategory? categoryObj = category.FirstOrDefault(x => x.Name == categoryName);

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
