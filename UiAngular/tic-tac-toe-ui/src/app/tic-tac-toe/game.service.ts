import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class GameService {
  private baseUrl = 'https://localhost:7077/api';

  constructor(private http: HttpClient) {}

  startGame(): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/games`, {});
  }

  makeMove(id: string, move: any, vsComputer: boolean = false): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/games/${id}/moves?vsComputer=${vsComputer}`, move);
  }

  makeComputerMove(id: string): Observable<any> {
  return this.http.post<any>(`${this.baseUrl}/games/${id}/computer-move`, {});
  }

  resetGame(id: string): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/games/${id}/reset`, {});
  }

  undoMove(id: string, vsComputer: boolean = false): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/games/${id}/undo?vsComputer=${vsComputer}`, {});
  }

  getScoreboard(): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/scoreboard`);
  }

  resetScoreboard(): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/scoreboard/reset`, {});
  }
}
