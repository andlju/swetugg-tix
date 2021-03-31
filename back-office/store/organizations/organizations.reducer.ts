import { Reducer } from "redux";
import { Organization, OrganizationActionTypes, OrganizationsAction } from "./organizations.actions";

export interface OrganizationsState {
  organizations: {
    [key: string]: Organization;
  },
  selectedOrganizationId?: string,
  editState: {
    errorCode?: string,
    errorMessage?: string,
    saving: boolean,
  },
  createInvite: {
    loading: boolean,
    token?: string,
    errorCode?: string,
    errorMessage?: string,
  },
  visibleOrganizations: {
    ids: string[],
    loading: boolean;
  };
}

const initialState: OrganizationsState = {
  organizations: {},
  editState: {
    saving: false
  },
  createInvite: {
    loading: false,
  },
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
    case OrganizationActionTypes.LOAD_ORGANIZATIONS:
      return {
        ...state,
        visibleOrganizations: {
          ids: state?.visibleOrganizations.ids ?? [],
          loading: true
        }
      };
    case OrganizationActionTypes.LOAD_ORGANIZATION:
      return {
        ...state,
        visibleOrganizations: {
          ids: state?.visibleOrganizations.ids ?? [],
          loading: true
        }
      };
    case OrganizationActionTypes.SELECT_ORGANIZATION:
      return {
        ...state,
        selectedOrganizationId: action.payload.organizationId
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
    case OrganizationActionTypes.CREATE_ORGANIZATION:
      return {
        ...state,
        editState: {
          saving: true,
          errorCode: undefined,
          errorMessage: undefined
        }
      };
    case OrganizationActionTypes.CREATE_ORGANIZATION_COMPLETE:
      return {
        ...state,
        editState: {
          saving: false,
          errorCode: undefined,
          errorMessage: undefined
        }
      };
    case OrganizationActionTypes.CREATE_ORGANIZATION_FAILED:
      return {
        ...state,
        editState: {
          saving: false,
          errorCode: action.payload.errorCode,
          errorMessage: action.payload.errorMessage
        }
      };
    case OrganizationActionTypes.CREATE_ORGANIZATION_INVITE:
      return {
        ...state,
        createInvite: {
          loading: true,
          token: undefined,
          errorCode: undefined,
          errorMessage: undefined
        }
      };
    case OrganizationActionTypes.CREATE_ORGANIZATION_INVITE_COMPLETE:
      return {
        ...state,
        createInvite: {
          loading: false,
          token: action.payload.token,
          errorCode: undefined,
          errorMessage: undefined
        }
      };
    case OrganizationActionTypes.CREATE_ORGANIZATION_INVITE_FAILED:
      return {
        ...state,
        createInvite: {
          loading: false,
          token: undefined,
          errorCode: action.payload.errorCode,
          errorMessage: action.payload.errorMessage
        }
      };
    default:
      return state;
  }
};

export { organizationsReducer };