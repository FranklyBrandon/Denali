import { createSelector } from "@ngrx/store";
import { IAlertState, IAppState } from "../state/state.model";

const selectAlerts = (state: IAppState) => state.alertState;

export const selectEntryAlerts = createSelector(
    selectAlerts,
    (state: IAlertState) => state.entryAlerts
);