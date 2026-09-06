import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TicTacToeComponent } from './tic-tac-toe/tic-tac-toe';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, TicTacToeComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('tic-tac-toe-ui');
}
