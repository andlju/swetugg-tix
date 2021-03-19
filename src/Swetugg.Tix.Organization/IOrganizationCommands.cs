using Swetugg.Tix.Organization.Contract;
using System;
using System.Threading.Tasks;

namespace Swetugg.Tix.Organization
{
    public interface IOrganizationQueries
    {
        Task<OrganizationInfo[]> ListOrganizations(Guid userId);
    }

    public interface IOrganizationCommands
    {
        Task CreateOrganization(OrganizationInfo info, Guid userId);
    }
}