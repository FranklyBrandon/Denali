import { ActionReducerMap } from "@ngrx/store";
import { IAppState } from "../state/state.model";
import { alertReducer } from "./alert.reducer";

export const appReducers: ActionReducerMap<IAppState, any> = {
    alertState: alertReducer
}