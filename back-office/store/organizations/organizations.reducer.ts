import { Reducer } from 'redux';

import { User } from '../auth/auth.actions';
import {
  initListState,
  loadList,
  loadListComplete,
  loadListFailed,
  loadListItem,
  TixListState,
} from '../common/list-state.models';
import { Organization, OrganizationActionTypes, OrganizationsAction } from './organizations.actions';

export interface OrganizationsState {
  organizations: TixListState<Organization>;
  selectedOrganizationId?: string;
  editState: {
    errorCode?: string;
    errorMessage?: string;
    saving: boolean;
  };
  organizationUsers: TixListState<User>;
  createInvite: {
    loading: boolean;
    token?: string;
    errorCode?: string;
    errorMessage?: string;
  };
  invite: {
    loading: boolean;
    invitedByUser?: User;
    organization?: Organization;
    accepted: boolean;
    errorCode?: string;
    errorMessage?: string;
  };
}

const initialState: OrganizationsState = {
  organizations: initListState((org) => org.organizationId),
  editState: {
    saving: false,
  },
  organizationUsers: initListState((user) => user.userId || ''),
  createInvite: {
    loading: false,
  },
  invite: {
    loading: false,
    accepted: false,
  },
};

const organizationsReducer: Reducer<OrganizationsState, OrganizationsAction> = (state, action) => {
  if (!state) {
    state = initialState;
  }
  switch (action.type) {
    case OrganizationActionTypes.LOAD_ORGANIZATIONS:
      return {
        ...state,
        organizations: loadList(state.organizations),
      };
    case OrganizationActionTypes.LOAD_ORGANIZATION:
      return {
        ...state,
        organizations: loadListItem(state.organizations, action.payload.organizationId),
      };
    case OrganizationActionTypes.LOAD_ORGANIZATIONS_COMPLETE:
      return {
        ...state,
        organizations: loadListComplete(state.organizations, action.payload.organizations),
      };
    case OrganizationActionTypes.LOAD_ORGANIZATIONS_FAILED:
      return {
        ...state,
        organizations: loadListFailed(state.organizations, action.payload.errorCode, action.payload.errorMessage),
      };
    case OrganizationActionTypes.LOAD_ORGANIZATION_USERS:
      return {
        ...state,
        organizationUsers: loadList(state.organizationUsers),
      };
    case OrganizationActionTypes.LOAD_ORGANIZATION_USERS_COMPLETE:
      return {
        ...state,
        organizationUsers: loadListComplete(state.organizationUsers, action.payload.users),
      };
    case OrganizationActionTypes.LOAD_ORGANIZATION_USERS_FAILED:
      return {
        ...state,
        organizationUsers: loadListFailed(state.organizationUsers, action.payload.errorCode, action.payload.errorMessage),
      };
    case OrganizationActionTypes.LOAD_ORGANIZATION_INVITE:
      return {
        ...state,
        invite: {
          ...state.invite,
          errorCode: undefined,
          errorMessage: undefined,
          loading: true,
        },
      };
    case OrganizationActionTypes.LOAD_ORGANIZATION_INVITE_COMPLETE:
      return {
        ...state,
        invite: {
          ...state.invite,
          invitedByUser: action.payload.invite.invitedByUser,
          organization: action.payload.invite.organization,
          loading: false,
        },
      };
    case OrganizationActionTypes.LOAD_ORGANIZATION_INVITE_FAILED:
      return {
        ...state,
        invite: {
          ...state.invite,
          errorCode: action.payload.errorCode,
          errorMessage: action.payload.errorMessage,
          loading: false,
        },
      };
    case OrganizationActionTypes.SELECT_ORGANIZATION:
      return {
        ...state,
        selectedOrganizationId: action.payload.organizationId,
      };
    case OrganizationActionTypes.CREATE_ORGANIZATION:
      return {
        ...state,
        editState: {
          saving: true,
          errorCode: undefined,
          errorMessage: undefined,
        },
      };
    case OrganizationActionTypes.CREATE_ORGANIZATION_COMPLETE:
      return {
        ...state,
        editState: {
          saving: false,
        },
      };
    case OrganizationActionTypes.CREATE_ORGANIZATION_FAILED:
      return {
        ...state,
        editState: {
          saving: false,
          errorCode: action.payload.errorCode,
          errorMessage: action.payload.errorMessage,
        },
      };
    case OrganizationActionTypes.CREATE_ORGANIZATION_INVITE:
      return {
        ...state,
        createInvite: {
          loading: true,
          token: undefined,
          errorCode: undefined,
          errorMessage: undefined,
        },
      };
    case OrganizationActionTypes.CREATE_ORGANIZATION_INVITE_COMPLETE:
      return {
        ...state,
        createInvite: {
          loading: false,
          token: action.payload.token,
          errorCode: undefined,
          errorMessage: undefined,
        },
      };
    case OrganizationActionTypes.CREATE_ORGANIZATION_INVITE_FAILED:
      return {
        ...state,
        createInvite: {
          loading: false,
          token: undefined,
          errorCode: action.payload.errorCode,
          errorMessage: action.payload.errorMessage,
        },
      };
    case OrganizationActionTypes.ACCEPT_ORGANIZATION_INVITE:
      return {
        ...state,
        invite: {
          ...state.invite,
          accepted: true,
          loading: true,
        },
      };
    case OrganizationActionTypes.ACCEPT_ORGANIZATION_INVITE_COMPLETE:
      return {
        ...state,
        invite: {
          ...state.invite,
          loading: false,
          accepted: true,
          errorCode: undefined,
          errorMessage: undefined,
        },
      };
    case OrganizationActionTypes.ACCEPT_ORGANIZATION_INVITE_FAILED:
      return {
        ...state,
        invite: {
          ...state.invite,
          loading: false,
          accepted: false,
          errorCode: action.payload.errorCode,
          errorMessage: action.payload.errorMessage,
        },
      };
    default:
      return state;
  }
};

export { organizationsReducer };
