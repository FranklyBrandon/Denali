import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EntrySignalListComponent } from './entry-signal-list.component';

describe('EntrySignalListComponent', () => {
  let component: EntrySignalListComponent;
  let fixture: ComponentFixture<EntrySignalListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EntrySignalListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EntrySignalListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
