import { Action } from 'redux';

import { User } from '../auth/auth.actions';

export interface Organization {
  organizationId: string;
  name: string;
}

export interface OrganizationInvite {
  organization: Organization;
  invitedByUser: User;
}

export enum OrganizationActionTypes {
  LOAD_ORGANIZATION = 'LOAD_ORGANIZATION',
  LOAD_ORGANIZATIONS = 'LOAD_ORGANIZATIONS',
  LOAD_ORGANIZATIONS_COMPLETE = 'LOAD_ORGANIZATIONS_COMPLETE',
  LOAD_ORGANIZATIONS_FAILED = 'LOAD_ORGANIZATIONS_FAILED',

  LOAD_ORGANIZATION_USERS = 'LOAD_ORGANIZATION_USERS',
  LOAD_ORGANIZATION_USERS_COMPLETE = 'LOAD_ORGANIZATION_USERS_COMPLETE',
  LOAD_ORGANIZATION_USERS_FAILED = 'LOAD_ORGANIZATION_USERS_FAILED',

  LOAD_ORGANIZATION_INVITE = 'LOAD_ORGANIZATION_INVITE',
  LOAD_ORGANIZATION_INVITE_COMPLETE = 'LOAD_ORGANIZATION_INVITE_COMPLETE',
  LOAD_ORGANIZATION_INVITE_FAILED = 'LOAD_ORGANIZATION_INVITE_FAILED',

  SELECT_ORGANIZATION = 'SELECT_ORGANIZATION',

  CREATE_ORGANIZATION = 'CREATE_ORGANIZATION',
  CREATE_ORGANIZATION_COMPLETE = 'CREATE_ORGANIZATION_COMPLETE',
  CREATE_ORGANIZATION_FAILED = 'CREATE_ORGANIZATION_FAILED',

  CREATE_ORGANIZATION_INVITE = 'CREATE_ORGANIZATION_INVITE',
  CREATE_ORGANIZATION_INVITE_COMPLETE = 'CREATE_ORGANIZATION_INVITE_COMPLETE',
  CREATE_ORGANIZATION_INVITE_FAILED = 'CREATE_ORGANIZATION_INVITE_FAILED',

  ACCEPT_ORGANIZATION_INVITE = 'ACCEPT_ORGANIZATION_INVITE',
  ACCEPT_ORGANIZATION_INVITE_COMPLETE = 'ACCEPT_ORGANIZATION_INVITE_COMPLETE',
  ACCEPT_ORGANIZATION_INVITE_FAILED = 'ACCEPT_ORGANIZATION_INVITE_FAILED',
}

export function loadOrganizations(): LoadOrganizationsAction {
  return {
    type: OrganizationActionTypes.LOAD_ORGANIZATIONS,
  };
}

export function loadOrganization(organizationId: string): LoadOrganizationAction {
  return {
    type: OrganizationActionTypes.LOAD_ORGANIZATION,
    payload: {
      organizationId: organizationId,
    },
  };
}

export function loadOrganizationsComplete(organizations: Organization[]): LoadOrganizationsCompleteAction {
  return {
    type: OrganizationActionTypes.LOAD_ORGANIZATIONS_COMPLETE,
    payload: {
      organizations: organizations,
    },
  };
}

export function selectOrganization(organizationId: string): SelectOrganizationAction {
  return {
    type: OrganizationActionTypes.SELECT_ORGANIZATION,
    payload: {
      organizationId: organizationId,
    },
  };
}

export function loadOrganizationUsers(organizationId: string): LoadOrganizationUsersAction {
  return {
    type: OrganizationActionTypes.LOAD_ORGANIZATION_USERS,
    payload: {
      organizationId: organizationId,
    },
  };
}

export function loadOrganizationUsersComplete(users: User[]): LoadOrganizationUsersCompleteAction {
  return {
    type: OrganizationActionTypes.LOAD_ORGANIZATION_USERS_COMPLETE,
    payload: {
      users: users,
    },
  };
}

export function loadOrganizationUsersFailed(errorCode: string, errorMessage: string): LoadOrganizationUsersFailedAction {
  return {
    type: OrganizationActionTypes.LOAD_ORGANIZATION_USERS_FAILED,
    payload: {
      errorCode,
      errorMessage,
    },
  };
}

export function loadOrganizationInvite(token: string): LoadOrganizationInviteAction {
  return {
    type: OrganizationActionTypes.LOAD_ORGANIZATION_INVITE,
    payload: {
      token: token,
    },
  };
}

export function loadOrganizationInviteComplete(invite: OrganizationInvite): LoadOrganizationInviteCompleteAction {
  return {
    type: OrganizationActionTypes.LOAD_ORGANIZATION_INVITE_COMPLETE,
    payload: {
      invite: invite,
    },
  };
}

export function loadOrganizationInviteFailed(errorCode: string, errorMessage: string): LoadOrganizationInviteFailedAction {
  return {
    type: OrganizationActionTypes.LOAD_ORGANIZATION_INVITE_FAILED,
    payload: {
      errorCode,
      errorMessage,
    },
  };
}

export function createOrganization(name: string): CreateOrganizationAction {
  return {
    type: OrganizationActionTypes.CREATE_ORGANIZATION,
    payload: {
      name,
    },
  };
}

export function createOrganizationComplete(): CreateOrganizationCompleteAction {
  return {
    type: OrganizationActionTypes.CREATE_ORGANIZATION_COMPLETE,
  };
}

export function createOrganizationFailed(errorCode: string, errorMessage: string): CreateOrganizationFailedAction {
  return {
    type: OrganizationActionTypes.CREATE_ORGANIZATION_FAILED,
    payload: {
      errorCode,
      errorMessage,
    },
  };
}

export function createOrganizationInvite(organizationId: string): CreateOrganizationInviteAction {
  return {
    type: OrganizationActionTypes.CREATE_ORGANIZATION_INVITE,
    payload: {
      organizationId,
    },
  };
}

export function createOrganizationInviteComplete(token: string): CreateOrganizationInviteCompleteAction {
  return {
    type: OrganizationActionTypes.CREATE_ORGANIZATION_INVITE_COMPLETE,
    payload: {
      token,
    },
  };
}

export function createOrganizationInviteFailed(errorCode: string, errorMessage: string): CreateOrganizationInviteFailedAction {
  return {
    type: OrganizationActionTypes.CREATE_ORGANIZATION_INVITE_FAILED,
    payload: {
      errorCode,
      errorMessage,
    },
  };
}

export function acceptOrganizationInvite(token: string): AcceptOrganizationInviteAction {
  return {
    type: OrganizationActionTypes.ACCEPT_ORGANIZATION_INVITE,
    payload: {
      token,
    },
  };
}

export function acceptOrganizationInviteComplete(): AcceptOrganizationInviteCompleteAction {
  return {
    type: OrganizationActionTypes.ACCEPT_ORGANIZATION_INVITE_COMPLETE,
  };
}

export function acceptOrganizationInviteFailed(errorCode: string, errorMessage: string): AcceptOrganizationInviteFailedAction {
  return {
    type: OrganizationActionTypes.ACCEPT_ORGANIZATION_INVITE_FAILED,
    payload: {
      errorCode,
      errorMessage,
    },
  };
}

export interface LoadOrganizationsAction extends Action {
  type: OrganizationActionTypes.LOAD_ORGANIZATIONS;
}

export interface LoadOrganizationAction extends Action {
  type: OrganizationActionTypes.LOAD_ORGANIZATION;
  payload: {
    organizationId: string;
  };
}

export interface LoadOrganizationsCompleteAction extends Action {
  type: OrganizationActionTypes.LOAD_ORGANIZATIONS_COMPLETE;
  payload: {
    organizations: Organization[];
  };
}

export interface LoadOrganizationsFailedAction extends Action {
  type: OrganizationActionTypes.LOAD_ORGANIZATIONS_FAILED;
  payload: {
    errorCode: string;
    errorMessage: string;
  };
}

export interface LoadOrganizationUsersAction extends Action {
  type: OrganizationActionTypes.LOAD_ORGANIZATION_USERS;
  payload: {
    organizationId: string;
  };
}

export interface LoadOrganizationUsersCompleteAction extends Action {
  type: OrganizationActionTypes.LOAD_ORGANIZATION_USERS_COMPLETE;
  payload: {
    users: User[];
  };
}

export interface LoadOrganizationUsersFailedAction extends Action {
  type: OrganizationActionTypes.LOAD_ORGANIZATION_USERS_FAILED;
  payload: {
    errorCode: string;
    errorMessage: string;
  };
}

export interface LoadOrganizationInviteAction extends Action {
  type: OrganizationActionTypes.LOAD_ORGANIZATION_INVITE;
  payload: {
    token: string;
  };
}

export interface LoadOrganizationInviteCompleteAction extends Action {
  type: OrganizationActionTypes.LOAD_ORGANIZATION_INVITE_COMPLETE;
  payload: {
    invite: OrganizationInvite;
  };
}

export interface LoadOrganizationInviteFailedAction extends Action {
  type: OrganizationActionTypes.LOAD_ORGANIZATION_INVITE_FAILED;
  payload: {
    errorCode: string;
    errorMessage: string;
  };
}

export interface SelectOrganizationAction extends Action {
  type: OrganizationActionTypes.SELECT_ORGANIZATION;
  payload: {
    organizationId: string;
  };
}

export interface CreateOrganizationAction extends Action {
  type: OrganizationActionTypes.CREATE_ORGANIZATION;
  payload: {
    name: string;
  };
}

export interface CreateOrganizationCompleteAction extends Action {
  type: OrganizationActionTypes.CREATE_ORGANIZATION_COMPLETE;
}

export interface CreateOrganizationFailedAction extends Action {
  type: OrganizationActionTypes.CREATE_ORGANIZATION_FAILED;
  payload: {
    errorCode: string;
    errorMessage: string;
  };
}

export interface CreateOrganizationInviteAction extends Action {
  type: OrganizationActionTypes.CREATE_ORGANIZATION_INVITE;
  payload: {
    organizationId: string;
  };
}

export interface CreateOrganizationInviteCompleteAction extends Action {
  type: OrganizationActionTypes.CREATE_ORGANIZATION_INVITE_COMPLETE;
  payload: {
    token: string;
  };
}

export interface CreateOrganizationInviteFailedAction extends Action {
  type: OrganizationActionTypes.CREATE_ORGANIZATION_INVITE_FAILED;
  payload: {
    errorCode: string;
    errorMessage: string;
  };
}

export interface AcceptOrganizationInviteAction extends Action {
  type: OrganizationActionTypes.ACCEPT_ORGANIZATION_INVITE;
  payload: {
    token: string;
  };
}

export interface AcceptOrganizationInviteCompleteAction extends Action {
  type: OrganizationActionTypes.ACCEPT_ORGANIZATION_INVITE_COMPLETE;
}

export interface AcceptOrganizationInviteFailedAction extends Action {
  type: OrganizationActionTypes.ACCEPT_ORGANIZATION_INVITE_FAILED;
  payload: {
    errorCode: string;
    errorMessage: string;
  };
}

export type OrganizationsAction =
  | LoadOrganizationsAction
  | LoadOrganizationAction
  | SelectOrganizationAction
  | LoadOrganizationsCompleteAction
  | LoadOrganizationsFailedAction
  | LoadOrganizationUsersAction
  | LoadOrganizationUsersCompleteAction
  | LoadOrganizationUsersFailedAction
  | LoadOrganizationInviteAction
  | LoadOrganizationInviteCompleteAction
  | LoadOrganizationInviteFailedAction
  | CreateOrganizationAction
  | CreateOrganizationCompleteAction
  | CreateOrganizationFailedAction
  | CreateOrganizationInviteAction
  | CreateOrganizationInviteCompleteAction
  | CreateOrganizationInviteFailedAction
  | AcceptOrganizationInviteAction
  | AcceptOrganizationInviteCompleteAction
  | AcceptOrganizationInviteFailedAction;
