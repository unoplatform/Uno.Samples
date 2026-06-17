# AI Agents Contribution & Coding Instructions

This document defines strict guardrails for any AI-assisted or automated agent contributions (including Copilot, custom prompt runners, or scripted refactors). Human contributors must also ensure generated changes comply before merge.

## Repository boundaries

- **NEVER modify the sibling `studio.live` repository** (or any repo outside this one) from work done here. It may be present in the same workspace and is useful **read-only** for understanding upstream behavior (e.g. the feedback `BundleClassifier`). Changes that belong upstream (classifier logic, signal computation, thresholds) MUST be captured as written proposals under `specs/` for the studio.live team to act on — not applied here. All code changes from this repo's tasks land in this repo only.

<flow_orchestration>

### 1. Plan Node Default
- Enter plan mode for ANY non-trivial task (3+ steps or architectural decisions)
- If something goes sideways, STOP and re-plan immediately - don't keep pushing
- Use plan mode for verification steps, not just building
- Write detailed specs upfront to reduce ambiguity

### 2. Subagent Strategy
- Use subagents liberally to keep main context window clean
- Offload research, exploration, and parallel analysis to subagents
- For complex problems, throw more compute at it via subagents
- One task per subagent for focused execution

### 3. Self-Improvement Loop
- After ANY correction from the user: update `specs/lessons.md` with the pattern. If the file does not exist, create it
- Write rules for yourself that prevent the same mistake
- Ruthlessly iterate on these lessons until mistake rate drops
- When modifying the agentic workflow (system prompts, sub-agent instructions, skills, phase pipeline, agent orchestration code), review and update `.claude/skills/studio-live-agentic-workflow-audit/SKILL.md` to keep its methodology and known issues current
- Review lessons at session start for relevant project

#### Where corrections are recorded

User corrections, "do this / never do that" rules, workflow guardrails, and tool-usage policies that should bind **every** agent working on this repo MUST be written to a checked-in, shared file:
- Repo-wide rules → `AGENTS.md` (this file).
- Workflow / orchestration rules → `.claude/skills/studio-live-agentic-workflow-audit/SKILL.md`.
- Skill-specific rules (e.g. how to use a particular tool/MCP) → the relevant `.claude/skills/<skill>/SKILL.md`.
- Domain lessons / postmortems → `specs/lessons.md`.

🚫 **Never** record cross-agent corrections in personal/auto memory (e.g. `~/.claude/projects/<project>/memory/`, `feedback_*.md`, individual user preference files). Personal memory is per-user and not shared via git, so other agents and contributors will not see it and the mistake will repeat. If a correction is general enough that any future agent should follow it, it belongs in a checked-in file. Reserve personal memory for things that are genuinely individual to one user (their role, their preferences) — not project rules.

When in doubt: if removing the rule would let any other agent on this repo repeat the same mistake, the rule is shared and must be checked in.

### 4. Verification Before Done
- Never mark a task complete without proving it works
- Diff behavior between main and your changes when relevant
- Ask yourself: "Would a staff engineer approve this?"
- Run tests, check logs, demonstrate correctness
- You MUST assume for a given branch, the main branch is correct and failures are specific to the current branch. You MUST assume that changes in the current branch are the cause of any new failures.

### 5. Demand Elegance (Balanced)
- For non-trivial changes: pause and ask "is there a more elegant way?"
- If a fix feels hacky: "Knowing everything I know now, implement the elegant solution"
- Skip this for simple, obvious fixes - don't over-engineer
- Challenge your own work before presenting it

### 6. Autonomous Bug Fixing
- When given a bug report: just fix it. Don't ask for hand-holding
- Point at logs, errors, failing tests - then resolve them
- Zero context switching required from the user
- Go fix failing CI tests without being told how

## Task Management

1. ** Plan First **: Write plan to `specs/xxx/progress.md` with checkable items
2. ** Verify Plan **: Check in before starting implementation
3. ** Track Progress **: Mark items complete as you go
4. ** Explain Changes **: High-level summary at each step
5. ** Document Results **: Add review section to `specs/xxx/progress.md`
6. ** Capture Lessons **: Update `specs/lessons.md` after corrections

## Core Principles

- ** Simplicity First **: Make every change as simple as possible. Impact minimal code.
- ** No Laziness **: Find root causes. No temporary fixes. Senior developer standards.
- ** Minimal Impact **: Changes should only touch what's necessary. Avoid introducing bugs.

</flow_orchestration>