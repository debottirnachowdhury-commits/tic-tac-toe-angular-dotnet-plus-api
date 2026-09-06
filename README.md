# tic-tac-toe-angular-dotnet-plus-api

This project contains a Tic Tac Toe game built with Angular frontend and .NET Core Web API backend.

## Project Overview
A browser-based Tic Tac Toe application with:
- Angular + TypeScript frontend
- .NET Core Web API backend
- REST API communication
- In-memory or SQLite storage

The backend is the source of truth for game state, rules, move validation, history, and scoreboard.

## Tech Stack
- Angular (UI)
- TypeScript
- .NET Core Web API
- REST APIs
- GitHub for source control

## Features Implemented
- 3x3 Tic Tac Toe board
- Two Player Mode and Computer Mode
- Move history tracking
- Undo functionality (see Clarifications below)
- Win/draw detection
- Scoreboard with reset option
- Reset Game option
- Highlight winning cells

## How to Run Backend
cd TicTacToeApi
dotnet run

Backend runs on http://localhost:5000

## How to Run Frontend
cd UiAngular
ng serve

Frontend runs on http://localhost:4200

## API Endpoints
- POST /api/games → Create new game
- GET /api/games/{id} → Get current game state
- POST /api/games/{id}/moves → Submit move
- POST /api/games/{id}/undo → Undo last move
- POST /api/games/{id}/reset → Reset game
- GET /api/scoreboard → Get scoreboard
- POST /api/scoreboard/reset → Reset scoreboard

## How to Run Tests
Backend unit tests:
cd TicTacToeApi.Tests
dotnet test

Frontend tests:
cd UiAngular
ng test

## Clarifications and Design Decisions
- Backend State Ownership: Backend is source of truth for game state.
- Undo and Scoreboard: Option A chosen → Undo disabled after game completion. Scoreboard remains final once a game ends.
- Reset Button: Reset is also disabled once a game ends (Win or Draw). This ensures the final scoreboard and game state remain consistent.

## AI Tools and Prompt Summary
- AI tools were used to generate scaffolding for Angular components and .NET controllers.
- Prompts focused on API design and service integration.
- Manual changes included: validation logic, disabling Undo/Reset after completion, scoreboard consistency, and test coverage.
- All generated code was reviewed and adjusted to meet acceptance criteria.

## Known Limitations
- Basic computer opponent (rule-based, not AI).
- No persistent database (in-memory/SQLite only).
- Limited UI styling.

## Future Improvements
- Stronger AI opponent (minimax).
- Persistent database with migrations.
- Enhanced UI/UX with animations.
- Multiplayer over network.

  ## Acceptance Criteria Checklist
- Angular application runs locally
- .NET API runs locally
- Frontend communicates with backend via REST APIs
- A new Tic Tac Toe game can be created
- Two Player Mode works correctly
- Computer Mode works correctly
- Turns alternate correctly
- Invalid moves are handled correctly
- Win detection works
- Draw detection works
- Winning cells are highlighted
- Move history is shown
- Undo disabled after completion
- Reset disabled after completion
- Scoreboard works correctly
- Reset Scoreboard works correctly
- Basic tests are included
- README explains how to run and review the solution

