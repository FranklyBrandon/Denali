import { EntryAlert } from "src/app/models/entryAlert.model";

export interface IAlertState {
    entryAlerts: EntryAlert[];
}

export const initialAlertState: IAlertState = {
     entryAlerts: [
         {
             ticker: 'AAPL',
             strategy: 'Gap Up',
             time: 'Today'
         }
     ]
}

export interface IAppState {
    alertState: IAlertState;
}

export const initialAppState: IAppState = {
    alertState: initialAlertState
}