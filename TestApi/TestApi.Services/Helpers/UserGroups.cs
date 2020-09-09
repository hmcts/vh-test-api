using System.Collections.Generic;
using TestApi.Common.Configuration;
using TestApi.Domain.Enums;

namespace TestApi.Services.Helpers
{
    public interface IUserGroupsStrategy
    {
        List<string> GetGroups();
    }

    public class UserGroups
    {
        public Dictionary<UserType, IUserGroupsStrategy> GetStrategies(UserGroupsConfiguration configuration)
        {
            return new Dictionary<UserType, IUserGroupsStrategy>
            {
                {UserType.Judge, new JudgeGroupsStrategy(configuration)},
                {UserType.Individual, new IndividualGroupsStrategy(configuration)},
                {UserType.PanelMember, new IndividualGroupsStrategy(configuration)},
                {UserType.Observer, new IndividualGroupsStrategy(configuration)},
                {UserType.Representative, new RepresentativeGroupsStrategy(configuration)},
                {UserType.CaseAdmin, new CaseAdminGroupsStrategy(configuration)},
                {UserType.VideoHearingsOfficer, new VideoHearingsOfficerGroupsStrategy(configuration)}
            };
        }
    }

    public class JudgeGroupsStrategy : IUserGroupsStrategy
    {
        private readonly UserGroupsConfiguration _configuration;

        public JudgeGroupsStrategy(UserGroupsConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<string> GetGroups()
        {
            var judgeGroups = _configuration.JudgeGroups;
            var kinlyGroups = _configuration.KinlyGroups;
            judgeGroups.AddRange(kinlyGroups);
            return judgeGroups;
        }
    }

    public class IndividualGroupsStrategy : IUserGroupsStrategy
    {
        private readonly UserGroupsConfiguration _configuration;

        public IndividualGroupsStrategy(UserGroupsConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<string> GetGroups()
        {
            return _configuration.IndividualGroups;
        }
    }

    public class RepresentativeGroupsStrategy : IUserGroupsStrategy
    {
        private readonly UserGroupsConfiguration _configuration;

        public RepresentativeGroupsStrategy(UserGroupsConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<string> GetGroups()
        {
            return _configuration.RepresentativeGroups;
        }
    }

    public class CaseAdminGroupsStrategy : IUserGroupsStrategy
    {
        private readonly UserGroupsConfiguration _configuration;

        public CaseAdminGroupsStrategy(UserGroupsConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<string> GetGroups()
        {
            return _configuration.CaseAdminGroups;
        }
    }

    public class VideoHearingsOfficerGroupsStrategy : IUserGroupsStrategy
    {
        private readonly UserGroupsConfiguration _configuration;

        public VideoHearingsOfficerGroupsStrategy(UserGroupsConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<string> GetGroups()
        {
            var vhoGroups = _configuration.VideoHearingsOfficerGroups;
            var kinlyGroups = _configuration.KinlyGroups;
            vhoGroups.AddRange(kinlyGroups);
            return vhoGroups;
        }
    }
}