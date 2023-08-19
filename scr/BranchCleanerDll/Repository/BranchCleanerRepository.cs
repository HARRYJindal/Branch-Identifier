using BranchCleanerDll.Interfaces;
using BranchCleanerDll.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Runtime.Intrinsics.X86;
using NLog;
using Microsoft.Extensions.Logging;

namespace BranchCleanerDll.Repository
{
    public class BranchCleanerRepository : IBranchCleaner
    {
        private readonly ILogger<BranchCleanerRepository> _logger;
        private string accountName = "owner name";
        private string baseURI = "base url for your oragansation";
        private readonly List<string> reposName = new List<string>() { "repo names" };
        private readonly HashSet<string> excludedBranches = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "master", "main", "performance" };

        public BranchCleanerRepository(ILogger<BranchCleanerRepository> logger)
        {
            _logger = logger;
        }

        public async Task GetAllRepository(string personalToken)
        {
            using(var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", personalToken);
                string apiUrl = $"{baseURI}/orgs/{accountName}/repos";
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("");
                }
                else
                {
                    Console.WriteLine($"Failed to retrieve repositories. Status Code: {response.StatusCode}");
                }
            }

        }
        public async Task<(Dictionary<string, List<string>>, string )> GetBranches(string personalToken)
        {
            Dictionary<string, List<string>> brancheswithreponame = new Dictionary<string, List<string>>();
            try
            {
                if (string.IsNullOrEmpty(personalToken))
                {
                    throw new Exception("personal token is not null");
                }
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", personalToken);
                    foreach (string repositoryName in reposName)
                    {
                        string finalUrl = $"{baseURI}/repos/{accountName}/{repositoryName}/branches";

                        var response = await httpClient.GetAsync(finalUrl);

                        if (response.IsSuccessStatusCode)
                        {
                            _logger.LogInformation("successfully call to url for fetching the branches");
                            string responseBody = await response.Content.ReadAsStringAsync();
                            dynamic branchesInfo = JsonConvert.DeserializeObject<List<GithubBranchResponse>>(responseBody);


                            List<string> branchlistofrepo = new List<string>();

                            foreach (var branchInfo in branchesInfo)
                            {
                                branchlistofrepo.Add(branchInfo.name);
                            }

                            brancheswithreponame.Add(repositoryName, branchlistofrepo);
                        }
                        else
                        {
                            _logger.LogError("Request to this url: "+finalUrl+" is not returing the success status code and its is returing this respinse code "+ response.StatusCode);
                        }
                    }
                }
            }

            catch (Exception ex) { return (brancheswithreponame, ex.Message); }

            return (brancheswithreponame, "");
        }

        public async Task<List<BranchDate>> GetCommit(string personalToken, List<string> allbranchesOfRepo, string userName, string repository)
        {
            personalToken = personalToken ?? throw new ArgumentNullException(nameof(personalToken), "personal token value cannot be null");
            List<BranchDate> userBranches = new List<BranchDate>();
            string owner = accountName;
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", personalToken);
                    foreach (var branchName in allbranchesOfRepo)
                    {
                        if (excludedBranches.Contains(branchName))
                        {
                            continue; // Skip these branches
                        }

                        string commitUrl = $"{baseURI}/repos/{owner}/{repository}/commits?sha={branchName}";
                        var response = await httpClient.GetAsync(commitUrl);
                        if (response.IsSuccessStatusCode)
                        {
                            _logger.LogInformation("successfully call to url for fetching the commit of branch: "+ branchName);
                            string responseBody = await response.Content.ReadAsStringAsync();
                            dynamic commitInfo = JsonConvert.DeserializeObject(responseBody);
                            if (commitInfo.Count > 0)
                            {
                                string authorName = commitInfo[0].commit.author.name;
                                string dateOfCommitString = commitInfo[0].commit.author.date.ToString("dd-MM-yyyy");
                                DateTime dateOfCommit = DateTime.Parse(dateOfCommitString);
                                DateTime presentDate = DateTime.Now;
                                TimeSpan duration = presentDate - dateOfCommit;
                                int numberOfDays = duration.Days;
                                if (string.Equals(userName, authorName, StringComparison.OrdinalIgnoreCase))
                                {
                                    BranchDate branchDate = new BranchDate();
                                    branchDate.BranchName = branchName;
                                    branchDate.NumberOfDays = numberOfDays;
                                    userBranches.Add(branchDate);
                                }
                            }
                        }
                        else
                        {
                            _logger.LogError("Request to this url: " + commitUrl + " is not returing the success status code and its is returing this respinse code " + response.StatusCode);
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return userBranches;
        }
    }
}
