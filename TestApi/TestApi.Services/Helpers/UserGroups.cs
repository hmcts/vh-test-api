using System.Collections.Generic;
using TestApi.Common.Configuration;
using TestApi.Contract.Enums;

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
                {UserType.PanelMember, new JohGroupsStrategy(configuration)},
                {UserType.Winger, new JohGroupsStrategy(configuration)},
                {UserType.Observer, new IndividualGroupsStrategy(configuration)},
                {UserType.Representative, new RepresentativeGroupsStrategy(configuration)},
                {UserType.CaseAdmin, new CaseAdminGroupsStrategy(configuration)},
                {UserType.VideoHearingsOfficer, new VideoHearingsOfficerGroupsStrategy(configuration)},
                {UserType.Tester, new TesterGroupsStrategy(configuration)},
                {UserType.Witness, new WitnessGroupsStrategy(configuration)},
                {UserType.Interpreter, new InterpreterGroupsStrategy(configuration)}
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
            var judgeGroups = ConvertGroupsStringToList.Convert(_configuration.JudgeGroups);
            var kinlyGroups = ConvertGroupsStringToList.Convert(_configuration.KinlyGroups);
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
            return ConvertGroupsStringToList.Convert(_configuration.
                IndividualGroups);
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
            return ConvertGroupsStringToList.Convert(_configuration.RepresentativeGroups);
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
            return ConvertGroupsStringToList.Convert(_configuration.CaseAdminGroups);
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
            var vhoGroups = ConvertGroupsStringToList.Convert(_configuration.VideoHearingsOfficerGroups);
            var kinlyGroups = ConvertGroupsStringToList.Convert(_configuration.KinlyGroups);
            vhoGroups.AddRange(kinlyGroups);
            return vhoGroups;
        }
    }

    public class JohGroupsStrategy : IUserGroupsStrategy
    {
        private readonly UserGroupsConfiguration _configuration;

        public JohGroupsStrategy(UserGroupsConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<string> GetGroups()
        {
            return ConvertGroupsStringToList.Convert(_configuration.JudicialOfficeGroups);
        }
    }

    public class TesterGroupsStrategy : IUserGroupsStrategy
    {
        private readonly UserGroupsConfiguration _configuration;

        public TesterGroupsStrategy(UserGroupsConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<string> GetGroups()
        {
            return ConvertGroupsStringToList.Convert(_configuration.TestWebGroups);
        }
    }

    public class WitnessGroupsStrategy : IUserGroupsStrategy
    {
        private readonly UserGroupsConfiguration _configuration;

        public WitnessGroupsStrategy(UserGroupsConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<string> GetGroups()
        {
            return ConvertGroupsStringToList.Convert(_configuration.WitnessGroups);
        }
    }

    public class InterpreterGroupsStrategy : IUserGroupsStrategy
    {
        private readonly UserGroupsConfiguration _configuration;

        public InterpreterGroupsStrategy(UserGroupsConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<string> GetGroups()
        {
            return ConvertGroupsStringToList.Convert(_configuration.InterpreterGroups);
        }
    }
}