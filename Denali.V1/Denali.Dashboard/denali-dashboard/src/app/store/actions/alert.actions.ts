import { createAction, props } from "@ngrx/store";
import { EntryAlert } from "src/app/models/entryAlert.model";

export enum AlertActions {
    AddEntryAlert = '[EntryAlerts] - Add Alert'
}

export const addEntryAlert = createAction(
    AlertActions.AddEntryAlert, 
    props<{payload: EntryAlert}>()
);