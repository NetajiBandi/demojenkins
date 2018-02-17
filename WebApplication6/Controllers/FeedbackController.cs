using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication6.Models;

namespace WebApplication6.Controllers
{
    [Produces("application/json")]
    [Route("api/Feedback")]
    public class FeedbackController : Controller
    {
        private static readonly int numberOfSchools = 5;
        private static readonly int numberOfParentsPerSchool = 150;
        private static readonly int[] years = new int[] { 2016, 2017, 2018 };
        private static readonly IList<string> transportCompanies = GetUniqueEntities(entityType: "Transport Company", startIndex: 1, numberOfEntities: 3).ToList();
        private static readonly IList<string> securityServices = GetUniqueEntities(entityType: "Security Service", startIndex: 1, numberOfEntities: 3).ToList();
        private static readonly IList<string> foodAndCaterings = GetUniqueEntities(entityType: "Food And Catering", startIndex: 1, numberOfEntities: 3).ToList();
        private static readonly IList<string> healthcareProviders = GetUniqueEntities(entityType: "Healthcare Provider", startIndex: 1, numberOfEntities: 3).ToList();
        private static readonly int[,] quarters = new int[,]
        {
            { 1, 3 },
            { 4, 6 },
            { 7, 9 },
            { 10, 12 }
        };

        // GET: api/Feedback
        [HttpGet]
        public HttpResponseMessage Get()
        {
            var commaSeparatedContent = new StringBuilder();

            PrepareHeader(commaSeparatedContent);
            PrepareValues(commaSeparatedContent);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StringContent(commaSeparatedContent.ToString());
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = "Feedback.csv";
            return result;
        }

        // GET: api/Feedback/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Feedback
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Feedback/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }


        private static void PrepareHeader(StringBuilder commaSeparatedContent)
        {
            commaSeparatedContent.Append(
                "Rated By, Rated Whome, DateTimeStamp, " +
                "Quality of Service - Q1, Quality of Service - Q2, " +
                "On Time Delivery - Q1, " +
                "On Time Payment - Q1, On Time Payment - Q2, " +
                "Customer Effort Score - Q1, Customer Effort Score - Q2, Customer Effort Score - Q3, " +
                "Customer Satisfaction - Q1, Customer Satisfaction - Q2, Customer Satisfaction - Q3, Customer Satisfaction - Q4, " +
                "Net Promoter Score - Q1, Net Promoter Score - Q2\r\n");
        }

        private void PrepareValues(StringBuilder commaSeparatedContent)
        {
            var schools = GetUniqueEntities(entityType: "School", startIndex: 1, numberOfEntities: numberOfSchools);
            var schoolsWithParents = GetParentsForEverySchool(schools);

            var numberOfQuarters = quarters.GetLength(0);
            foreach (var year in years)
            {
                for (int quarter = 0; quarter < numberOfQuarters; quarter++)
                {
                    foreach (var school in schoolsWithParents.Keys)
                    {
                        SchoolRatesItSelf(commaSeparatedContent, year, quarter, school);

                        var transportCompany = transportCompanies[Randomizers.GetRandomNumber(0, transportCompanies.Count)];
                        SchoolRatesEnitity(commaSeparatedContent, year, quarter, school, transportCompany);
                        EnitityRatesSchool(commaSeparatedContent, year, quarter, school, transportCompany);

                        var securityService = securityServices[Randomizers.GetRandomNumber(0, securityServices.Count)];
                        SchoolRatesEnitity(commaSeparatedContent, year, quarter, school, securityService);
                        EnitityRatesSchool(commaSeparatedContent, year, quarter, school, securityService);

                        var foodAndCatering = foodAndCaterings[Randomizers.GetRandomNumber(0, foodAndCaterings.Count)];
                        SchoolRatesEnitity(commaSeparatedContent, year, quarter, school, foodAndCatering);
                        EnitityRatesSchool(commaSeparatedContent, year, quarter, school, foodAndCatering);

                        var healthcareProvider = healthcareProviders[Randomizers.GetRandomNumber(0, healthcareProviders.Count)];
                        SchoolRatesEnitity(commaSeparatedContent, year, quarter, school, healthcareProvider);
                        EnitityRatesSchool(commaSeparatedContent, year, quarter, school, healthcareProvider);

                        if (quarter != Randomizers.GetRandomNumber(0, numberOfQuarters))
                        {
                            ParentsRateSchool(commaSeparatedContent, schoolsWithParents, year, quarter, school);
                        }
                    }
                }
            }
        }

        private static void SchoolRatesEnitity(StringBuilder commaSeparatedContent, int year, int quarter, string school, string enitity)
        {
            commaSeparatedContent.AppendFormat("{0},", school);
            commaSeparatedContent.AppendFormat("{0},", enitity);
            AppendRandomScores(commaSeparatedContent, year, quarter);
        }

        private static void EnitityRatesSchool(StringBuilder commaSeparatedContent, int year, int quarter, string school, string enitity)
        {
            commaSeparatedContent.AppendFormat("{0},", enitity);
            commaSeparatedContent.AppendFormat("{0},", school);
            AppendRandomScores(commaSeparatedContent, year, quarter);
        }

        private static void ParentsRateSchool(StringBuilder commaSeparatedContent, Dictionary<string, IEnumerable<string>> schoolsWithParents, int year, int quarter, string school)
        {
            var currentSchoolParents = schoolsWithParents[school];
            foreach (var parent in currentSchoolParents)
            {
                commaSeparatedContent.AppendFormat("{0},", parent);
                commaSeparatedContent.AppendFormat("{0},", school);
                AppendRandomScores(commaSeparatedContent, year, quarter);
            }
        }

        private static void SchoolRatesItSelf(StringBuilder commaSeparatedContent, int year, int quarter, string school)
        {
            commaSeparatedContent.AppendFormat("{0},", school);
            commaSeparatedContent.AppendFormat("{0},", school);
            AppendRandomScores(commaSeparatedContent, year, quarter);
        }

        private Dictionary<string, IEnumerable<string>> GetParentsForEverySchool(IEnumerable<string> schools)
        {
            var parents = new Dictionary<string, IEnumerable<string>>();
            var index = 0;
            foreach (var school in schools)
            {
                IEnumerable<string> associatedParents = null;
                if (index == 0)
                {
                    associatedParents = GetUniqueEntities(entityType: "Parent", startIndex: ++index, numberOfEntities: numberOfParentsPerSchool);
                }
                else
                {
                    associatedParents = GetUniqueEntities(entityType: "Parent", startIndex: index++ * numberOfParentsPerSchool + 1, numberOfEntities: numberOfParentsPerSchool);
                }

                parents.Add(school, associatedParents);
            }

            return parents;
        }

        private static void AppendRandomScores(StringBuilder commaSeparatedContent, int year, int quarter)
        {
            var currentQuarter = GetQuarter(quarter);
            commaSeparatedContent.AppendFormat("{0},", Randomizers.GenerateRandomDate(year, currentQuarter.Item1, currentQuarter.Item2));

            commaSeparatedContent.AppendFormat("{0},", Randomizers.GetRandomNumber(1, 11));
            commaSeparatedContent.AppendFormat("{0},", Randomizers.GetRandomNumber(1, 11));
            commaSeparatedContent.AppendFormat("{0},", Randomizers.GetRandomNumber(1, 11));
            commaSeparatedContent.AppendFormat("{0},", Randomizers.GetRandomNumber(1, 11));
            commaSeparatedContent.AppendFormat("{0},", Randomizers.GetRandomNumber(1, 11));
            commaSeparatedContent.AppendFormat("{0},", Randomizers.GetRandomNumber(1, 11));
            commaSeparatedContent.AppendFormat("{0},", Randomizers.GetRandomNumber(1, 11));
            commaSeparatedContent.AppendFormat("{0},", Randomizers.GetRandomNumber(1, 11));
            commaSeparatedContent.AppendFormat("{0},", Randomizers.GetRandomNumber(1, 11));
            commaSeparatedContent.AppendFormat("{0},", Randomizers.GetRandomNumber(1, 11));
            commaSeparatedContent.AppendFormat("{0},", Randomizers.GetRandomNumber(1, 11));
            commaSeparatedContent.AppendFormat("{0},", Randomizers.GetRandomNumber(1, 11));
            commaSeparatedContent.AppendFormat("{0},", Randomizers.GetRandomNumber(1, 11));
            commaSeparatedContent.AppendFormat("{0},\r\n", Randomizers.GetRandomNumber(1, 11));
        }

        private static IEnumerable<string> GetUniqueEntities(string entityType, int startIndex, int numberOfEntities)
        {
            var parentsList = new List<string>();
            var count = 0;
            do
            {
                parentsList.Add(entityType + " " + startIndex);
                startIndex++; count++;
            } while (count < numberOfEntities);

            return parentsList;
        }

        private static Tuple<int, int> GetQuarter(int number)
        {
            return new Tuple<int, int>(quarters[number, 0], quarters[number, 1]);
        }
    }
}
