import { Action } from "redux";

export interface Organization {
  organizationId: string;
  name: string;
}

export enum OrganizationActionTypes {
  LOAD_ORGANIZATION = 'LOAD_ORGANIZATION',
  LOAD_ORGANIZATIONS = 'LOAD_ORGANIZATIONS',
  LOAD_ORGANIZATIONS_COMPLETE = 'LOAD_ORGANIZATIONS_COMPLETE',
  LOAD_ORGANIZATIONS_FAILED = 'LOAD_ORGANIZATIONS_FAILED',
  
  CREATE_ORGANIZATION = 'CREATE_ORGANIZATION',
  CREATE_ORGANIZATION_COMPLETE = 'CREATE_ORGANIZATION_COMPLETE',
  CREATE_ORGANIZATION_FAILED = 'CREATE_ORGANIZATION_FAILED',
}

export function loadOrganizations() : LoadOrganizationsAction {
  return {
    type: OrganizationActionTypes.LOAD_ORGANIZATIONS,
  }
}

export function loadOrganization(organizationId: string) : LoadOrganizationAction {
  return {
    type: OrganizationActionTypes.LOAD_ORGANIZATION,
    payload: {
      organizationId: organizationId,
    }
  }
}

export function loadOrganizationsComplete(organizations: Organization[]) : LoadOrganizationsCompleteAction {
  return {
    type: OrganizationActionTypes.LOAD_ORGANIZATIONS_COMPLETE,
    payload: {
      organizations: organizations
    }
  };
}

export function createOrganization(name: string) : CreateOrganizationAction {
  return {
    type: OrganizationActionTypes.CREATE_ORGANIZATION,
    payload: {
      name
    }
  }
}

export function createOrganizationComplete() : CreateOrganizationCompleteAction {
  return {
    type: OrganizationActionTypes.CREATE_ORGANIZATION_COMPLETE,
  };
}

export function createOrganizationFailed(errorCode: string, errorMessage: string) : CreateOrganizationFailedAction {
  return {
    type: OrganizationActionTypes.CREATE_ORGANIZATION_FAILED,
    payload: {
      errorCode,
      errorMessage
    }
  };
}

export interface LoadOrganizationAction extends Action {
  type: OrganizationActionTypes.LOAD_ORGANIZATION;
  payload: { 
    organizationId: string
  }
}

export interface LoadOrganizationsAction extends Action {
  type: OrganizationActionTypes.LOAD_ORGANIZATIONS
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

export interface CreateOrganizationAction extends Action {
  type: OrganizationActionTypes.CREATE_ORGANIZATION;
  payload: {
    name: string
  }
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

export type OrganizationsAction =
  | LoadOrganizationAction
  | LoadOrganizationsAction
  | LoadOrganizationsCompleteAction
  | LoadOrganizationsFailedAction
  | CreateOrganizationAction
  | CreateOrganizationCompleteAction
  | CreateOrganizationFailedAction;