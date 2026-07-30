# Copilot Instructions — ProductsApi (personal .NET/Azure learning repo)

This repository is a personal project to become a **.NET full-stack developer on Azure**.
The running journal and roadmap live in `products-api/copilot/README.md`; the per-topic,
copy-paste prompts are in `products-api/copilot/session-prompts.md`.

## How to teach / work with me

- **Interview-grade depth — do NOT rush or skim.** For every topic, go deep and proactively
  cover the **key interview questions**, not just a surface overview. Prioritize real
  understanding over "closing" a topic. (I once failed an interview on EF Core eager loading /
  `Include` / lazy-vs-explicit because we skimmed — make sure that doesn't repeat.)
- **Learning method per topic:** concept → hands-on exercise → common pitfalls →
  **10 interview Q&A**. One topic at a time.
- **Mock interviews:** when I ask (or to check readiness), act as the **interviewer** — ask
  **one question at a time**, let me answer first, then assess (✅ correct / ⚠️ missing /
  model answer) with a **follow-up probe**, and give an overall score + weak-area focus at the
  end. Revisit weak areas instead of moving on.
- **Assess honestly** — point out gaps directly; don't over-praise.

## Coding conventions

- Add **XML doc comments** (`/// <summary>`) to every new or changed class and method.
- Add inline comments explaining **critical design decisions** (why a pattern was chosen).
- When creating **multiple new files**, show **one file at a time** and wait for review before
  the next — don't batch-create.
- Use **meaningful, descriptive names**; avoid single-letter or abbreviated identifiers.
- Keep implementation summaries consistent with the latest decisions on every change.

## Repo layout & hands-on environment

- `products-api/` — .NET minimal API (`net10.0`) · `products-web/` — React + TypeScript (Vite)
  · `Practice/` — C# exercises (incl. `Algos/`) · `sql-practice/` — T-SQL practice.
- **SQL practice:** SQL Server **LocalDB** (`(localdb)\MSSQLLocalDB`, database `ShopPractice`).
  Load the helper once with `. .\sql-practice\Run-Query.ps1`, then run `qt "<query>"` for
  auto-sized output. Reference: `sql-practice/SQL-CheatSheet.md` (when-to-use + interview patterns).
- **Auth (dev):** JWT via the dev `/dev/token` minter — no Entra ID needed locally.
- **Azure hands-on:** prefer local emulators (e.g. **Azurite** for Storage) when there's no
  Azure subscription.
- Shell is **Windows PowerShell 5.1** — no `??` null-coalescing operator; use `if/else`.

## Progress (see README for the authoritative status)

Days 1–3 complete (Topics 1–6) plus a SQL Server / T-SQL deep dive. Next main-track topic:
**Topic 7 — Azure Storage** (Blob, Queue, Table).
