using System.Collections.Generic;
using TestApi.Common.Configuration;
using TestApi.Domain.Enums;

namespace TestApi.Services.Helpers
{
    public interface IUserGroupsStrategy
    {
        List<string> GetGroups(UserGroupsConfiguration configuration);
    }

    public class UserGroups
    {
        public Dictionary<UserType, IUserGroupsStrategy> GetStrategies()
        {
            return new Dictionary<UserType, IUserGroupsStrategy>
            {
                {UserType.Judge, new JudgeGroupsStrategy()},
                {UserType.Individual, new IndividualGroupsStrategy()},
                {UserType.PanelMember, new IndividualGroupsStrategy()},
                {UserType.Observer, new IndividualGroupsStrategy()},
                {UserType.Representative, new RepresentativeGroupsStrategy()},
                {UserType.CaseAdmin, new CaseAdminGroupsStrategy()},
                {UserType.VideoHearingsOfficer, new VideoHearingsOfficerGroupsStrategy()}
            };
        }
    }

    public class JudgeGroupsStrategy : IUserGroupsStrategy
    {
        public List<string> GetGroups(UserGroupsConfiguration configuration)
        {
            var judgeGroups = configuration.JudgeGroups;
            var kinlyGroups = configuration.KinlyGroups;
            judgeGroups.AddRange(kinlyGroups);
            return judgeGroups;
        }
    }

    public class IndividualGroupsStrategy : IUserGroupsStrategy
    {
        public List<string> GetGroups(UserGroupsConfiguration configuration)
        {
            return configuration.IndividualGroups;
        }
    }

    public class RepresentativeGroupsStrategy : IUserGroupsStrategy
    {
        public List<string> GetGroups(UserGroupsConfiguration configuration)
        {
            return configuration.RepresentativeGroups;
        }
    }

    public class CaseAdminGroupsStrategy : IUserGroupsStrategy
    {
        public List<string> GetGroups(UserGroupsConfiguration configuration)
        {
            return configuration.CaseAdminGroups;
        }
    }

    public class VideoHearingsOfficerGroupsStrategy : IUserGroupsStrategy
    {
        public List<string> GetGroups(UserGroupsConfiguration configuration)
        {
            var vhoGroups = configuration.VideoHearingsOfficerGroups;
            var kinlyGroups = configuration.KinlyGroups;
            vhoGroups.AddRange(kinlyGroups);
            return vhoGroups;
        }
    }
}