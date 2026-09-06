import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TicTacToe } from './tic-tac-toe';

describe('TicTacToe', () => {
  let component: TicTacToe;
  let fixture: ComponentFixture<TicTacToe>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TicTacToe],
    }).compileComponents();

    fixture = TestBed.createComponent(TicTacToe);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
