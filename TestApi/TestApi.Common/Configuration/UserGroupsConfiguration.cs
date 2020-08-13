using System.Collections.Generic;

namespace TestApi.Common.Configuration
{
    public class UserGroupsConfiguration
    {
        public List<string> JudgeGroups { get; set; }
        public List<string> IndividualGroups { get; set; }
        public List<string> RepresentativeGroups { get; set; }
        public List<string> VideoHearingsOfficerGroups { get; set; }
        public List<string> CaseAdminGroups { get; set; }
        public List<string> KinlyGroups { get; set; }
        public string TestAccountGroup { get; set; }
        public string PerformanceTestAccountGroup { get; set; }
    }
}