import { Reducer } from "redux";
import { Organization, OrganizationActionTypes, OrganizationsAction } from "./organizations.actions";

export interface OrganizationsState {
  organizations: {
    [key: string]: Organization;
  },
  visibleOrganizations: {
    ids: string[],
    loading: boolean;
  };
}

const initialState: OrganizationsState = {
  organizations: {},
  visibleOrganizations: {
    ids: [],
    loading: false
  }
};

const organizationsReducer: Reducer<OrganizationsState, OrganizationsAction> = (state, action) => {
  if (!state) {
    state = initialState;
  }
  switch (action.type) {
    case OrganizationActionTypes.LOAD_ORGANIZATION:
      return {
        ...state,
        visibleOrganizations: {
          ids: state?.visibleOrganizations.ids ?? [],
          loading: true
        }
      };
    case OrganizationActionTypes.LOAD_ORGANIZATIONS:
      return {
        ...state,
        visibleOrganizations: {
          ids: state?.visibleOrganizations.ids ?? [],
          loading: true
        }
      };
    case OrganizationActionTypes.LOAD_ORGANIZATIONS_COMPLETE:
      return {
        ...state,
        organizations: action.payload.organizations.reduce((organizations, organization) => ({ ...organizations, [organization.organizationId]: organization }), state.organizations),
        visibleOrganizations: {
          ids: action.payload.organizations.map(o => o.organizationId),
          loading: false
        }
      };
    case OrganizationActionTypes.LOAD_ORGANIZATIONS_FAILED:
      return {
        ...state,
        visibleOrganizations: {
          ids: state?.visibleOrganizations.ids ?? [],
          loading: false
        }
      };
    default:
      return state;
  }
};

export { organizationsReducer };