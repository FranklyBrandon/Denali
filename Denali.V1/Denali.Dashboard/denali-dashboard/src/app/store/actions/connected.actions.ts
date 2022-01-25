import { createAction, props } from "@ngrx/store";

export enum ConnectedActions {
    setConnectedStatus = '[Connected] - Set Connected Status'
}

export const setConnectedStatus = createAction(
    ConnectedActions.setConnectedStatus, 
    props<{payload: boolean}>()
);