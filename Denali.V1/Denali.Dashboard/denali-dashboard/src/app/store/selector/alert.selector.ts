import { createSelector } from "@ngrx/store";
import { IAlertState } from "../state/state.alert.model";
import { IAppState } from "../state/state.app.model";

const selectAlerts = (state: IAppState) => state.alertState;

export const selectEntryAlerts = createSelector(
    selectAlerts,
    (state: IAlertState) => state.entryAlerts
);