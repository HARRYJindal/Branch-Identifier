namespace BranchCleaner.Models
{
    public class RepoBranchDays
    {
        public string repoName { get; set; }
        public List<BranchDays> branchDays { get; set; }
    }
}
