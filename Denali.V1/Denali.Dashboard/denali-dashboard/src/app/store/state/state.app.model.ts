import { IAlertState, initialAlertState } from "./state.alert.model";

export interface IAppState {
    alertState: IAlertState;
    connected: boolean;
}

export const initialAppState: IAppState = {
    alertState: initialAlertState,
    connected: false
}