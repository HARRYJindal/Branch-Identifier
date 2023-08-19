using AutoMapper;
using BranchCleaner.Models;
using BranchCleanerDll.Model;
using System.Security.Cryptography.Xml;

namespace BranchCleaner.Mapper
{
    public class BranchDays_BranchDate:Profile
    {
        public BranchDays_BranchDate(): base("BranchDays_BranchDate")
        {
            CreateMap<BranchDays, BranchDate>().ReverseMap();
        }
    }
}
