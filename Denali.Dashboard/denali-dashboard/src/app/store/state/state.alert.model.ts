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
         },
         {
            ticker: 'BOW',
            strategy: 'Gap Up',
            time: 'Today'
        },
        {
            ticker: 'TSLA',
            strategy: 'Gap Up',
            time: 'Today'
        }
     ]
}