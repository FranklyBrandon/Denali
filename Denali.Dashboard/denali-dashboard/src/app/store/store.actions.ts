import { createAction, props } from '@ngrx/store';
import { EntryAlert } from '../models/entry-alert.model';

export const addAlert = createAction(
    '[Alerts] Add Alter',
    props<{ alert: EntryAlert }>()
  );