import { createSelector } from '@ngrx/store';
import { AppState } from './store.state';

export const selectAlerts = (state: AppState) => state.alerts;