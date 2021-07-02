import { ActionReducerMap } from "@ngrx/store";
import { IAppState } from "../state/state.app.model";
import { alertReducer } from "./alert.reducer";
import { connectedReducer } from "./connected.reducer";

export const appReducers: ActionReducerMap<IAppState, any> = {
    alertState: alertReducer,
    connected: connectedReducer
}