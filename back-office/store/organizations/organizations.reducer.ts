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
import { initState, setFailed, setFetching, setSaving, setState, TixState } from '../common/state.models';
import {
  Organization,
  OrganizationActionTypes,
  OrganizationInvite,
  OrganizationInviteToken,
  OrganizationsAction,
} from './organizations.actions';

export interface OrganizationsState {
  organizations: TixListState<Organization>;
  selectedOrganizationId?: string;
  organization: TixState<Organization>;
  organizationUsers: TixListState<User>;
  createInvite: TixState<OrganizationInviteToken>;
  invite: TixState<OrganizationInvite>;
}

const initialState: OrganizationsState = {
  organizations: initListState((org) => org.organizationId),
  organization: initState(),
  organizationUsers: initListState((user) => user.userId || ''),
  createInvite: initState(),
  invite: initState(),
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
        invite: setFetching(state.invite),
      };
    case OrganizationActionTypes.LOAD_ORGANIZATION_INVITE_COMPLETE:
      return {
        ...state,
        invite: setState(state.invite, {
          invitedByUser: action.payload.invite.invitedByUser,
          organization: action.payload.invite.organization,
          accepted: false,
        }),
      };
    case OrganizationActionTypes.LOAD_ORGANIZATION_INVITE_FAILED:
      return {
        ...state,
        invite: setFailed(state.invite, action.payload.errorCode, action.payload.errorMessage),
      };
    case OrganizationActionTypes.SELECT_ORGANIZATION:
      return {
        ...state,
        selectedOrganizationId: action.payload.organizationId,
      };
    case OrganizationActionTypes.CREATE_ORGANIZATION:
      return {
        ...state,
        organization: setSaving(state.organization),
      };
    case OrganizationActionTypes.CREATE_ORGANIZATION_COMPLETE:
      return {
        ...state,
        organization: setState(state.organization, state.organization.current),
      };
    case OrganizationActionTypes.CREATE_ORGANIZATION_FAILED:
      return {
        ...state,
        organization: setFailed(state.organization, action.payload.errorCode, action.payload.errorMessage),
      };
    case OrganizationActionTypes.CREATE_ORGANIZATION_INVITE:
      return {
        ...state,
        createInvite: setSaving(state.createInvite),
      };
    case OrganizationActionTypes.CREATE_ORGANIZATION_INVITE_COMPLETE:
      return {
        ...state,
        createInvite: setState(state.createInvite, { token: action.payload.token }),
      };
    case OrganizationActionTypes.CREATE_ORGANIZATION_INVITE_FAILED:
      return {
        ...state,
        createInvite: setFailed(state.createInvite, action.payload.errorCode, action.payload.errorMessage),
      };
    case OrganizationActionTypes.ACCEPT_ORGANIZATION_INVITE:
      return {
        ...state,
        invite: setSaving(state.invite),
      };
    case OrganizationActionTypes.ACCEPT_ORGANIZATION_INVITE_COMPLETE:
      return {
        ...state,
        invite: setState(state.invite, state.invite.current && { ...state.invite.current, accepted: true }),
      };
    case OrganizationActionTypes.ACCEPT_ORGANIZATION_INVITE_FAILED:
      return {
        ...state,
        invite: setFailed(state.invite, action.payload.errorCode, action.payload.errorMessage),
      };
    default:
      return state;
  }
};

export { organizationsReducer };
