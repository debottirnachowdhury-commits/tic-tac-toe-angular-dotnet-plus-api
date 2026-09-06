import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GameService } from './game.service';

export enum GameStatus {
  InProgress = 0,
  Won = 1,
  Draw = 2
}

@Component({
  selector: 'app-tic-tac-toe',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './tic-tac-toe.html',
  styleUrls: ['./tic-tac-toe.css']
})
export class TicTacToeComponent {
  gameStatus: number = GameStatus.InProgress;
  GameStatus = GameStatus;
  board: string[] = Array(9).fill('');
  currentPlayer: string = 'X';
  gameId: string | null = null;
  scoreboard: any = {};
  vsComputer: boolean = false;

  constructor(private gameService: GameService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.startGame(false); // default: two player
    this.loadScoreboard();
  }

  // Start a new game
  startGame(vsComputer: boolean = false) {
    this.vsComputer = vsComputer;
    this.gameService.startGame().subscribe({
      next: response => {
        this.gameId = response.id;
        this.applyResponse(response);
        this.cdr.detectChanges(); // force refresh
      },
      error: err => {
        console.error('Start game failed:', err);
        alert('Could not start game. Check backend.');
      }
    });
  }

  // Make a move
  makeMove(index: number) {
    if (!this.board[index] && this.gameId) {
      const move = {
        moveNumber: this.board.filter(x => x).length + 1,
        player: this.currentPlayer,
        row: Math.floor(index / 3),
        col: index % 3
      };

      this.gameService.makeMove(this.gameId, move, this.vsComputer).subscribe({
        next: response => {
          this.applyResponse(response);
          this.cdr.detectChanges(); // refresh after human move

          // If Computer Mode, trigger computer move immediately
          if (this.vsComputer && response.status === GameStatus.InProgress && response.currentPlayer === 'O') {
            this.gameService.makeComputerMove(this.gameId!).subscribe({
              next: compResponse => {
                this.applyResponse(compResponse);
                this.cdr.detectChanges(); // refresh after computer move
              },
              error: err => console.error('Computer move failed:', err)
            });
          }
        },
        error: err => {
          console.error('Move failed:', err);
          alert('Move failed — check backend route.');
        }
      });
    }
  }

  // Apply backend response to component state
  private applyResponse(response: any) {
    this.board = [...response.board]; // new array reference
    this.currentPlayer = response.currentPlayer;
    this.gameStatus = response.status;

    // if (response.winner) {
    //   alert(`Winner: ${response.winner}`);
    //   this.loadScoreboard();
    // } else if (response.status === GameStatus.Draw) {
    //   alert('Match Draw!');
    //   this.loadScoreboard();
    // }
    this.cdr.detectChanges(); // force UI refresh
    setTimeout(() => {
      if (response.winner) {
        alert(`Winner: ${response.winner}`);
        this.loadScoreboard();
      } else if (response.status === GameStatus.Draw) {
        alert('Match Draw!');
        this.loadScoreboard();
      }
    });
  }

  // Reset game
  resetGame() {
    if (this.gameId) {
      this.gameService.resetGame(this.gameId).subscribe({
        next: response => {
          this.applyResponse(response);
          this.cdr.detectChanges(); // refresh
        },
        error: err => {
          console.error('Reset failed:', err);
          alert('Could not reset game.');
        }
      });
    }
  }

  // Undo last move
  undoMove() {
    if (this.gameId) {
      this.gameService.undoMove(this.gameId, this.vsComputer).subscribe({
        next: response => {
          this.applyResponse(response);
          this.cdr.detectChanges(); // refresh
        },
        error: err => {
          console.error('Undo failed:', err);
          alert('Could not undo move.');
        }
      });
    }
  }

  // Load scoreboard
  loadScoreboard() {
    this.gameService.getScoreboard().subscribe({
      next: response => {
        this.scoreboard = response;
        this.cdr.detectChanges(); // refresh
      },
      error: err => {
        console.error('Scoreboard failed:', err);
      }
    });
  }

  // Reset scoreboard
  resetScoreboard() {
    this.gameService.resetScoreboard().subscribe({
      next: () => {
        this.loadScoreboard();
        this.cdr.detectChanges(); // refresh
      },
      error: err => {
        console.error('Reset scoreboard failed:', err);
      }
    });
  }
}
