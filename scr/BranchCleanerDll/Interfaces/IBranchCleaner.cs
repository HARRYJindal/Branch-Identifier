using BranchCleanerDll.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchCleanerDll.Interfaces
{
    public interface IBranchCleaner
    {
        public  Task GetAllRepository(string personalToken);
        public Task<(Dictionary<string, List<string>>, string)> GetBranches(string personalToken);
        public Task<List<BranchDate>> GetCommit(string personalToken, List<string> allbranchesOfRepo, string userName, string repository);
    }
}
