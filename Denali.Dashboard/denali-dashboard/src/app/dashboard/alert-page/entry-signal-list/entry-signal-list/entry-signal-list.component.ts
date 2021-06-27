import { Component, OnInit } from '@angular/core';
import { select, Store } from '@ngrx/store';
import { selectEntryAlerts } from 'src/app/store/selector/alert.selector';
import { IAppState } from 'src/app/store/state/state.model';

@Component({
  selector: 'app-entry-signal-list',
  templateUrl: './entry-signal-list.component.html',
  styleUrls: ['./entry-signal-list.component.scss']
})
export class EntrySignalListComponent implements OnInit {

  entryAlerts$ = this.store.pipe(select(selectEntryAlerts));

  constructor(private store: Store<IAppState>) { }

  ngOnInit(): void {
  }

}
