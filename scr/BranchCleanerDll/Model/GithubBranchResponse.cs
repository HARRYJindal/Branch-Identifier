using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchCleanerDll.Model
{
    public class GithubBranchResponse
    {
        public string name { get; set; }
        public Commit commit { get; set; }

        public bool Protected {get; set;}
    }
}
